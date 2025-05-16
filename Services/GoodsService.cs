using KursuchServer.DataStructures;

namespace KursuchServer.Services;

public class GoodsService
{
    private static GoodsService instance = null;
    
    public static GoodsService Instance
    {
        get
        {
            if (instance == null) instance = new();
            return instance;
        }
        private set { instance = value; }
    }
    private List<Good> _cart;

    public void AddToCart(Good good)
    {
        _cart.Add(good);
    }
    
    public void RequestAddGood(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, $"gta;{data.Query};goodName,sellerName,gameName,description,paymentMethod,stock,price;{DataParsingExtension.GOTableName}",
                DBCommandType.ValueAdd,
                TCPConnectorService.Instance.GenericResult));
    }
    public void RequestAddSeller(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, $"gsa;{data.Query};sellerName,accountLogin,rate;{DataParsingExtension.STableName}",
                DBCommandType.ValueAdd,
                TCPConnectorService.Instance.GenericResult));
    }

    public void RequestAddGame(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, $"gga;{data.Query};gameName;{DataParsingExtension.GATableName}", DBCommandType.ValueAdd,
                TCPConnectorService.Instance.GenericResult));
    }

    public void RequestAddPaymentMethod(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, $"gpa;{data.Query};methodName;{DataParsingExtension.PMTableName}", DBCommandType.ValueAdd,
                TCPConnectorService.Instance.GenericResult));
    }

    // go with me fortuna 812
    public void RequestDeleteSeller(GCommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client, $"gsd;{data.Query};accountLogin;{DataParsingExtension.STableName}",
            DBCommandType.ValueDelete,
            TCPConnectorService.Instance.GenericResult));
    }
    public void RequestDeleteGood(GCommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client, $"gtd;{data.Query};goodName,sellerName,gameName;{DataParsingExtension.GOTableName}",
            DBCommandType.ValueDelete,
            TCPConnectorService.Instance.GenericResult));
    }
    public void RequestDeleteGoodAP(GCommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client, $"gtpd;{data.Query};goodName,sellerName,gameName;{DataParsingExtension.GOTableName}",
            DBCommandType.ValueDelete,
            TCPConnectorService.Instance.GenericResult));
    }

    public void RequestDeletePaymentMethod(GCommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client, $"gpd;{data.Query};methodName;{DataParsingExtension.PMTableName}",
            DBCommandType.ValueDelete,
            TCPConnectorService.Instance.GenericResult));
    }

    public void RequestDeleteGame(GCommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client, $"ggd;{data.Query};gameName;{DataParsingExtension.GATableName}",
            DBCommandType.ValueDelete,
            TCPConnectorService.Instance.GenericResult));
    }

    public void RequestModifySeller(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, "gsm;" +
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                $";sellerName,accountLogin,rate;{DataParsingExtension.STableName};" +
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[1],
                DBCommandType.ValueModify, TCPConnectorService.Instance.GenericResult));
    }
    public void RequestModifyGood(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,"gtm;" +
                                      data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                                      $";goodName,sellerName,gameName,description,paymentMethod,stock,price;{DataParsingExtension.GOTableName};" +
                                      data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[1],
                DBCommandType.ValueModify, TCPConnectorService.Instance.GenericResult));
    }
    public void RequestModifyGame(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,"ggm;" +
                                      data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                                      $";gameName;{DataParsingExtension.GATableName};" +
                                      data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[1],
                DBCommandType.ValueModify, TCPConnectorService.Instance.GenericResult));
    }
    
    public void RequestModifyPaymentMethod(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,"gpm;" +
                                      data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                                      $";methodName;{DataParsingExtension.PMTableName};" +
                                      data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[1],
                DBCommandType.ValueModify, TCPConnectorService.Instance.GenericResult));
    }
    
    public void RequestGetGood(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, "sgt" + DataParsingExtension.AdditionalQuerySplitter +
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                $";goodName;{DataParsingExtension.GOTableName}",
                DBCommandType.ValueGet, GenericGetResult));
    }
    public void RequestGetSeller(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, "sgs" + DataParsingExtension.AdditionalQuerySplitter +
                                       data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                                       $";sellerName;{DataParsingExtension.STableName}",
                DBCommandType.ValueGet, GenericGetResult));
    }
    
    public void RequestGetGame(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, "sgg" + DataParsingExtension.AdditionalQuerySplitter +
                                       data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                                       $";gameName;{DataParsingExtension.GATableName}",
                DBCommandType.ValueGet, GenericGetResult));
    }
    public void RequestGetPaymentMethod(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, "sgp" + DataParsingExtension.AdditionalQuerySplitter +
                                       data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                                       $";methodName;{DataParsingExtension.PMTableName}",
                DBCommandType.ValueGet, GenericGetResult));
    }
    public void RequestGetAllGame(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, 
                $"lg{DataParsingExtension.QuerySplitter}*{DataParsingExtension.QuerySplitter}{DataParsingExtension.GATableName}",
                DBCommandType.ValueGetAll,  TCPConnectorService.Instance.GenericGetAllResult));
    }
    
    public void RequestGetAllPaymentMethod(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                $"lp{DataParsingExtension.QuerySplitter}*{DataParsingExtension.QuerySplitter}{DataParsingExtension.PMTableName}",
                DBCommandType.ValueGetAll,  TCPConnectorService.Instance.GenericGetAllResult));
    }
    
    public void RequestGetAllSeller(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                $"ls{DataParsingExtension.QuerySplitter}*{DataParsingExtension.QuerySplitter}{DataParsingExtension.STableName}",
                DBCommandType.ValueGetAll,  TCPConnectorService.Instance.GenericGetAllResult));
    }
    
    // public void RequestGetAllAquireType(GCommand data) //
    // {
    //     ServerApp.Instance.AddCommand(
    //         new DBCommand(data.Client,
    //             $"*,{DataParsingExtension.ATTableName}",
    //             DBCommandType.ValueGetAll, GenericGetAllResult));
    // }
    public void RequestGetAllGoods(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                $"ltd{DataParsingExtension.QuerySplitter}*{DataParsingExtension.QuerySplitter}{DataParsingExtension.GOTableName}",
                DBCommandType.ValueGetAll, TCPConnectorService.Instance.GenericGetAllResult));
    }
    
    public void RequestGetAllGoodsAP(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                $"ltp{DataParsingExtension.QuerySplitter}goodName,sellerName,gameName{DataParsingExtension.QuerySplitter}{DataParsingExtension.GOTableName}",
                DBCommandType.ValueGetAll, TCPConnectorService.Instance.GenericGetAllResult));
    }
    
    public void GenericGetResult(Object resultObj)
    {
        var result = (DBCommand)resultObj;
        
        if (result.Query == "ERR")
        {
            ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
                $"es{DataParsingExtension.QuerySplitter}TF{DataParsingExtension.QuerySplitter}Ошибка",
                TCPCommandType.SendSingleValue));
            return;
        }

        ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
            result,
            TCPCommandType.SendSingleValueLabeled));
        //TCPConnectorService.Instance.SendSingleValue(result);
    }
}