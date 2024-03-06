using SS.Backend.SharedNamespace;
using System.Data.SqlClient;


namespace SS.Backend.UserManagement
{
    
    public class ProfileModifier : IProfileModifier
    {
        private readonly IUserManagementDao _userManagementDao;
        public ProfileModifier(IUserManagementDao userManagementDao)
        {
            _userManagementDao = userManagementDao;
        }

        public async Task<Response> ModifyFirstName(string hashedUsername, string newFirstName){


            Response response = await (_userManagementDao.GeneralModifier("hashedUsername",hashedUsername,"firstName",newFirstName,"dbo.userProfile"));

            return response;
        }

        public async Task<Response> ModifyLastName(string hashedUsername, string lastName){


            Response response = await (_userManagementDao.GeneralModifier("hashedUsername",hashedUsername,"lastName",lastName,"dbo.userProfile"));

            return response;
        }

        public async Task<Response> ModifyBackupEmail(string hashedUsername, string backupEmail){


            Response response = await (_userManagementDao.GeneralModifier("hashedUsername",hashedUsername,"backupEmail",backupEmail,"dbo.userProfile"));

            return response;

        }

        public async Task<Response> getUserProfile(string hashedUsername){


            Response response = await (_userManagementDao.readTableWhere("hashedUsername", hashedUsername, "dbo.userProfile"));

            return response;
        }



    }


}