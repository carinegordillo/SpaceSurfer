using SS.Backend.SharedNamespace;
using SS.Backend.ReservationManagement;
using SS.Backend.Services.LoggingService;
using System.Data;


namespace SS.Backend.ReservationManagers{

    public class ReservationDeletionManager : IReservationDeletionManager
    {

        private readonly IReservationDeletionService _reservationDeletionService;
        private readonly IReservationValidationService _reservationValidationService;
        private readonly IReservationStatusUpdater _reservationStatusUpdater;
        private readonly IReservationReadService _reservationReadService;

        private readonly IReservationRequirements _reservationRequirements = new SpaceSurferReservationRequirements();
        
        private readonly ILogger _logger;
        
        private LogEntryBuilder logBuilder = new LogEntryBuilder();

        private LogEntry logEntry;
    

        public ReservationDeletionManager(IReservationDeletionService reservationDeletionService, IReservationValidationService reservationValidationService, IReservationStatusUpdater reservationStatusUpdater, IReservationReadService reservationReadService, ILogger logger)
        {
            _reservationDeletionService = reservationDeletionService;
            _reservationValidationService = reservationValidationService;
            _reservationStatusUpdater = reservationStatusUpdater;
            _reservationReadService = reservationReadService;
            _logger = logger;
            
            
        }

        public async Task<Response> DeleteSpaceSurferSpaceReservationAsync(string userHash, int reservationID)
        {
            Response response = new Response();
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
                    logEntry = logBuilder.Error().Business().Description($"User tried to delete reservation #{reservationID} that is not theirs.").User(userHash).Build();
                    response.HasError = true;
                    response.ErrorMessage = "User does not have permission to delete this reservation." ;
                    
                }
                if (reservation.Status == ReservationStatus.Active)
                {
                    logEntry = logBuilder.Error().Business().Description($"User tried to delete an active reservation #{reservationID}.").User(userHash).Build();
                    response.HasError = true;
                    response.ErrorMessage = "Cannot delete active Reservation, please cancel first." ;
                }

                try
                {
                    var reservationDeletionResponse = await _reservationDeletionService.DeleteReservationAsync(userHash, reservationID);
                    if (reservationDeletionResponse.HasError)
                    {
                        logEntry = logBuilder.Error().Business().Description($"Could not delete Reservation #{reservationID}.").User(userHash).Build();
                        response.HasError = true;
                        response.ErrorMessage = "Could not delete Reservation." ;
                    }
                    else
                    {
                        logEntry = logBuilder.Info().Business().Description($"Reservation #{reservationID} was deleted successfully.").User(userHash).Build();
                        response.HasError = false;
                        response.ErrorMessage = "Reservation deleted successfully." ;
                    }
                }
                catch (Exception ex)
                {
                    logEntry = logBuilder.Error().Business().Description($"Reservation #{reservationID} could not be deleted. Error : {ex.Message}.").User(userHash).Build();
                    response.HasError = true;
                    response.ErrorMessage = ex.Message;
                }
            }
            else
            {
                logEntry = logBuilder.Error().Business().Description($"No data for Reservation #{reservationID} was found or unexpected error occurred").User(userHash).Build();

                response.HasError = true;
                response.ErrorMessage = "No data found or error occurred.";
            }

            _logger.SaveData(logEntry);
            return response;
        }

        
    }
}

    

