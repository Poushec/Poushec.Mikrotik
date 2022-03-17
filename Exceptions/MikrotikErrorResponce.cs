using System.Text.Json.Serialization;

namespace Poushec.Mikrotik.Exceptions
{
    public class MikrotikErrorResponce
    {
        [JsonPropertyName("error")]
        public int ErrorCode { get; set; }
        public string Message { get; set; }
        public string Detail { get; set; }
    }
}