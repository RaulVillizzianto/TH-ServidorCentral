using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AlarmaApi.Comunicacion.Models
{
    public class Notificacion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string? usuario { get; set; }
        public string? idDispositivo { get; set; }
        public string? fecha { get; set; }
        public string? descripcionDispositivo { get; set; }
        public string? nombreDispositivo { get; set; }
        public string? texto { get; set; }
        public string? titulo { get; set;}
        public int? evento { get; set; }
        public bool?   visto { get; set; }
        public bool?  entregado { get; set; }
        
    }
}
