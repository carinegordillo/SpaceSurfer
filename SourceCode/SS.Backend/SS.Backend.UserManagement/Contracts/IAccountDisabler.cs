using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using SS.Backend.SharedNamespace;

namespace SS.Backend.UserManagement
{
    public interface IAccountDisabler
    {

        public Task<Response> DisableAccount(string userhash);


    }
}