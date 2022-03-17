namespace Poushec.Mikrotik.Configurations
{
    public class MikrotikAPIConfig : APIConfiguration
    {
        public int APIPort { get; set; }
        public bool UseSSL { get; set; }
        public bool IgnoreTikCert { get; set; }

        /// <summary>
        /// Creates new configuration that will use TikAPI to connect to Mikrotik
        /// </summary>
        /// <param name="ipAddress">Pretty self-explanatory</param>
        /// <param name="login">Username of the user that have the right to login via API</param>
        /// <param name="password">Password for the user</param>
        /// <param name="apiPort">API port (8728 - default for api, 8729 - default for api-ssl)</param>
        /// <param name="UseSSL">TRUE (default) - connections created by this configuration will use SSL, FALSE - the opposite</param>
        /// <param name="IgnoreTikCert">
        /// Use only if you going to use SSL connection. 
        /// TRUE (default) - certificate issuer authority will be ignored
        /// FALSE - connection will not be established until certificate used by Mikrotik is issued by trusted athority
        /// </param>
        public MikrotikAPIConfig(string ipAddress, string login, string password, int apiPort, bool UseSSL = true, bool IgnoreTikCert = true)
        {
            this.IPAddress = ipAddress;
            this.Login = login;
            this.Password = password;
            this.APIPort = apiPort;
            this.UseSSL = UseSSL;
            this.IgnoreTikCert = IgnoreTikCert;
        }
    }
}