using Microsoft.JSInterop;

namespace DockQueue.Services.UI;

public class AppState
{
    public string ColorTheme { get; set; } = "dark";
    public string Direction { get; set; } = "ltr";
    public string NavigationStyles { get; set; } = "vertical";
    public string MenuStyles { get; set; } = "";
    public string LayoutStyles { get; set; } = "default-menu";
    public string PageStyles { get; set; } = "regular";
    public string WidthStyles { get; set; } = "fullwidth";
    public string MenuPosition { get; set; } = "fixed";
    public string HeaderPosition { get; set; } = "fixed";
    public string MenuColor { get; set; } = "dark";
    public string HeaderColor { get; set; } = "dark";
    public string ThemePrimary { get; set; } = "";
    public string ThemeBackground { get; set; } = "";
    public string ThemeBackground1 { get; set; } = "";
    public string BackgroundImage { get; set; } = "";


    public MainMenuItems? currentItem { get; set; } = null;


    public bool IsDifferentFrom(AppState other)
    {
        return ColorTheme != other.ColorTheme ||
               Direction != other.Direction ||
               NavigationStyles != other.NavigationStyles ||
               MenuStyles != other.MenuStyles ||
               LayoutStyles != other.LayoutStyles ||
               PageStyles != other.PageStyles ||
               WidthStyles != other.WidthStyles ||
               MenuPosition != other.MenuPosition ||
               HeaderPosition != other.HeaderPosition ||
               MenuColor != other.MenuColor ||
               HeaderColor != other.HeaderColor ||
               ThemePrimary != other.ThemePrimary ||
               ThemeBackground != other.ThemeBackground ||
               ThemeBackground1 != other.ThemeBackground1 ||
               BackgroundImage != other.BackgroundImage ||
               (currentItem != null ? !currentItem.Equals(other.currentItem) : other.currentItem != null);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        AppState other = (AppState)obj;


        return ColorTheme == other.ColorTheme &&
               Direction == other.Direction &&
               NavigationStyles == other.NavigationStyles &&
               MenuStyles == other.MenuStyles &&
               LayoutStyles == other.LayoutStyles &&
               PageStyles == other.PageStyles &&
               WidthStyles == other.WidthStyles &&
               MenuPosition == other.MenuPosition &&
               HeaderPosition == other.HeaderPosition &&
               MenuColor == other.MenuColor &&
               HeaderColor == other.HeaderColor &&
               ThemePrimary == other.ThemePrimary &&
               ThemeBackground == other.ThemeBackground &&
               ThemeBackground1 == other.ThemeBackground1 &&
               BackgroundImage == other.BackgroundImage &&
               Equals(currentItem, other.currentItem);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public async Task InitializeFromSession(AppState sessionState, SessionService _sessionService)
    {
        var _currentState = new AppState();
        var stored = await _sessionService.GetInitalAppStateFromSession();
        if (stored != null && _currentState.IsDifferentFrom(stored))
        {
            ColorTheme = ColorTheme;
            Direction = Direction;
            NavigationStyles = NavigationStyles;
            MenuStyles = MenuStyles;
            LayoutStyles = LayoutStyles;
            PageStyles = PageStyles;
            WidthStyles = WidthStyles;
            MenuPosition = MenuPosition;
            HeaderPosition = HeaderPosition;
            MenuColor = MenuColor;
            HeaderColor = HeaderColor;
            ThemePrimary = ThemePrimary;
            ThemeBackground = ThemeBackground;
            ThemeBackground1 = ThemeBackground1;
            BackgroundImage = BackgroundImage;
            await _sessionService.SetInitalAppStateToSession(_currentState);
        }
        else if (sessionState != null)
        {
            ColorTheme = sessionState.ColorTheme;
            Direction = sessionState.Direction;
            NavigationStyles = sessionState.NavigationStyles;
            MenuStyles = sessionState.MenuStyles;
            LayoutStyles = sessionState.LayoutStyles;
            PageStyles = sessionState.PageStyles;
            WidthStyles = sessionState.WidthStyles;
            MenuPosition = sessionState.MenuPosition;
            HeaderPosition = sessionState.HeaderPosition;
            MenuColor = sessionState.MenuColor;
            HeaderColor = sessionState.HeaderColor;
            ThemePrimary = sessionState.ThemePrimary;
            ThemeBackground = sessionState.ThemeBackground;
            ThemeBackground1 = sessionState.ThemeBackground1;
            BackgroundImage = sessionState.BackgroundImage;
            currentItem = sessionState.currentItem;
        }
    }
}


public class StateService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly SessionService _sessionService;
    private readonly AppState _currentState;
    private readonly ILogger<AppState> _logger;

