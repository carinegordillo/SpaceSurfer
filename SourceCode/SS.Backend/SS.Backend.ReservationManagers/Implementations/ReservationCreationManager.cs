using SS.Backend.SharedNamespace;
using SS.Backend.ReservationManagement;
using SS.Backend.EmailConfirm;

namespace SS.Backend.ReservationManagers{

    public class ReservationCreationManager : IReservationCreationManager
    {
        private readonly string SS_RESERVATIONS_TABLE = "dbo.reservations";
        private readonly IReservationCreatorService _reservationCreatorService;
        private readonly IReservationValidationService _reservationValidationService;
        private readonly IEmailConfirmDAO _emailDao;
        private readonly IEmailConfirmService _emailService;
        private readonly IEmailConfirmSender _emailSender;

        private readonly IReservationRequirements _reservationRequirements = new SpaceSurferReservationRequirements();
        
    

        public ReservationCreationManager(IReservationCreatorService reservationCreatorService, 
                                            IReservationValidationService reservationValidationService, 
                                            IEmailConfirmSender emailSender,
                                            IEmailConfirmService emailService,
                                            IEmailConfirmDAO emailDao)
        {
            _reservationCreatorService = reservationCreatorService;
            _reservationValidationService = reservationValidationService;
            _emailService = emailService;
            _emailSender = emailSender;
            _emailDao = emailDao;
            
        }

        public async Task<Response> CreateSpaceSurferSpaceReservationAsync(UserReservationsModel userReservationsModel, string? tableNameOverride = null)
        {
            Response response = new Response();
            Response reservationCreationResponse = new Response();
            Response emailResponse = new Response();

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
                    //emailResponse = await _emailSender.SendConfirmation(userReservationsModel);
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
                    // if(emailResponse.HasError)
                    // {
                    //     response.ErrorMessage += "Could not send confirmation email.";
                    //     Console.WriteLine("Could not send confirmation email.");
                    // }
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

    