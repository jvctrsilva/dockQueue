using DockQueue.Domain.Validation;
using DockQueue.Domain.ValueObjects;

namespace DockQueue.Domain.Entities
{
    // Agregado raiz para permissões de um operador específico (UserId).
    public class OperatorPermissions
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }           // FK para User
        public Screen AllowedScreens { get; private set; } // flags de telas/menus liberados

        // Coleções simples para Status e Boxes liberados
        public ICollection<OperatorStatusPermission> AllowedStatuses { get; private set; } = new List<OperatorStatusPermission>();
        public ICollection<OperatorBoxPermission> AllowedBoxes { get; private set; } = new List<OperatorBoxPermission>();

        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        protected OperatorPermissions() { }

        public OperatorPermissions(int userId, Screen allowedScreens, DateTime nowUtc)
        {
            DomainExceptionValidation.When(userId <= 0, "UserId inválido");
            UserId = userId;
            AllowedScreens = allowedScreens;
            CreatedAt = nowUtc;
            UpdatedAt = nowUtc;
        }

        // Atualiza apenas o conjunto de telas; listas são gerenciadas por métodos dedicados
        public void UpdateScreens(Screen screens, DateTime nowUtc)
        {
            AllowedScreens = screens;
            UpdatedAt = nowUtc;
        }

        public void SetStatuses(IEnumerable<int> statusIds, DateTime nowUtc)
        {
            AllowedStatuses.Clear();
            foreach (var id in statusIds.Distinct())
                AllowedStatuses.Add(new OperatorStatusPermission(UserId, id));
            UpdatedAt = nowUtc;
        }

        public void SetBoxes(IEnumerable<int> boxIds, DateTime nowUtc)
        {
            AllowedBoxes.Clear();
            foreach (var id in boxIds.Distinct())
                AllowedBoxes.Add(new OperatorBoxPermission(UserId, id));
            UpdatedAt = nowUtc;
        }
    }

    // Item da coleção: status permitido para o operador
    public class OperatorStatusPermission
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }   // redundante no item para facilitar queries (índice)
        public int StatusId { get; private set; }

        protected OperatorStatusPermission() { }
        public OperatorStatusPermission(int userId, int statusId)
        {
            UserId = userId;
            StatusId = statusId;
        }
    }

    // Item da coleção: box permitido para o operador
    public class OperatorBoxPermission
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public int BoxId { get; private set; }

        protected OperatorBoxPermission() { }
        public OperatorBoxPermission(int userId, int boxId)
        {
            UserId = userId;
            BoxId = boxId;
        }
    }
}
