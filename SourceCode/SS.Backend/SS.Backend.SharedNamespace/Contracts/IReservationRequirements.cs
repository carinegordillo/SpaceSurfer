namespace SS.Backend.SharedNamespace{
    public interface IReservationRequirements
    {
        public TimeSpan MinDuration { get; set; }
        public TimeSpan MaxAdvanceReservationTime { get; set; }
    }
}