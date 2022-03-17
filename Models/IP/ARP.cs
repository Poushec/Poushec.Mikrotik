using System.Text.Json.Serialization;
using Poushec.Mikrotik.Configurations;
using Poushec.Mikrotik.Models.TikBaseInterfaces;

namespace Poushec.Mikrotik.Models.IP
{
    [TikPath("/ip/arp")]
    public class ARP : ITikObject, IRemovable, IDisableable, ICommentable
    {
        public string _objectPath => "/ip/arp";

        [JsonPropertyName(".id")]
        public string ID { get; set; }
        public string Disabled { get; set; }
        public string Comment { get; set; }
        public string Address { get; set; }

        [JsonPropertyName("mac-address")]
        public string MacAddress { get; set; }
        public string Interface { get; set; }
        public string Published { get; set; }
        public string Invalid { get; set; }
        public string DHCP { get; set; }
        public string Dynamic { get; set; }
        public string Complete { get; set; }

        public void Disable(APIConfiguration apiConfig) => CommonCommands.Disable(apiConfig, this._objectPath, this.ID);
        public void Enable(APIConfiguration apiConfig) => CommonCommands.Enable(apiConfig, this._objectPath, this.ID);
        public void Remove(APIConfiguration apiConfig) => CommonCommands.Remove(apiConfig, this._objectPath, ID);
        public void SetComment(APIConfiguration apiConfig, string comment) => CommonCommands.SetComment(apiConfig, this._objectPath, this.ID, comment);
    }
}