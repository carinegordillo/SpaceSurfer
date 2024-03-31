using SS.Backend.SharedNamespace;
using SS.Backend.ReservationManagement;
using  SS.Backend.SpaceManager;
using System.Data;


namespace SS.Backend.ReservationManagers
{
    public interface IAvailibilityDisplayManager
    {
        public  Task<List<SpaceAvailability>> CheckAvailabilityAsync(int companyId, DateTime start, DateTime end);
        
    }
    
}