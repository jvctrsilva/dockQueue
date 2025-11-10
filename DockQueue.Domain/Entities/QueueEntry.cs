using DockQueue.Domain.Enums;

namespace DockQueue.Domain.Entities
{
    public class QueueEntry
    {
        public int Id { get; set; }

        public QueueType Type { get; set; }  // Carregamento / Descarregamento

        public int DriverId { get; set; }
        public Driver Driver { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Posição na fila dentro daquele tipo (carregamento ou descarregamento).
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Se quiser priorizar alguns motoristas (menor = mais prioridade).
        /// </summary>
        public int? Priority { get; set; }

        // Status atual (já aproveitando sua entidade Status)
        public int StatusId { get; set; }
        public Status Status { get; set; } = null!;

        // Box atribuído para operação (pode ser null enquanto estiver aguardando)
        public int? BoxId { get; set; }
        public Box? Box { get; set; }

        // Operador que fez a última alteração (opcional)
        public int? LastUpdatedByUserId { get; set; }
        public User? LastUpdatedByUser { get; set; }

        // construtor helper (não é base entity, é só pra ajudar na criação)
        public QueueEntry(
            QueueType type,
            int driverId,
            int statusId,
            int position,
            int? priority = null)
        {
            Type = type;
            DriverId = driverId;
            StatusId = statusId;
            Position = position;
            Priority = priority;
            CreatedAt = DateTime.UtcNow;
        }

        // construtor vazio pro EF
        public QueueEntry() { }

        public void UpdateStatus(int newStatusId, int? userId = null)
        {
            StatusId = newStatusId;
            LastUpdatedByUserId = userId;
        }

        public void AssignBox(int boxId, int? userId = null)
        {
            BoxId = boxId;
            LastUpdatedByUserId = userId;
        }

        public void UpdatePosition(int newPosition)
        {
            Position = newPosition;
        }
    }
}
