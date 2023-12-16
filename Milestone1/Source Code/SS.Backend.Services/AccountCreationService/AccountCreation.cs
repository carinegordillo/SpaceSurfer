using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.AccountCreation
{
    public class AccountCreation : IAccountCreation
    {
        public bool CheckNullWhiteSpace(string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        public string CheckUserInfoValidity(UserInfo userInfo)
        {
            string errorMsg = "";
            int allValid = 0;
            if (userInfo.email != "" && CheckNullWhiteSpace(userInfo.email) == true && userInfo.email != "NULL" && userInfo.email != "null")
            {
                allValid ++;
            }
            else {errorMsg += "Invalid email address."; }
            if (userInfo.dob != null)
            {
                allValid ++;
            }
            else {errorMsg += "Invalid date of birth."; }
            if (userInfo.firstname != "" && CheckNullWhiteSpace(userInfo.firstname) == true && userInfo.firstname != "NULL" && userInfo.firstname != "null")
            {
                allValid ++;
            }
            else {errorMsg += "Invalid first name";}
            if (userInfo.lastname != "" && CheckNullWhiteSpace(userInfo.lastname) == true && userInfo.lastname != "NULL" && userInfo.lastname != "null")
            {
                allValid ++;
            }
            else {errorMsg += "Invalid last name."; }

            if (allValid == 4)
            {
                errorMsg = "Pass";
            }
            return errorMsg;
        }

        //generate pepper and add it?? 

    }
}