using System.Net.Sockets;

namespace KursuchServer;

public class ACommand: Command
{
    public ACommandType SubType;

    public ACommand(TcpClient tcpClient, String data, ACommandType subType)
    {
        Type = CommandType.ACommand;
        Client = tcpClient;
        Data = data;
        SubType = subType;
    }
    public ACommand()
    {
        Type = CommandType.ACommand;
    }
    
    public override void Execute()
    {
        
    }
    public override void Undo()
    {
        
    }
}