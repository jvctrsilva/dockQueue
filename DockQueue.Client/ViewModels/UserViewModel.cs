using DockQueue.Application.DTOs;
using DockQueue.Client.Services;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DockQueue.Client.ViewModels;

/// Controla o estado da tela /operadores (lista + formulário).
public class UserViewModel
{
    private readonly UserService _service;

    public UserViewModel(UserService userService)
    {
        _service = userService;
    }

    public List<UserDto> Users { get; private set; } = new();
    public CreateUserForm Form { get; set; } = new();
    public bool IsLoading { get; private set; }
    public string? ErrorMessage { get; private set; }

    public async Task LoadAsync()
    {
        try
        {
            IsLoading = true;
            Users = await _service.GetAllAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erro ao carregar pessoas: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task AddUserAsync()
    {
        try
        {
            var dto = new CreateUserDto
            {
                Name = Form.Nome.Trim(),
                Number = Form.Number?.Trim() ?? string.Empty,   // se quiser coletar depois
                Email = Form.Login.Trim(),                      // "Login" = Email na sua API
                Password = Form.Senha.Trim(),
                Role = Form.Role,                               // "ADMIN" ou "OPERADOR"
            };

            var created = await _service.CreateAsync(dto);
            if (created != null) Users.Add(created);

            Form = new CreateUserForm();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erro ao adicionar: {ex.Message}";
        }
    }



    /// Remove um user e atualiza a lista local.
    public async Task RemoveUserAsync(UserDto user)
    {
        try
        {
            var ok = await _service.DeleteAsync(user.Id);
            if (ok) Users.Remove(user);
            else ErrorMessage = "Falha ao remover usuário.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erro ao remover: {ex.Message}";
        }
    }


    /// <summary>
    /// Modelo de formulário com validações para o campo Name.
    /// </summary>
    public class CreateUserForm
    {
        [Required(ErrorMessage = "Nome obrigatório")]
        [MinLength(2, ErrorMessage = "Mínimo 2 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Login (email) obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha obrigatória")]
        [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
        public string Senha { get; set; } = string.Empty;

        public string Role { get; set; } = "OPERADOR"; // default
        public string? Number { get; set; }            // opcional (se quiser capturar)
    }
}
