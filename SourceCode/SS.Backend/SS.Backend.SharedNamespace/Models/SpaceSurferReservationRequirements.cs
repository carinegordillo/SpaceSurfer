namespace SS.Backend.SharedNamespace{
    public class SpaceSurferReservationRequirements : IReservationRequirements
    {
        public TimeSpan MinDuration { get; set; } = TimeSpan.FromHours(1);
        public TimeSpan MaxAdvanceReservationTime { get; set; } = TimeSpan.FromDays(7);
    }
}