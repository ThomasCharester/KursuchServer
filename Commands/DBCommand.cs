using System.Net.Sockets;
using KursuchServer.Services;

namespace KursuchServer;

public class DBCommand : Command
{
    public DBCommandType SubType;
    public DBCommand()
    {
        Type = CommandType.DBCommand;
    }
    public DBCommand(TcpClient tcpClient, String data, DBCommandType subType)
    {
        Type = CommandType.DBCommand;
        Client = tcpClient;
        Data = data;
        SubType = subType;
    }
    public override void Execute()
    {
        TCPCommand debugCommand = new TCPCommand();
        debugCommand.Type = CommandType.TCPCommand;
        debugCommand.SubType = TCPCommandType.SendDefaultMessage;
        debugCommand.Client = Client;
        debugCommand.Data = $"Запрос на {SubType} выполнен ";
        
        switch (SubType)
        {
            case DBCommandType.AccountAdd:
                DatabaseService.Instance.AddAccount(Data.StringToAccount());
                break;
            
            case DBCommandType.CheckAdminKey:
                DatabaseService.Instance.CheckAdminKey(Data);
                break;
            
            case DBCommandType.AccountGetAll:
                OutputFunc(DatabaseService.Instance.GetAllAccounts().Result);
                break;
        }
        
        ServerApp.Instance.AddCommand(debugCommand);
    }
    public override void Undo()
    {
        
    }
}