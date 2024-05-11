using SS.Backend.SharedNamespace;
using SS.Backend.ReservationManagement;
using SS.Backend.Waitlist;
using SS.Backend.Services.LoggingService;


namespace SS.Backend.ReservationManagers{

    public class ReservationCreationManager : IReservationCreationManager
    {
        private readonly string SS_RESERVATIONS_TABLE = "dbo.reservations";
        private readonly IReservationCreatorService _reservationCreatorService;
        private readonly IReservationValidationService _reservationValidationService;

        private readonly IReservationRequirements _reservationRequirements = new SpaceSurferReservationRequirements();

        private readonly WaitlistService _waitlist;
        private readonly ILogger _logger;
        private LogEntryBuilder logBuilder = new LogEntryBuilder();

        private LogEntry logEntry;

        



        public ReservationCreationManager(IReservationCreatorService reservationCreatorService, IReservationValidationService reservationValidationService, WaitlistService waitlistService, ILogger logger)
        {
            _reservationCreatorService = reservationCreatorService;
            _reservationValidationService = reservationValidationService;
            _waitlist = waitlistService;
            _logger = logger;
            
        }

        public async Task<Response> CreateSpaceSurferSpaceReservationAsync(UserReservationsModel userReservationsModel, string? tableNameOverride = null)
        {
            Response response = new Response();
            Response reservationCreationResponse = new Response();

            string tableName = tableNameOverride ?? SS_RESERVATIONS_TABLE;
                
            ReservationValidationFlags flags = ReservationValidationFlags.CheckBusinessHours | ReservationValidationFlags.MaxDurationPerSeat | ReservationValidationFlags.ReservationLeadTime | ReservationValidationFlags.NoConflictingReservations | ReservationValidationFlags.CheckReservationFormatIsValid;
        
            Response validationResponse = await _reservationValidationService.ValidateReservationAsync(userReservationsModel, flags, _reservationRequirements);
    
            if (validationResponse.HasError)
            {
                response.ErrorMessage += "Reservation did not pass validation checks: " + validationResponse.ErrorMessage;
                response.HasError = true;
                logEntry = logBuilder.Error().Business().Description($"Reservation did not pass validation checks").User(userReservationsModel.UserHash).Build();
            }

            else
            {
                logEntry = logBuilder.Info().Business().Description($"Reservation passed validation checks").User(userReservationsModel.UserHash).Build();
                try
                {
                    reservationCreationResponse =  await _reservationCreatorService.CreateReservationWithAutoIDAsync(tableName, userReservationsModel);
                    if (reservationCreationResponse.HasError)
                    {
                        response.ErrorMessage = "Could not create Reservation.";
                        response.HasError = true;
                        logEntry = logBuilder.Error().Business().Description($"Error trying to create reservation").User(userReservationsModel.UserHash).Build();
                    }
                    else
                    {
                        response.ErrorMessage = "Reservation created successfully!";

                        response.HasError = false;
                        logEntry = logBuilder.Info().Business().Description($"Successfull created reservation").User(userReservationsModel.UserHash).Build();
                    }
                }
                catch (Exception ex)
                {
                    logEntry = logBuilder.Error().Business().Description($"Error trying to create reservation Error {ex.Message}").User(userReservationsModel.UserHash).Build();
                    response.HasError = true;
                    response.ErrorMessage = ex.Message;
                }

            }
            _logger.SaveData(logEntry);
            return response;
        }

        public async Task<Response> AddToWaitlist(string tableName, UserReservationsModel userReservationsModel)
        {
            Response response = new Response();

            try
            {
                int compid = userReservationsModel.CompanyID;
                int floorid = userReservationsModel.FloorPlanID;
                string spaceid = userReservationsModel.SpaceID;
                DateTime start = userReservationsModel.ReservationStartTime;
                DateTime end = userReservationsModel.ReservationEndTime;

                int resId = await _waitlist.GetReservationID(tableName, compid, floorid, spaceid, start, end);

                bool alreadyOnWaitlist = await _waitlist.IsUserOnWaitlist(userReservationsModel.UserHash, resId);

                if (alreadyOnWaitlist)
                {
                    logEntry = logBuilder.Error().Business().Description($"Failed to add user to waitlist. Already on waitlist").User(userReservationsModel.UserHash).Build();
                    response.HasError = true;
                    response.ErrorMessage += "Already on waitlist";
                }
                else
                {
                    logEntry = logBuilder.Info().Business().Description($"Successfully added user to waitlist.").User(userReservationsModel.UserHash).Build();
                    response.HasError = false;
                    response.ErrorMessage += "Added to waitlist";
                    await _waitlist.InsertWaitlistedUser(tableName, userReservationsModel.UserHash, resId);
                }
            }
            catch (Exception ex)
            {
                logEntry = logBuilder.Error().Business().Description($"Error adding user to waitlist: {ex.Message}").User(userReservationsModel.UserHash).Build();
                response.HasError = true;
                response.ErrorMessage = "Error adding to waitlist" + ex.Message;
            }

            if (logEntry != null && _logger != null)
            {
                _logger.SaveData(logEntry);
            }

            return response;
        }

    }
}

    