using System.ComponentModel.DataAnnotations;
using DockQueue.Application.DTOs;
using DockQueue.Client.Services;

namespace DockQueue.Client.ViewModels;

public class StatusEditViewModel
{
    private readonly StatusesService _service;
    public StatusEditViewModel(StatusesService service) => _service = service;

    public bool IsLoading { get; private set; }
    public bool IsSaving { get; private set; }
    public string? Error { get; private set; }

    // id nulo => criando
    public int? Id { get; private set; }

    // Form model (espelha Create/Update DTOs)
    public FormModel Model { get; private set; } = new();

    public async Task LoadAsync(int? id)
    {
        try
        {
            IsLoading = true;
            Error = null;
            Id = id;

            if (id is null) // novo
            {
                Model = new FormModel(); // defaults
            }
            else
            {
                var dto = await _service.GetByIdAsync(id.Value);
                if (dto == null) throw new InvalidOperationException("Status não encontrado.");
                Model = new FormModel
                {
                    Code = dto.Code,
                    Name = dto.Name,
                    Description = dto.Description,
                    DisplayOrder = dto.DisplayOrder,
                    IsDefault = dto.IsDefault,
                    IsTerminal = dto.IsTerminal,
                    Active = dto.Active
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
                var create = new CreateStatusDto
                {
                    Code = Model.Code.Trim(),
                    Name = Model.Name.Trim(),
                    Description = Model.Description,
                    DisplayOrder = Model.DisplayOrder,
                    IsDefault = Model.IsDefault,
                    IsTerminal = Model.IsTerminal,
                    Active = Model.Active
                };
                await _service.CreateAsync(create);
            }
            else
            {
                var update = new UpdateStatusDto
                {
                    Code = Model.Code.Trim(),
                    Name = Model.Name.Trim(),
                    Description = Model.Description,
                    DisplayOrder = Model.DisplayOrder,
                    IsDefault = Model.IsDefault,
                    IsTerminal = Model.IsTerminal,
                    Active = Model.Active
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
        [Required, MinLength(2)]
        public string Code { get; set; } = string.Empty;

        [Required, MinLength(2)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Range(0, int.MaxValue)]
        public int DisplayOrder { get; set; } = 0;

        public bool IsDefault { get; set; } = false;
        public bool IsTerminal { get; set; } = false;
        public bool Active { get; set; } = true;
    }
}
