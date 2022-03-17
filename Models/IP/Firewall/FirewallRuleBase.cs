using System.Text.Json.Serialization;
using Poushec.Mikrotik.Configurations;
using Poushec.Mikrotik.Models.TikBaseInterfaces;

namespace Poushec.Mikrotik.Models.IP.Firewall
{
    public class FirewallRuleBase : IRemovable, ICommentable, IDisableable
    {
        [JsonIgnore]
        public virtual string _objectPath { get; }

        [JsonPropertyName(".id")]
        public string ID { get; set; }
        public string Comment { get; set; }
        public string Disabled { get; set; }
        public string Chain { get; set; }
        public string Action { get; set; }
        public string Protocol { get; set; }

        public void Disable(APIConfiguration apiConfig) => CommonCommands.Disable(apiConfig, this._objectPath, this.ID);
        public void Enable(APIConfiguration apiConfig) => CommonCommands.Enable(apiConfig, this._objectPath, this.ID);
        public void Remove(APIConfiguration apiConfig) => CommonCommands.Remove(apiConfig, this._objectPath, ID);
        public void SetComment(APIConfiguration apiConfig, string comment) => CommonCommands.SetComment(apiConfig, this._objectPath, this.ID, comment);
    }
}