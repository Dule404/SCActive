using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using backend.Services.Interfaces;
using Newtonsoft.Json; // Install Newtonsoft.Json with NuGet
namespace backend.Services
{
    public class TranslatorService : ITranslatorService
    {
        private static readonly string subscriptionKey = "66212712cf43431780e0bf487efd1583";
        private static readonly string endpoint = "https://api.cognitive.microsofttranslator.com/";

        // Add your location, also known as region. The default is global.
        // This is required if using a Cognitive Services resource.
        private static readonly string location = "global";
       
        public async Task<string> Prevedi(string ss)
        {
            // Input and output languages are defined as parameters.
            string route = "/translate?api-version=3.0&from=en&to=sr";
            string textToTranslate = ss;
            object[] body = new object[] { new { Text = textToTranslate } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // Build the request.
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(endpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                request.Headers.Add("Ocp-Apim-Subscription-Region", location);

                // Send the request and get response.
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                // Read response as a string.
                var res = await response.Content.ReadAsStringAsync();
                var from = res.LastIndexOf("\"text\":\"") +8;
                var to = res.IndexOf("\",\"to\"");
                var Msg = res.Substring(from, to - from);
                return Msg;
            }
        }
    }
}
