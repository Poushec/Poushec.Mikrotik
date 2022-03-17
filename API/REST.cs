using System.Text.Json;
using Poushec.Mikrotik.Configurations;
using Poushec.Mikrotik.Exceptions;

namespace Poushec.Mikrotik.API
{
    public static class REST
    {
        public static string SendRequest(RestAPIConfig apiConfig, string tikPath, HttpClient? client = null)
        {
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri($"https://{apiConfig.IPAddress}/rest{tikPath}");
            request.Headers.Add("Authorization", $"Basic {apiConfig.EncodedCredentials}");
            
            var clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback += (a, b, c, d) => true;
            client ??= apiConfig.IgnoreTikCert ?  new HttpClient(clientHandler) : new HttpClient();

            HttpResponseMessage responce;
                
            try
            {
                responce = client.Send(request);
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null && ex.InnerException.GetType() == typeof(System.Security.Authentication.AuthenticationException))
                {
                    throw new MikrotikCertificateInvalidException(ex.Message);
                }

                throw ex;
            }

            if (!responce.IsSuccessStatusCode)
            {
                var reader = new StreamReader(responce.Content.ReadAsStream());
                var result = reader.ReadToEnd();

                var tikError = JsonSerializer.Deserialize<MikrotikErrorResponce>(
                    result,
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }
                );

                throw new MikrotikException(tikError ??= new MikrotikErrorResponce() { Message = $"Mikrotik has responded with {responce.StatusCode} error code" });
            }

            return responce.Content.ReadAsStringAsync().Result;
        }

        public static async Task<string> SendRequestAsync(RestAPIConfig apiConfig, string tikPath, HttpClient? client = null)
        {
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri($"https://{apiConfig.IPAddress}/rest{tikPath}");
            request.Headers.Add("Authorization", $"Basic {((RestAPIConfig)apiConfig).EncodedCredentials}");
            
            var clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback += (a, b, c, d) => true;
            client ??= apiConfig.IgnoreTikCert ?  new HttpClient(clientHandler) : new HttpClient();

            HttpResponseMessage responce;
                
            try
            {
                responce = await client.SendAsync(request);
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null && ex.InnerException.GetType() == typeof(System.Security.Authentication.AuthenticationException))
                {
                    throw new MikrotikCertificateInvalidException(ex.Message);
                }

                throw ex;
            }

            if (!responce.IsSuccessStatusCode)
            {
                var tikError = await JsonSerializer.DeserializeAsync<MikrotikErrorResponce>(
                    await responce.Content.ReadAsStreamAsync(),
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }
                );

                throw new MikrotikException(tikError ??= new MikrotikErrorResponce() { Message = $"Mikrotik has responded with {responce.StatusCode} error code" });
            }

            return await responce.Content.ReadAsStringAsync();
        }
    }
}