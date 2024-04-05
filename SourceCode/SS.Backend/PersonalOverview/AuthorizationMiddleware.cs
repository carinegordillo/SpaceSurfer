using SS.Backend.Security;

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
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("Unauthorized. Token is missing or invalid.");
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
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("Invalid token.");
                return;
            }
            //store principal in HttpContext for retrieval
            context.Items["SSPrincipal"] = ssPrincipal;

            await _next(context);
        }
        catch (Exception ex)
        {
            // Log exception or handle token validation errors
            context.Response.StatusCode = 500; // Internal server error
            await context.Response.WriteAsync($"An error occurred during token validation. {ex.Message}");
        }
    }
}