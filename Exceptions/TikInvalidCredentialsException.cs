namespace Poushec.Mikrotik.Exceptions
{
    public class TikInvalidCredentialsException : Exception
    {
        public TikInvalidCredentialsException() : base() { }
        public TikInvalidCredentialsException(string message) : base(message) { }       
    }
}