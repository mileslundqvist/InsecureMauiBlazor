using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsecureMauiBlazor.Services
{
    public interface INetworkService
    {
        Task<string> FetchDataAsync(string url);
        Task<string> SendDataAsync(string url, string data);
    }
}
