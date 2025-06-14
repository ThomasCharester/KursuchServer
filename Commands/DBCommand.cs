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
                    Output = DatabaseService.Instance
                        .GetRowsOfAnyTable(Query).Result;
                    //
                }
                    break;

                case DBCommandType.ValueGetAllCondition:
                {
                    Output = DatabaseService.Instance
                        .GetRowsOfAnyTableCondition(Query).Result;
                    //
                }
                    break;
                case DBCommandType.ValueGet:
                {
                    Output = DatabaseService.Instance.GetValueAnyTable(Query).Result;
                }
                    break;
                case DBCommandType.ValueGetFirstElementQuery:
                {
                    Output = DatabaseService.Instance.GetValueAnyTable(Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0]).Result;
                    Query = Query.Split(DataParsingExtension.AdditionalQuerySplitter)[1];
                }
                    break;

                case DBCommandType.CheckData:
                {
                    if (!DatabaseService.Instance.CheckDataAnyTable(Query).Result) Query = "ERR";
                }
                    break;
                
                case DBCommandType.ExecuteFunction:
                {
                    if (!DatabaseService.Instance.ExecuteFunction(Query).Result) Query = "ERR";
                }
                    break;
            }

            OutputFunc(this);
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