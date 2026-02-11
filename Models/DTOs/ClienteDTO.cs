namespace GymManager.Models.DTOs
{
    public class ClienteDTO
    {
        public long cliente_id { get; set; }
        public string dni { get; set; } = string.Empty;
        public string NombreCompleto => $"{nombre} {apellido}";
        public string nombre { get; set; } = string.Empty;
        public string apellido { get; set; } = string.Empty;
        public string estado { get; set; } = "Activo";
    }
}