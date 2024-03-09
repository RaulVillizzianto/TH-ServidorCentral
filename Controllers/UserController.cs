using AlarmaApi.Comunicacion;
using AlarmaApi.Comunicacion.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AlarmaApi.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpPost]
        public dynamic Post([FromBody] IniciarSesion login)
        {
            if(login == null)
            {
                return BadRequest();
            }
            Comunicacion.Models.Respuesta.InicioSesion inicioSesion = SQL.GetInstance().IniciarSesion(login);
            if(inicioSesion.jwt != String.Empty)
            {
                return inicioSesion;
            }
            return Unauthorized(inicioSesion);
        }
        [HttpGet("{jwt}")]
        public dynamic Get(string jwt)
        {
            if (String.IsNullOrEmpty(jwt.Trim()))
            {
                return BadRequest();
            }
            Comunicacion.Models.Usuario usuario = SQL.GetInstance().ObtenerUsuario(jwt);
            if (usuario != null)
            {
                return usuario;
            }
            return Unauthorized();
        }
        [HttpGet("notifications/all/{jwt}")]
        public dynamic ObtenerNotificaciones(string jwt)
        {
            if (String.IsNullOrEmpty(jwt.Trim()))
            {
                return BadRequest();
            }
            List<Notificacion> notificaciones = AlarmaApi.Comunicacion.MongoDB.GetInstance().ObtenerNotificaciones(jwt);
            if (notificaciones != null)
            {
                return notificaciones;
            }
            return Unauthorized();
        }
        [HttpGet("notifications/detail/{jwt}/{id}")]
        public dynamic ObtenerNotificacion(string jwt, string id)
        {
            if (String.IsNullOrEmpty(jwt.Trim()))
            {
                return BadRequest();
            }
            Notificacion notificacion = AlarmaApi.Comunicacion.MongoDB.GetInstance().ObtenerNotificacion(jwt, id);
            if (notificacion != null)
            {
                return notificacion;
            }
            return Unauthorized();
        }

        [HttpGet("notifications/not_delivered/{jwt}")]
        public dynamic ObtenerNotificacionesNoEntregadas(string jwt)
        {
            if (String.IsNullOrEmpty(jwt.Trim()))
            {
                return BadRequest();
            }
            List<Notificacion> notificaciones = AlarmaApi.Comunicacion.MongoDB.GetInstance().ObtenerNotificacionesNoEntregadas(jwt);
            if (notificaciones != null)
            {
                return notificaciones;
            }
            return Unauthorized();
        }

        [HttpGet("notifications/new/{jwt}")]
        public dynamic ObtenerNotificacionesNuevas(string jwt)
        {
            if (String.IsNullOrEmpty(jwt.Trim()))
            {
                return BadRequest();
            }
            List<Notificacion> notificaciones = AlarmaApi.Comunicacion.MongoDB.GetInstance().ObtenerNuevasNotificaciones(jwt);
            if (notificaciones != null)
            {
                return notificaciones;
            }
            return Unauthorized();
        }
        [HttpPost("notifications/new")]
        public dynamic InsertarNuevaNotificacion([FromBody] Notificacion notificacion)
        {

           // AlarmaApi.Comunicacion.MongoDB.GetInstance().(notificacion);
            return Ok();
        }
        [HttpPost("notifications/delivered")]
        public dynamic MarcarNotificacionComoEntregada([FromBody] Notificacion notificacion)
        {
            if (AlarmaApi.Comunicacion.MongoDB.GetInstance().MarcarNotificacionComoEntregada(notificacion) != null)
            {
                return Ok();
            }
            return Unauthorized();
        }
        [HttpPost("notifications/seen")]
        public dynamic MarcarNotificacionComoVista([FromBody] Notificacion notificacion)
        {
            if (AlarmaApi.Comunicacion.MongoDB.GetInstance().MarcarNotificacionComoVista(notificacion) != null)
            {
                return Ok();
            }
            return Unauthorized();
        }
    }
}
