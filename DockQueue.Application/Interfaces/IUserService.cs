using DockQueue.Application.DTOs;

namespace DockQueue.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> AuthenticateAsync(LoginUserDto loginUserDto);
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(int id);
        Task<UserDto> CreateUserAsync(CreateUserDto createdUserDto);
    }
}
