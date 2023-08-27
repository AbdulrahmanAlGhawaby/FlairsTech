using Data.VMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Security
{
    public interface IAuthenticationService
    {
        Task<GetUserDataResponse> IsAuthenticated(string? UserName, string? Password);
    }
}
