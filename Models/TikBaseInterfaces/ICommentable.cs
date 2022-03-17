using Poushec.Mikrotik.API.TCP;
using Poushec.Mikrotik.Configurations;

namespace Poushec.Mikrotik.Models.TikBaseInterfaces
{
    public interface ICommentable : ITikObject
    {
        string Comment { get; set; }

        void SetComment(APIConfiguration apiConfig, string comment);
    }
}