using System.Net.Sockets;
using KursuchServer.Services;

namespace KursuchServer;

public class GCommand: Command
{
    public GCommandType SubType;

    public GCommand()
    {
        Type = CommandType.GCommand;
    }

    public GCommand(TcpClient tcpClient, String query, GCommandType subType, Action<Object> outputFunc = null)
    {
        OutputFunc = outputFunc;
        Type = CommandType.TCPCommand;
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
                case GCommandType.SellerAdd:
                    break;
                case GCommandType.SellerDelete:
                    break;
                case GCommandType.SellerModify:
                    break;
                case GCommandType.SellerGet:
                    break;
                case GCommandType.SellerGetAll:
                    break;
                case GCommandType.GoodAdd:
                    break;
                case GCommandType.GoodDelete:
                    break;
                case GCommandType.GoodModify:
                    break;
                case GCommandType.GoodGet:
                    break;
                case GCommandType.GoodGetAll:
                    break;
                case GCommandType.AquireTypeAdd:
                    break;
                case GCommandType.AquireTypeDelete:
                    break;
                case GCommandType.AquireTypeGet:
                    break;
                case GCommandType.AquireTypeGetAll:
                    break;
                case GCommandType.PaymentMethodAdd:
                    break;
                case GCommandType.PaymentMethodDelete:
                    break;
                case GCommandType.PaymentMethodGet:
                    break;
                case GCommandType.PaymentMethodGetAll:
                    break;
                case GCommandType.GameAdd:
                    break;
                case GCommandType.GameDelete:
                    break;
                case GCommandType.GameGet:
                    break;
                case GCommandType.GameGetAll:
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