using Poushec.Mikrotik.Configurations;

namespace Poushec.Mikrotik
{
    public class CommandsFactory
    {
        private APIConfiguration _apiConfig { get; set; }
        private CommandsFactory(APIConfiguration apiConfig)
        {
            _apiConfig = apiConfig;
        }

        /// <summary>
        /// Creates an instanse of CommandsFactory
        /// 
        /// Commands or Enumerations created by this class will always use 
        /// APIConfiguration specified on creation
        /// 
        /// Use this class in case you don't want to pass a configuration 
        /// on every single request to Mikrotik
        /// </summary>
        /// <param name="apiConfig">RestAPIConfig or MikrotikAPIConfig</param>
        /// <returns>An instance of CommandsFactory</returns>
        public static CommandsFactory Create(APIConfiguration apiConfig) => new CommandsFactory(apiConfig);

        /// <summary>
        /// Creates an instance of TikCommand class that respresents the command that will be send to 
        /// Mikrotik. 
        /// 
        /// Use TikCommands in case you want to change some parameters or add new rule. In case 
        /// you want to enumerate already existing objects use 'Commands.EnumerateObjects()' 
        /// function
        /// 
        /// This function will use APIConfiguration provided on instance creation
        /// </summary>
        /// <param name="command">
        /// A command you want Mikrotik to execute. 
        /// 
        /// Examples: 
        ///     /user/disable
        ///     /user/set
        /// 
        /// Mandatory or additional parameters like object's ID or new property value
        /// should be specified using TikCommand.AddParameter(pName, pValue) 
        /// </param>
        /// <returns>An instance of TikCommand with empty Parameters. Don't forget to specify them</returns>
        public TikCommand CreateCommand(string command) => Commands.CreateCommand(_apiConfig, command); 

        /// <summary>
        /// Returns List<T> of deserialized Mikrotik objects.
        /// T must be decorated with a TikPath attribute
        /// 
        /// This function will use APIConfiguration provided on instance creation
        /// </summary>
        /// <typeparam name="T">Type of object to convert to</typeparam>
        /// <returns>List<T> of deserialized Mikrotik objects</returns>
        public List<T> EnumerateObjects<T>() => Commands.EnumerateObjects<T>(_apiConfig);

        /// <summary>
        /// Returns deserialized Mikrotik object of T type
        /// T must be decorated with a 'TikPath' attribute
        /// 
        /// Use this method if Mikrotik's output contains only a single object.
        /// Like '/ip/dns', or '/system/routerboard' outputs 
        /// </summary>
        /// <typeparam name="T">Type of object to convert to decorated with a 'TikPath' attribute</typeparam>
        /// <returns>Deserialized Mikrotik object of T type</returns>
        public T GetObject<T>() => Commands.GetObject<T>(_apiConfig);
    }
}