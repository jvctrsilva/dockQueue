namespace DockQueue.Application.DTOs
{
    public class CreateUserDto
    {
        public string Name { get; set; } = null!;
        public string Number { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
