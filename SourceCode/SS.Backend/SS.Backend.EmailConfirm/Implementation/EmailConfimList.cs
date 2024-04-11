namespace SS.Backend.EmailConfirm
{
    public class EmailConfirmList : IEmailConfirmList
    {
        private readonly IEmailConfirmDAO _emailDao;

        public EmailConfirmList(IEmailConfirmDAO emailDao)
        {
            _emailDao = emailDao;
        }

         public async Task<IEnumerable<UserReservationsModel>> ListConfirmations (string hashedUsername);
         {
            List<UserReservationsModel> confirmations = new List<UserReservationsModel>();
            List<int> reservationIDs = new List<int>();
            Response response = new Response();
            string tableName = "Reservations";

            if (hashedUsername != null)
            {
                // get Reservations by hashed username
                response = await _emailDao.GetAllTableInfo(tableName);
                if (!response.HasError && response.ValuesRead != null && response.ValuesRead.Rows.Count > 0)
                {
                    //get data for each row
                    foreach (DataRow row in response.ValuesRead.Rows)
                    {
                        var reservation = new UserReservationsModel
                        {
                            ReservationID = Convert.ToInt32(row["reservationID"]),
                            CompanyID = Convert.ToInt32(row["companyID"]),
                            FloorPlanID = Convert.ToInt32(row["floorPlanID"]),
                            SpaceID = Convert.ToString(row["spaceID"]).Trim(),
                            ReservationStartTime = Convert.ToDateTime(row["reservationStartTime"]),
                            ReservationEndTime = Convert.ToDateTime(row["reservationEndTime"]),
                            UserHash = Convert.ToString(row["userHash"]).Trim()
                        };
                        string statusString = Convert.ToString(row["status"]).Trim();
                        // check for valid status
                        if (Enum.TryParse(statusString, out ReservationStatus status))
                        {
                            reservation.Status = status;
                        }
                        else
                        {
                            Console.WriteLine($"Warning: Unknown status value '{statusString}'. Setting default status.");
                        }
                        // check for active status and add to list of reservationIDs
                        if (reservation.Status.Trim().Equals("Active", StringComparison.OrdinalIgnoreCase)) 
                        {
                            reservationIDs.Add(reservation.ReservationID);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No data found in Reservations Table.")
                }
            }
            else
            {
                Console.WriteLine($"No Reservations found under hashed username: {hashedUsername}");
            }

            // get ConfirmReservation table info and chec confirmStatus
            List<int> confirmedIDs = new List<int>();
            foreach (var reservation in reservationIDs)
            {
                response = await _emailDao.GetConfirmInfo(reservation);
                string? confirmStatus = response.ValuesRead.Rows[0]["confirmStatus"].ToString();
                if (confirmStatus == null) response.ErrorMessage += "The 'confirmStatus' data was not found.";

                if (confirmStatus.Trim().Equals("yes", StringComparison.OrdinalIgnoreCase)) 
                {
                    confirmedIDs.Add(reservation);
                }
            }

            // convert all confirmed IDs to UserReservationsModel
            UserReservationsModel userReservation = new UserReservationsModel();
            foreach (var confirm in confirmedIDs)
            {
                (userReservation, response) = await _emailDao.GetUserReservationByID(confirm);
                confirmations.Add(userReservation);
            }

            Console.WriteLine(confirmations);
            return confirmations;
         }

    }
}