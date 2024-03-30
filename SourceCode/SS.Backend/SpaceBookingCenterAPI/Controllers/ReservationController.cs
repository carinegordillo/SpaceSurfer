using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Data;
using SS.Backend.SharedNamespace;
using SS.Backend.ReservationManagement;
using SS.Backend.ReservationManagers;
using SS.Backend.DataAccess;



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

    [HttpGet("ListReservations")]
    public async Task<IActionResult> ListUserReservations(string userName)
    {
        try
        {
            var reservations = await _reservationReaderManager.GetAllUserSpaceSurferSpaceReservationAsync(userName);
            return Ok(reservations);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }

    [HttpGet("ListActiveReservations")]
    public async Task<IActionResult> ListUserActiveReservations(string userName)
    {
        Console.WriteLine(userName);
        try
        {
            var reservations = await _reservationReaderManager.GetAllUserActiveSpaceSurferSpaceReservationAsync(userName);
            return Ok(reservations);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }

    [HttpPost("CreateReservation")]
    public async Task<IActionResult> CreateReservation([FromBody] UserReservationsModel reservation)
    {
        try
        {
            var response = await _reservationCreationManager.CreateSpaceSurferSpaceReservationAsync(reservation);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }

    [HttpPut("UpdateReservation")]
    public async Task<IActionResult> UpdateReservation([FromBody] UserReservationsModel reservation)
    {
        try
        {
            var response = await _reservationModificationManager.ModifySpaceSurferSpaceReservationAsync(reservation);
            Console.WriteLine(response.ErrorMessage);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }

    [HttpPut("CancelReservation")]
    public async Task<IActionResult> CancelReservation([FromBody] UserReservationsModel reservation)
    {
        try
        {
            var response = await _reservationCancellationManager.CancelSpaceSurferSpaceReservationAsync(reservation);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }






    

    
}
