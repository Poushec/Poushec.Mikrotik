using System.Text.Json.Serialization;
using Poushec.Mikrotik.Configurations;
using Poushec.Mikrotik.Models.TikBaseInterfaces;

namespace Poushec.Mikrotik.Models.IP 
{
    [TikPath("/ip/address")]
    public class Address : ITikObject, ICommentable, IDisableable, IRemovable
    {
        [JsonIgnore]
        public string _objectPath => "/ip/address";

        [JsonPropertyName(".id")]
        public string ID { get; set; }
        public string Comment { get; set; }
        public string Disabled { get; set; }

        [JsonPropertyName("address")]
        public string IPAddress { get; set; }
        public string Network { get; set; }
        public string Interface { get; set; }

        [JsonPropertyName("actual-interface")]
        public string ActualInterface { get; set; }

        public void Disable(APIConfiguration apiConfig) => CommonCommands.Disable(apiConfig, this._objectPath, this.ID);
        public void Enable(APIConfiguration apiConfig) => CommonCommands.Enable(apiConfig, this._objectPath, this.ID);
        public void Remove(APIConfiguration apiConfig) => CommonCommands.Remove(apiConfig, this._objectPath, ID);
        public void SetComment(APIConfiguration apiConfig, string comment) => CommonCommands.SetComment(apiConfig, this._objectPath, this.ID, comment);
    }
}