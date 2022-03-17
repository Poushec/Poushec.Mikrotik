using System.Text.Json.Serialization;
using Poushec.Mikrotik.API.TCP;
using Poushec.Mikrotik.Configurations;

namespace Poushec.Mikrotik.Models.TikBaseInterfaces
{
    public interface IDisableable : ITikObject
    {
        [JsonIgnore]
        string Disabled { get; set; }

        void Disable(APIConfiguration apiConfig);
        void Enable(APIConfiguration apiConfig);
    }
}