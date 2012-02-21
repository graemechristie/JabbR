
namespace JabbR.Commands
{
    public interface ICommand 
    {
        void Handle(string[] parts, string userId, string roomName, string clientId, string userAgent);
    }
}
