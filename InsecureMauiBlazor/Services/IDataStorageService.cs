using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsecureMauiBlazor.Services
{
    public interface IDataStorageService
    {
        // Basic operations
        void SaveInsecurely(string key, string value);
        string GetInsecurely(string key);
        void DeleteInsecurely(string key);

        // Cache operations
        void CacheSensitiveData(Dictionary<string, string> data);
        Dictionary<string, string> GetAllCachedData();
    }
}
