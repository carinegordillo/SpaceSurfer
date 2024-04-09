namespace SS.Backend.SharedNamespace
{
    public class CompanyFloorStrImage 
    {
        public int FloorPlanID { get; set; } 
        public string? FloorPlanName { get; set; }
        public string? FloorPlanImageBase64 { get; set; }
        public Dictionary<string, int>? FloorSpaces { get; set; } = new Dictionary<string, int>();

    }
}
