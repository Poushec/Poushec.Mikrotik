namespace Poushec.Mikrotik.Models
{
    public class TikPathAttribute : Attribute
    {
        public string ObjectPath { get; set; }
        public TikPathAttribute(string ObjectPath) 
        {
            this.ObjectPath = ObjectPath;
        }
    }
}