namespace ATBapi.Models.DTOs
{
    public class EstadisticasDTO
    {
        public string TiempoPromedioDeEspera { get; set; } = null!;
        public string TiempoPromedioDeAtencion { get; set; } = null!;
        public List<CajaDatosDTO> DatosCaja { get; set; } = new();
    }

    public class CajaDatosDTO
    {
        public string NombreCaja { get; set; } = null!;
        public string NumeroAtendidos { get; set; } = null!;
        public string NumeroCancelados { get; set; } = null!;
    }
}
