namespace AlarmaApi.Comunicacion.Models
{
    public class Usuario
    {
        public string usuario { get; set; } 
        public string contrasenia { get; set; }

        public string nombre { get; set; }
        public string apellido { get; set; }
        public string ultimaConexion { get; set; }
        public string jwt { get; set; }
        public string fechaToken { get; set; }

    }
}
