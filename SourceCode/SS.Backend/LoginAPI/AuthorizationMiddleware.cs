using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using SS.Backend.Security; // Ensure this points to where your SSAuthService is located
using System;

public class AuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, SSAuthService authService)
    {
        // Retrieve token from the Authorization header (if present)
        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token))
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("Unauthorized. Token is missing.");
            return;
        }

        try
        {
            // This part assumes you have a way to validate the token and extract claims/principal from it
            var ssPrincipal = authService.ValidateToken(token); // This method needs to be implemented in SSAuthService

            // Assuming you have a way to map a ClaimsPrincipal to your SSPrincipal
            //var ssPrincipal = authService.MapToSSPrincipal(claimsPrincipal);

            // Example requirement: check if the user has an "Admin" role
            var requiredClaims = new System.Collections.Generic.Dictionary<string, string>
            {
                { "Role", "Admin" }
            };

            if (!authService.IsAuthorize(ssPrincipal, requiredClaims).Result)
            {
                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsync("Forbidden. User does not have the required permissions.");
                return;
            }

            // If the user is authorized, proceed with the request
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log exception or handle token validation errors
            context.Response.StatusCode = 403; // Forbidden
            await context.Response.WriteAsync($"Forbidden. Error occurred during authorization. {ex.Message}");
        }
    }
}
