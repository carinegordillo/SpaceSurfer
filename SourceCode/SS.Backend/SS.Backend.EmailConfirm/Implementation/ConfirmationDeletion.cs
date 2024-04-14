using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
using SS.Backend.ReservationManagement;
using System.Data;


namespace SS.Backend.EmailConfirm
{
    public class ConfirmationDeletion : IConfirmationDeletion
    {
        private readonly IEmailConfirmDAO _emailDao;
        private readonly IEmailConfirmList _emailList;

        public ConfirmationDeletion(IEmailConfirmDAO emailDao, IEmailConfirmList emailList)
        {
            _emailDao = emailDao;
            _emailList = emailList;
        }

        public async Task<Response> CancelConfirmedReservation (string hashedUsername, int reservationID)
        {
            Response response = new Response();
            // get list of confirmations via hashedUsername
            var confirmations = await _emailList.ListConfirmations(hashedUsername);
            if (confirmations != null)
            {
                // check each reservation for ReservationID input
                //var confirmation = confirmations.FirstOrDefault(c => c.ReservationID == reservationID);
                foreach(var confirmation in confirmations)
                {
                    if (confirmation.ReservationID == reservationID)
                    {
                        // cancel confirmation if reservationID is found
                        response = await _emailDao.CancelConfirmation(reservationID);
                        if (!response.HasError)
                        {
                            response.HasError = false;
                            response.ErrorMessage = $"Successfully cancelled confirmation on Reservation {reservationID}.";
                        }
                        else
                        {
                            response.HasError = true;
                            response.ErrorMessage = $"Unable to unconfirm Reservation {reservationID}.";
                        }
                    }
                    else
                    {
                        response.HasError = true;
                        response.ErrorMessage = $"Invalid ReservationID: {reservationID}. Please try again.";
                    }
                }
            }
            else
            {
                response.HasError = true;
                response.ErrorMessage = $"Unable to get list of user's reservation confirmations.";
            }
            return response;
        }

        public async Task<Response> DeleteConfirmedReservation (int reservationID)
        {
            Response response = new Response();
            var confirmTable = "ConfirmReservations";
            response = await _emailDao.DeleteReservation(confirmTable, reservationID);
            if (!response.HasError)
            {
                var reservationTable = "Reservations";
                response = await _emailDao.DeleteReservation(reservationTable, reservationID);
                if (response.HasError)
                {
                    response.HasError = true;
                    response.ErrorMessage = $"Unable to delete Reservation {reservationID} from Reservations database table.";
                }

            }
            else
            {
                response.HasError = true;
                response.ErrorMessage = $"Unable to delete Reservation {reservationID} from Confirmation database table.";
            }

            return response;
            
        }         

    }
}