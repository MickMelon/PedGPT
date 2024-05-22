using System.Threading.Tasks;
using IntelliPed.FiveM.Messages.Navigation;

namespace IntelliPed.FiveM.Messages;

public interface IAgentHub
{
    Task CreatePuppet();
    Task MoveToPosition(MoveToPositionRequest request);
}