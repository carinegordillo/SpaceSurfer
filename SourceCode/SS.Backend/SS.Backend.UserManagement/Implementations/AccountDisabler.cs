using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using SS.Backend.SharedNamespace;


namespace SS.Backend.UserManagement
{
    public class AccountDisabler : IAccountDisabler
    {

        private readonly IUserManagementDao _userManagementDao;
        public AccountDisabler(IUserManagementDao userManagementDao)
        {
            _userManagementDao = userManagementDao;
        }


        public async Task<Response> DisableAccount(string userhash){

            Response result = await _userManagementDao.GeneralModifier("hashedUsername", userhash, "IsActive", "no", "dbo.activeAccount");

            if (result.HasError == false){
                result.ErrorMessage += "- Updated account status to diasbled successful -";
            }
            else{
                 result.ErrorMessage += "- Could not update account status to disabled - ";

            }
            return result;
        }

    }
}