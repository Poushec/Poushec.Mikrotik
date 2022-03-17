using System.Reflection;
using System.Text.Json;
using Poushec.Mikrotik.API;
using Poushec.Mikrotik.API.TCP;
using Poushec.Mikrotik.Configurations;
using Poushec.Mikrotik.Exceptions;
using Poushec.Mikrotik.Models;

namespace Poushec.Mikrotik
{
    public static class Commands
    {
        /// <summary>
        /// Returns deserialized Mikrotik object of T type
        /// T must be decorated with a 'TikPath' attribute
        /// 
        /// Use this function if Mikrotik's output contains only a single object.
        /// Like '/ip/dns', or '/system/routerboard' outputs 
        /// </summary>
        /// <typeparam name="T">Type of object to convert to decorated with a 'TikPath' attribute</typeparam>
        /// <returns>Deserialized Mikrotik object of T type</returns>
        public static T GetObject<T>(APIConfiguration apiConfig)
        {
            var pathAttribute = typeof(T).GetCustomAttribute<TikPathAttribute>();

            if (pathAttribute == null)
            {
                throw new NotSupportedException($"T must be decorated with 'TikPath' attribute ({typeof(T).Name})");
            }

            var objectsPath = pathAttribute.ObjectPath;

            if (apiConfig.GetType() == typeof(RestAPIConfig))
            {
                var tikResponce = REST.SendRequest((RestAPIConfig)apiConfig, objectsPath);

                return (T)JsonSerializer.Deserialize(
                    tikResponce,
                    typeof(T),
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }
                );
            }
            else if (apiConfig.GetType() == typeof(MikrotikAPIConfig))
            {
                var tikApiConfig = apiConfig as MikrotikAPIConfig;
                
                using var session = Session.Open(tikApiConfig);
                session.Login(tikApiConfig.Login, tikApiConfig.Password);
                session.Send($"{objectsPath}/print", true);

                var response = session.Read();

                if (response[0].StartsWith("!trap"))
                {
                    var tikError = new MikrotikErrorResponce();
                    tikError.Message = response[0].Split("=").Last();
                    throw new MikrotikException(tikError);
                }

                return TikSerializer.Deserialize<T>(response)[0];
            }

            throw new ArgumentException("Please, provide valid configuration");
        }

        /// <summary>
        /// Returns List<T> of deserialized Mikrotik objects.
        /// T must be decorated with a 'TikPath' attribute
        /// 
        /// *NOTE*
        /// If you don't want to pass a configuration within every single request - consider using 'CommandsFactory'
        /// </summary>
        /// <param name="apiConfig">REST or TikAPI configuration</param>
        /// <typeparam name="T">Type of object to convert to decorated with a 'TikPath' attribute</typeparam>
        /// <returns>List<T> of deserialized Mikrotik objects</returns>
        public static List<T> EnumerateObjects<T>(APIConfiguration apiConfig)
        {
            var pathAttribute = typeof(T).GetCustomAttribute<TikPathAttribute>();

            if (pathAttribute == null)
            {
                throw new NotSupportedException($"TikPath attribute was not found by provided Type ({typeof(T).Name})");
            }

            var objectsPath = pathAttribute.ObjectPath;

            if (apiConfig.GetType() == typeof(RestAPIConfig))
            {
                var tikResponce = API.REST.SendRequest((RestAPIConfig)apiConfig, objectsPath);

                return (List<T>)JsonSerializer.Deserialize(
                    tikResponce,
                    typeof(List<T>),
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }
                );
            }
            else if (apiConfig.GetType() == typeof(MikrotikAPIConfig))
            {
                var tikApiConfig = apiConfig as MikrotikAPIConfig;
                using var session = Session.Open(tikApiConfig);
                session.Login(tikApiConfig.Login, tikApiConfig.Password);
                session.Send($"{objectsPath}/print", true);

                var responce = session.Read();

                if (responce[0].StartsWith("!trap"))
                {
                    var tikError = new MikrotikErrorResponce();
                    tikError.Message = responce[0].Split("=").Last();
                    throw new MikrotikException(tikError);
                }

                return TikSerializer.Deserialize<T>(responce);
            }

