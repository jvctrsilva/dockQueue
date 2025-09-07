namespace DockQueue.Application.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Number { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public UserDto() { }

        public UserDto(Domain.Entities.User user)
        {
            Id = user.Id;
            Name = user.Name;
            Number = user.Number;
            Email = user.Email;
            Role = user.Role;
            CreatedAt = user.CreatedAt;
        }
    }
}
