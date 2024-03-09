using AlarmaApi.Comunicacion;
using AlarmaApi.Comunicacion.Procesamiento;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AlarmaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        // GET: api/<DeviceController>
        [HttpGet("{jwt}")]
        public dynamic Get(string jwt)
        {
            var dispositivos = SQL.GetInstance().ObtenerDispositivos(jwt);
            if(dispositivos != null)
            {
                foreach (var dispositivo in dispositivos)
                {
                    Comunicacion.Models.Solicitud solicitud = new Comunicacion.Models.Solicitud()
                    {
                        nombreDispositivo = dispositivo.nombreDispositivo,
                        idDispositivo = dispositivo.idDispositivo,
                        jwt = jwt,
                        comandoDispositivo = Comandos.OBTENER_ESTADO_DISPOSITIVO
                    };
                    Comunicacion.Procesamiento.Solicitudes.ProcesarSolicitud(solicitud);
                }
                return JsonConvert.SerializeObject(dispositivos);
            }
            return Unauthorized();
          
        }

        [HttpGet("tipoDispositivos")]
        public string ObtenerTipoDispositivos()
        {
            return JsonConvert.SerializeObject(SQL.GetInstance().ObtenerTipoDispositivos());
        }

        // GET api/<DeviceController>/default
        [HttpGet("{jwt}/{id}")]
        public string Get(string jwt,string id)
        {
            Comunicacion.Models.Solicitud solicitud = new Comunicacion.Models.Solicitud()
            {
                idDispositivo = id,
                jwt = jwt,
                comandoDispositivo = Comandos.OBTENER_ESTADO_DISPOSITIVO
            };
            Comunicacion.Procesamiento.Solicitudes.ProcesarSolicitud(solicitud);
            return JsonConvert.SerializeObject(SQL.GetInstance().ObtenerDispositivo(id));
        }

        // POST api/<DeviceController>
        [HttpPost]
        public StatusCodeResult Post([FromBody] Comunicacion.Models.Solicitud solicitud)
        {
            if(solicitud == null)
            {
                return BadRequest();
            }
            if(String.IsNullOrEmpty(solicitud.nombreDispositivo) || String.IsNullOrWhiteSpace(solicitud.nombreDispositivo))
            {
                return BadRequest();
            }
            if (String.IsNullOrEmpty(solicitud.comandoDispositivo) || String.IsNullOrWhiteSpace(solicitud.comandoDispositivo))
            {
                return BadRequest();
            }
            return Comunicacion.Procesamiento.Solicitudes.ProcesarSolicitud(solicitud);
        }

        // DELETE api/<DeviceController>/5
        [HttpDelete("{id}")]
        public StatusCodeResult Delete(string id)
        {
            return SQL.GetInstance().EliminarDispositivo(id) == true ? Ok() : BadRequest();
        }
    }
}
