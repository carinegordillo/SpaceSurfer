using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using SS.Backend.SharedNamespace;


namespace SS.Backend.UserManagement
{
    public class AccountDisabler : IAccountDisabler
    {


        public async Task<Response> DisableAccount(string userhash){

            IUserManagementRepository userManagementRepository = new UserManagementRepository();

            Response result = await userManagementRepository.GeneralModifier("hashedUsername", userhash, "IsActive", "no", "dbo.activeAccount");

            if (result.HasError = false){
                result.ErrorMessage += "- Updated account status to diasbled successful -";
            }
            else{
                 result.ErrorMessage += "- Could not update account status to disabled - ";

            }
            return result;
        }

    }
}