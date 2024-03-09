using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmaApi.Comunicacion.Models
{
    public class Dispositivo
    {
        public string nombreDispositivo { get; set; }

        public string usuario { get; set; }
        public string descripcionDispositivo { get; set; }
        public string fechaRegistro { get; set; }
        public string tipoDispositivo { get; set; }
        public string estadoDispositivo { get; set; }
        public string fechaHeartBeat { get; set; }
        public string fechaEstado { get; set; }
        public string estadoConexion { get; set;}
        public string idDispositivo { get; set; }

        public enum ESTADO_CONEXION
        {
            CONECTADO,
            DESCONECTADO
        }

        public enum ESTADO
        {
            APAGADO,
            ENCENDIDO,
            ACTIVADO_POR_MOVIMIENTO
        }
        public enum TIPO_DISPOSITIVO
        {
            SENSOR_MOVIMIENTO = 1,
        }
    }
}
