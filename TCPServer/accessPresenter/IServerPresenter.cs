using System.Net;

namespace TCPServer.accessPresenter
{
    public interface IServerPresenter
    {
        void StartListening(IPAddress ipAddress, int port);
        void StopListening();
        void GetReceivedData(byte[] data);
        void GetControlMessage(string data);
    }
}