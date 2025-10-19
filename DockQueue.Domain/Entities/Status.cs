using DockQueue.Domain.Validation;

namespace DockQueue.Domain.Entities;

public class Status
{
    public int Id { get; private set; }
    public string Code { get; private set; } = default!;   // ex.: CHEGADA, PATIO_1, DOCA_1
    public string Name { get; private set; } = default!;   // ex.: Chegada, Pátio 1, Doca 1
    public string? Description { get; private set; }
    public int DisplayOrder { get; private set; }          // ordenação no fluxo
    public bool IsDefault { get; private set; }            // status inicial (apenas 1)
    public bool IsTerminal { get; private set; }           // status que encerra o fluxo (fila concluída)
    public bool Active { get; private set; }               // soft-disable sem apagar
    public DateTime CreatedAt { get; private set; }

    protected Status() { }

    public Status(string code, string name, string? description, int displayOrder,
                  bool isDefault, bool isTerminal, bool active, DateTime createdAt)
    {
        ValidateDomain(code, name, description, displayOrder, isDefault, isTerminal, active, createdAt);
    }

    public Status(int id, string code, string name, string? description, int displayOrder,
                  bool isDefault, bool isTerminal, bool active, DateTime createdAt)
    {
        Id = id;
        ValidateDomain(code, name, description, displayOrder, isDefault, isTerminal, active, createdAt);
    }

    public void Update(string code, string name, string? description, int displayOrder,
                       bool isDefault, bool isTerminal, bool active)
    {
        ValidateDomain(code, name, description, displayOrder, isDefault, isTerminal, active, CreatedAt);
    }

    private void ValidateDomain(string code, string name, string? description, int displayOrder,
                                bool isDefault, bool isTerminal, bool active, DateTime createdAt)
    {
        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(code), "Código do status é obrigatório");
        DomainExceptionValidation.When(code.Length > 50, "Código do status muito longo (máx 50)");
        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(name), "Nome do status é obrigatório");
        DomainExceptionValidation.When(name.Length > 100, "Nome do status muito longo (máx 100)");
        DomainExceptionValidation.When(displayOrder < 0, "DisplayOrder deve ser >= 0");

        Code = code.Trim().ToUpperInvariant(); // normaliza
        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        DisplayOrder = displayOrder;
        IsDefault = isDefault;
        IsTerminal = isTerminal;
        Active = active;
        CreatedAt = createdAt;
    }
}
