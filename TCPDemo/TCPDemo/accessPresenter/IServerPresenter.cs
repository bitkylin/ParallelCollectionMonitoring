using System.Net;

namespace TCPDemo.accessPresenter
{
    public interface IServerPresenter
    {
        void StartListening(IPAddress ipAddress, int port);
        void StopListening();

        void GetReceivedData(byte[] data);
        void GetControlMessage(string data);
    }
}