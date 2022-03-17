using Poushec.Mikrotik.API.TCP;
using Poushec.Mikrotik.Configurations;

namespace Poushec.Mikrotik.Models.TikBaseInterfaces
{
    public interface IRemovable : ITikObject
    {
        void Remove(APIConfiguration apiConfig);
        
        // void Remove(APIConfiguration apiConfig)
        // {
        //     var cmd = Commands.CreateCommand(apiConfig, $"{_objectPath}/remove");
        //     cmd.AddParameter(".id", ID);
        //     cmd.Execute();
        // }
    }
}