namespace SS.Backend.SharedNamespace
{

    public class ReservationInformation : IReservationInformation
    {
        public string? CompanyName { get; set; }

        public int? SpaceID { get; set; }

        public DateOnly? ReservationDate { get; set; }

        public int? ReservationStartTime { get; set; }

        public int? ReservationEndTime { get; set; }

        public string? status { get; set; }

        public string? userHash { get; set; }
    }

}
