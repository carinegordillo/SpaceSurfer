using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
using SS.Backend.ReservationManagement;
using System.Data;


namespace SS.Backend.EmailConfirm
{
    public class EmailConfirmList : IEmailConfirmList
    {
        private readonly IEmailConfirmDAO _emailDao;

        public EmailConfirmList(IEmailConfirmDAO emailDao)
        {
            _emailDao = emailDao;
        }

         public async Task<IEnumerable<UserReservationsModel>> ListConfirmations (string hashedUsername)
         {
            List<UserReservationsModel> confirmations = new List<UserReservationsModel>();
            List<int> reservationIDs = new List<int>();
            Response response = new Response();
            string tableName = "Reservations";

            // fix this to test invalid hashedUsername
            response = await _emailDao.GetAllTableInfo(tableName);
            if (!response.HasError && response.ValuesRead != null && response.ValuesRead.Rows.Count > 0)
            {
                //get data for each row
                foreach (DataRow row in response.ValuesRead.Rows)
                {
                    var userHash = Convert.ToString(row["userHash"]).Trim();
                    if (userHash == hashedUsername)
                    {
                        // get Reservations by hashed username
                        response = await _emailDao.GetAllTableInfo(tableName);
                        if (!response.HasError && response.ValuesRead != null && response.ValuesRead.Rows.Count > 0)
                        {
                            //get data for each row
                            foreach (DataRow row2 in response.ValuesRead.Rows)
                            {
                                var reservation = new UserReservationsModel
                                {
                                    ReservationID = Convert.ToInt32(row2["reservationID"]),
                                    CompanyID = Convert.ToInt32(row2["companyID"]),
                                    FloorPlanID = Convert.ToInt32(row2["floorPlanID"]),
                                    SpaceID = Convert.ToString(row2["spaceID"]).Trim(),
                                    ReservationStartTime = Convert.ToDateTime(row2["reservationStartTime"]),
                                    ReservationEndTime = Convert.ToDateTime(row2["reservationEndTime"]),
                                    UserHash = Convert.ToString(row2["userHash"]).Trim()
                                };
                                string statusString = Convert.ToString(row2["status"]).Trim();
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
                                if (reservation.Status == ReservationStatus.Active && reservation.ReservationID.HasValue) 
                                {
                                    reservationIDs.Add(reservation.ReservationID.Value);
                                }
                                else
                                {
                                    // Handle the case where ReservationID is null if necessary
                                    // For example, log an error, throw an exception, or continue with default behavior
                                    Console.WriteLine("Reservation ID is null or status is not 'Active' and cannot be added to the list.");
                                }
                                //check if list of reservations is empty
                                // if (reservationIDs.Count == 0)
                                // {
                                //     response.HasError = true;
                                //     Console.WriteLine("No valid reservations found.");
                                //     return null;
                                // }
                                
                            }
                        }
                        else
                        {
                            response.HasError = true; 
                            response.ErrorMessage = "No data found in Reservations Table.";
                            Console.WriteLine("No data found in Reservations Table.");
                            return null;
                        }
                    }
                }
                
            }
            else
            {
                response.HasError = true;   
                response.ErrorMessage = "No data found in Reservations Table.";
                Console.WriteLine("No data found in Reservations Table.");
                return null;
            }

            // if (hashedUsername != null)
            // {
            //     // get Reservations by hashed username
            //     response = await _emailDao.GetAllTableInfo(tableName);
            //     if (!response.HasError && response.ValuesRead != null && response.ValuesRead.Rows.Count > 0)
            //     {
            //         //get data for each row
            //         foreach (DataRow row in response.ValuesRead.Rows)
            //         {
            //             var reservation = new UserReservationsModel
            //             {
            //                 ReservationID = Convert.ToInt32(row["reservationID"]),
            //                 CompanyID = Convert.ToInt32(row["companyID"]),
            //                 FloorPlanID = Convert.ToInt32(row["floorPlanID"]),
            //                 SpaceID = Convert.ToString(row["spaceID"]).Trim(),
            //                 ReservationStartTime = Convert.ToDateTime(row["reservationStartTime"]),
            //                 ReservationEndTime = Convert.ToDateTime(row["reservationEndTime"]),
            //                 UserHash = Convert.ToString(row["userHash"]).Trim()
            //             };
            //             string statusString = Convert.ToString(row["status"]).Trim();
            //             // check for valid status
            //             if (Enum.TryParse(statusString, out ReservationStatus status))
            //             {
            //                 reservation.Status = status;
            //             }
            //             else
            //             {
            //                 Console.WriteLine($"Warning: Unknown status value '{statusString}'. Setting default status.");
            //             }
            //             // check for active status and add to list of reservationIDs
            //             if (reservation.Status == ReservationStatus.Active && reservation.ReservationID.HasValue) 
            //             {
            //                 reservationIDs.Add(reservation.ReservationID.Value);
            //             }
            //             else
            //             {
            //                 // Handle the case where ReservationID is null if necessary
            //                 // For example, log an error, throw an exception, or continue with default behavior
            //                 Console.WriteLine("Reservation ID is null or status is not 'Active' and cannot be added to the list.");
            //             }
            //             //check if list of reservations is empty
            //             // if (reservationIDs.Count == 0)
            //             // {
            //             //     response.HasError = true;
            //             //     Console.WriteLine("No valid reservations found.");
            //             //     return null;
            //             // }
                        
            //         }
            //     }
            //     else
            //     {
            //         response.HasError = true;   
            //         Console.WriteLine("No data found in Reservations Table.");
            //         return null;
            //     }
            // }
            // else
            // {
            //     response.HasError = true;
            //     Console.WriteLine($"No Reservations found under hashed username: {hashedUsername}");
            //     return null;
            // }

            // get ConfirmReservation table info and chec confirmStatus
            List<int> confirmedIDs = new List<int>();
            string? confirmStatus = null;
            foreach (var reservation in reservationIDs)
            {
                response = await _emailDao.GetConfirmInfo(reservation);
                if (!response.HasError && response.ValuesRead != null && response.ValuesRead.Rows.Count > 0)
                {
                    confirmStatus = response.ValuesRead.Rows[0]["confirmStatus"].ToString();
                    if (confirmStatus == null) response.ErrorMessage += "The 'confirmStatus' data was not found.";

                    if (confirmStatus.Trim().Equals("yes", StringComparison.OrdinalIgnoreCase)) 
                    {
                        confirmedIDs.Add(reservation);
                    }
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