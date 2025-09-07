using DockQueue.Application.DTOs;
using DockQueue.Application.Interfaces;
using DockQueue.Application.Security;
using DockQueue.Domain.Entities;
using DockQueue.Domain.Interfaces;

namespace DockQueue.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => new UserDto(u)).ToList();
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return new UserDto(user);
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = new User(
                createUserDto.Name,
                createUserDto.Number,
                createUserDto.Email,
                PasswordHasher.Hash(createUserDto.Password),
                createUserDto.Role,
                DateTime.UtcNow
            );

            var createdUser = await _userRepository.AddAsync(user);
            return new UserDto(createdUser);
        }
    }
}
