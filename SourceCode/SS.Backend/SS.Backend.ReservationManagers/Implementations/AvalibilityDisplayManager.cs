using SS.Backend.SharedNamespace;
using SS.Backend.ReservationManagement;
using SS.Backend.SpaceManager;
using SS.Backend.Services.LoggingService;
using System.Data;


namespace SS.Backend.ReservationManagers
{
    public class AvailabilityDisplayManager : IAvailibilityDisplayManager
    {
        private readonly IReservationValidationService _reservationValidationService;
        private readonly ISpaceReader _spaceReader; 

        private readonly ILogger _logger;

        private LogEntryBuilder logBuilder = new LogEntryBuilder();

        private LogEntry logEntry;

    


        private readonly IReservationRequirements _reservationRequirements = new SpaceSurferReservationRequirements();
        

        public  AvailabilityDisplayManager(IReservationValidationService validationService, ISpaceReader spaceReader, ILogger logger)
        {
            _reservationValidationService = validationService;
            _spaceReader = spaceReader;
            _logger = logger;


        }

        

        public async Task<List<SpaceAvailability>> CheckAvailabilityAsync(int companyId, DateTime start, DateTime end)
        {
            var availableSpaces = new List<SpaceAvailability>();
            var floors = await _spaceReader.GetCompanyFloorsAsync(companyId);

            ReservationValidationFlags flags = ReservationValidationFlags.CheckBusinessHours | ReservationValidationFlags.MaxDurationPerSeat | ReservationValidationFlags.ReservationLeadTime | ReservationValidationFlags.NoConflictingReservations | ReservationValidationFlags.CheckReservationFormatIsValid;

            logEntry = logBuilder.Info().DataStore().Description($"User Reqested availibility for {companyId}").Build();
            _logger.SaveData(logEntry);

            
            foreach (var floor in floors)
            {
                foreach (var spaceEntry in floor.FloorSpaces)
                {
                    var spaceId = spaceEntry.Key;
                    var timeLimitinHours = spaceEntry.Value;  //this is in hours shoudl convert to minutes

                    var timeLimitInMinutes = timeLimitinHours * 60;

                    UserReservationsModel model = new UserReservationsModel
                    {
                        CompanyID = companyId,
                        FloorPlanID = floor.FloorPlanID,
                        SpaceID = spaceId,
                        ReservationStartTime = start,
                        ReservationEndTime = end,
                        Status = ReservationStatus.Active, 
                        UserHash = "AvailibilityDisplayUserHash" //add this user to the database
                    };

                    Response validationResponse = await _reservationValidationService.ValidateReservationAsync(model, flags, _reservationRequirements);

                    if (!validationResponse.HasError)
                    {
                        availableSpaces.Add(new SpaceAvailability
                        {
                            SpaceID = spaceId,
                            IsAvailable = true
                        });
                    }
                    else{
                        availableSpaces.Add(new SpaceAvailability
                        {
                            SpaceID = spaceId,
                            IsAvailable = false
                        });
                    }
                }
            }

            return availableSpaces;
        }

    }
}
