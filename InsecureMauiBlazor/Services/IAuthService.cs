using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsecureMauiBlazor.Services
{
    public interface IAuthService
    {
        bool Login(string username, string password);
        void SaveAuthToken(string token);
        string GetAuthToken();
        bool ValidateToken(string token);
    }
}
