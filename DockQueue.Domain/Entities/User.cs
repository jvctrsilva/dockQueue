using DockQueue.Domain.Validation;

namespace DockQueue.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Number { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Role { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    // EF Core precisa de um ctor sem parâmetros
    protected User() { }

    public User(string name, string number, string email,
                string password, string role, DateTime createdAt)
    {
        ValidateDomain(name, number, email, password, role, createdAt);
    }

    public User(int id, string name, string number, string email,
                string password, string role, DateTime createdAt)
    {
        Id = id;
        ValidateDomain(name, number, email, password, role, createdAt);
    }

    public void Update(string name, string number, string email,
                       string password, string role, DateTime createdAt)
    {
        ValidateDomain(name, number, email, password, role, createdAt);
    }

    private void ValidateDomain(string name, string number, string email,
                                string password, string role, DateTime createdAt)
    {
        DomainExceptionValidation.When(name.Length > 200, "Nome inválido, muito longo, máximo 200 caracteres");

        Name = name;
        Number = number;
        Email = email;
        Password = password;
        Role = role;
        CreatedAt = createdAt;
    }
}
