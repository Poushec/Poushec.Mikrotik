namespace Poushec.Mikrotik.Exceptions
{
    public class MikrotikCertificateInvalidException : Exception
    {
        public MikrotikCertificateInvalidException() : base() { }
        public MikrotikCertificateInvalidException(string message) : base(message) { }
    }
}