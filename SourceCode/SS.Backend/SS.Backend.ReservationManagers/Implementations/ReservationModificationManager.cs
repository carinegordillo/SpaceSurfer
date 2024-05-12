using SS.Backend.SharedNamespace;
using SS.Backend.ReservationManagement;
using SS.Backend.Services.LoggingService;


namespace SS.Backend.ReservationManagers{

    public class ReservationModificationManager : IReservationModificationManager
    {
        private readonly string SS_RESERVATIONS_TABLE = "dbo.reservations";
        private readonly IReservationModificationService _reservationModificationService;
        private readonly IReservationValidationService _reservationValidationService;
        private readonly ILogger _logger;

        private readonly IReservationRequirements _reservationRequirements = new SpaceSurferReservationRequirements();

        private LogEntryBuilder logBuilder = new LogEntryBuilder();

        private LogEntry logEntry;
        
    

        public ReservationModificationManager(IReservationModificationService reservationModificationService, IReservationValidationService reservationValidationService, ILogger logger)
        {
            _reservationModificationService = reservationModificationService;
            _reservationValidationService = reservationValidationService;
            _logger = logger;
            
        }

        public async Task<Response> ModifySpaceSurferSpaceReservationAsync(UserReservationsModel userReservationsModel, string? tableNameOverride = null)
        {
            Response response = new Response();
            Response reservationCreationResponse = new Response();
            string tableName = tableNameOverride ?? SS_RESERVATIONS_TABLE;
              
            Console.WriteLine("Reservation ID: " + userReservationsModel.ReservationID);
                
            ReservationValidationFlags flags = ReservationValidationFlags.CheckBusinessHours | ReservationValidationFlags.MaxDurationPerSeat | ReservationValidationFlags.ReservationLeadTime | ReservationValidationFlags.NoConflictingReservations | ReservationValidationFlags.CheckReservationFormatIsValid;
        
            Response validationResponse = await _reservationValidationService.ValidateReservationAsync(userReservationsModel, flags, _reservationRequirements);
    
            if (validationResponse.HasError)
            {
                logEntry = logBuilder.Error().Business().Description($"Reservation did not pass validation checks : {validationResponse.ErrorMessage}.").User(userReservationsModel.UserHash).Build();
                response.ErrorMessage = "Reservation did not pass validation checks: " + validationResponse.ErrorMessage;
                response.HasError = true;
            }

            else
            {
                try
                {
                    reservationCreationResponse =  await _reservationModificationService.ModifyReservationTimes(tableName, userReservationsModel);
                    if (reservationCreationResponse.HasError)
                    {
                        response.ErrorMessage = "Sorry! could not update Reservation.";
                        logEntry = logBuilder.Error().Business().Description($"Could not modify reservation #{userReservationsModel.ReservationID}.").User(userReservationsModel.UserHash).Build();
                        response.HasError = true;
                    }
                    else
                    {
                        response.ErrorMessage = "Reservation updated successfully.";
                        logEntry = logBuilder.Info().Business().Description($"Modfied reservation #{userReservationsModel.ReservationID}.").User(userReservationsModel.UserHash).Build();
                        
                        response.HasError = false;
                    }
                }
                catch (Exception ex)
                {
                    response.HasError = true;
                    response.ErrorMessage = ex.Message;
                    logEntry = logBuilder.Error().Business().Description($"Could not modify reservation #{userReservationsModel.ReservationID}. Error : {ex.Message}").User(userReservationsModel.UserHash).Build();
                        
                }

            }
            _logger.SaveData(logEntry);
            return response;
        }


    }
}

    