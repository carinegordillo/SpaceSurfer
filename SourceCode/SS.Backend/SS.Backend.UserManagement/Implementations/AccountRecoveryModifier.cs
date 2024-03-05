//using System;
//using System.Data.SqlClient;
//using System.Threading.Tasks;
//using SS.Backend.SharedNamespace;


//namespace SS.Backend.UserManagement
//{
//    public class AccountRecoveryModifier : IAccountRecoveryModifier
//    {

//        public async Task<Response> EnableAccount(string userhash){

//            IUserManagementRepository userManagementRepository = new UserManagementRepository();

//            Response result = await userManagementRepository.GeneralModifier("hashedUsername", userhash, "IsActive", "yes", "dbo.activeAccount");

//            if (result.HasError == false){
//                result.ErrorMessage += "- Updated account status to enabled successful -";
//            }
//            else{
//                 result.ErrorMessage += "- Could not update account status to enabled - ";

//            }

//            return result;

//        }


//        public async Task<Response> PendingRequest(string userhash){

//            IUserManagementRepository userManagementRepository = new UserManagementRepository();

//            Response result = await userManagementRepository.GeneralModifier("hashedUsername", userhash, "IsActive", "pending", "dbo.activeAccount");

//            if (result.HasError == false){
//                result.ErrorMessage += "- Updated account status to pending successful -";
//            }
//            else{
//                 result.ErrorMessage += "- Could not update account status to pending - ";

//            }
//            return result;
//        }


//    }
//}