using Microsoft.AspNetCore.Mvc;

namespace AlarmaApi.Comunicacion.Procesamiento
{
    public class Solicitudes
    {
        public static StatusCodeResult ProcesarSolicitud(Models.Solicitud solicitud)
        {
            if(!SQL.GetInstance().VerificarSesion(solicitud.jwt))
            {
                return new UnauthorizedResult();
            }

            var dispositivo = SQL.GetInstance().ObtenerDispositivo(solicitud.idDispositivo);
            if(dispositivo == null && !solicitud.comandoDispositivo.Equals(Comandos.AGREGAR_DISPOSITIVO))
            {
                return new StatusCodeResult(404);
            }
            switch(solicitud.comandoDispositivo)
            {
                case Comandos.ESTABLECER_NOMBRE_DISPOSITIVO:
                    {
                        Procesamiento.Comandos.EstablecerNombreDispositivo(Program.arduino, dispositivo, solicitud);
                        return new OkResult();
                    }
                case Comandos.AGREGAR_DISPOSITIVO:
                    {
                        var usuario = SQL.GetInstance().ObtenerUsuario(solicitud.jwt);
                        Models.Dispositivo nuevoDispositivo = new Models.Dispositivo()
                        {
                            usuario = usuario.usuario,
                            nombreDispositivo = solicitud.nombreDispositivo,
                            tipoDispositivo = solicitud.tipoDispositivo,
                            descripcionDispositivo = solicitud.descripcionDispositivo,
                            idDispositivo = solicitud.idDispositivo,
                        };
                        SQL.GetInstance().AgregarDispositivo(nuevoDispositivo);
                        return new OkResult();
                    }
                case Comandos.ENCENDER_DISPOSITIVO:
                    {
                        Comandos.EncenderDispositivo(Program.arduino, dispositivo);
                        return new OkResult();
                    }
                case Comandos.APAGAR_DISPOSITIVO:
                    {
                        Comandos.ApagarDispositivo(Program.arduino, dispositivo);
                        return new OkResult();
                    }
                case Comandos.HEARTBEAT:
                    {
                        Comandos.HeartBeat(Program.arduino, dispositivo);
                        return new OkResult();
                    }
                case Comandos.OBTENER_ESTADO_DISPOSITIVO:
                    {
                        Comandos.ObtenerEstadoDispositivo(Program.arduino, dispositivo);
                        return new OkResult();
                    }
                default:
                    {
                        return new StatusCodeResult(400);
                    }
            }
        }
    } 
}
