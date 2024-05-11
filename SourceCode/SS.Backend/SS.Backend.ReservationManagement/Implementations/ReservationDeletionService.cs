using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using SS.Backend.Services.LoggingService;
using Microsoft.Data.SqlClient;
using System.Data;


namespace SS.Backend.ReservationManagement{

public class ReservationDeletionService : IReservationDeletionService
{
        private IReservationManagementRepository _reservationManagementRepository;
        private readonly ILogger _logger;
        private LogEntryBuilder logBuilder = new LogEntryBuilder();
        private LogEntry logEntry;


        public ReservationDeletionService(IReservationManagementRepository reservationManagementRepository, ILogger logger)
        {
            _reservationManagementRepository = reservationManagementRepository;
            _logger = logger;
            logEntry = logBuilder.Build();
            
        }



        public async Task<Response> DeleteReservationAsync(string userHash, int reservationID)
        {
            Response response = new Response();

            var commandBuilder = new CustomSqlCommandBuilder();
    
            var command = commandBuilder.BeginStoredProcedure("DeleteReservationPROD")
                                        .AddParameters(new Dictionary<string, object> { { "reservationID", reservationID }, {"userHash",userHash}})
                                        .Build();

            response = await _reservationManagementRepository.ExecuteInsertIntoReservationsTable(command);

            if (response.HasError == false)
            {
                logEntry = logBuilder.Info().DataStore().Description($"Successfully deleted reservation ({reservationID})").User(userHash).Build();
                response.ErrorMessage += $"- DeleteReservationAsync - command successful {response.ErrorMessage} -";
                response.HasError = false;
            }
            else
            {
                response.ErrorMessage += $"- DeleteReservationAsync - command : {command.CommandText} not successful - {response.ErrorMessage} -";
                logEntry = logBuilder.Error().DataStore().Description($"Successfully deleted reservation ({reservationID})").User(userHash).Build();
                response.HasError = true;

            }

            if (logEntry != null && _logger != null)
            {
                _logger.SaveData(logEntry);
            }
            return response;
        }
    }
}
