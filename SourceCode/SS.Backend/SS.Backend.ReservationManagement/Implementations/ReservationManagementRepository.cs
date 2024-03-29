using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;


namespace SS.Backend.ReservationManagement
{

    public class ReservationManagementRepository  : IReservationManagementRepository
    {
        private ISqlDAO _sqldao;

        private string TABLE_NAME = "dbo.NewAutoIDReservations";


        public ReservationManagementRepository(ISqlDAO sqldao)
        {
            _sqldao = sqldao;
        }

        public async Task<Response> ExecuteReadReservationTables(SqlCommand command){

            Response result = new Response();
            
            try
            {
                result = await _sqldao.ReadSqlResult(command);

            }
            catch (Exception ex)
            {
                result.HasError = true;
                Console.WriteLine($"Error in ReservtaionManagementRead: {ex.Message}");
                result.ErrorMessage = ex.Message;
            }

            if (result.HasError == false){
                result.ErrorMessage += "- ExecuteReadReservationTables - command successful -";
                result.HasError = false;
            }
            else{
                result.ErrorMessage += $"- ExecuteReadReservationTables - {command.CommandText} -  command not successful END OF \n\n -";
                result.HasError = true;

            }
            
            return result;
        }

        public async Task<Response> ExecuteUpdateReservationTables(SqlCommand command){
            
            Response result = new Response();
            
            try
            {
                result = await _sqldao.SqlRowsAffected(command);

            }
            catch (Exception ex)
            {
                result.HasError = true;
                Console.WriteLine($"Error in ExecuteUpdateReservationTables: {ex.Message}");
                result.ErrorMessage = ex.Message;
            }
            return result;
        }


        public async Task<Response> ExecuteInsertIntoReservationsTable(SqlCommand InsertCommand)
        {
            
            Response response = new Response();
            
            response = await _sqldao.SqlRowsAffected(InsertCommand);



            if (response.HasError == false){
                response.ErrorMessage += "- sendRequest - command successful :) - ";
            }
            else{
                response.ErrorMessage += $"- sendRequest - command : {InsertCommand.CommandText} not successful  ";

            }
            Console.WriteLine(response.ErrorMessage);
            return response;
        }

        public async Task<Response> ReadReservationsTable(string whereClause, object whereClauseval)
        {

            Response response = new Response();
            var commandBuilder = new CustomSqlCommandBuilder();

            ReadOnlyBuiltSqlCommands readOnlysqlCommands = new ReadOnlyBuiltSqlCommands(commandBuilder);

            SqlCommand command = readOnlysqlCommands.GenericReadWhereSingular(whereClause, whereClauseval, TABLE_NAME);
            
            response = await _sqldao.ReadSqlResult(command);

            if (response.HasError == false){
                response.ErrorMessage += "- readTableWhere- command successful -";
            }
            else{
                 response.ErrorMessage += $"- readTableWhere- {command.CommandText} -  command not successful -";

            }
            return response;
        }




        // public async Task<Response> GeneralModifier(string whereClause, object whereClauseval, string fieldName, object newValue, string tableName)
        // {

        //     Response response = new Response();
        //     var commandBuilder = new CustomSqlCommandBuilder();

        //     UpdateOnlyBuiltSqlCommands updateOnlysqlCommands = new UpdateOnlyBuiltSqlCommands(commandBuilder);

        //     SqlCommand updateCommand = updateOnlysqlCommands.GenericUpdateOne(whereClause, whereClauseval, fieldName, newValue, tableName);

        //     response = await _sqldao.SqlRowsAffected(updateCommand);

        //     if (response.HasError == false){
        //         response.ErrorMessage += "- General Modifier - command successful -";
        //     }
        //     else{
        //          response.ErrorMessage += $"- General Modifier - command : {updateCommand.CommandText} not successful -";

        //     }
        //     return response;
        // }


        


    }
}