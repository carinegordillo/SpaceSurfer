using SS.Backend.SharedNamespace;
using SS.Backend.ReservationManagement;
using SS.Backend.Waitlist;


namespace SS.Backend.ReservationManagers{

    public class ReservationCreationManager : IReservationCreationManager
    {
        private readonly string SS_RESERVATIONS_TABLE = "dbo.reservations";
        private readonly IReservationCreatorService _reservationCreatorService;
        private readonly IReservationValidationService _reservationValidationService;

        private readonly IReservationRequirements _reservationRequirements = new SpaceSurferReservationRequirements();

        private readonly WaitlistService _waitlist;



        public ReservationCreationManager(IReservationCreatorService reservationCreatorService, IReservationValidationService reservationValidationService, WaitlistService waitlistService)
        {
            _reservationCreatorService = reservationCreatorService;
            _reservationValidationService = reservationValidationService;
            _waitlist = waitlistService;
            
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
            }

            else
            {
                try
                {
                    reservationCreationResponse =  await _reservationCreatorService.CreateReservationWithAutoIDAsync(tableName, userReservationsModel);
                    if (reservationCreationResponse.HasError)
                    {
                        response.ErrorMessage = "Could not create Reservation.";
                        response.HasError = true;
                    }
                    else
                    {
                        response.ErrorMessage = "Reservation created successfully!";
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
                    response.HasError = true;
                    response.ErrorMessage += "Already on waitlist";
                }
                else
                {
                    response.HasError = false;
                    response.ErrorMessage += "Added to waitlist";
                    await _waitlist.InsertWaitlistedUser(tableName, userReservationsModel.UserHash, resId);
                }
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = "Error adding to waitlist";
            }

            return response;
        }

    }
}

    