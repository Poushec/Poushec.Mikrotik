using System.Text.Json;
using System.Text.Json.Serialization;
using Poushec.Mikrotik.API.TCP;
using Poushec.Mikrotik.Configurations;
using Poushec.Mikrotik.Exceptions;
using Poushec.Mikrotik.Models.TikBaseInterfaces;

namespace Poushec.Mikrotik.Models.System
{
    [TikPath("/system/routerboard")]
    public class RouterBoard
    {
        [JsonIgnore]
        public string _objectPath => "/system/routerboard";
        public string Routerboard { get; set; }

        [JsonPropertyName("board-name")]
        public string BoardName { get; set; }
        public string Model { get; set; }

        [JsonPropertyName("serial-number")]
        public string SerialNumber { get; set; }

        [JsonPropertyName("firmware-type")]
        public string FirmwareType { get; set; }

        [JsonPropertyName("factory-firmware")]
        public string FactoryFirmware { get; set; }

        [JsonPropertyName("current-firmware")]
        public string CurrentFirmware { get; set; }

        [JsonPropertyName("upgrade-firmware")]
        public string UpgradeFirmware { get; set; }
    }
}