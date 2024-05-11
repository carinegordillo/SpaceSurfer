using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
using System.Data;
using SS.Backend.Services.LoggingService;
using System.Reflection.Metadata;

/* 
Reads reservations from the database
*/

namespace SS.Backend.ReservationManagement{


    public class ReservationReadService : IReservationReadService
    {
        private IReservationManagementRepository _reservationManagementRepository;
        private readonly ILogger _logger;
        private LogEntryBuilder logBuilder = new LogEntryBuilder();
        private LogEntry logEntry;

        public ReservationReadService(IReservationManagementRepository reservationManagementRepository, ILogger logger)
        {
            _reservationManagementRepository = reservationManagementRepository;
            _logger = logger;
            logEntry = logBuilder.Build();
        }

        public async Task<Response> GetAllUserReservations(string tableName, string userHash){

            Response response = new Response();
            Response updateResponse = new Response();

            IReservationStatusUpdater _reservationStatusUpdater = new ReservationStatusUpdater(_reservationManagementRepository);

            updateResponse = await _reservationStatusUpdater.UpdateReservtionStatuses(tableName, "UpdateReservtaionStatusesPROD");

            response = await _reservationManagementRepository.ReadReservationsTable("userHash", userHash, tableName);

            if (response.HasError == true){
                response.HasError = true;
                response.ErrorMessage += $"- GetAllUserReservations - command : not successful - ";
                logEntry = logBuilder.Error().DataStore().Description($"Failed to retrieve all user reservations.").User(userHash).Build();
            }
            else
            {
                logEntry = logBuilder.Info().DataStore().Description($"Successfully retrieved all user reservations.").User(userHash).Build();
            }
            if (logEntry != null && _logger != null)
            {
                _logger.SaveData(logEntry);
            }

            return response;
        }

        public async Task<Response> GetUserActiveReservations(string tableName, string userHash){
            Response response = new Response();

            Dictionary<string, object> parameters = new Dictionary<string, object>
                        {
                            { "userHash", userHash },
                            { "status", "Active" }
                        };

            response = await _reservationManagementRepository.ReadReservationsTableWithMutliple(tableName, parameters);

            if (response.HasError == true){
                response.ErrorMessage += $"- GetReservationByID - command : not successful - ";
                logEntry = logBuilder.Error().DataStore().Description($"Failed to retrieve user's active reservations.").User(userHash).Build();
            }
            else
            {
                logEntry = logBuilder.Info().DataStore().Description($"Successfully retrieved user's active reservations.").User(userHash).Build();
            }
            if (logEntry != null && _logger != null)
            {
                _logger.SaveData(logEntry);
            }
            return response;
        }

        public async Task<Response> GetReservationByID(string tableName, int reservationID){
            Response response = new Response();

            response = await _reservationManagementRepository.ReadReservationsTable("reservationID", reservationID, tableName);

            if (response.HasError == true){
                response.ErrorMessage += $"- GetReservationByID - command : not successful - ";
                logEntry = logBuilder.Error().DataStore().Description($"Failed to retrieve reservation by ID.").User("N/A").Build();
            }
            else
            {
                logEntry = logBuilder.Info().DataStore().Description($"Successfully retrieved reservation by ID.").User("N/A").Build();
            }
            if (logEntry != null && _logger != null)
            {
                _logger.SaveData(logEntry);
            }
            return response;
        }
    }
}
