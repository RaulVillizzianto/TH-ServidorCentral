namespace AlarmaApi.Comunicacion.Models
{
    public class Solicitud
    {
        public string idDispositivo { get; set; }
        public string nombreDispositivo { get; set; }
        public string? nuevoNombreDispositivo { get; set; }
        public string? descripcionDispositivo { get; set; }
        public string? tipoDispositivo { get; set; }
        public string comandoDispositivo { get; set; }
        public string jwt { get; set; }

    }
}
