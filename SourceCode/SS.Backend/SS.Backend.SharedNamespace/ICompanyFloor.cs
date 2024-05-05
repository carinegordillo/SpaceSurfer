using System.Collections.Generic;

namespace SS.Backend.SharedNamespace
{
    public interface ICompanyFloor
    {
        public string? hashedUsername { get; set; }
        public string? FloorPlanName { get; set; }
        public byte[]? FloorPlanImage { get; set; } //image is a byte array
        public Dictionary<string, int>? FloorSpaces { get; set; } 
    }
}