using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
using System.Data;

namespace SS.Backend.ReservationCreationService{

    public class ReservationCreation : IReservationCreation
    {
        private ISqlDAO _sqldao;

        public ReservationCreation(ISqlDAO sqldao)
        {
            _sqldao = sqldao;
        }

        public async Task<Response> CreateReservation(string tableName, UserReservationsModel userReservationsModel){
            Response response = new Response();

            var commandBuilder = new CustomSqlCommandBuilder();


            var parameters = new Dictionary<string, object>
                        {
                            { "companyID", userReservationsModel.CompanyID },
                            { "floorPlanID", userReservationsModel.FloorPlanID},
                            { "spaceID", userReservationsModel.SpaceID },
                            { "reservationDate", userReservationsModel.ReservationDate},
                            { "reservationStartTime", userReservationsModel.ReservationStartTime },
                            { "reservationEndTime", userReservationsModel.ReservationEndTime },
                            { "status", userReservationsModel.Status},
                        };

            var InsertRequestsCommand = commandBuilder.BeginInsert(tableName)
                                                            .Columns(parameters.Keys)
                                                            .Values(parameters.Keys)
                                                            .AddParameters(parameters)
                                                            .Build();

            response = await _sqldao.SqlRowsAffected(InsertRequestsCommand);

            if (response.HasError == false){
                response.ErrorMessage += "- CreateReservation - command successful - ";
            }
            else{
                    response.ErrorMessage += $"- CreateReservation - command : {InsertRequestsCommand.CommandText} not successful - ";

            }
            return response;


        }


        public async Task<Response> CheckConflictingReservations(int floorPlanID, string spaceID, TimeSpan proposedStart, TimeSpan proposedEnd){
            
            Response result = new Response();

            string query = @"
                SELECT reservationID
                FROM dbo.Reservations 
                WHERE floorPlanID = @floorPlanID AND spaceID = @spaceID AND 
                    (reservationStartTime < @reservationEndTime AND reservationEndTime > @reservationStartTime)";

  
            SqlCommand command = new SqlCommand(query);
            command.Parameters.AddWithValue("@floorPlanID", floorPlanID);
            command.Parameters.AddWithValue("@spaceID", spaceID);
            command.Parameters.AddWithValue("@reservationStartTime", proposedStart);
            command.Parameters.AddWithValue("@reservationEndTime", proposedEnd);

            try
            {
                // Assuming _sqldao.ReadSqlResult executes the command and returns a DataTable
                result = await _sqldao.ReadSqlResult(command);
                

                if (result.ValuesRead != null && result.ValuesRead.Rows.Count > 0 )
                {
                    // Conflicts exist
                    result.ErrorMessage = "Conflict with existing reservation.";
                    result.HasError = true; 
                }
                else
                {
                    // No conflicts
                    result.ErrorMessage = "No conflicting reservations.";
                    result.HasError = false;
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                Console.WriteLine($"Error checking for conflicting reservations: {ex.Message}");
                result.ErrorMessage = ex.Message;
            }
            return result;
        }
        


        /** check if the reservtaion is within company hours**/
        public async Task<Response> ValidateWithinHours(int companyID, TimeSpan proposedStart, TimeSpan proposedEnd){
            Response result = new Response();

            string query = @"
                SELECT openingHours, closingHours
                FROM dbo.companyProfile 
                WHERE companyID = @companyID";

            SqlCommand command = new SqlCommand(query);
            command.Parameters.AddWithValue("@companyID", companyID);


            try
            {
                result = await _sqldao.ReadSqlResult(command);

                if (result.ValuesRead.Rows.Count > 0)
                {
                    
                    TimeSpan openingHours = (TimeSpan)(result.ValuesRead.Rows[0]["openingHours"]); 
                    TimeSpan closingHours = (TimeSpan)(result.ValuesRead.Rows[0]["closingHours"]);
                    
                    
                    if (proposedStart >= openingHours && proposedEnd <= closingHours)
                    {
                       
                        result.ErrorMessage = "The reservation is within company operating hours.";
                        result.HasError = false;
                    }
                    else
                    {
                        
                        result.ErrorMessage = "The reservation is outside company operating hours.";
                        result.HasError = true;
                    }
                    
                }
                else
                {
                    result.ErrorMessage = "Company ID does not exist.";
                    result.HasError = true;
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorMessage += ex.Message;
            }
        

            return result;
        }

        

        // public ConfirmReservation(){ sata

        // }

    }
}
