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

    public DBCommand(TcpClient tcpClient, String query, DBCommandType subType, Action<Object> outputFunc = null)
    {
        OutputFunc = outputFunc;
        Type = CommandType.DBCommand;
        Client = tcpClient;
        Query = query;
        SubType = subType;
    }

    public override void Execute()
    {
        try
        {
            TCPCommand debugCommand = new TCPCommand();
            debugCommand.Type = CommandType.TCPCommand;
            debugCommand.SubType = TCPCommandType.SendSingleValue;
            debugCommand.Client = Client;
            debugCommand.Query = $"Запрос на {SubType} выполнен ";
            switch (SubType)
            {
                case DBCommandType.ValueAdd:
                {
                    if (!DatabaseService.Instance.AddValueToAnyTable(Query).Result) Query = "ERR";
                }
                    break;
                case DBCommandType.ValueDelete:
                {
                    if (!DatabaseService.Instance.DeleteValueFromAnyTable(Query).Result) Query = "ERR";
                }
                    break;
                case DBCommandType.ValueModify:
                {
                    if (!DatabaseService.Instance.ModifyValueAnyTable(Query).Result) Query = "ERR";
                }
                    break;

                case DBCommandType.ValueGetAll:
                {
                    Output = DatabaseService.Instance.GetRowsOfAnyTable(Query).Result;
                }
                    break;
                case DBCommandType.ValueGet:
                {
                    Output = DatabaseService.Instance.GetValueAnyTable(Query).Result;
                }
                    break;

                case DBCommandType.CheckData:
                {
                    if (!DatabaseService.Instance.CheckDataAnyTable(Query).Result) Query = "ERR";
                }
                    break;
            }
            OutputFunc(this);

            ServerApp.Instance.AddCommand(debugCommand);
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