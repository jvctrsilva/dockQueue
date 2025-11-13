using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using DockQueue.Application.DTOs;

namespace DockQueue.Client.ViewModels;

public class LoginViewModel : INotifyPropertyChanged
{
    private string _email = string.Empty;
    private string _password = string.Empty;
    private bool _isLoading = false;
    private string _errorMessage = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email
    {
        get => _email;
        set
        {
            if (_email != value)
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
                ClearError();
            }
        }
    }

    // Senha é opcional para permitir primeiro login sem senha
    // A validação será feita no backend
    public string Password
    {
        get => _password;
        set
        {
            if (_password != value)
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
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

    public LoginUserDto ToDto()
    {
        return new LoginUserDto
        {
            Email = Email,
            Password = Password
        };
    }

    public void ClearError()
    {
        ErrorMessage = string.Empty;
    }

    public void SetError(string message)
    {
        ErrorMessage = message;
    }

    public void Reset()
    {
        Email = string.Empty;
        Password = string.Empty;
        ErrorMessage = string.Empty;
        IsLoading = false;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
