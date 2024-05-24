using System.Threading.Tasks;
using IntelliPed.Messages.Navigation;
using IntelliPed.Messages.Speech;

namespace IntelliPed.Messages;

public interface IAgentHub
{
    Task CreatePuppet();
    Task MoveToPosition(MoveToPositionRequest request);
    Task Speak(SpeakRequest request);
}