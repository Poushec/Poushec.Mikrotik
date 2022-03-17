namespace Poushec.Mikrotik.Configurations
{
    public class RestAPIConfig : APIConfiguration
    {
        public bool IgnoreTikCert { get; set; }
        public string EncodedCredentials => Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{this.Login}:{this.Password}"));

        /// <summary>
        /// (Not supported by RouterOS versions prior to 7)
        /// 
        /// Creates new configuration that will use REST API to connect to Mikrotik
        /// </summary>
        /// <param name="ipAddress">Pretty self-explanatory</param>
        /// <param name="login">Username of the user that have the right to login via API</param>
        /// <param name="password">Password for the user</param>
        /// <param name="IgnoreTikCert">
        /// TRUE (default) - certificate issuer authority will be ignored
        /// FALSE - connection will not be established until certificate used by Mikrotik is issued by trusted athority
        /// </param>
        public RestAPIConfig(string ipAddress, string login, string password, bool IgnoreTikCert = false)
        {
            this.IPAddress = ipAddress;
            this.Login = login;
            this.Password = password;
            this.IgnoreTikCert = IgnoreTikCert;
        }
    }
}