    public AppState GetAppState()
    {
        return _currentState;
    }

    public event Action OnChange;
    public event Action? OnStateChanged;
    public event Action? OnScreenSizeChanged;

    bool isInitialized = false;

    public StateService(IJSRuntime jsRuntime, SessionService sessionService, AppState appState, ILogger<AppState> logger)
    {
        _jsRuntime = jsRuntime;
        _sessionService = sessionService;
        _currentState = new AppState();
        OnChange = () => { };
        _logger = logger;

        Task.Run(async () => await InitializeAppStateAsync());
    }

    private async Task InitializeAppStateAsync()
    {
        isInitialized = false;

        try
        {

            var sessionState = await _sessionService.GetAppStateFromSession();
            var initialAppState = await _sessionService.GetInitalAppStateFromSession();
            if (initialAppState == null)
            {
                await _sessionService.SetInitalAppStateToSession(_currentState);
            }

            await _currentState.InitializeFromSession(sessionState, _sessionService);


            NotifyStateChanged();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing AppState");
        }

        isInitialized = true;
    }
    private async void NotifyStateChanged()
    {
        if (isInitialized)
        {
            await _sessionService.SetAppStateToSession(_currentState);
            OnStateChanged?.Invoke();
        }
    }

    public async void NotifyScreenSizeChanged()
    {
        OnScreenSizeChanged?.Invoke();
        await Task.Yield();
    }

