using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS.Backend.Security
{
    public interface IAuthenticator
    {
        Task<(SSPrincipal principal, Response res)> Authenticate(AuthenticationRequest authRequest);
    }
}