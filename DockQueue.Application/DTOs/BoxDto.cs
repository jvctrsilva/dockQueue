namespace DockQueue.Application.DTOs
{
    public class BoxDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Status { get; set; }
        public int? DriverId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateBoxDto
    {
        public string Name { get; set; } = string.Empty;
        public bool Status { get; set; } = false; // default
        public int? DriverId { get; set; } = null; // default
    }

    public class UpdateBoxDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Status { get; set; } 
        public int? DriverId { get; set; }
        // CreatedAt não precisa, usamos o da entidade
    }
}
