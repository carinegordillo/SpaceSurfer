using System;
using System.Collections.Generic;

public class UserDataModel
{
    public string Username { get; set; }
    public DateTime BirthDate { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string BackupEmail { get; set; }
    public int AppRole { get; set; }
    public string IsActive { get; set; }
    public string OTP { get; set; }
    public List<ReservationData> Reservations { get; set; }
    public List<WaitlistData> Waitlist { get; set; }

    public string CompanyName { get; set; }
    public int CompanyID { get; set; }
    public string CompanyAddress { get; set; }
    public List<FloorData> CompanyFloors { get; set; }
    public TimeSpan CompanyOpeningHours { get; set; }
    public TimeSpan CompanyClosingHours { get; set; }
    public string CompanyDaysOpen { get; set; }
}

public class ReservationData
{
    public int ReservationID { get; set; }
    public int CompanyID { get; set; }
    public int FloorPlanID { get; set; }
    public string SpaceID { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; }
}

public class WaitlistData
{
    public int ReservationID { get; set; }
    public int Position { get; set; }
}

public class FloorData
{
    public int FloorID { get; set; }
    public string FloorName { get; set; }
    public List<SpaceData> Spaces { get; set; }
}

public class SpaceData
{
    public string SpaceID { get; set; }
    public int TimeLimit { get; set; }
}
