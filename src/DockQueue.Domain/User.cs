namespace DockQueue.Domain
{
    public class User
    {
        public int Id { get; set; }             // PK
        public string Name { get; set; }        // nome completo do usuário
        public string Number { get; set; }    // username do usuário, utilizar numero do telefone
        public string Email { get; set; }       // email do usuário
        public string Password { get; set; }    // senha (hash)
        public string Role { get; set; }        // "Admin" ou "Operator"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
