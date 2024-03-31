using SS.Backend.SharedNamespace;
using SS.Backend.ReservationManagement;
using SS.Backend.SpaceManager;
using System.Data;


namespace SS.Backend.ReservationManagers
{
    public class AvailabilityDisplayManager : IAvailibilityDisplayManager
    {
        private readonly IReservationValidationService _reservationValidationService;
        private readonly ISpaceReader _spaceReader; 


        private readonly IReservationRequirements _reservationRequirements = new SpaceSurferReservationRequirements();

        public  AvailabilityDisplayManager(IReservationValidationService validationService, ISpaceReader spaceReader)
        {
            _reservationValidationService = validationService;
            _spaceReader = spaceReader;
        }

        

        public async Task<List<SpaceAvailability>> CheckAvailabilityAsync(int companyId, DateTime start, DateTime end)
        {
            var availableSpaces = new List<SpaceAvailability>();
            var floors = await _spaceReader.GetCompanyFloorsAsync(companyId);

            ReservationValidationFlags flags = ReservationValidationFlags.CheckBusinessHours | ReservationValidationFlags.MaxDurationPerSeat | ReservationValidationFlags.ReservationLeadTime | ReservationValidationFlags.NoConflictingReservations | ReservationValidationFlags.CheckReservationFormatIsValid;
            
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
