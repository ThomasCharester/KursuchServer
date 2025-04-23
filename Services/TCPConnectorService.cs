using System.Net.Sockets;

namespace KursuchServer.Services;

public class TCPConnectorService
{
    private String _ipAddressString;
    private String _port;
    private TcpListener _tcpListener;
    private bool _serverRunning = false;
    
    private static TCPConnectorService instance = null;
    public static TCPConnectorService Instance
    {
        get { return instance;}
        private set { instance = value; }
    }

    public TCPConnectorService(String ipAddressString, String port)
    {
        instance = this;
        
        _ipAddressString = ipAddressString;
        _port = port;
    }
    public void StartServer() //
    {
        
    }
    public void StopServer() //
    {
        
    }

    private void HandleClient(TcpClient tcpClient) //
    {
        
    }

    private void SendToClient(TCPCommand data) //
    {
        
    }
    
}