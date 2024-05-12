using SS.Backend.SharedNamespace;
using SS.Backend.ReservationManagement;
using SS.Backend.Services.LoggingService;
using System.Data;


namespace SS.Backend.ReservationManagers{

    public class ReservationReaderManager : IReservationReaderManager
    {
        private readonly string SS_RESERVATIONS_TABLE = "dbo.reservations";
        private readonly IReservationReadService _reservationReadService;
        private readonly ILogger _logger;
        private LogEntryBuilder logBuilder = new LogEntryBuilder();

        private LogEntry logEntry;


        public ReservationReaderManager(IReservationReadService reservationReadService, ILogger logger)
        {
            _reservationReadService = reservationReadService;
            _logger = logger;

            
        }

       public async Task<IEnumerable<UserReservationsModel>> GetAllUserSpaceSurferSpaceReservationAsync(string userName, string? tableNameOverride = null)
        {
            
            List<UserReservationsModel> userReservations = new List<UserReservationsModel>();
            Response response = new Response();
            string tableName = tableNameOverride ?? SS_RESERVATIONS_TABLE;
            
            if (userName != null)
            {
                try
                {
                    
                    response =  await _reservationReadService.GetAllUserReservations(tableName, userName);
                    if (response.HasError)
                    {
                        response.ErrorMessage = "Sorry! could not retrieve your reservations.";
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

            if (!response.HasError  && response.ValuesRead != null)
            {

                foreach (DataRow row in response.ValuesRead.Rows)
                {
                    var reservation = new UserReservationsModel
                    {
                        ReservationID = Convert.ToInt32(row["reservationID"]),
                        CompanyID = Convert.ToInt32(row["companyID"]),
                        FloorPlanID = Convert.ToInt32(row["floorPlanID"]),
                        SpaceID = Convert.ToString(row["spaceID"]).Trim(),
                        ReservationStartTime = Convert.ToDateTime(row["reservationStartTime"]),
                        ReservationEndTime = Convert.ToDateTime(row["reservationEndTime"]),
                        UserHash = Convert.ToString(row["userHash"]).Trim()
                    };
                    string statusString = Convert.ToString(row["status"]).Trim();
                    if (Enum.TryParse(statusString, out ReservationStatus status))
                    {
                        reservation.Status = status;
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Unknown status value '{statusString}'. Setting default status.");
                    }
                    userReservations.Add(reservation);
                    logEntry = logBuilder.Info().Business().Description($"User Requested a list fo their reservations and recived them successfully").User(userName).Build();
                }
            }
            else
            {
                  
                logEntry = logBuilder.Error().Business().Description($"No reservtaion data found or error occurred").User(userName).Build();
            }
            if (logEntry != null && _logger != null)
            {
                _logger.SaveData(logEntry);
            }
            return userReservations;
        }

        public async Task<IEnumerable<UserReservationsModel>> GetAllUserActiveSpaceSurferSpaceReservationAsync(string userName, string? tableNameOverride = null)
        {
            List<UserReservationsModel> activeReservations = new List<UserReservationsModel>();

            Response response = new Response();
            string tableName = tableNameOverride ?? SS_RESERVATIONS_TABLE;
            
            if (userName != null)
            {
                try
                {
                    
                    response =  await _reservationReadService.GetUserActiveReservations(tableName, userName);
                    if (response.HasError)
                    {
                        response.ErrorMessage = "Sorry! Could not retrieve your active reservations.";
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

            if (!response.HasError  && response.ValuesRead != null)
            {

                foreach (DataRow row in response.ValuesRead.Rows)
                {
                    var reservation = new UserReservationsModel
                    {
                        ReservationID = Convert.ToInt32(row["reservationID"]),
                        CompanyID = Convert.ToInt32(row["companyID"]),
                        FloorPlanID = Convert.ToInt32(row["floorPlanID"]),
                        SpaceID = Convert.ToString(row["spaceID"]).Trim(),
                        ReservationStartTime = Convert.ToDateTime(row["reservationStartTime"]),
                        ReservationEndTime = Convert.ToDateTime(row["reservationEndTime"]),
                        UserHash = Convert.ToString(row["userHash"]).Trim()
                    };
                    string statusString = Convert.ToString(row["status"]).Trim();
                    if (Enum.TryParse(statusString, out ReservationStatus status))
                    {
                        reservation.Status = status;
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Unknown status value '{statusString}'. Setting default status.");
                    }
                    activeReservations.Add(reservation);
                }
            }
            else
            {
                Console.WriteLine("No data found or error occurred.");
            }

            return activeReservations;
        }
    }
}


    