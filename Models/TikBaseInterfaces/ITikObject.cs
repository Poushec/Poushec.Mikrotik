using System.Text.Json.Serialization;

namespace Poushec.Mikrotik.Models.TikBaseInterfaces
{
    public interface ITikObject
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        string _objectPath { get; }

        [JsonPropertyName(".id")]
        string ID { get; set; }
    }
}