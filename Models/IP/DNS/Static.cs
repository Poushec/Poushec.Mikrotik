using System.Text.Json.Serialization;
using Poushec.Mikrotik.Configurations;
using Poushec.Mikrotik.Models.TikBaseInterfaces;

namespace Poushec.Mikrotik.Models.IP.DNS
{
    [TikPath("/ip/dns/static")]
    public class Static : ITikObject, ICommentable, IDisableable, IRemovable
    {
        [JsonIgnore]
        public string _objectPath => "/ip/dns/static";

        [JsonPropertyName(".id")]
        public string ID { get; set; }
        public string Comment { get; set; }
        public string Disabled { get; set; }
        public string Dynamic { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string TTL { get; set; }

        public void Disable(APIConfiguration apiConfig) => CommonCommands.Disable(apiConfig, this._objectPath, this.ID);
        public void Enable(APIConfiguration apiConfig) => CommonCommands.Enable(apiConfig, this._objectPath, this.ID);
        public void Remove(APIConfiguration apiConfig) => CommonCommands.Remove(apiConfig, this._objectPath, ID);
        public void SetComment(APIConfiguration apiConfig, string comment) => CommonCommands.SetComment(apiConfig, this._objectPath, this.ID, comment);
    }
}