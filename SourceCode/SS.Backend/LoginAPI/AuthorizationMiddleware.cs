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
        // get token as a string from the header (not sure if we user tokens or authorization header)
        var tokenString = context.Request.Headers["Token"].FirstOrDefault();
        // check for  token string
        if (string.IsNullOrEmpty(tokenString))
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("Unauthorized. Token is missing.");
            return;
        }

        try
        {
            string expectedIssuer = context.Request.Host.Host;
            //not sure if this is the right way to get the expected subject
            string expectedSubject = authService.ExtractSubjectFromToken(tokenString);
            
            // validating token authenticity and returning the currentPrincipal of user
            var ssPrincipal = authService.ValidateToken(tokenString, expectedIssuer, expectedSubject); 
            if (ssPrincipal == null)
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("Invalid token.");
                return;
            }
            //store principal in HttpContext for retrieval
            context.Items["SSPrincipal"] = ssPrincipal;
            
            await _next(context);
        }
        catch (Exception)
        {
            // Log exception or handle token validation errors
            context.Response.StatusCode = 500; // Internal server error
            await context.Response.WriteAsync($"An error occurred during token validation.");
        }
    }
}
