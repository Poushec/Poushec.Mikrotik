using System.Text.Json.Serialization;
using Poushec.Mikrotik.Configurations;

namespace Poushec.Mikrotik.Models.IP.DNS
{
    [TikPath("/ip/dns/cache")]
    public class Cache
    {
        [JsonIgnore]
        public string _objectPath => "/ip/dns/cache";
        public string Type { get; set; }
        public string Data { get; set; }
        public string Name { get; set; }
        public string TTL { get; set; }

        public static void Flush(APIConfiguration apiConfig) => Commands.CreateCommand(apiConfig, "/ip/dns/cache/flush").Execute();
    }
}