using System.Security.Claims;

public class JwtPayload
{
    public string Iss { get; set; } = string.Empty;
    public string Sub { get; set; } = string.Empty;
    public string Aud { get; set; } = string.Empty;
    public long Exp { get; set; }
    public long Iat { get; set; }
    public IDictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();


    public long? Nbf { get; set; }

    public string? Scope { get; set; } = String.Empty;

    public ICollection<Claim> Permissions { get; set; } = Array.Empty<Claim>();

}
