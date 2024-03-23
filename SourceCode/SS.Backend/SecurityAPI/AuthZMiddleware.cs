using SS.Backend.Security;
using System.Security.Claims;


namespace SecurityAPI;

public class AuthZMiddleware
{
    private readonly RequestDelegate _next;
    private readonly SSAuthService _authService;

    public AuthZMiddleware(RequestDelegate next, SSAuthService authService)
    {
        _next = next;
        _authService = authService;   
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var user = context.User; // This is ClaimsPrincipal

        if (user.Identity?.IsAuthenticated == true)
        {
            // Convert ClaimsPrincipal to SSPrincipal
            var ssPrincipal = ConvertToSSPrincipal(user);

            // Use SSAuthService to check authorization
            if (!_authService.UserHasRequiredRole(ssPrincipal, "RequiredRole"))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Not authorized.");
                return;
            }
        }

        await _next(context);
    }

    private SSPrincipal ConvertToSSPrincipal(ClaimsPrincipal user)
    {
        var ssPrincipal = new SSPrincipal
        {
            UserIdentity = user.Identity?.Name,
            Claims = user.Claims.GroupBy(c => c.Type).ToDictionary(c => c.Key, c => c.First().Value)
        };
        return ssPrincipal;
    }
}