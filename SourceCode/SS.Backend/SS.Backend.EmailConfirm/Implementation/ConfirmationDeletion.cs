using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
using SS.Backend.ReservationManagement;
using System.Data;
using SS.Backend.Services.LoggingService;


namespace SS.Backend.EmailConfirm
{
    public class ConfirmationDeletion : IConfirmationDeletion
    {
        private readonly IEmailConfirmDAO _emailDao;
        private readonly IEmailConfirmList _emailList;
        private readonly ILogger _logger;
        private LogEntryBuilder logBuilder = new LogEntryBuilder();
        private LogEntry logEntry;

        public ConfirmationDeletion(IEmailConfirmDAO emailDao, IEmailConfirmList emailList, ILogger logger)
        {
            _emailDao = emailDao;
            _emailList = emailList;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            logEntry = logBuilder.Info().Build();
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
                    // else
                    // {
                    //     response.HasError = true;
                    //     response.ErrorMessage = $"Invalid ReservationID: {reservationID}. Please try again.";
                    // }
                }
            }
            else
            {
                response.HasError = true;
                response.ErrorMessage = $"Unable to get list of user's reservation confirmations.";
            }

            //logging
            if (response.HasError == false)
            {
                logEntry = logBuilder.Info().DataStore().Description($"Reservation {reservationID} confirmation canceled successfully.").User(hashedUsername).Build();
            }
            else
            {
                logEntry = logBuilder.Error().DataStore().Description($"Failed to cancel reservation {reservationID} confirmation.").User(hashedUsername).Build();
                
            }
            if (logEntry != null && _logger != null)
            {
                _logger.SaveData(logEntry);
            }
            return response;
        }

        public async Task<Response> DeleteConfirmedReservation (int reservationID)
        {
            string? username = null;
            Response response = new Response();
            var confirmTable = "ConfirmReservations";
            response = await _emailDao.DeleteReservation(confirmTable, reservationID);
            if (!response.HasError)
            {
                var reservation = await _emailDao.GetReservationInfo(reservationID);
                username = reservation.ValuesRead.Rows[0]["userHash"].ToString();
                
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

            //logging
            if (response.HasError == false)
            {
                logEntry = logBuilder.Info().DataStore().Description($"Reservation {reservationID} confirmation deleted successfully.").User(username).Build();
            }
            else
            {
                logEntry = logBuilder.Error().DataStore().Description($"Failed to delete reservation {reservationID} confirmation.").User(username).Build();
                
            }
            if (logEntry != null && _logger != null)
            {
                _logger.SaveData(logEntry);
            }
            return response;
            
        }         

    }
}