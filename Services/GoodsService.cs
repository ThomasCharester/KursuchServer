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
            new DBCommand(data.Client, data.Query + $";goodName,sellerName,gameName,description,paymentMethod,stock,price;{DataParsingExtension.GOTableName}",
                DBCommandType.ValueAdd,
                GenericResult));
    }
    public void RequestAddSeller(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, data.Query + $";sellerName,accountLogin,rate;{DataParsingExtension.STableName}",
                DBCommandType.ValueAdd,
                GenericResult));
    }

    public void RequestAddGame(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, data.Query + $";gameName;{DataParsingExtension.GATableName}", DBCommandType.ValueAdd,
                GenericResult));
    }

    // public void RequestAddAquireType(GCommand data) //
    // {
    //     ServerApp.Instance.AddCommand(
    //         new DBCommand(data.Client, data.Query + $";aquireType;{DataParsingExtension.ATTableName}", DBCommandType.ValueAdd,
    //             GenericResult));
    // }

    public void RequestAddPaymentMethod(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, data.Query + $";methodName;{DataParsingExtension.PMTableName}", DBCommandType.ValueAdd,
                GenericResult));
    }

    // go with me fortuna 812
    public void RequestDeleteSeller(GCommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client, data.Query + $";accountLogin;{DataParsingExtension.STableName}",
            DBCommandType.ValueDelete,
            GenericResult));
    }
    public void RequestDeleteGood(GCommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client, data.Query + $";goodName,sellerName,price;{DataParsingExtension.GOTableName}",
            DBCommandType.ValueDelete,
            GenericResult));
    }

    public void RequestDeletePaymentMethod(GCommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client, data.Query + $";methodName;{DataParsingExtension.PMTableName}",
            DBCommandType.ValueDelete,
            GenericResult));
    }

    // public void RequestDeleteAquireType(GCommand data) //
    // {
    //     ServerApp.Instance.AddCommand(new DBCommand(data.Client, data.Query + $";aquireType;{DataParsingExtension.ATTableName}",
    //         DBCommandType.ValueDelete,
    //         GenericResult));
    // }

    public void RequestDeleteGame(GCommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client, data.Query + $";gameName;{DataParsingExtension.GATableName}",
            DBCommandType.ValueDelete,
            GenericResult));
    }

    public void RequestModifySeller(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                $";sellerName,accountLogin,rate;{DataParsingExtension.STableName};" +
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[1],
                DBCommandType.ValueModify, GenericResult));
    }
    public void RequestModifyGood(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                $";goodName,sellerName,gameName,description,paymentMethod,stock,price;{DataParsingExtension.GOTableName};" +
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[1],
                DBCommandType.ValueModify, GenericResult));
    }
    public void RequestModifyGame(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                $";gameName;{DataParsingExtension.GATableName};" +
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[1],
                DBCommandType.ValueModify, GenericResult));
    }
    
    public void RequestModifyPaymentMethod(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                $";methodName;{DataParsingExtension.PMTableName};" +
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[1],
                DBCommandType.ValueModify, GenericResult));
    }
    
    // public void RequestModifyAquireType(GCommand data) //
    // {
    //     ServerApp.Instance.AddCommand(
    //         new DBCommand(data.Client,
    //             data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
    //             $";aquireType;{DataParsingExtension.ATTableName};" +
    //             data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[1],
    //             DBCommandType.ValueModify, GenericResult));
    // }
    public void RequestGetGood(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                $";goodName;{DataParsingExtension.GOTableName}",
                DBCommandType.ValueGet, GenericGetResult));
    }
    public void RequestGetSeller(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                $";sellerName;{DataParsingExtension.STableName}",
                DBCommandType.ValueGet, GenericGetResult));
    }
    // public void RequestGetAquireType(GCommand data) //
    // {
    //     ServerApp.Instance.AddCommand(
    //         new DBCommand(data.Client,
    //             data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
    //             $";aquireType;{DataParsingExtension.ATTableName}",
    //             DBCommandType.ValueGet, GenericGetResult));
    // }
    
    public void RequestGetGame(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                $";gameName;{DataParsingExtension.GATableName}",
                DBCommandType.ValueGet, GenericGetResult));
    }
    public void RequestGetPaymentMethod(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                $";methodName;{DataParsingExtension.PMTableName}",
                DBCommandType.ValueGet, GenericGetResult));
    }
    public void RequestGetAllGame(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                $"lg{DataParsingExtension.QuerySplitter}*,{DataParsingExtension.GATableName}",
                DBCommandType.ValueGetAll, GenericGetAllResult));
    }
    
    public void RequestGetAllPaymentMethod(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                $"lp{DataParsingExtension.QuerySplitter}*,{DataParsingExtension.PMTableName}",
                DBCommandType.ValueGetAll, GenericGetAllResult));
    }
    
    public void RequestGetAllSeller(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                $"ls{DataParsingExtension.QuerySplitter}*,{DataParsingExtension.STableName}",
                DBCommandType.ValueGetAll, GenericGetAllResult));
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
                $"lt{DataParsingExtension.QuerySplitter}*,{DataParsingExtension.GOTableName}",
                DBCommandType.ValueGetAll, GenericGetAllResult));
    }
    
    
    public void GenericResult(Object resultObj)
    {
        var result = (DBCommand)resultObj;

        if (result.Query == "ERR")
        {
            ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
                $"se{DataParsingExtension.QuerySplitter}TF{DataParsingExtension.QuerySplitter}Ошибка",
                TCPCommandType.SendSingleValue));
            return;
        }

        ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
            $"te{DataParsingExtension.QuerySplitter} {result.Query}",
            TCPCommandType.SendSingleValue));
    }
    
    public void GenericGetResult(Object resultObj)
    {
        var result = (DBCommand)resultObj;
        
        if (result.Query == "ERR")
        {
            ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
                $"se{DataParsingExtension.QuerySplitter}TF{DataParsingExtension.QuerySplitter}Ошибка",
                TCPCommandType.SendSingleValue));
            return;
        }

        TCPConnectorService.Instance.SendSingleValue(result);
    }
    public void GenericGetAllResult(Object resultObj)
    {
        var result = (DBCommand)resultObj;

        if (result.Output == null) result.Query = "ERR";
        
        if (result.Query == "ERR")
        {
            ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
                $"se{DataParsingExtension.QuerySplitter}TF{DataParsingExtension.QuerySplitter}Ошибка",
                TCPCommandType.SendSingleValue));
            return;
        }

        TCPConnectorService.Instance.SendMultipleValue(result);
    }
}