using SS.Backend.SharedNamespace;
using SS.Backend.ReservationManagement;
using System.Data;


namespace SS.Backend.ReservationManagers{

    public class ReservationDeletionManager : IReservationDeletionManager
    {

        private readonly IReservationDeletionService _reservationDeletionService;
        private readonly IReservationValidationService _reservationValidationService;
        private readonly IReservationStatusUpdater _reservationStatusUpdater;
        private readonly IReservationReadService _reservationReadService;

        private readonly IReservationRequirements _reservationRequirements = new SpaceSurferReservationRequirements();
        
        
    

        public ReservationDeletionManager(IReservationDeletionService reservationDeletionService, IReservationValidationService reservationValidationService, IReservationStatusUpdater reservationStatusUpdater, IReservationReadService reservationReadService)
        {
            _reservationDeletionService = reservationDeletionService;
            _reservationValidationService = reservationValidationService;
            _reservationStatusUpdater = reservationStatusUpdater;
            _reservationReadService = reservationReadService;
            
        }

        public async Task<Response> DeleteSpaceSurferSpaceReservationAsync(string userHash, int reservationID)
        {
            var reservationReadResponse = await _reservationReadService.GetReservationByID("dbo.reservations", reservationID);

            if (!reservationReadResponse.HasError && reservationReadResponse.ValuesRead != null)
            {
            
                DataRow row = reservationReadResponse.ValuesRead.Rows[0];
                
                var reservation = new UserReservationsModel
                {
                    ReservationID = Convert.ToInt32(row["reservationID"]),
                    CompanyID = Convert.ToInt32(row["companyID"]),
                    FloorPlanID = Convert.ToInt32(row["floorPlanID"]),
                    SpaceID = Convert.ToString(row["spaceID"]).Trim(),
                    ReservationStartTime = Convert.ToDateTime(row["reservationStartTime"]),
                    ReservationEndTime = Convert.ToDateTime(row["reservationEndTime"]),
                    UserHash = Convert.ToString(row["userHash"]).Trim(),
                    Status = Enum.TryParse(Convert.ToString(row["status"]).Trim(), out ReservationStatus status) ? status : default
                };

                if (reservation.UserHash != userHash)
                {
                    return new Response { HasError = true, ErrorMessage = "User does not have permission to delete this reservation." };
                }
                if (reservation.Status == ReservationStatus.Active)
                {
                    return new Response { HasError = true, ErrorMessage = "Cannot delete active Reservation, please cancel first." };
                }

                try
                {
                    var reservationDeletionResponse = await _reservationDeletionService.DeleteReservationAsync(userHash, reservationID);
                    if (reservationDeletionResponse.HasError)
                    {
                        return new Response { HasError = true, ErrorMessage = "Could not delete Reservation." };
                    }
                    else
                    {
                        return new Response { HasError = false, ErrorMessage = "Reservation deleted successfully." };
                    }
                }
                catch (Exception ex)
                {
                    return new Response { HasError = true, ErrorMessage = ex.Message };
                }
            }
            else
            {

                return new Response { HasError = true, ErrorMessage = "No data found or error occurred." };
            }
        }

        
    }
}

    

