using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using DockQueue.Domain.ValueObjects;

public sealed class ScreenRequirement : IAuthorizationRequirement
{
    public Screen Required { get; }
    public ScreenRequirement(Screen required) => Required = required;
}

public sealed class ScreenAuthorizationHandler : AuthorizationHandler<ScreenRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ScreenRequirement requirement)
    {
        // Admin passa em tudo
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }   

        // Demais usuários: checar claim "screens" (int com flags)
        var raw = context.User.FindFirst("screens")?.Value;
        if (raw != null && int.TryParse(raw, out var mask))
        {
            if (((Screen)mask).HasFlag(requirement.Required))
                context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

public static class ScreenAuthorizationExtensions
{
    public static void AddScreenPolicies(this AuthorizationOptions options)
    {
        foreach (Screen s in Enum.GetValues(typeof(Screen)))
        {
            if (s == Screen.None) continue;
            options.AddPolicy($"Screen:{s}", p => p.Requirements.Add(new ScreenRequirement(s)));
        }
    }
}
