using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DockQueue.Application.DTOs;
using DockQueue.Client.Services;

namespace DockQueue.Client.ViewModels;

public class SetPasswordViewModel : INotifyPropertyChanged
{
    private readonly UserService _userService;
    private readonly AuthViewModel _authViewModel;
    
    private string _newPassword = string.Empty;
    private string _confirmPassword = string.Empty;
    private bool _isLoading = false;
    private string _errorMessage = string.Empty;

    public SetPasswordViewModel(UserService userService, AuthViewModel authViewModel)
    {
        _userService = userService;
        _authViewModel = authViewModel;
    }

    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Senha deve ter pelo menos 6 caracteres")]
    public string NewPassword
    {
        get => _newPassword;
        set
        {
            if (_newPassword != value)
            {
                _newPassword = value;
                OnPropertyChanged(nameof(NewPassword));
                ClearError();
            }
        }
    }

    [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
    [Compare(nameof(NewPassword), ErrorMessage = "As senhas não coincidem")]
    public string ConfirmPassword
    {
        get => _confirmPassword;
        set
        {
            if (_confirmPassword != value)
            {
                _confirmPassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));
                ClearError();
            }
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (_errorMessage != value)
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
    }

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public void ClearError()
    {
        ErrorMessage = string.Empty;
    }

    public void SetError(string message)
    {
        ErrorMessage = message;
    }

    public async Task<bool> SavePasswordAsync()
    {
        try
        {
            IsLoading = true;
            ClearError();

            if (_authViewModel.CurrentUser == null)
            {
                SetError("Usuário não autenticado.");
                return false;
            }

            if (NewPassword != ConfirmPassword)
            {
                SetError("As senhas não coincidem.");
                return false;
            }

            var updatePasswordDto = new UpdatePasswordDto
            {
                NewPassword = NewPassword
            };

            var success = await _userService.UpdatePasswordAsync(_authViewModel.CurrentUser.Id, updatePasswordDto);

            if (!success)
            {
                SetError("Erro ao atualizar senha. Tente novamente.");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            SetError($"Erro ao salvar senha: {ex.Message}");
            return false;
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void Reset()
    {
        NewPassword = string.Empty;
        ConfirmPassword = string.Empty;
        ErrorMessage = string.Empty;
        IsLoading = false;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

