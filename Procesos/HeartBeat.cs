using AlarmaApi.Comunicacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmaApi.Procesos
{
    public sealed class HeartBeat
    {
        private HeartBeat() { }
        private static HeartBeat _instance;
        public static HeartBeat GetInstance()
        {
            if (_instance == null)
            {
                _instance = new HeartBeat();
            }
            return _instance;
        }

        private static bool trabajando = true;
        private const int INTERVALO = 10000;
        private const double TIMEOUT_DISPOSITIVO = 120;
        // <key> = <valor>
        public static List<KeyValuePair<Comunicacion.Models.Dispositivo, DateTime>> timeOuts = new List<System.Collections.Generic.KeyValuePair<Comunicacion.Models.Dispositivo, DateTime>>();
        public static void Inicio(Arduino arduino)
        {
            Console.WriteLine($"{DateTime.Now.ToShortDateString()} => HeartBeatProcess");
            while (trabajando)
            {
                foreach(var dispositivo in SQL.GetInstance().ObtenerDispositivos())
                {
                    arduino.HeartBeat(dispositivo);
                    if (!timeOuts.Exists(x => x.Key.idDispositivo == dispositivo.idDispositivo))
                    {
                        timeOuts.Add(new KeyValuePair<Comunicacion.Models.Dispositivo, DateTime>(dispositivo, DateTime.Now.AddSeconds(TIMEOUT_DISPOSITIVO)));
                        SQL.GetInstance().ActualizarDispositivo(dispositivo, new Comunicacion.Models.Dispositivo
                        {
                            nombreDispositivo = dispositivo.nombreDispositivo,
                            descripcionDispositivo = dispositivo.descripcionDispositivo,
                            estadoDispositivo=dispositivo.estadoDispositivo,
                            fechaHeartBeat=DateTime.Now.ToString(),
                            fechaEstado = dispositivo.fechaEstado,
                            fechaRegistro = dispositivo.fechaRegistro,
                            tipoDispositivo=dispositivo.tipoDispositivo,
                            estadoConexion = dispositivo.estadoConexion,
                            idDispositivo = dispositivo.idDispositivo
                        });
                    }
                    else
                    {
                        VerificarTimeOuts(dispositivo);
                    }
                }
                Thread.Sleep(INTERVALO);
            }               
        }
        private static void VerificarTimeOuts(Comunicacion.Models.Dispositivo dispositivo)
        {
            if (timeOuts.Exists(x => x.Key.idDispositivo == dispositivo.idDispositivo))
            {
                var timeOutDispositivo = timeOuts.Where(x => x.Key.idDispositivo == dispositivo.idDispositivo).First().Value;
                if (DateTime.Now > timeOutDispositivo && dispositivo.estadoConexion != Comunicacion.Models.Dispositivo.ESTADO_CONEXION.DESCONECTADO.ToString())
                {
                    SQL.GetInstance().ActualizarDispositivo(dispositivo, new Comunicacion.Models.Dispositivo
                    {
                        nombreDispositivo = dispositivo.nombreDispositivo,
                        descripcionDispositivo = dispositivo.descripcionDispositivo,
                        estadoDispositivo = dispositivo.estadoDispositivo,
                        fechaEstado = DateTime.Now.ToString(),
                        fechaHeartBeat = dispositivo.fechaHeartBeat,
                        fechaRegistro = dispositivo.fechaRegistro,
                        tipoDispositivo = dispositivo.tipoDispositivo,
                        idDispositivo = dispositivo.idDispositivo,
                        estadoConexion = Comunicacion.Models.Dispositivo.ESTADO_CONEXION.DESCONECTADO.ToString(),
                    });
                    AlarmaApi.Comunicacion.MongoDB.GetInstance().InsertarNotificacionDispositivoDesconectado(dispositivo);

                }
            }
        }
    }
}
