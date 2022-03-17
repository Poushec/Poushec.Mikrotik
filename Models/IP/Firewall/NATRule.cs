namespace Poushec.Mikrotik.Models.IP.Firewall
{
    [TikPath("/ip/firewall/nat")]
    public class NATRule : FirewallRuleBase
    {
        public override string _objectPath => "/ip/firewall/nat";
    }
}