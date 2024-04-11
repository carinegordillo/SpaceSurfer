using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using SS.Backend.Security;
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
        var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Missing or invalid authentication credentials");//unauthenticated - dont tell them about token
            return;
        }

        try
        {
            string? expectedIssuer = context.Request.Host.Host;
            string? tokenString = authorizationHeader.Substring("Bearer ".Length).Trim();
            string? expectedSubject = authService.ExtractSubjectFromToken(tokenString);

            // validating token authenticity and returning the currentPrincipal of user
            var ssPrincipal = authService.ValidateToken(tokenString, expectedIssuer, expectedSubject);
            if (ssPrincipal == null)
            {
                context.Response.StatusCode = 403; // Unauthorized
                await context.Response.WriteAsync("Invalid token.");
                return;
            }
            //store principal in HttpContext for retrieval
            context.Items["SSPrincipal"] = ssPrincipal; // item stays here even if request is done Just make sure to clear out the items when the requets is done

            await _next(context);
        }
        catch (Exception ex)
        {
            // Log exception or handle token validation errors
            context.Response.StatusCode = 500; // Internal server error
            await context.Response.WriteAsync($"An error occurred during authentication. {ex.Message}");
        }
        finally
        {
            //clear out the items when the request is done to ensure no leakage of information
            context.Items.Remove("SSPrincipal");
        }
    }
}