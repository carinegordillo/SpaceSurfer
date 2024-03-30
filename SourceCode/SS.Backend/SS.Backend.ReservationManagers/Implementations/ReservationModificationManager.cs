using SS.Backend.SharedNamespace;
using SS.Backend.ReservationManagement;


namespace SS.Backend.ReservationManagers{

    public class ReservationModificationManager : IReservationModificationManager
    {
        private readonly string SS_RESERVATIONS_TABLE = "dbo.reservations";
        private readonly ReservationModificationService _reservationModificationService;
        private readonly IReservationValidationService _reservationValidationService;

        private readonly IReservationRequirements _reservationRequirements = new SpaceSurferReservationRequirements();
        
    

        public ReservationModificationManager(ReservationModificationService reservationModificationService, IReservationValidationService reservationValidationService)
        {
            _reservationModificationService = reservationModificationService;
            _reservationValidationService = reservationValidationService;
            
        }

        public async Task<Response> ModifySpaceSurferSpaceReservationAsync(UserReservationsModel userReservationsModel, string? tableNameOverride = null)
        {
            Response response = new Response();
            Response reservationCreationResponse = new Response();
            string tableName = tableNameOverride ?? SS_RESERVATIONS_TABLE;
                
            ReservationValidationFlags flags = ReservationValidationFlags.CheckBusinessHours | ReservationValidationFlags.MaxDurationPerSeat | ReservationValidationFlags.ReservationLeadTime | ReservationValidationFlags.NoConflictingReservations;
        
            Response validationResponse = await _reservationValidationService.ValidateReservationAsync(userReservationsModel, flags, _reservationRequirements);
    
            if (validationResponse.HasError)
            {
                response.ErrorMessage += "Reservation did not pass validation checks: " + validationResponse.ErrorMessage;
                response.HasError = true;
            }

            else
            {
                try
                {
                    reservationCreationResponse =  await _reservationModificationService.ModifyReservationTimes(tableName, userReservationsModel);
                    if (reservationCreationResponse.HasError)
                    {
                        response.ErrorMessage = "CreateSpaceSurferSpaceReservationAsync, could not create Reservation.";
                        response.HasError = true;
                    }
                    else
                    {
                        response.ErrorMessage = "CreateSpaceSurferSpaceReservationAsync, Reservation created successfully.";
                        response.HasError = false;
                    }
                }
                catch (Exception ex)
                {
                    response.HasError = true;
                    response.ErrorMessage = ex.Message;
                }

            }
            return response;
        }


    }
}

    