namespace ATBapi.Models.DTOs
{
    public class UserCompleteDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string? Contraseña { get; set; }
        public int IdCaja { get; set; }
        public int IdRol { get; set; }
    }
}
