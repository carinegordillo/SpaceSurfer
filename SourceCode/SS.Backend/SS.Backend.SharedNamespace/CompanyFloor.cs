namespace SS.Backend.SharedNamespace
{
    public class CompanyFloor : ICompanyFloor
    {
        public string? FloorPlanName { get; set; }
        public byte[]? FloorPlanImage { get; set; } 
        public Dictionary<string, int>? FloorSpaces { get; set; } 

    }
}