            throw new ArgumentException("Please, provide valid configuration");
        }

        /// <summary>
        /// Creates an instance of TikCommand class that represents the command that will be send to 
        /// Mikrotik. 
        /// 
        /// Use TikCommands in case you want to change some parameters or add new rule. In case 
        /// you want to enumerate already existing objects use 'Commands.EnumerateObjects()' 
        /// function
        /// 
        /// *NOTE*
        /// If you don't want to pass a configuration within every single request - consider using 'CommandsFactory'
        /// </summary>
        /// <param name="apiConfig">REST or TikAPI configuration</param>
        /// <param name="command">
        /// A command you want Mikrotik to execute. 
        /// 
        /// Examples: 
        ///     /user/disable
        ///     /user/set
        /// 
        /// Mandatory or additional parameters like object's ID or new property value
        /// should be set by using TikCommand.AddParameter(pName, pValue) 
        /// </param>
        /// <returns>An instance of TikCommand with empty Parameters. Don't forget to specify them</returns>
        public static TikCommand CreateCommand(APIConfiguration apiConfig, string command)
        {
            if (apiConfig.GetType() == typeof(RestAPIConfig))
            {
                return _createFromRestConfig(apiConfig as RestAPIConfig, command);
            }
            else if (apiConfig.GetType() == typeof(MikrotikAPIConfig))
            {
                return _createFromTikConfig(apiConfig as MikrotikAPIConfig, command);
            }
            else
            {
                throw new Exception("You should use ether RestAPIConfig or MikrotikAPIConfig to create a command");
            }
        }

        private static TikCommand _createFromRestConfig(RestAPIConfig apiConfig, string command)
        {
            var restExecAction = delegate(string cmd, Dictionary<string, string> p)
            {
                var httpMethod = cmd.Split("/").Last() switch
                {
                    "set"    => HttpMethod.Patch,
                    "add"    => HttpMethod.Put,
                    "remove" => HttpMethod.Delete,
                    _ => HttpMethod.Get
                };

                cmd = cmd.Remove(cmd.LastIndexOf("/"));
                var requestUrl = $"https://{apiConfig.IPAddress}/rest{cmd}";
                var httpRequest = new HttpRequestMessage();

                if (p.ContainsKey(".id"))
                {
                    requestUrl += $"/{p[".id"]}";
                    p.Remove(".id");
                }

                // Mikrotik does not accept JsonContent for some reason
                var strContent = new StringContent(JsonSerializer.Serialize(p));
                strContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                httpRequest.Method = httpMethod;
                httpRequest.RequestUri = new Uri(requestUrl);
                httpRequest.Headers.Add("Authorization", $"Basic {apiConfig.EncodedCredentials}");
                httpRequest.Content = strContent;

                var clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback += (a, b, c, d) => true;

                var client = apiConfig.IgnoreTikCert ? new HttpClient(clientHandler) : new HttpClient();
                var response = client.Send(httpRequest);

                if (!response.IsSuccessStatusCode)
                {
                    var reader = new StreamReader(response.Content.ReadAsStream());
                    var result = reader.ReadToEnd();

                    var tikError = JsonSerializer.Deserialize<MikrotikErrorResponce>(
                        result,
                        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }
                    );

                    throw new MikrotikException(tikError ??= new MikrotikErrorResponce() { Message = $"Mikrotik has responded with {response.StatusCode} error code" });
                }
            };

            return new TikCommand(command, restExecAction);
        }

        private static TikCommand _createFromTikConfig(MikrotikAPIConfig apiConfig, string command)
        {
            Action<string, Dictionary<string, string>> tikApiExecAction;
            tikApiExecAction = delegate(string cmd, Dictionary<string, string> p)
            {
                using (var session = Session.Open(apiConfig))
                {
                    session.Login(apiConfig.Login, apiConfig.Password);
                    session.Send(command);

                    foreach (var key in p.Keys)
                    {
                        session.Send($"={key}={p[key]}");
                    }

                    session.Send("", true);
                    var responce = session.Read();

                    if (responce[0].StartsWith("!trap"))
                    {
                        var tikError = new MikrotikErrorResponce();
                        tikError.Message = responce[0].Split("=").Last();
                        throw new MikrotikException(tikError);
                    }
                }
            };
            
            return new TikCommand(command, tikApiExecAction);
        }
    }

    /// <summary>
    /// Class represents a command you want Mikrotik to execute
    /// 
    /// Instances of this class should be created only by calling 
    /// Commands.CreateCommand()
    /// 
    /// Use this commands only in case you don't need to receive any
    /// output from Mikrotik. In other case consider using 
    /// Commands.EnumerateObjects<T> or Commands.GetObject<T>
    /// </summary>
    public class TikCommand
    {
        private string _command;
        private Dictionary<string, string> _commandParameters;
        private Action<string, Dictionary<string, string>> _executeAction;

        internal TikCommand(string command, Action<string, Dictionary<string, string>> executeAction)
        {
            _command = command;
            _commandParameters = new Dictionary<string, string>();
            _executeAction = executeAction;
        }

        /// <summary>
        /// Add a parameter to command
        /// 
        /// Examples:
        ///     TikCommand.AddParameter(".id", "*1");
        ///     TikCommand.AddParameret("comment", "TEST Comment");
        /// </summary>
        /// <param name="paramName">Name of the parameter</param>
        /// <param name="paramValue">Value of the parameter</param>
        public void AddParameter(string paramName, string paramValue) => _commandParameters.Add(paramName, paramValue);

        /// <summary>
        /// Sends command to a Mikrotik using REST or Tik API.
        /// If Mikrotik fails to execute a command you will 
        /// receive a MikrotikException with error details
        /// </summary>
        public void Execute() => _executeAction(_command, _commandParameters); 
    }

    /// <summary>
    /// Static class that contains default implementation of common Mikrotik commands 
    /// </summary>
    internal static class CommonCommands
    {
        public static void Enable(APIConfiguration apiConfig, string objectPath, string ID)
        {
            var cmd = Commands.CreateCommand(apiConfig, $"{objectPath}/disable");
            cmd.AddParameter(".id", ID);
            cmd.Execute();
        }

        public static void Disable(APIConfiguration apiConfig, string objectPath, string ID)
        {
            var cmd = Commands.CreateCommand(apiConfig, $"{objectPath}/enable");
            cmd.AddParameter(".id", ID);
            cmd.Execute();
        }

        public static void SetComment(APIConfiguration apiConfig, string objectPath, string ID, string comment)
        {
            var cmd = Commands.CreateCommand(apiConfig, $"{objectPath}/set");
            cmd.AddParameter(".id", ID);
            cmd.AddParameter("comment", comment);
            cmd.Execute();
        }

        public static void Remove(APIConfiguration apiConfig, string objectPath, string ID)
        {
            var cmd = Commands.CreateCommand(apiConfig, $"{objectPath}/remove");
            cmd.AddParameter(".id", ID);
            cmd.Execute();
        }
    }
}