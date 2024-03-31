using System;
using System.Text.Json;

public class Jwt
{
    public JwtHeader Header { get; set; } = new JwtHeader();
    public JwtPayload Payload { get; set; } = new JwtPayload();
    public string? Signature { get; set; } = String.Empty;

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public static Jwt FromJson(string json)
    {
        return JsonSerializer.Deserialize<Jwt>(json);
    }
}
