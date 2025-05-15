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

    public TCPCommand(TcpClient tcpClient, String query, TCPCommandType subType, Action<Object> outputFunc = null)
    {
        OutputFunc = outputFunc;
        Type = CommandType.TCPCommand;
        Client = tcpClient;
        Query = query;
        SubType = subType;
    }
    public TCPCommand(TcpClient tcpClient, Command command, TCPCommandType subType, Action<Object> outputFunc = null)
    {
        OutputFunc = outputFunc;
        Type = CommandType.TCPCommand;
        Client = tcpClient;
        Output = command.Output;
        Query = command.Query;
        SubType = subType;
    }

    public override void Execute()
    {
        try
        {
            switch (SubType)
            {
                case TCPCommandType.SendSingleValue:
                    TCPConnectorService.Instance.SendSingleValue(this);
                    break;
                case TCPCommandType.SendMultipleValue:
                    TCPConnectorService.Instance.SendMultipleValue(this);
                    break;
                case TCPCommandType.SendMultipleValueLabeled:
                    TCPConnectorService.Instance.SendMultipleValue(this);
                    break;
                case TCPCommandType.DisconnectClient:
                    TCPConnectorService.Instance.KillClient(this);
                    break;
            }
        }
        catch (Exception ex)
        {
            ServerApp.Instance.AddCommand(new TCPCommand(Client, $"ew{DataParsingExtension.QuerySplitter}{ex.Message}",
                TCPCommandType.SendSingleValue));
        }
    }

    public override void Undo()
    {
    }
}