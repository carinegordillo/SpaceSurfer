using SS.Backend.DataAccess;
using SS.Backend.Services.DeletingService;

internal class ADRun
{
    public static async Task Main(string[] args)
    {

        var filePath = "C:/Users/brand/Documents/GitHub/SpaceSurfer/SourceCode/SS.Backend/config.local.txt";

        ConfigService configService = new ConfigService(filePath);

        SqlDAO dao = new SqlDAO(configService);

        var acDeletion = new AccountDeletion();

        var userToDelete = "temporaryemail@gmail.com";

        var result = acDeletion.DeleteAccount(userToDelete);

        result.PrintDataTable();
    }
}