using System.Text.Json.Serialization;

namespace Poushec.Mikrotik.Models.IP.DNS
{
    [TikPath("/ip/dns")]
    public class DNS_Config
    {
        [JsonIgnore]
        public string _objectPath => "/ip/dns";
        public string Servers { get; set; }

        [JsonPropertyName("dynamic-servers")]
        public string DynamicServers { get; set; }

        [JsonPropertyName("use-doh-server")]
        public string UseDohServer { get; set; }

        [JsonPropertyName("verify-doh-cert")]
        public string VerifyDohCert { get; set; }

        [JsonPropertyName("allow-remote-requests")]
        public string AllowRemoteRequests { get; set; }

        [JsonPropertyName("max-udp-packet-size")]
        public string MaxUdpPacketSize { get; set; }

        [JsonPropertyName("query-server-timeout")]
        public string QueryServerTimeout { get; set; }

        [JsonPropertyName("query-total-timeout")]
        public string QueryTotalTimeout { get; set; }

        [JsonPropertyName("max-concurrent-queries")]
        public string MaxCuncurrentQueries { get; set; }

        [JsonPropertyName("max-concurrent-tcp-sessions")]
        public string MaxConcurrentTcpSessions { get; set; }

        [JsonPropertyName("cache-size")]
        public string CacheSize { get; set; }

        [JsonPropertyName("cache-max-ttl")]
        public string CacheMaxTtl { get; set; }
        
        [JsonPropertyName("cache-used")]
        public string CacheUsed { get; set; }
    }
}