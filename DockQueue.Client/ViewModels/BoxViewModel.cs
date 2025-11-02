using System.ComponentModel.DataAnnotations;
using DockQueue.Application.DTOs;
using DockQueue.Client.Services;

namespace DockQueue.Client.ViewModels;

/// <summary>
/// Controla o estado da tela Boxes.
/// Responsável por carregar, adicionar e remover boxes.
/// </summary>
public class BoxViewModel
{
    private readonly BoxService _boxService;

    public BoxViewModel(BoxService boxService)
    {
        _boxService = boxService;
    }

    // Lista de boxes exibida na UI
    public List<BoxDto> Boxes { get; private set; } = new();

    // Formulário de criação
    public CreateBoxForm Form { get; set; } = new();

    public bool IsLoading { get; private set; }
    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// Carrega todos os boxes da API.
    /// </summary>
    public async Task LoadAsync()
    {
        try
        {
            IsLoading = true;
            Boxes = await _boxService.GetAllAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erro ao carregar boxes: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Cria um novo box via API e adiciona à lista local.
    /// </summary>
    public async Task AddBoxAsync()
    {
        try
        {
            // Monta o DTO conforme o backend espera
            var dto = new CreateBoxDto
            {
                Name = Form.Name.Trim(),
                Status = false,
                DriverId = null
            };

            var created = await _boxService.CreateAsync(dto);

            if (created != null)
                Boxes.Add(created);

            Form = new CreateBoxForm(); // limpa formulário
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erro ao adicionar box: {ex.Message}";
        }
    }

    /// <summary>
    /// Remove um box e atualiza a lista local.
    /// </summary>
    public async Task RemoveBoxAsync(BoxDto box)
    {
        try
        {
            var ok = await _boxService.DeleteAsync(box.Id);
            if (ok)
                Boxes.Remove(box);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erro ao remover box: {ex.Message}";
        }
    }

    /// <summary>
    /// Modelo de formulário com validações para o campo Name.
    /// </summary>
    public class CreateBoxForm
    {
        [Required(ErrorMessage = "Campo obrigatório.")]
        [MinLength(2, ErrorMessage = "Mínimo 2 caracteres.")]
        public string Name { get; set; } = string.Empty;
    }
}
