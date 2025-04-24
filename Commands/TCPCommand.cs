using System.Net.Sockets;
using KursuchServer.Services;

namespace KursuchServer;

public class TCPCommand : Command
{
    public TCPCommandType SubType;

    public TCPCommand()
    {
        Type = CommandType.TCPCommand;
    }

    public TCPCommand(TcpClient tcpClient, String data, TCPCommandType subType, Action<Object> outputFunc = null)
    {
        OutputFunc = outputFunc;
        Type = CommandType.TCPCommand;
        Client = tcpClient;
        Data = data;
        SubType = subType;
    }

    public override void Execute()
    {
        try
        {
            switch (SubType)
            {
                case TCPCommandType.SendDefaultMessage:
                    TCPConnectorService.Instance.SendToClient(this);
                    break;
                case TCPCommandType.DisconnectClient:
                    TCPConnectorService.Instance.KillClient(this);
                    break;
            }
        }
        catch (Exception ex)
        {
            ServerApp.Instance.AddCommand(new TCPCommand(Client, $"ew;{ex.Message}",
                TCPCommandType.SendDefaultMessage));
        }
    }

    public override void Undo()
    {
    }
}