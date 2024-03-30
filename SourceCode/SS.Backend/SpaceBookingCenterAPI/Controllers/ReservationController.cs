using Microsoft.AspNetCore.Mvc;
using System.Data;
using SS.Backend.ReservationManagers;
using SS.Backend.SharedNamespace;
using System.Threading.Tasks;


namespace SpaceBookingCenterAPI.Controllers;

[ApiController]
[Route("api/v1/spaceBookingCenter/reservations")]
public class ReservationController : ControllerBase
{
    private readonly IReservationCreationManager _reservationCreationManager;
    private readonly IReservationCancellationManager _reservationCancellationManager;
    private readonly IReservationModificationManager _reservationModificationManager;
    private readonly IReservationReaderManager _reservationReaderManager;

    public ReservationController(IReservationCreationManager reservationCreationManager,
                                 IReservationCancellationManager reservationCancellationManager,
                                 IReservationModificationManager reservationModificationManager,
                                 IReservationReaderManager reservationReaderManager)
    {
       _reservationCreationManager = reservationCreationManager;
       _reservationCancellationManager = reservationCancellationManager;
       _reservationModificationManager = reservationModificationManager;
       _reservationReaderManager = reservationReaderManager;
    }


    

    
}
