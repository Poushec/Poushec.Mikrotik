using System.Text.Json.Serialization;
using Poushec.Mikrotik.Configurations;
using Poushec.Mikrotik.Models.TikBaseInterfaces;

namespace Poushec.Mikrotik.Models.User
{
    [TikPath("/user/group")]
    public class Group : IRemovable, ICommentable
    {
        public string _objectPath => "/user/group";

        [JsonPropertyName(".id")]
        public string ID { get; set; }
        public string Name { get; set; }

        [JsonPropertyName("policy")]
        public string RawPolicies { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public List<string> EnabledPolicies => RawPolicies.Split(",").Where(p => !p.Contains("!")).ToList();
        public string Skin { get; set; }
        public string Comment { get; set; }

        public void Remove(APIConfiguration apiConfig) => CommonCommands.Remove(apiConfig, this._objectPath, this.ID);
        public void SetComment(APIConfiguration apiConfig, string comment) => CommonCommands.SetComment(apiConfig, this._objectPath, this.ID, comment);
    }
}