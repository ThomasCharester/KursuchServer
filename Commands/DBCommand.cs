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

    public DBCommand(TcpClient tcpClient, String data, DBCommandType subType, Action<Object> outputFunc = null)
    {
        OutputFunc = outputFunc;
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
            {
                if(!DatabaseService.Instance.AddAccount(Data.StringToAccount()).Result) Data = "ERR";

                OutputFunc(this);
            }
                break;
            case DBCommandType.AccountDelete:
            {
                if(!DatabaseService.Instance.DeleteAccount(Data.StringToAccount().Login).Result) Data = "ERR";

                OutputFunc(this);
            }
                break;

            case DBCommandType.CheckAdminKey:
                OutputFunc(DatabaseService.Instance.CheckAdminKey(Data).Result);
                break;

            case DBCommandType.AccountGetAll:
                OutputFunc(DatabaseService.Instance.GetAllAccounts().Result);
                break;
            
            case DBCommandType.AccountGetAllAsString:
                Data = DatabaseService.Instance.GetAllAccounts().Result.AccountsToString();
                OutputFunc(this);
                break;

            case DBCommandType.CheckAccountData:
            {
                if(!DatabaseService.Instance.CheckAccountData(Data.StringToAccount()).Result) Data = "ERR";
                
                OutputFunc(this);
            }
                break;
        }

        ServerApp.Instance.AddCommand(debugCommand);
    }

    public override void Undo()
    {
    }
}