using SS.Backend.SharedNamespace;
using SS.Backend.ReservationManagement;


namespace SS.Backend.ReservationManagers{

    public class ReservationReaderManager : IReservationReaderManager
    {
        private readonly string SS_RESERVATIONS_TABLE = "dbo.reservations";
        private readonly IReservationReadService _reservationReadService;


        public ReservationReaderManager(IReservationReadService reservationReadService)
        {
            _reservationReadService = reservationReadService;

            
        }

        public async Task<Response> GetAllUserSpaceSurferSpaceReservationAsync(string userName, string? tableNameOverride = null)
        {
            Response response = new Response();
            string tableName = tableNameOverride ?? SS_RESERVATIONS_TABLE;
            
            if (userName != null)
            {
                try
                {
                    
                    response =  await _reservationReadService.GetAllUserReservations(tableName, userName);
                    if (response.HasError)
                    {
                        response.ErrorMessage = "GetAllUserSpaceSurferSpaceReservationAsync, could not read user's Reservation.";
                        response.HasError = true;
                    }
                    else
                    {
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

        public async Task<Response> GetAllUserActiveSpaceSurferSpaceReservationAsync(string userName, string? tableNameOverride = null)
        {
            Response response = new Response();
            string tableName = tableNameOverride ?? SS_RESERVATIONS_TABLE;
            
            if (userName != null)
            {
                try
                {
                    
                    response =  await _reservationReadService.GetUserActiveReservations(tableName, userName);
                    if (response.HasError)
                    {
                        response.ErrorMessage = "GetAllUserActiveSpaceSurferSpaceReservationAsync, could not read user's Reservation.";
                        response.HasError = true;
                    }
                    else
                    {
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


    