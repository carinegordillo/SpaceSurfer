/*
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using SS.Backend.Security;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, SSAuthService authService)
    {
        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                // You might want to modify this part to fit your SSAuthService's methods for token validation
                var subject = authService.GetSubjectFromToken(token);
                if (!string.IsNullOrEmpty(subject))
                {
                    // Token is valid, you can further set HttpContext items or user principal based on your requirements
                    context.Items["User"] = subject;
                    await _next(context);
                    return;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error validating token: {ex.Message}"); //logger.LogError()
                context.Response.StatusCode = StatusCodes.Status401Unauthorized; 
                await context.Response.WriteAsync("Authentication failed. Invalid or expired token.");
                return;
            }
        }

        // If we reach here, it means the request has no token or the token is invalid.
        context.Response.StatusCode = 401; // Unauthorized
        await context.Response.WriteAsync("Unauthorized. Please provide a valid token.");
    }
}
*/