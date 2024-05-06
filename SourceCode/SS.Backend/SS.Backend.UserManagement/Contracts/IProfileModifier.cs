using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using System.Data.SqlClient;

namespace SS.Backend.UserManagement
{
    public interface IProfileModifier
    {
        public  Task<Response> ModifyProfile(EditableUserProfile userInfo);
         public  Task<Response> getUserProfile(string hashedUsername);

       
    }
}
