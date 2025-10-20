using System.ComponentModel;
using DockQueue.Application.DTOs;

namespace DockQueue.ViewModels;

public class AuthViewModel : INotifyPropertyChanged
{
    private UserDto? _currentUser;
    private string _accessToken = string.Empty;
    private string _refreshToken = string.Empty;
    private bool _isAuthenticated = false;

    public UserDto? CurrentUser
    {
        get => _currentUser;
        set
        {
            if (_currentUser != value)
            {
                _currentUser = value;
                OnPropertyChanged(nameof(CurrentUser));
                OnPropertyChanged(nameof(IsAuthenticated));
            }
        }
    }

    public string AccessToken
    {
        get => _accessToken;
        set
        {
            if (_accessToken != value)
            {
                _accessToken = value;
                OnPropertyChanged(nameof(AccessToken));
            }
        }
    }

    public string RefreshToken
    {
        get => _refreshToken;
        set
        {
            if (_refreshToken != value)
            {
                _refreshToken = value;
                OnPropertyChanged(nameof(RefreshToken));
            }
        }
    }

    public bool IsAuthenticated
    {
        get => _isAuthenticated && CurrentUser != null && !string.IsNullOrEmpty(AccessToken);
        set
        {
            if (_isAuthenticated != value)
            {
                _isAuthenticated = value;
                OnPropertyChanged(nameof(IsAuthenticated));
            }
        }
    }

    public void SetAuthData(AuthResponseDto authResponse)
    {
        CurrentUser = authResponse.User;
        AccessToken = authResponse.Token;
        RefreshToken = authResponse.RefreshToken;
        IsAuthenticated = true;
    }

    public void ClearAuthData()
    {
        CurrentUser = null;
        AccessToken = string.Empty;
        RefreshToken = string.Empty;
        IsAuthenticated = false;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