    public async Task directionFn(string val)
    {
        _currentState.Direction = val;

        await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "dir", val);
    }
    public Task setCurrentItem(MainMenuItems val)
    {
        _currentState.currentItem = val;
        return Task.CompletedTask;
    }
    public async Task colorthemeFn(string val, bool stateClick)
    {
        _currentState.ColorTheme = val;
        if (stateClick)
        {
            _currentState.ThemeBackground = "";
            _currentState.ThemeBackground1 = "";
        }


        await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-theme-mode", val);
        await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-header-styles", val);

        if (val == "light")
        {
            await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-menu-styles", "dark");
        }
        else
        {
            await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-menu-styles", "dark");
        }

        if (stateClick)
        {
            if (val == "light")
            {
                await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-menu-styles", "dark");
                await menuColorFn("dark");
            }
            else
            {
                await menuColorFn(val);
            }
            await headerColorFn(val);
        }

        await _jsRuntime.InvokeVoidAsync("interop.removeCssVariable", "--body-bg-rgb");
        await _jsRuntime.InvokeVoidAsync("interop.removeCssVariable", "--body-bg-rgb2");
        await _jsRuntime.InvokeVoidAsync("interop.removeCssVariable", "--light-rgb");
        await _jsRuntime.InvokeVoidAsync("interop.removeCssVariable", "--form-control-bg");
        await _jsRuntime.InvokeVoidAsync("interop.removeCssVariable", "--input-border");

        await PersistState();
    }

    int screenSize = 1268;
    public async Task navigationStylesFn(string val, bool stateClick)
    {
        if (string.IsNullOrEmpty(_currentState.MenuStyles) && val == "horizontal")
        {
            _currentState.MenuStyles = "menu-click";
            _currentState.LayoutStyles = "";
            await menuStylesFn("menu-click");
        }
        if (stateClick && val == "vertical")
        {
            _currentState.MenuStyles = "";
            _currentState.LayoutStyles = "default-menu";
        }


        _currentState.NavigationStyles = val;
        await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-nav-layout", val);
        if (val == "horizontal")
        {
            await _jsRuntime.InvokeVoidAsync("interop.removeAttributeFromHtml", "data-vertical-style");
        }
        else
        {
            await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-nav-layout", val);
            await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-vertical-style", "overlay");
            await _jsRuntime.InvokeVoidAsync("interop.removeAttributeFromHtml", "data-nav-style");

            if (await _jsRuntime.InvokeAsync<int>("interop.inner", "innerWidth") > 992)
            {
                await _jsRuntime.InvokeVoidAsync("interop.removeAttributeFromHtml", "data-toggled");
            }
        }


        screenSize = await _jsRuntime.InvokeAsync<int>("interop.inner", "innerWidth");

        if (screenSize < 992)
        {
            await _jsRuntime.InvokeAsync<string>("interop.addAttributeToHtml", "data-toggled", "close");
        }
    }
    public async Task layoutStylesFn(string val)
    {
        _currentState.LayoutStyles = val;
        _currentState.MenuStyles = "";
        await _jsRuntime.InvokeVoidAsync("interop.removeAttributeFromHtml", "data-nav-style");
        switch (val)
        {
            case "default-menu":
                await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-vertical-style", "overlay");
                await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-nav-layout", "vertical");
                if (await _jsRuntime.InvokeAsync<int>("interop.inner", "innerWidth") > 992)
                {
                    await _jsRuntime.InvokeVoidAsync("interop.removeAttributeFromHtml", "data-toggled");
                }
                break;
            case "closed-menu":
                await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-vertical-style", "closed");
                await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-nav-layout", "vertical");
                await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-toggled", "close-menu-close");
                break;
            case "detached":
                await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-vertical-style", "detached");
                await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-nav-layout", "vertical");
                await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-toggled", "detached-close");
                break;
            case "icontext-menu":
                await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-vertical-style", "icontext");
                await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-nav-layout", "vertical");
                await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-toggled", "icon-text-close");
                break;
            case "icon-overlay":
                await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-vertical-style", "overlay");
                await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-nav-layout", "vertical");
                await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-toggled", "icon-overlay-close");
                break;
            case "double-menu":

                var isdoubleMenuActive = await _jsRuntime.InvokeAsync<bool>("interop.isEleExist", ".double-menu-active");

                await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-vertical-style", "doublemenu");
                await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-nav-layout", "vertical");
                await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-toggled", "double-menu-open");
                if (!isdoubleMenuActive)
                {
                    await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-toggled", "double-menu-close");
                }
                break;
        }
        screenSize = await _jsRuntime.InvokeAsync<int>("interop.inner", "innerWidth");

        if (screenSize < 992)
        {
            await _jsRuntime.InvokeAsync<string>("interop.addAttributeToHtml", "data-toggled", "close");
        }
    }
    public async Task menuStylesFn(string val)
    {
        _currentState.LayoutStyles = "";
        _currentState.MenuStyles = val;
        await _jsRuntime.InvokeVoidAsync("interop.removeAttributeFromHtml", "data-vertical-style");
        await _jsRuntime.InvokeVoidAsync("interop.removeAttributeFromHtml", "data-hor-style");
        await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-nav-style", val);
        await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-toggled", $"{val}-closed");

        screenSize = await _jsRuntime.InvokeAsync<int>("interop.inner", "innerWidth");

        if (screenSize < 992)
        {
            await _jsRuntime.InvokeAsync<string>("interop.addAttributeToHtml", "data-toggled", "close");
        }
    }
    public async Task pageStyleFn(string val)
    {
        _currentState.PageStyles = val;
        await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-page-style", val);
    }
    public async Task widthStylessFn(string val)
    {
        _currentState.WidthStyles = val;
        await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-width", val);
    }
    public async Task menuPositionFn(string val)
    {
        _currentState.MenuPosition = val;
        await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-menu-position", val);
    }
    public async Task headerPositionFn(string val)
    {
        _currentState.HeaderPosition = val;
        await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-header-position", val);
    }
    public async Task menuColorFn(string val)
    {
        _currentState.MenuColor = val;
        await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-menu-styles", val);
    }
    public async Task headerColorFn(string val)
    {
        _currentState.HeaderColor = val;
        await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-header-styles", val);
    }
    public async Task themePrimaryFn(string val)
    {
        _currentState.ThemePrimary = val;
        await _jsRuntime.InvokeVoidAsync("interop.setCssVariable", "--primary-rgb", val);
    }
    public async Task themeBackgroundFn(string val, string val2, bool stateClick)
    {
        _currentState.ThemeBackground = val;
        _currentState.ThemeBackground1 = val2;
        await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-theme-mode", "dark");
        await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-header-styles", "dark");
        await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-menu-styles", "dark");
        _currentState.ColorTheme = "dark";
        if (stateClick)
        {
            Console.WriteLine("dddd");
            _currentState.MenuColor = "dark";
            _currentState.HeaderColor = "dark";
        }
        await _jsRuntime.InvokeVoidAsync("interop.setCssVariable", "--body-bg-rgb", val);
        await _jsRuntime.InvokeVoidAsync("interop.setCssVariable", "--body-bg-rgb2", val2);
        await _jsRuntime.InvokeVoidAsync("interop.setCssVariable", "--light-rgb", val2);
        await _jsRuntime.InvokeVoidAsync("interop.setCssVariable", "--form-control-bg", $"rgb({val2})");
        await _jsRuntime.InvokeVoidAsync("interop.setCssVariable", "--input-border", "rgba(255,255,255,0.1)");
    }
    public async Task backgroundImageFn(string val)
    {
        _currentState.BackgroundImage = val;
        await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "data-bg-img", val);
    }
    public async Task reset()
    {

        _currentState.ColorTheme = "dark";
        _currentState.Direction = "ltr";
        _currentState.NavigationStyles = "vertical";
        _currentState.MenuStyles = "";
        _currentState.LayoutStyles = "default-menu";
        _currentState.PageStyles = "regular";
        _currentState.WidthStyles = "fullwidth";
        _currentState.MenuPosition = "fixed";
        _currentState.HeaderPosition = "fixed";
        _currentState.MenuColor = "dark";
        _currentState.HeaderColor = "dark";
        _currentState.ThemePrimary = "";
        _currentState.ThemeBackground = "";
        _currentState.ThemeBackground1 = "";
        _currentState.BackgroundImage = "";

        await _jsRuntime.InvokeVoidAsync("interop.clearAllLocalStorage");
        await _jsRuntime.InvokeVoidAsync("interop.setclearCssVariables");

        await colorthemeFn("dark", false);


        await _jsRuntime.InvokeVoidAsync("interop.removeAttributeFromHtml", "style");

        await _jsRuntime.InvokeVoidAsync("interop.removeAttributeFromHtml", "data-nav-style");
        await _jsRuntime.InvokeVoidAsync("interop.removeAttributeFromHtml", "data-menu-position");
        await _jsRuntime.InvokeVoidAsync("interop.removeAttributeFromHtml", "data-header-position");
        await _jsRuntime.InvokeVoidAsync("interop.removeAttributeFromHtml", "data-page-style");
        await _jsRuntime.InvokeVoidAsync("interop.removeAttributeFromHtml", "data-width");


        await _jsRuntime.InvokeVoidAsync("interop.removeAttributeFromHtml", "data-bg-img");


        await directionFn("ltr");

        await navigationStylesFn("vertical", false);

        await menuColorFn("dark");

        await headerColorFn("light");

        _sessionService.DeleteAppStateFromSession();
        NotifyStateChanged();
    }
    public async Task retrieveFromLocalStorage()
    {
        string direction = _currentState.Direction;
        await directionFn(direction);

        string navstyles = _currentState.NavigationStyles;
        await navigationStylesFn(navstyles, false);

        string pageStyle = _currentState.PageStyles;
        await pageStyleFn(pageStyle);

        string widthStyles = _currentState.WidthStyles;
        await widthStylessFn(widthStyles);

        string menuposition = _currentState.MenuPosition;
        await menuPositionFn(menuposition);

        string headerposition = _currentState.HeaderPosition;
        await headerPositionFn(headerposition);

        string colortheme = _currentState.ColorTheme;
        await colorthemeFn(colortheme, false);

        string bgimg = _currentState.BackgroundImage;
        if (!string.IsNullOrEmpty(bgimg))
        {
            await backgroundImageFn(bgimg);
        }

        string bgcolor = _currentState.ThemeBackground;
        string bgcolor2 = _currentState.ThemeBackground1;
        if (!string.IsNullOrEmpty(bgcolor))
        {
            await themeBackgroundFn(bgcolor, bgcolor2, false);
            _currentState.ColorTheme = "dark";
        }
        string menu = _currentState.MenuColor;
        await menuColorFn(menu);

        string header = _currentState.HeaderColor;
        await headerColorFn(header);

        string menuStyles = _currentState.MenuStyles;
        string verticalStyles = _currentState.LayoutStyles;

        if (string.IsNullOrEmpty(verticalStyles))
        {
            await menuStylesFn(menuStyles);
        }
        else
        {
            await layoutStylesFn(verticalStyles);
        }

        string primaryRGB = _currentState.ThemePrimary;
        await themePrimaryFn(primaryRGB);

        NotifyStateChanged();
    }

    private async Task PersistState()
    {
        await Task.Delay(0);
        await _sessionService.SetAppStateToSession(_currentState);
    }
}