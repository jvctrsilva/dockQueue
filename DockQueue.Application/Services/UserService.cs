using System.Security.Claims;
using System.Linq;
using DockQueue.Application.DTOs;
using DockQueue.Application.Interfaces;
using DockQueue.Domain.Entities;
using DockQueue.Domain.Interfaces;
using DockQueue.Application.Security;


namespace DockQueue.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly ITokenGenerator _tokenGen;

        public UserService(IUserRepository repo, ITokenGenerator tokenGen)
        {
            _repo = repo;
            _tokenGen = tokenGen;
        }

        // ========= Autenticação / CRUD =========

        public async Task<UserDto?> AuthenticateAsync(LoginUserDto loginUserDto)
        {
            var user = await _repo.GetByEmailAsync(loginUserDto.Email);
            if (user is null) return null;

            if (!PasswordHasher.Verify(loginUserDto.Password, user.Password))
                return null;

            return Map(user);
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _repo.GetAllAsync();
            return users.Select(Map).ToList();
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _repo.GetByIdAsync(id);
            return Map(user);
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createdUserDto)
        {
            var hashed = PasswordHasher.Hash(createdUserDto.Password);

            var entity = new User(
                createdUserDto.Name,
                createdUserDto.Number,
                createdUserDto.Email,
                hashed,
                createdUserDto.Role,
                DateTime.UtcNow
            );

            entity = await _repo.AddAsync(entity);
            return Map(entity);
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var user = await _repo.GetByEmailAsync(email);
            return user is null ? null : Map(user);
        }

        public async Task<bool> UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime? expiry)
        {
            var user = await _repo.GetByIdAsync(userId);
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = expiry;

            await _repo.UpdateAsync(user);
            return true;
        }

        // ========= Refresh Token =========

        public async Task<(string accessToken, string refreshToken)?> RotateRefreshTokenAsync(string refreshToken, string accessToken)
        {
            // 1) extrai principal do access token (mesmo expirado)
            ClaimsPrincipal principal;
            try
            {
                principal = _tokenGen.GetPrincipalFromExpiredToken(accessToken);
            }
            catch
            {
                return null;
            }

            // pega e-mail de forma resiliente
            var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
                     ?? principal.Claims.FirstOrDefault(c => c.Type == "email")?.Value
                     ?? principal.Identity?.Name;

            if (string.IsNullOrWhiteSpace(email))
                return null;

            // 2) carrega usuário e valida refresh atual
            var user = await _repo.GetByEmailAsync(email);
            if (user is null) return null;

            var tokenOk = user.RefreshToken == refreshToken
                          && user.RefreshTokenExpiryTime.HasValue
                          && user.RefreshTokenExpiryTime.Value > DateTime.UtcNow;

            if (!tokenOk) return null;

            // 3) gera novos tokens (rotação do refresh)
            var newAccess = _tokenGen.GenerateAccessToken(user.Id.ToString(), user.Email, user.Role);
            var (newRefresh, expiry) = _tokenGen.GenerateRefreshToken();

            user.RefreshToken = newRefresh;
            user.RefreshTokenExpiryTime = expiry;

            await _repo.UpdateAsync(user);

            return (newAccess, newRefresh);
        }

        // ========= Helpers =========

        private static UserDto Map(User u) => new()
        {
            Id = u.Id,
            Name = u.Name,
            Number = u.Number,
            Email = u.Email,
            Role = u.Role,
            CreatedAt = u.CreatedAt
        };
    }
}
