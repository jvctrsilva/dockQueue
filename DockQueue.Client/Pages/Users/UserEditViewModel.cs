using DockQueue.Application.DTOs;
using DockQueue.Client.Services;

namespace DockQueue.Client.Pages.Users;
public class UserEditViewModel
{
    private readonly UserService _service;
    public UserEditViewModel(UserService service) => _service = service;
    public bool IsLoading { get; private set; }
    public bool IsSaving { get; private set; }
    public string? Error { get; private set; }
    public int? Id { get; private set; }
    public FormModel Model { get; private set; } = new();

    public async Task LoadAsync(int? id)
    {
        try
        {
            IsLoading = true;
            Error = null;
            Id = id;

            if (id is null) 
            {
                Model = new FormModel();
            }
            else
            {
                var dto = await _service.GetByIdAsync(id.Value);
                if (dto == null) throw new InvalidOperationException("Usuário não encontrado.");
                Model = new FormModel
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Number = dto.Number,
                    Email = dto.Email,
                    Role = dto.Role,
                    CreatedAt = dto.CreatedAt
                };
            }
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally { IsLoading = false; }
    }

    public async Task<bool> SaveAsync()
    {
        try
        {
            IsSaving = true;
            Error = null;

            if (Id is null)
            {
                var create = new CreateUserDto
                {
                    Name = Model.Name.Trim(),
                    Number = Model.Number.Trim(),
                    Email = Model.Email.Trim(),
                    Password = "Default",
                    Role = Model.Role.Trim()
                };
                await _service.CreateAsync(create);
            }
            else
            {
                var update = new UpdateUserDto
                {
                    Name = Model.Name.Trim(),
                    Number = Model.Number.Trim(),
                    Role = Model.Role.Trim()
                };
                await _service.UpdateAsync(Id.Value, update);
            }
            return true;
        }
        catch (Exception ex)
        {
            Error = ex.Message;
            return false;
        }
        finally { IsSaving = false; }
    }

    public class FormModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
