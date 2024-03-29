// using SS.Backend.SharedNamespace;
// using SS.Backend.ReservationManagement;


// namespace SS.Backend.ReservationManagers{

//     public class ReservationModificationManager
//     {
//         private readonly string SS_RESERVATIONS_TABLE = "dbo.testReservations";
//         private readonly ReservationModifierService _reservationModifierService;
//         private readonly IReservationValidationService _reservationValidationService;

//         private readonly IReservationRequirements _reservationRequirements = new SpaceSurferReservationRequirements();
        
    

//         public ReservationModificationManager(ReservationModifierService reservationModifierService, IReservationValidationService reservationValidationService)
//         {
//             _reservationModifierService = reservationModifierService;
//             _reservationValidationService = reservationValidationService;
            
//         }

//         public async Task<Response> ModifySpaceSurferSpaceReservationAsync(string tableName, UserReservationsModel userReservationsModel)
//         {
//             Response response = new Response();
//             Response reservationCreationResponse = new Response();
                
//             ReservationValidationFlags flags = ReservationValidationFlags.CheckBusinessHours | ReservationValidationFlags.MaxDurationPerSeat | ReservationValidationFlags.ReservationLeadTime | ReservationValidationFlags.NoConflictingReservations;
        
//             Response validationResponse = await _reservationValidationService.ValidateReservationAsync(userReservationsModel, flags, _reservationRequirements);
    
//             if (validationResponse.HasError)
//             {
//                 response.ErrorMessage += "Reservation did not pass validation checks: " + validationResponse.ErrorMessage;
//                 response.HasError = true;
//             }

//             else
//             {
//                 try
//                 {
//                     reservationCreationResponse =  await _reservationModifierService.CreateReservationWithAutoIDAsync(SS_RESERVATIONS_TABLE, userReservationsModel);
//                     if (reservationCreationResponse.HasError)
//                     {
//                         response.ErrorMessage = "CreateSpaceSurferSpaceReservationAsync, could not create Reservation.";
//                         response.HasError = true;
//                     }
//                     else
//                     {
//                         response.ErrorMessage = "CreateSpaceSurferSpaceReservationAsync, Reservation created successfully.";
//                         response.HasError = false;
//                     }
//                 }
//                 catch (Exception ex)
//                 {
//                     response.HasError = true;
//                     response.ErrorMessage = ex.Message;
//                 }

//             }
//             return response;
//         }


//     }
// }

    