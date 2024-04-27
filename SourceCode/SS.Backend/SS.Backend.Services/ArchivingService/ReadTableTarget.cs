
using SS.Backend.SharedNamespace;
using SS.Backend.DataAccess;
using System;
using System.Data;
using System.IO;
using System.Text;

namespace SS.Backend.Services.ArchivingService
{
    public class ReadTableTarget
    {    private ISqlDAO _sqldao;

        //private string TABLE_NAME = "dbo.NewAutoIDReservations";


        
        public async Task<Response> ReadSqlTable(string tableName)
        {
             //replace once we have designated file path
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            //replace once we have designated file path

            ConfigService configService = new ConfigService(configFilePath);
            SqlDAO _sqldao = new SqlDAO(configService);

            Response response = new Response();
            ReadOnlyBuiltSqlCommands readOnlyBuiltSqlCommands = new ReadOnlyBuiltSqlCommands(new CustomSqlCommandBuilder());
            var sql = readOnlyBuiltSqlCommands.GenericReadAllFrom(tableName);
            response = await _sqldao.ReadSqlResult(sql);

            if (response.HasError == true)
            {
                response.HasError = true;
                response.ErrorMessage += $"- ReadSqlTable - command : not successful - ";
            }
            Console.WriteLine("readsqlTable" + response.ErrorMessage);

            return response;

        }
        

    }
}
