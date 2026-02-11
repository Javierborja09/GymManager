namespace GymManager.Models.DTOs
{
    public class UsuarioDTO
    {
        public long usuario_id { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string rol { get; set; } = "Vendedor";
        public bool activo { get; set; }
    }
}