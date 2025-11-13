namespace DockQueue.Domain.Entities
{
    public class Driver
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        // CPF ou documento que você for usar
        public string DocumentNumber { get; set; } = string.Empty;

        // Placa do veículo principal
        public string VehiclePlate { get; set; } = string.Empty;

        // Se quiser, depois dá pra adicionar telefone, empresa, etc.
    }
}
