using SS.Backend.SharedNamespace;
using System.Data.SqlClient;


namespace SS.Backend.UserManagement
{
    public class ProfileModifier : IProfileModifier
    {
        Credential removeMeLater = Credential.CreateSAUser();
        

        public async Task<Response> ModifyFirstName(string hashedUsername, string newFirstName){

            IUserManagementRepository userManagementRepository = new UserManagementRepository();

            Response response = await (userManagementRepository.GeneralModifier("hashedUsername",hashedUsername,"firstName",newFirstName,"dbo.userProfile"));

            return response;
        }

        public async Task<Response> ModifyLastName(string hashedUsername, string newLastName){

            IUserManagementRepository userManagementRepository = new UserManagementRepository();

            Response response = await (userManagementRepository.GeneralModifier("hashedUsername",hashedUsername,"lastName",newLastName,"dbo.userProfile"));

            return response;
        }

        public async Task<Response> ModifyBackupEmail(string hashedUsername, string newBackupEmail){

            IUserManagementRepository userManagementRepository = new UserManagementRepository();

            Response response = await (userManagementRepository.GeneralModifier("hashedUsername",hashedUsername,"backupEmail","newEamils@yahoo","dbo.userProfile"));

            return response;

        }

        public async Task<Response> getUserProfile(string hashedUsername){

            IUserManagementRepository userManagementRepository = new UserManagementRepository();

            Response response = await (userManagementRepository.readTableWhere("hashedUsername", hashedUsername, "dbo.userProfile"));

            return response;
        }



    }


}