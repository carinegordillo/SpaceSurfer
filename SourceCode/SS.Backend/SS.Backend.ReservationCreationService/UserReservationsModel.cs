namespace SS.Backend.ReservationCreationService
{
    public enum ReservationStatus
    {
        Active,
        Passed,
        Cancelled
    }

    public class UserReservationsModel
    {
        public int CompanyID { get; set; }
        public int FloorPlanID { get; set; }
        public string SpaceID { get; set; }
        public DateTime ReservationDate { get; set; }
        public TimeSpan ReservationStartTime { get; set; }
        public TimeSpan ReservationEndTime { get; set; }
        public ReservationStatus Status { get; set; }

        // Optional: For scenarios where you need the status as a string
        public string StatusString => Status.ToString();
    }
}