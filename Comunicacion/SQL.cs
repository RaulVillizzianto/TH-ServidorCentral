using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace AlarmaApi.Comunicacion
{
    internal class SQL
    {
        public const string connectionString = "data source=DESKTOP-TK7BU23\\SQLEXPRESS;initial catalog=alarma;trusted_connection=true;MultipleActiveResultSets=true";
        public SqlConnection? sqlConnection;

        private SQL() {

            sqlConnection = new SqlConnection(connectionString);
            if (sqlConnection != null)
            {
                sqlConnection.Open();
                sqlConnection.StateChange += SqlConnection_StateChange;
            }
        }
        private static SQL _instance;
        public static SQL GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SQL();
            }
            return _instance;
        }
        public Models.Usuario ObtenerUsuario(string jwt)
        {
            Models.Usuario respuesta = null;
            respuesta = sqlConnection.Query<Models.Usuario>($"SELECT * FROM usuarios WHERE token='{jwt}'").FirstOrDefault();
            if (respuesta != null && VerificarSesion(jwt))
            {
                return respuesta;
            }
            return respuesta;
        }
        public Models.Respuesta.InicioSesion IniciarSesion(Models.IniciarSesion iniciarSesion)
        {
            var respuesta = new Models.Respuesta.InicioSesion()
            {
                jwt = String.Empty
            };
            if (sqlConnection.Query<Models.Usuario>($"SELECT * FROM usuarios WHERE usuario='{iniciarSesion.usuario}' AND contrasenia='{iniciarSesion.contrasenia}'").Count() != 0)
            {
                respuesta.jwt = Guid.NewGuid().ToString();
                sqlConnection.Query($"UPDATE usuarios SET token='{respuesta.jwt}',fechaToken='{DateTime.Now.ToString()}',ultimaConexion='{DateTime.Now.ToString()}' WHERE usuario='{iniciarSesion.usuario}'");
                return respuesta;
            }
            return respuesta;
        }

        public bool VerificarSesion(string jwt)
        {
            var usuario = sqlConnection.Query<Models.Usuario>($"SELECT * FROM usuarios WHERE token='{jwt}'").FirstOrDefault();
            if(usuario != null)
            {
                TimeSpan tiempoTranscurrido = DateTime.Now - DateTime.Parse(usuario.fechaToken);
                if(tiempoTranscurrido.Minutes <= 5)
                {
                    sqlConnection.Query($"UPDATE usuarios SET fechaToken='{DateTime.Now.ToString()}' WHERE token='{jwt}'");
                    return true;
                }
            }
            return false;
        }
        public List<Models.Dispositivo> ObtenerDispositivos(string jwt)
        {
            Models.Usuario usuario = ObtenerUsuario(jwt);
            if(usuario != null)
            {          
                return sqlConnection.Query<Models.Dispositivo>($"SELECT * FROM dispositivos where usuario='{usuario.usuario}'").ToList();
            }
            return null;
        }
        public List<Models.TipoDispositivo> ObtenerTipoDispositivos()
        {
            return sqlConnection.Query<Models.TipoDispositivo>($"SELECT * FROM tipoDispositivos").ToList();
        }
        public List<Models.Dispositivo> ObtenerDispositivos()
        {
                return sqlConnection.Query<Models.Dispositivo>($"SELECT * FROM dispositivos").ToList();
        }


        public Models.Dispositivo? ObtenerDispositivo(string id)
        {
            return sqlConnection.Query<Models.Dispositivo>($"SELECT * FROM dispositivos WHERE idDispositivo='{id}'").FirstOrDefault();
        }


        public bool EliminarDispositivo(string id)
        {
            return sqlConnection.Execute($"DELETE FROM dispositivos WHERE idDispositivo='{id}'") == 0 ? false : true;
        }

        public bool AgregarDispositivo(Models.Dispositivo dispositivo)
        {
            string query = $@"INSERT INTO [dbo].[dispositivos] ([usuario],[nombreDispositivo],[idDispositivo],[descripcionDispositivo],[tipoDispositivo],[fechaRegistro],[estadoDispositivo],[fechaHeartBeat],[fechaEstado],[estadoConexion]) VALUES 
           ('{dispositivo.usuario}'
           ,'{dispositivo.nombreDispositivo}'
           ,'{dispositivo.idDispositivo}'
           ,'{dispositivo.descripcionDispositivo}'
           ,'{dispositivo.tipoDispositivo}'
           ,'{DateTime.Now.ToString()}'
           ,'Sin estado'
           ,'Sin fecha de heartbeat'
           ,'Sin fecha estado'
           ,'DESCONECTADO')";
            return sqlConnection.Execute(query) == 0 ? false : true;
        }


        public bool ActualizarDispositivo(Models.Dispositivo viejoDispositivo, Models.Dispositivo nuevoDispositivo)
        {
            string query = $@"UPDATE [dbo].[dispositivos] SET [nombreDispositivo] = '{nuevoDispositivo.nombreDispositivo}'
                              ,[idDispositivo] =  '{nuevoDispositivo.idDispositivo}'
                              ,[descripcionDispositivo] =  '{nuevoDispositivo.descripcionDispositivo}'
                              ,[tipoDispositivo] =  '{nuevoDispositivo.tipoDispositivo}'
                              ,[fechaRegistro] =  '{nuevoDispositivo.fechaRegistro}'
                              ,[estadoDispositivo] =  '{nuevoDispositivo.estadoDispositivo}'
                              ,[fechaHeartBeat] =  '{nuevoDispositivo.fechaHeartBeat}'
                              ,[fechaEstado] = '{nuevoDispositivo.fechaEstado}'  
                              ,[estadoConexion] = '{nuevoDispositivo.estadoConexion}' 
                                WHERE [idDispositivo] = '{viejoDispositivo.idDispositivo}'";
            return sqlConnection.Execute(query) == 0 ? false : true;
        }



        private void SqlConnection_StateChange(object sender, System.Data.StateChangeEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
