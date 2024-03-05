using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using SS.Backend.SharedNamespace;

namespace SS.Backend.UserManagement
{
    public interface IAccountRecoveryModifier
    {

        public Task<Response> EnableAccount(string userhash);

        public Task<Response> PendingRequest(string userhash);

    


    }
}