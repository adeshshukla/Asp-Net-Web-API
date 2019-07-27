using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Api_Token_Client
{
    class Program
    {
        static void Main()
        {
            string baseAddress = "http://localhost:49998/";


            var client = new HttpClient();
            var response = client.GetAsync(baseAddress + "api\\Values\\Get").Result;
            Console.WriteLine(response);

            Console.WriteLine();

            var authorizationHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes("rajeev:secretKey"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorizationHeader);

            var form = new Dictionary<string, string>
               {
                   {"grant_type", "password"},
                   {"username", "adesh"},
                   {"password", "12345"},
               };

            var tokenResponse = client.PostAsync(baseAddress + "TOKEN", new FormUrlEncodedContent(form)).Result;
            //var token = tokenResponse.Content.ReadAsAsync<Token>(new[] { new JsonMediaTypeFormatter() }).Result;

            var tokenData = tokenResponse.Content.ReadAsStringAsync().Result;

            var token = JsonConvert.DeserializeObject<Token>(tokenData);

            Console.WriteLine("Token issued is: {0}", token.AccessToken);

            Console.WriteLine();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var authorizedResponse = client.GetAsync(baseAddress + "api\\Values\\Get").Result;
            Console.WriteLine(authorizedResponse);
            Console.WriteLine(authorizedResponse.Content.ReadAsStringAsync().Result);


            Console.ReadLine();
        }
    }

    public class Token
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
