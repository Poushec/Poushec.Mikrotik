namespace Poushec.Mikrotik.Models.IP.Firewall
{
    [TikPath("/ip/firewall/filter")]
    public class FilterRule : FirewallRuleBase
    {
        public override string _objectPath => "/ip/firewall/filter";
    }
}