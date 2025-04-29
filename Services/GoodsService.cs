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

    public void RequestAddGood(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, data.Query + ";goodName,sellerName,gameName,description,paymentMethod,aquireType,stock,price;goods",
                DBCommandType.ValueAdd,
                GenericResult));
    }
    public void RequestAddSeller(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, data.Query + ";sellerName,accountLogin,rate;sellers",
                DBCommandType.ValueAdd,
                GenericResult));
    }

    public void RequestAddGame(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, data.Query + ";gameName;games", DBCommandType.ValueAdd,
                GenericResult));
    }

    public void RequestAddAquireType(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, data.Query + ";aquireType;aquireTypes", DBCommandType.ValueAdd,
                GenericResult));
    }

    public void RequestAddPaymentMethod(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, data.Query + ";methodName;paymentMethods", DBCommandType.ValueAdd,
                GenericResult));
    }

    // go with me fortuna 812
    public void RequestDeleteSeller(GCommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client, data.Query + ";accountLogin;sellers",
            DBCommandType.ValueDelete,
            GenericResult));
    }
    public void RequestDeleteGood(GCommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client, data.Query + ";goodName,sellerName,price;goods",
            DBCommandType.ValueDelete,
            GenericResult));
    }

    public void RequestDeletePaymentMethod(GCommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client, data.Query + ";methodName;paymentMethod",
            DBCommandType.ValueDelete,
            GenericResult));
    }

    public void RequestDeleteAquireType(GCommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client, data.Query + ";aquireType;aquireTypes",
            DBCommandType.ValueDelete,
            GenericResult));
    }

    public void RequestDeleteGame(GCommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client, data.Query + ";gameName;games",
            DBCommandType.ValueDelete,
            GenericResult));
    }

    public void RequestModifySeller(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                ";sellerName,accountLogin,rate;sellers;" +
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[1],
                DBCommandType.ValueModify, GenericResult));
    }
    public void RequestModifyGood(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                ";goodName,sellerName,gameName,description,paymentMethod,aquireType,stock,price;goods;" +
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[1],
                DBCommandType.ValueModify, GenericResult));
    }
    public void RequestModifyGame(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                ";gameName;games;" +
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[1],
                DBCommandType.ValueModify, GenericResult));
    }
    
    public void RequestModifyPaymentMethod(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                ";methodName;paymentMethods;" +
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[1],
                DBCommandType.ValueModify, GenericResult));
    }
    
    public void RequestModifyAquireType(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                ";aquireType;aquireTypes;" +
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[1],
                DBCommandType.ValueModify, GenericResult));
    }
    public void RequestGetGood(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                ";goodName;goods",
                DBCommandType.ValueGet, GenericGetResult));
    }
    public void RequestGetSeller(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                ";sellerName;sellers",
                DBCommandType.ValueGet, GenericGetResult));
    }
    public void RequestGetAquireType(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                ";aquireType;aquireTypes",
                DBCommandType.ValueGet, GenericGetResult));
    }
    
    public void RequestGetGame(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                ";gameName;games",
                DBCommandType.ValueGet, GenericGetResult));
    }
    public void RequestGetPaymentMethod(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                ";methodName;paymentMethods",
                DBCommandType.ValueGet, GenericGetResult));
    }
    public void RequestGetAllGame(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                "*,games",
                DBCommandType.ValueGetAll, GenericGetAllResult));
    }
    
    public void RequestGetAllPaymentMethod(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                "*,paymentMethods",
                DBCommandType.ValueGetAll, GenericGetAllResult));
    }
    
    public void RequestGetAllSeller(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                "*,sellers",
                DBCommandType.ValueGetAll, GenericGetAllResult));
    }
    
    public void RequestGetAllAquireType(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                "*,aquireTypes",
                DBCommandType.ValueGetAll, GenericGetAllResult));
    }
    public void RequestGetAllGoods(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                "*,goods",
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