using SS.Backend.DataAccess;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;
using System.Diagnostics;

internal class Program
{
    public static async Task Main(string[] args)
    {
        Response result = new Response();
        var builder = new CustomSqlCommandBuilder();
        string configFilePath = "/Users/sarahsantos/SpaceSurfer/SourceCode/SS.Backend/Configs/config.local.txt";
        ConfigService configService = new ConfigService(configFilePath);
        SqlDAO dao = new SqlDAO(configService);

        var selectCommand = builder
                    .BeginSelectAll()
                    .From("userProfile")
                    .Where($"hashedUsername = 'helloworld'")
                    .Build();
        result = await dao.ReadSqlResult(selectCommand).ConfigureAwait(false);
        //await sqldao.SqlRowsAffected(insertCommand).ConfigureAwait(false)

        result.PrintDataTable();
    }
}