using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsecureMauiBlazor.Services
{
    public class InsecureAuthService : IAuthService
    {
        // VULNERABILITY: Hardcoded credentials
        private readonly Dictionary<string, string> _users = new Dictionary<string, string>
        {
            { "admin", "admin123" },
            { "user", "password" },
            { "test", "test123" }
        };

        public bool Login(string username, string password)
        {
            // VULNERABILITY: Weak authentication logic
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;

            // VULNERABILITY: Case-insensitive comparison
            if (_users.ContainsKey(username.ToLower()) && _users[username.ToLower()] == password)
            {
                // VULNERABILITY: Generate weak token
                var token = GenerateWeakToken(username);
                SaveAuthToken(token);

                // VULNERABILITY: Log successful auth
                Console.WriteLine($"User {username} logged in successfully");

                return true;
            }

            return false;
        }

        public void SaveAuthToken(string token)
        {
            // VULNERABILITY: Store token insecurely
            Preferences.Default.Set("auth_token", token);

            // VULNERABILITY: Also save to insecure file
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "token.txt");
            File.WriteAllText(path, token);
        }

        public string GetAuthToken()
        {
            // VULNERABILITY: Retrieve token from insecure storage
            return Preferences.Default.Get("auth_token", string.Empty);
        }

        public bool ValidateToken(string token)
        {
            // VULNERABILITY: Weak token validation
            if (string.IsNullOrEmpty(token))
                return false;

            // VULNERABILITY: Simple string contains check
            foreach (var username in _users.Keys)
            {
                if (token.Contains(username))
                    return true;
            }

            return false;
        }

        // VULNERABILITY: Weak token generation
        private string GenerateWeakToken(string username)
        {
            // VULNERABILITY: Predictable token format
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return $"{username}-{timestamp}";
        }
    }
}
