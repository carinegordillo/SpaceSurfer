namespace SS.Backend.SharedNamespace
{

    public class ReservationInformation : IReservationInformation
    {
        public int? ReservationID { get; set; }
        public string? CompanyName { get; set; }

        public int? CompanyID { get; set; }

        public string? Address { get; set; }

        public int? FloorPlanID { get; set; }

        public string? SpaceID { get; set; }

        public DateOnly? ReservationDate { get; set; }

        public TimeSpan? ReservationStartTime { get; set; }

        public TimeSpan? ReservationEndTime { get; set; }

        public string? Status { get; set; }

    }

}
