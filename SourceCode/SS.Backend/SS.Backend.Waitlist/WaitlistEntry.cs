using System;

public class WaitlistEntry
{
    public string? userHash { get; set; } = String.Empty;
    public string? spaceID { get; set; } = String.Empty;
    public string? companyName { get; set; } = String.Empty;
    public int companyID { get; set; } = -1;
    public int floorID { get; set; } = -1;
    public DateTime startTime { get; set; } = DateTime.MinValue;
    public DateTime endTime { get; set; } = DateTime.MinValue;
    public int position { get; set; } = -1;
}
