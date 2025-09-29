using DockQueue.Domain.Validation;
using System;

namespace DockQueue.Domain.Entities
{
    public class Box
    {
        public int Id { get; private set; } // auto incremento
        public string Name { get; private set; } = string.Empty;
        public bool Status { get; private set; } = false;
        public int? DriverId { get; private set; } // FK para ve�culo
        public Driver? Driver { get; private set; } // navega��o
        public DateTime CreatedAt { get; private set; }

        // Construtor
        public Box(string name, bool status, int? driverId, DateTime createdAt)
        {
            ValidateDomain(name, status, driverId, createdAt);
        }

        // Construtor com ID (para updates vindos do banco)
        public Box(int id, string name, bool status, int? driverId, DateTime createdAt)
        {
            DomainExceptionValidation.When(id < 0, "Id inv�lido.");
            Id = id;
            ValidateDomain(name, status, driverId, createdAt);
        }

        public void Update(string name, bool status, int? driverId, DateTime createdAt)
        {
            ValidateDomain(name, status, driverId, createdAt);
        }

        private void ValidateDomain(string name, bool status, int? driverId, DateTime createdAt)
        {
            DomainExceptionValidation.When(string.IsNullOrEmpty(name), "Nome do box � obrigat�rio");
            DomainExceptionValidation.When(name.Length > 100, "Nome inv�lido, m�ximo 100 caracteres");

            Name = name;
            Status = status;
            DriverId = driverId;
            CreatedAt = createdAt;
        }
    }
}
