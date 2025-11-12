using DockQueue.Application.DTOs;

namespace DockQueue.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> AuthenticateAsync(LoginUserDto loginUserDto);
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(int id);
        Task<UserDto> CreateUserAsync(CreateUserDto createdUserDto);
        Task<UserDto?> GetByEmailAsync(string email);
        Task UpdateUserAsync(int id, UpdateUserDto updateUserDto);
        Task DeleteUserAsync(int id);
        Task<bool> UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime? expiry);
        Task<(string accessToken, string refreshToken)?> RotateRefreshTokenAsync(string refreshToken, string accessToken);
    }
}
