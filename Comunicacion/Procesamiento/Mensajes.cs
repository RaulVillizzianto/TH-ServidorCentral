using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmaApi.Comunicacion.Procesamiento
{
    internal class Mensajes
    {
        public static void ActualizarEstadoSensorMovimiento(string idDispositivo, int estadoSensorMovimiento)
        {
            var item = SQL.GetInstance().ObtenerDispositivo(idDispositivo);
            string _estadoDispositivo = string.Empty;
            switch(estadoSensorMovimiento)
            {
                case (int)Models.Dispositivo.ESTADO.APAGADO:
                    {
                        _estadoDispositivo = Models.Dispositivo.ESTADO.APAGADO.ToString();
                        break;
                    }
                case (int)Models.Dispositivo.ESTADO.ENCENDIDO:
                    {
                        _estadoDispositivo = Models.Dispositivo.ESTADO.ENCENDIDO.ToString();
                        break;
                    }
                case (int)Models.Dispositivo.ESTADO.ACTIVADO_POR_MOVIMIENTO:
                    {
                        _estadoDispositivo = Models.Dispositivo.ESTADO.ACTIVADO_POR_MOVIMIENTO.ToString();
                        break;
                    }
            }


            SQL.GetInstance().ActualizarDispositivo(item, new Comunicacion.Models.Dispositivo
            {
                nombreDispositivo = item.nombreDispositivo,
                descripcionDispositivo = item.descripcionDispositivo,
                estadoDispositivo = _estadoDispositivo,
                fechaEstado = DateTime.Now.ToString(),
                fechaHeartBeat = item.fechaHeartBeat,
                fechaRegistro = item.fechaRegistro,
                tipoDispositivo = item.tipoDispositivo,
                estadoConexion = item.estadoConexion,
                idDispositivo = item.idDispositivo
            });
        }
        public static void HeartBeat(string idDispositivo)
        {
            var item = Procesos.HeartBeat.timeOuts.Where(x => x.Key.idDispositivo == idDispositivo).FirstOrDefault();
            if(Procesos.HeartBeat.timeOuts.Contains(item))
            {
                SQL.GetInstance().ActualizarDispositivo(item.Key, new Comunicacion.Models.Dispositivo
                {
                    nombreDispositivo = item.Key.nombreDispositivo,
                    descripcionDispositivo = item.Key.descripcionDispositivo,
                    estadoDispositivo = item.Key.estadoDispositivo,
                    fechaEstado = item.Key.fechaEstado,
                    fechaHeartBeat = DateTime.Now.ToString(),
                    fechaRegistro = item.Key.fechaRegistro,
                    tipoDispositivo = item.Key.tipoDispositivo,
                    idDispositivo = item.Key.idDispositivo,
                    estadoConexion = Models.Dispositivo.ESTADO_CONEXION.CONECTADO.ToString()
                });
                Procesos.HeartBeat.timeOuts.Remove(item);
            }
        }
        public static void SensorMovimientoActivado(string idDispositivo)
        {
            var item = SQL.GetInstance().ObtenerDispositivo(idDispositivo);
            SQL.GetInstance().ActualizarDispositivo(item, new Comunicacion.Models.Dispositivo
            {
                nombreDispositivo = item.nombreDispositivo,
                descripcionDispositivo = item.descripcionDispositivo,
                estadoDispositivo = Models.Dispositivo.ESTADO.ACTIVADO_POR_MOVIMIENTO.ToString(),
                fechaEstado = DateTime.Now.ToString(),
                fechaHeartBeat = item.fechaHeartBeat,
                fechaRegistro = item.fechaRegistro,
                tipoDispositivo = item.tipoDispositivo,
                estadoConexion = item.estadoConexion,
                idDispositivo = item.idDispositivo

            });
            MongoDB.GetInstance().InsertarNotificacionSensorMovimientoActivado(item);
        }
        public enum Eventos
        {
            HEARTBEAT,
            SENSOR_MOVIMIENTO_ACTIVADO,
            OBTENER_ESTADO_SENSOR_MOVIMIENTO = 4,
        }
    }
}
