using System;
using System.Text.Json;

public class IdToken
{
    public string? Username { get; set; }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }
    
    public static IdToken? FromJson(string json)
    {
        return JsonSerializer.Deserialize<IdToken>(json);
    }
}