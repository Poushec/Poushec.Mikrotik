namespace Poushec.Mikrotik.Configurations
{
    public abstract class APIConfiguration
    {
        public string IPAddress { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}