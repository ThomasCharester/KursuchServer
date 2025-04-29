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

    public void RequestAddSeller(GCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, data.Query + ";sellerName,accountLogin,rate,checked;sellers",
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
            new DBCommand(data.Client, data.Query + ";methodName;paymentMethod", DBCommandType.ValueAdd,
                GenericResult));
    }

    // go with me fortuna 812
    public void RequestDeleteSeller(GCommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client, data.Query + ";accountLogin;sellers",
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
                ";sellerName,accountLogin,rate,checked;sellers;" +
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
                ";AquireType;aquireTypes;" +
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[1],
                DBCommandType.ValueModify, GenericResult));
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
}