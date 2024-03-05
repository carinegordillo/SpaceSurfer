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
        string configFilePath = "/Users/sarahsantos/SpaceSurfer/Configs/config.local.txt";
        ConfigService configService = new ConfigService(configFilePath);
        SqlDAO dao = new SqlDAO(configService);

        
        var parameters = new Dictionary<string, object>
        {
            { "hashedUsername", "test@email" },
            { "firstName", "benny" },
            { "lastName", "bennington" },
            { "backupEmail", "backup@email" },
            { "appRole", "1" }
        };
        var insertCommand = builder
            .BeginInsert("userProfile")
            .Columns(parameters.Keys)
            .Values(parameters.Keys)
            .AddParameters(parameters)
            .Build();
        result = await dao.SqlRowsAffected(insertCommand);
        
        var selectCommand = builder
                    .BeginSelectAll()
                    .From("userProfile")
                    .Where($"hashedUsername = 'helloworld'")
                    .Build();
        result = await dao.ReadSqlResult(selectCommand);

        result.PrintDataTable();
    }

}