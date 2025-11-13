using System.ComponentModel.DataAnnotations;
using DockQueue.Application.DTOs;
using DockQueue.Client.Services;

namespace DockQueue.Client.ViewModels.Box;

public class BoxEditViewModel
{
    private readonly BoxService _service;
    public BoxEditViewModel(BoxService service) => _service = service;

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
                if (dto == null) throw new InvalidOperationException("Box não encontrado.");
                Model = new FormModel
                {
                    Name = dto.Name,
                    Status = dto.Status,
                    DriverId = dto.DriverId,
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
                var create = new CreateBoxDto
                {
                    Name = Model.Name.Trim(),
                };
                await _service.CreateAsync(create);
            }
            else
            {
                var update = new UpdateBoxDto
                {
                    Id = Id.Value,
                    Name = Model.Name.Trim(),
                    Status = Model.Status,
                    DriverId = Model.DriverId,
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
        public string Name { get; set; } = string.Empty;
        public bool Status { get; set; } = false; // default
        public int? DriverId { get; set; } = null; // default
    }
}
