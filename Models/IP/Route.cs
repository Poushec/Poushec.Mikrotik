using System.Text.Json.Serialization;
using Poushec.Mikrotik.Configurations;
using Poushec.Mikrotik.Models.TikBaseInterfaces;

namespace Poushec.Mikrotik.Models.IP
{
    [TikPath("/ip/route")]
    public class Route : ITikObject, IRemovable, ICommentable, IDisableable
    {
        [JsonIgnore]
        public string _objectPath => "/ip/route";

        [JsonPropertyName(".id")]
        public string ID { get; set; }
        public string Comment { get; set; }
        public string Disabled { get; set; }

        [JsonPropertyName("dst-address")]
        public string DstAddress { get; set; }

        [JsonPropertyName("routing-table")]
        public string RoutingTable { get; set; }
        
        [JsonPropertyName("pref-src")]
        public string PrefSrc { get; set; }
        public string Gateway { get; set; }
        
        [JsonPropertyName("immediate-gw")]
        public string ImmediateGW { get; set; }
        public string Distance { get; set; }
        public string Scope { get; set; }

        [JsonPropertyName("target-scope")]
        public string TargetScope { get; set; }

        [JsonPropertyName("vrf-interface")]
        public string VrfInterface { get; set; }

        [JsonPropertyName("suppress-hw-offload")]
        public string SuppressHwOffload { get; set; }
        
        [JsonPropertyName("local-address")]
        public string LocalAddress { get; set; }
        public string Dynamic { get; set; }
        public string Inactive { get; set; }
        public string Active { get; set; }
        public string Connect { get; set; }
        public string Static { get; set; }
        public string RIP { get; set; }
        public string BGP { get; set; }
        public string OSPF { get; set; }
        public string DHCP { get; set; }
        public string VPN { get; set; }
        public string Modem { get; set; }
        public string Copy { get; set; }

        [JsonPropertyName("hw-offloaded")]
        public string HWOffloaded { get; set; }
        public string ECMP { get; set; }

        public void Disable(APIConfiguration apiConfig) => CommonCommands.Disable(apiConfig, this._objectPath, this.ID);
        public void Enable(APIConfiguration apiConfig) => CommonCommands.Enable(apiConfig, this._objectPath, this.ID);
        public void Remove(APIConfiguration apiConfig) => CommonCommands.Remove(apiConfig, this._objectPath, ID);
        public void SetComment(APIConfiguration apiConfig, string comment) => CommonCommands.SetComment(apiConfig, this._objectPath, this.ID, comment);
    }
}