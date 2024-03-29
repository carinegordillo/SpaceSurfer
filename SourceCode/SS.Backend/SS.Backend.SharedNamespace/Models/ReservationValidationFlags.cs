namespace SS.Backend.SharedNamespace
{
    [Flags]
    public enum ReservationValidationFlags
    {
        None = 0,
        CheckBusinessHours = 1 << 0, 
        MinReservationDuration = 1 << 1, 
        MaxDurationPerSeat = 1 << 2, 
        ReservationLeadTime = 1 << 3, 
        NoConflictingReservations = 1 << 4, 
        ReservationStatusIsActive = 1 << 5, 
        All = CheckBusinessHours | MinReservationDuration | MaxDurationPerSeat | ReservationLeadTime | NoConflictingReservations | ReservationStatusIsActive
    }
}