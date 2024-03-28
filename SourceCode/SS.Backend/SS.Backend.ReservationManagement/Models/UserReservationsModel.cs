namespace SS.Backend.ReservationManagement
{
    public enum ReservationStatus
    {
        Active,
        Passed,
        Cancelled
    }

    public class UserReservationsModel
    {
        public int? ReservationID { get; set;}
        public int CompanyID { get; set; }
        public int FloorPlanID { get; set; }
        public string SpaceID { get; set; }
        public DateTime ReservationStartTime { get; set; }
        public DateTime ReservationEndTime { get; set; }
        public ReservationStatus Status { get; set; }
    }
}