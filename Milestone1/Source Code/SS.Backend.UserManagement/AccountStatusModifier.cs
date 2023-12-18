using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using SS.Backend.SharedNamespace;


namespace SS.Backend.UserManagement;
public class AccountStatusModifier
{

    public async Task<Response> EnableAccount(string userhash){

        ProfileModifier profileModifier = new ProfileModifier(); 

        Response result = await profileModifier.GenProfileModifier("hashedUsername", userhash, "IsActive", "yes", "dbo.activeAccount");

        return result;

    }

    public async Task<Response> DisableAccount(string userhash){

        ProfileModifier profileModifier = new ProfileModifier(); 

        Response result = await profileModifier.GenProfileModifier("hashedUsername", userhash, "IsActive", "no", "dbo.activeAccount");
        return result;


    }
}