using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmaApi.Comunicacion.Procesamiento
{
    internal class Comandos
    {
        public static bool HeartBeat(Arduino arduino, Comunicacion.Models.Dispositivo dispositivo)
        {
            if (arduino._serialPort != null)
            {
                if (arduino._serialPort.IsOpen)
                {
                    string mensaje = $"{(int)Ordenes.HEARTBEAT}|{dispositivo.idDispositivo}";
                    arduino._serialPort.WriteLine(mensaje);
                    return Boolean.Parse(arduino.BloquearHastaObtenerRespuesta());
                }
            }
            return false;
        }
        public static bool EstablecerNombreDispositivo(Arduino arduino, Models.Dispositivo dispositivo, Models.Solicitud solicitud)
        {
            bool resultado = false;
            if(arduino._serialPort != null)
            {
                if (arduino._serialPort.IsOpen)
                {
                    string mensaje = $"{(int)Ordenes.ESTABLECER_NOMBRE_DISPOSITIVO}|{dispositivo.nombreDispositivo},{solicitud.nuevoNombreDispositivo}";
                    arduino._serialPort.WriteLine(mensaje);
                    resultado = Boolean.Parse(arduino.BloquearHastaObtenerRespuesta());
                    if (resultado)
                    {

                        SQL.GetInstance().ActualizarDispositivo(dispositivo, new Comunicacion.Models.Dispositivo
                        {
                            nombreDispositivo = solicitud.nuevoNombreDispositivo,
                            descripcionDispositivo = dispositivo.descripcionDispositivo,
                            estadoDispositivo = dispositivo.estadoDispositivo,
                            fechaHeartBeat = dispositivo.fechaHeartBeat,
                            fechaEstado = dispositivo.fechaEstado,
                            fechaRegistro = dispositivo.fechaRegistro,
                            tipoDispositivo = dispositivo.tipoDispositivo,
                            estadoConexion = dispositivo.estadoConexion,
                            idDispositivo = dispositivo.idDispositivo
                        });
                    }    
                }
            }
            return resultado;
        }
        public static void EncenderDispositivo(Arduino arduino, Comunicacion.Models.Dispositivo dispositivo)
        {
            if (arduino._serialPort != null)
            {
                if (arduino._serialPort.IsOpen)
                {
                    string mensaje = $"{(int)Ordenes.ENCENDER_DISPOSITIVO}|{dispositivo.idDispositivo}";
                    arduino._serialPort.WriteLine(mensaje);
                }
            }
        }
        public static void ApagarDispositivo(Arduino arduino, Comunicacion.Models.Dispositivo dispositivo)
        {
            if (arduino._serialPort != null)
            {
                if (arduino._serialPort.IsOpen)
                {
                    string mensaje = $"{(int)Ordenes.APAGAR_DISPOSITIVO}|{dispositivo.idDispositivo}";
                    arduino._serialPort.WriteLine(mensaje);
                }
            }
        }
        public static void ObtenerEstadoDispositivo(Arduino arduino, Comunicacion.Models.Dispositivo dispositivo)
        {
            if (arduino._serialPort != null)
            {
                if (arduino._serialPort.IsOpen)
                {
                    string mensaje = $"{(int)Ordenes.OBTENER_ESTADO_DISPOSITIVO}|{dispositivo.idDispositivo}";
                    arduino._serialPort.WriteLine(mensaje);
                }
            }
        }

        public const string HEARTBEAT = "HEARTBEAT";
        public const string ESTABLECER_NOMBRE_DISPOSITIVO = "ESTABLECER_NOMBRE_DISPOSITIVO";
        public const string ENCENDER_DISPOSITIVO = "ENCENDER_DISPOSITIVO";
        public const string APAGAR_DISPOSITIVO = "APAGAR_DISPOSITIVO";
        public const string OBTENER_ESTADO_DISPOSITIVO = "OBTENER_ESTADO_DISPOSITIVO";
        public const string AGREGAR_DISPOSITIVO = "AGREGAR_DISPOSITIVO";

        public enum Ordenes
        {
            HEARTBEAT,
            ESTABLECER_NOMBRE_DISPOSITIVO,
            ENCENDER_DISPOSITIVO,
            APAGAR_DISPOSITIVO,
            OBTENER_ESTADO_DISPOSITIVO,
            AGREGAR_DISPOSITIVO
        }
    }
}
