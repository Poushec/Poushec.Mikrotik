namespace Poushec.Mikrotik.Exceptions
{
    public class MikrotikException : Exception
    {
        public MikrotikException() : base() { }
        public MikrotikException(MikrotikErrorResponce error) : base($"{error.Message} {error.Detail??=""}") { }        
    }
}