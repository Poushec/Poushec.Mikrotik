namespace Poushec.Mikrotik.API.TCP
{
    public class TikAPIException : Exception
    {
        public TikAPIException() : base() { }
        public TikAPIException(string message) : base(message) { }
    }
}