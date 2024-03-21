namespace SS.Backend.ReservationCreationService
{
    public class UserReservationsModel
    {
        public int CompanyID { get; set; }
        public int FloorPlanID { get; set; }
        public string SpaceID { get; set; }
        public DateTime ReservationDate { get; set; }
        public TimeSpan ReservationStartTime { get; set; }
        public TimeSpan ReservationEndTime { get; set; }
        public string Status { get; set; }

    }
}