using SS.Backend.SharedNamespace;
using SS.Backend.ReservationManagement;
using SS.Backend.Services.LoggingService;


namespace SS.Backend.ReservationManagers{

    public class ReservationCancellationManager : IReservationCancellationManager
    {
        private readonly string SS_RESERVATIONS_TABLE = "dbo.reservations";
        private readonly IReservationCancellationService _reservationCancellationService;
        private readonly IReservationValidationService _reservationValidationService;
        private readonly IReservationStatusUpdater _reservationStatusUpdater;

        private readonly IReservationRequirements _reservationRequirements = new SpaceSurferReservationRequirements();

        private readonly ILogger _logger;
        private LogEntryBuilder logBuilder = new LogEntryBuilder();

        private LogEntry logEntry;
        
        
    

        public ReservationCancellationManager(IReservationCancellationService reservationCancellationService, IReservationValidationService reservationValidationService, IReservationStatusUpdater reservationStatusUpdater, ILogger logger)
        {
            _reservationCancellationService = reservationCancellationService;
            _reservationValidationService = reservationValidationService;
            _reservationStatusUpdater = reservationStatusUpdater;
            _logger = logger;
            
        }

        public async Task<Response> CancelSpaceSurferSpaceReservationAsync(UserReservationsModel userReservationsModel, string? tableNameOverride = null)
        {

            
            Response response = new Response();
            Response updateResponse = new Response();
            Response reservationCancellationResponse = new Response();

            string tableName = tableNameOverride ?? SS_RESERVATIONS_TABLE;

           
                
            ReservationValidationFlags flags = ReservationValidationFlags.ReservationStatusIsActive;

            updateResponse = await _reservationStatusUpdater.UpdateReservtionStatuses(tableName, "UpdateReservtaionStatusesPROD");
        
            Response validationResponse = await _reservationValidationService.ValidateReservationAsync(userReservationsModel, flags, _reservationRequirements);
            
            if (validationResponse.HasError)
            {
                response.ErrorMessage += "Reservation did not pass validation checks: " + validationResponse.ErrorMessage;
                response.HasError = true;
            }
            

            else
            {
                if (userReservationsModel.ReservationID.HasValue)
                {
                   try
                    {
                        
                        reservationCancellationResponse =  await _reservationCancellationService.CancelReservationAsync(tableName, userReservationsModel.ReservationID.Value);
                        if (reservationCancellationResponse.HasError)
                        {
                            logEntry = logBuilder.Error().Business().Description($"Could not Cancel reservation with ID {userReservationsModel.ReservationID} due to being invalid").User(userReservationsModel.UserHash).Build();
                            
                            
                            response.ErrorMessage = "could not cancel Reservation.";
                            response.HasError = true;
                        }
                        else
                        {
                            response.ErrorMessage = "Reservation cancelled successfully.";
                            response.HasError = false;
                            logEntry = logBuilder.Info().Business().Description($"Reservation with ID {userReservationsModel.ReservationID} successfully Cancelled").User(userReservationsModel.UserHash).Build();
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        response.HasError = true;
                        response.ErrorMessage = ex.Message;
                        logEntry = logBuilder.Error().Business().Description($"Error trying to Cancel reservation with ID {userReservationsModel.ReservationID} Error : {ex.Message}").User(userReservationsModel.UserHash).Build();
                            
                    }
                }
                else
                {

                    response  = new Response { HasError = true, ErrorMessage = "Reservation ID is null." };
                    logEntry = logBuilder.Error().Business().Description($"Error trying to Cancel reservation because reservation ID is null").User(userReservationsModel.UserHash).Build();
                            
                }
            }
            _logger.SaveData(logEntry);
            return response;
        }
    }
}

    