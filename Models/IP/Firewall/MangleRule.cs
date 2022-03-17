namespace Poushec.Mikrotik.Models.IP.Firewall
{
    [TikPath("/ip/firewall/mangle")]
    public class MangleRule : FirewallRuleBase
    {
        public override string _objectPath => "/ip/firewall/mangle";
    }
}