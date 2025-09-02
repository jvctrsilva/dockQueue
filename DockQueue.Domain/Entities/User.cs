using DockQueue.Domain.Validation;

namespace DockQueue.Domain.Entities;

public class User
{
    public int Id { get; private set; }
    public string Name { get; private set; } 
    public string Number { get; private set; } 
    public string Email { get; private set; }
    public string Password { get; private set; }
    public string Role { get; private set; }
    public DateTime CreatedAt { get; private set; }

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
    public void ValidateDomain(string name, string number, string email,
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
