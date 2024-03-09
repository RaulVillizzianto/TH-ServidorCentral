using MongoDB.Driver;
using MongoDB.Bson;
using AlarmaApi.Comunicacion.Models;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization;

namespace AlarmaApi.Comunicacion
{
    internal class MongoDB
    {
        private const string _connectionString = "mongodb+srv://raulvillizzianto:79olheprkasBuWvm@cluster0.b6129yb.mongodb.net/";
        private MongoClient _client = null;
        private MongoDB()
        {
            if (_connectionString == null)
            {
                Console.WriteLine("You must set your 'MONGODB_URI' environment variable. To learn how to set it, see https://www.mongodb.com/docs/drivers/csharp/current/quick-start/#set-your-connection-string");
                Environment.Exit(0);
            }
            _client = new MongoClient(_connectionString);

        }
        private static MongoDB _instance;
        public static MongoDB GetInstance()
        {
            if (_instance == null)
            {
                _instance = new MongoDB();
            }
            return _instance;
        }
        //testing
        private void InsertarNotificacion(Notificacion notificacion)
        {
            notificacion.id = ObjectId.GenerateNewId().ToString();

            _client.GetDatabase("Apps").GetCollection<BsonDocument>("Notificaciones").InsertOne(notificacion.ToBsonDocument());
        }

        public void InsertarNotificacionSensorMovimientoActivado(Dispositivo dispositivo)
        {
            Notificacion notificacion = new Notificacion();
            notificacion.id = ObjectId.GenerateNewId().ToString();
            notificacion.titulo = "Activación del sensor de movimiento";
            notificacion.texto = $"Se activó el {dispositivo.nombreDispositivo} en {dispositivo.descripcionDispositivo} a las {dispositivo.fechaEstado}";
            notificacion.entregado = false;
            notificacion.visto = false;
            notificacion.fecha = DateTime.Now.ToString();
            notificacion.nombreDispositivo = dispositivo.nombreDispositivo;
            notificacion.descripcionDispositivo = dispositivo.descripcionDispositivo;
            notificacion.idDispositivo = dispositivo.idDispositivo;
            notificacion.usuario = dispositivo.usuario;
            _client.GetDatabase("Apps").GetCollection<BsonDocument>("Notificaciones").InsertOne(notificacion.ToBsonDocument());
        }


        public dynamic MarcarNotificacionComoEntregada(Notificacion notificacion)
        {
            if(notificacion.id != null)
            {

                var collection = _client.GetDatabase("Apps").GetCollection<Notificacion>("Notificaciones");
                var filter2 = Builders<Notificacion>.Filter.Eq(r => r.id, notificacion.id);
                var update = Builders<Notificacion>.Update.Set("entregado",true);
                var result = collection.UpdateOne( filter2, update);
                return result.ModifiedCount > 0 ? true : null;
            }
            return null;
        }

        public dynamic MarcarNotificacionComoVista(Notificacion notificacion)
        {
            if (notificacion.id != null)
            {

                var collection = _client.GetDatabase("Apps").GetCollection<Notificacion>("Notificaciones");
                var filter2 = Builders<Notificacion>.Filter.Eq(r => r.id, notificacion.id);
                var update = Builders<Notificacion>.Update.Set("visto", true);
                var result = collection.UpdateOne(filter2, update);
                return result.ModifiedCount > 0 ? true : null;
            }
            return null;
        }


        public List<Notificacion> ObtenerNotificaciones(string jwt)
        {
            SQL sql = SQL.GetInstance();
            if(sql.VerificarSesion(jwt))
            {
                Usuario usuario = sql.ObtenerUsuario(jwt);
                var collection = _client.GetDatabase("Apps").GetCollection<Notificacion>("Notificaciones");
                    
                var filter = Builders<Notificacion>.Filter.Eq(r => r.usuario, usuario.usuario);
                var cursor = collection.Find(filter).ToCursor().ToList().OrderByDescending(r => r.fecha);
                
                return BsonSerializer.Deserialize<List<Notificacion>>(cursor.ToJson());
            }
            return null;
        }
        public Notificacion ObtenerNotificacion(string jwt, string id)
        {
            SQL sql = SQL.GetInstance();
            if (sql.VerificarSesion(jwt))
            {
                Usuario usuario = sql.ObtenerUsuario(jwt);
                var collection = _client.GetDatabase("Apps").GetCollection<Notificacion>("Notificaciones");

                var filter = Builders<Notificacion>.Filter.Eq(r => r.usuario, usuario.usuario);
                var filter2 = Builders<Notificacion>.Filter.Eq(r => r.id, ObjectId.Parse(id).ToString());
                //var filter = Builders<BsonDocument>.Filter.In(p => p.Id, productObjectIDs);
                var cursor = collection.Find(filter & filter2).ToCursor().FirstOrDefault();
                if(cursor != null)
                {
                    return BsonSerializer.Deserialize<Notificacion>(cursor.ToJson());
                }
            }
            return null;
        }
        public List<Notificacion> ObtenerNuevasNotificaciones(string jwt)
        {
            SQL sql = SQL.GetInstance();
            if (sql.VerificarSesion(jwt))
            {
                Usuario usuario = sql.ObtenerUsuario(jwt);
                var collection = _client.GetDatabase("Apps").GetCollection<Notificacion>("Notificaciones");

                var filter = Builders<Notificacion>.Filter.Eq(r => r.usuario, usuario.usuario);
                var filter2 = Builders<Notificacion>.Filter.Eq(r => r.visto,false);
                var cursor = collection.Find(filter & filter2).ToCursor().ToList().OrderByDescending(r => r.fecha);

                return BsonSerializer.Deserialize<List<Notificacion>>(cursor.ToJson());
            }
            return null;
        }
        public List<Notificacion> ObtenerNotificacionesNoEntregadas(string jwt)
        {
            SQL sql = SQL.GetInstance();
            if (sql.VerificarSesion(jwt))
            {
                Usuario usuario = sql.ObtenerUsuario(jwt);
                var collection = _client.GetDatabase("Apps").GetCollection<Notificacion>("Notificaciones");

                var filter = Builders<Notificacion>.Filter.Eq(r => r.usuario, usuario.usuario);
                var filter2 = Builders<Notificacion>.Filter.Eq(r => r.visto, false);
                var filter3 = Builders<Notificacion>.Filter.Eq(r => r.entregado, false);

                var cursor = collection.Find(filter & filter2 & filter3).ToCursor().ToList().OrderByDescending(r => r.fecha);

                return BsonSerializer.Deserialize<List<Notificacion>>(cursor.ToJson());
            }
            return null;
        }
    }
}
