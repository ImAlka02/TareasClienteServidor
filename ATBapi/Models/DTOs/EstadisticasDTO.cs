namespace ATBapi.Models.DTOs
{
    public class EstadisticasDTO
    {
        public string TiempoPromedioDeEspera { get; set; } = null!;
        public string TiempoPromedioDeAtencion { get; set; } = null!;
        public Dictionary<string, int> PersonasAtendidasPorCaja { get; set; } = new();
    }


}
