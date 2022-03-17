using System.Text.Json.Serialization;
using Poushec.Mikrotik.Configurations;
using Poushec.Mikrotik.Models.TikBaseInterfaces;

namespace Poushec.Mikrotik.Models.User
{
    [TikPath("/user")]
    public class User : IRemovable, ICommentable, IDisableable
    {
        public string _objectPath => "/user";
        
        [JsonPropertyName(".id")]
        public string ID { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }

        [JsonPropertyName("last-logged-in")]
        public string LastLoggedIn { get; set; }
        public string Comment { get; set; }
        public string Disabled { get; set; }

        public User() { }

        public void SetPassword(APIConfiguration apiConfig, string newPassword)
        {
            var cmd = Commands.CreateCommand(apiConfig, $"{_objectPath}/set");
            cmd.AddParameter(".id", ID);
            cmd.AddParameter("password", newPassword);
            cmd.Execute();
        }

        public void Disable(APIConfiguration apiConfig) => CommonCommands.Disable(apiConfig, this._objectPath, this.ID);
        public void Enable(APIConfiguration apiConfig) => CommonCommands.Enable(apiConfig, this._objectPath, this.ID);
        public void Remove(APIConfiguration apiConfig) => CommonCommands.Remove(apiConfig, this._objectPath, ID);
        public void SetComment(APIConfiguration apiConfig, string comment) => CommonCommands.SetComment(apiConfig, this._objectPath, this.ID, comment);
    }
}