using System.Net.Sockets;
using KursuchServer.Services;

namespace KursuchServer;

public class GCommand : Command
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
                case GCommandType.SellerGet:
                    GoodsService.Instance.RequestGetSeller(this);
                    break;
                case GCommandType.SellerGetAll:
                    GoodsService.Instance.RequestGetAllSeller(this);
                    break;
                case GCommandType.AquireTypeGet:
                    GoodsService.Instance.RequestGetAquireType(this);
                    break;
                case GCommandType.AquireTypeGetAll:
                    GoodsService.Instance.RequestGetAllAquireType(this);
                    break;
                case GCommandType.PaymentMethodGet:
                    GoodsService.Instance.RequestGetPaymentMethod(this);
                    break;
                case GCommandType.PaymentMethodGetAll:
                    GoodsService.Instance.RequestGetAllPaymentMethod(this);
                    break;
                case GCommandType.GameGet:
                    GoodsService.Instance.RequestGetGame(this);
                    break;
                case GCommandType.GameGetAll:
                    GoodsService.Instance.RequestGetAllGame(this);
                    break;
                case GCommandType.GoodGet:
                    GoodsService.Instance.RequestGetGood(this);
                    break;
                case GCommandType.GoodGetAll:
                    GoodsService.Instance.RequestGetAllGoods(this);
                    break;
            }

            if (AccountService.Instance.GetClient(Client)?.AdminKey != "NAN")
                switch (SubType)
                {
                    case GCommandType.SellerAdd:
                        GoodsService.Instance.RequestAddSeller(this);
                        break;
                    case GCommandType.SellerDelete:
                        GoodsService.Instance.RequestDeleteSeller(this);
                        break;
                    case GCommandType.SellerModify:
                        GoodsService.Instance.RequestModifySeller(this);
                        break;
                    case GCommandType.GoodAdd:
                        GoodsService.Instance.RequestAddGood(this);
                        break;
                    case GCommandType.GoodDelete:
                        GoodsService.Instance.RequestDeleteGood(this);
                        break;
                    case GCommandType.GoodModify:
                        GoodsService.Instance.RequestModifyGood(this);
                        break;
                    case GCommandType.AquireTypeAdd:
                        GoodsService.Instance.RequestAddAquireType(this);
                        break;
                    case GCommandType.AquireTypeDelete:
                        GoodsService.Instance.RequestDeleteAquireType(this);
                        break;
                    case GCommandType.AquireTypeModify:
                        GoodsService.Instance.RequestModifyAquireType(this);
                        break;
                    case GCommandType.PaymentMethodAdd:
                        GoodsService.Instance.RequestAddPaymentMethod(this);
                        break;
                    case GCommandType.PaymentMethodDelete:
                        GoodsService.Instance.RequestDeletePaymentMethod(this);
                        break;
                    case GCommandType.PaymentMethodModify:
                        GoodsService.Instance.RequestModifyPaymentMethod(this);
                        break;
                    case GCommandType.GameAdd:
                        GoodsService.Instance.RequestAddGame(this);
                        break;
                    case GCommandType.GameDelete:
                        GoodsService.Instance.RequestDeleteGame(this);
                        break;
                    case GCommandType.GameModify:
                        GoodsService.Instance.RequestModifyGame(this);
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