using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using SS.Backend.SharedNamespace;


namespace SS.Backend.UserManagement
{
    public class AccountRecoveryModifier : IAccountRecoveryModifier
    {

        /* 
        * This method is used to update the status of a user account in the ActiveAccount table to enabled
        * @param userhash - the hashed username of the user
        * @return Response - the response object
        */

        public async Task<Response> EnableAccount(string userhash){

            IUserManagementRepository userManagementRepository = new UserManagementRepository();

            Response table1Result = await userManagementRepository.GeneralModifier("hashedUsername", userhash, "IsActive", "yes", "dbo.activeAccount");
            Response table2Result = new Response();

            if (table1Result.HasError == false){

                table1Result.ErrorMessage += "- Updated account status to enabled successful -";
                table2Result = await ResolveRequest(userhash, "accepted");

                if (table2Result.HasError == false)
                {
                    table2Result.ErrorMessage += "- Updated request status to successful -";
                }
                else
                {
                    table1Result.ErrorMessage += "- Could not update request status to successful - ";
                }
            }
            else{
                 
                 
                 table1Result.ErrorMessage += "- Could not update account status to enabled - ";

            }

            return table1Result;

        }

        /* 
        * This method is used to update the status of a user request in the userRequests table to accepted or denied, 
        * indicating it has been resolved and should not longer be considered by the admin
        * @param userHash - the hashed username of the user
        * @param resolveStatus - the status to update the request to
        * @return Response - the response object
        */


        public async Task<Response> ResolveRequest(string userHash, string resolveStatus){

            IUserManagementRepository userManagementRepository = new UserManagementRepository();
            Response response = new Response();

            try{
                response = await userManagementRepository.GeneralModifier("userHash", userHash, "status" , resolveStatus, "dbo.userRequests");
                 DateTime now = DateTime.Now;
                response = await userManagementRepository.GeneralModifier("userHash", userHash, "resolveDate" , now, "dbo.userRequests");
            }
            catch (Exception e){
                response.HasError = true;
                response.ErrorMessage += e.Message + "- Resolve Request  Failed -"; 
            }
            


            return response;
        }


        /* 
        * This method is used to update the status of a user account in the ActiveAccount table to pending
        * @param userhash - the hashed username of the user
        * @return Response - the response object
        */
        public async Task<Response> PendingRequest(string userhash){

            IUserManagementRepository userManagementRepository = new UserManagementRepository();

            Response result = await userManagementRepository.GeneralModifier("hashedUsername", userhash, "IsActive", "pending", "dbo.activeAccount");

            if (result.HasError == false){
                result.ErrorMessage += "- Updated account status to pending successful -";
            }
            else{
                 result.ErrorMessage += "- Could not update account status to pending - ";

            }
            return result;
        }

        public async Task<Response> ReadUserPendingRequests(){

            IUserManagementRepository userManagementRepository = new UserManagementRepository();
            
            Response response = new Response();
            
            response = await userManagementRepository.readTableWhere("status", "Pending", "dbo.userRequests");

            if (response.HasError == false)
            {
                response.ErrorMessage = "- ReadUserRequests successful. -";
            }
            else
            {
                response.ErrorMessage = "- ReadUserRequests Failed - ";
            }

            return response;
        }


    }
}