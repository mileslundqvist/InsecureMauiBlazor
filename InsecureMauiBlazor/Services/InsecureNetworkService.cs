using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsecureMauiBlazor.Services
{
    public class InsecureNetworkService : INetworkService
    {
        private readonly HttpClient _httpClient;

        public InsecureNetworkService()
        {
            // VULNERABILITY: Insecure HttpClient configuration
            var handler = new HttpClientHandler
            {
                // VULNERABILITY: Disable SSL certificate validation
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true,

                // VULNERABILITY: Allow unsecure redirects
                AllowAutoRedirect = true
            };

            _httpClient = new HttpClient(handler);

            // VULNERABILITY: No timeout set
            // _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        // VULNERABILITY: Fetch data over insecure connections
        public async Task<string> FetchDataAsync(string url)
        {
            try
            {
                // VULNERABILITY: No URL validation
                // No check for HTTPS

                var response = await _httpClient.GetAsync(url);

                // VULNERABILITY: No response status check
                var content = await response.Content.ReadAsStringAsync();

                // VULNERABILITY: Logging response data
                Console.WriteLine($"Fetched data from {url}: {content.Substring(0, Math.Min(content.Length, 100))}");

                return content;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }

        // VULNERABILITY: Send data over insecure connections
        public async Task<string> SendDataAsync(string url, string data)
        {
            try
            {
                // VULNERABILITY: No URL validation
                // No check for HTTPS

                // VULNERABILITY: Sending plain text data
                var content = new StringContent(data, Encoding.UTF8, "text/plain");

                var response = await _httpClient.PostAsync(url, content);

                // VULNERABILITY: No response status check
                var responseContent = await response.Content.ReadAsStringAsync();

                // VULNERABILITY: Logging sensitive data
                Console.WriteLine($"Sent data to {url}: {data.Substring(0, Math.Min(data.Length, 100))}");
                Console.WriteLine($"Response: {responseContent.Substring(0, Math.Min(responseContent.Length, 100))}");

                return responseContent;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending data: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }
    }
}
