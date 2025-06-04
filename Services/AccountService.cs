using System.Net.Sockets;
using System.Xml;
using KursuchServer.DataStructures;

namespace KursuchServer.Services;

public class AccountService
{
    private List<Client> _authorizedClients = new();

    private static AccountService instance;

    public static AccountService Instance
    {
        get
        {
            if (instance == null) instance = new();
            return instance;
        }
        private set { instance = value; }
    }

    public void SVCheats(Client client, bool cheats) //
    {
        client.SV_Cheats = cheats;
    }

    public void CheckInCharge(Object resultObj)
    {
        var result = (DBCommand)resultObj;

        if (result.Query == "ERR") return;

        var output = (String)result.Output;
        
        ServerApp.Instance.AddCommand(
            new DBCommand(result.Client,
                output.Split(DataParsingExtension.ValueSplitter)[2].DBReadable() +
                $";adminKey;{DataParsingExtension.AKTableName}",
                DBCommandType.CheckData,
                GiveCheats));
    }

    public void GiveCheats(Object resultObj)
    {
        var result = (DBCommand)resultObj;

        if (result.Query == "ERR")
        {
            GetClient(result.Client).SV_Cheats = false;

            ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
                $"lov{DataParsingExtension.QuerySplitter}{result.Query}",
                TCPCommandType.SendSingleValue));

            return;
        }
        GetClient(result.Client).SV_Cheats = true;
        
        ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
            $"loa{DataParsingExtension.QuerySplitter}{result.Query}",
            TCPCommandType.SendSingleValue));
    }

    public void RequestLogin(ACommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.ValueSplitter)[0] + DataParsingExtension.ValueSplitter +
                data.Query.Split(DataParsingExtension.ValueSplitter)[1] +
                $";login,password;{DataParsingExtension.ATableName}", DBCommandType.CheckData,
                LoginResult));

        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.ValueSplitter)[0] +
                $";login;{DataParsingExtension.ATableName}",
                DBCommandType.ValueGet,
                CheckInCharge));
    }

    public void LoginResult(Object resultObj)
    {
        var result = (DBCommand)resultObj;

        if (result.Query == "ERR")
        {
            ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
                $"ef{DataParsingExtension.QuerySplitter}LE1 Неправильный логин или пароль",
                TCPCommandType.SendSingleValue));
            return;
        }

        result.Query = result.Query.HumanReadable();

        _authorizedClients.Add(new(result.Query.Split(DataParsingExtension.QuerySplitter)[0].StringToAccountS(),
            result.Client));

        ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
            $"los{DataParsingExtension.QuerySplitter}{result.Query}",
            TCPCommandType.SendSingleValue));
    }

    public void Logout(ACommand data) //
    {
        Client client = new(data.Query.StringToAccountS(), data.Client);
        String login = new String(client.Login.Where(c => c != '\'').ToArray());

        _authorizedClients.Remove(_authorizedClients.First(x => x.Login == login));

        ServerApp.Instance.AddCommand(new TCPCommand(data.Client, $"lo{DataParsingExtension.QuerySplitter}(",
            TCPCommandType.DisconnectClient));
    }

    public void RequestRegister(ACommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client,
            $"rs;{data.Query};login,password,adminKey;{DataParsingExtension.ATableName}",
            DBCommandType.ValueAdd, TCPConnectorService.Instance.GenericResult));
    }

    public void RequestModify(ACommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                "am;" + data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                $";login,password,adminKey;{DataParsingExtension.ATableName};" +
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[1], DBCommandType.ValueModify,
                TCPConnectorService.Instance.GenericResult));
    }

    public void RequestModifySelf(ACommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, "as;" + data.Query
                                             + $";login,password;{DataParsingExtension.ATableName};" +
                                             GetClient(data.Client).ClientToStringVS(),
                DBCommandType.ValueModify, RegisterModifySelf));
    }

    public void RegisterModifySelf(Object resultObj)
    {
        var result = (DBCommand)resultObj;

        if (result.Query == "ERR")
        {
            ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
                $"ef{DataParsingExtension.QuerySplitter}am1 Аккаунта не существует",
                TCPCommandType.SendSingleValue));
            return;
        }

        Client modifyMe = GetClient(result.Query
            .Split(DataParsingExtension.QuerySplitter)[4]
            .Split(DataParsingExtension.ValueSplitter)[0].HumanReadable());

        Account modifiers = result.Query.Split(DataParsingExtension.QuerySplitter)[1].StringToAccountS();

        modifyMe.Login = modifiers.Login.HumanReadable();
        modifyMe.Password = modifiers.Password.HumanReadable();

        ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
            result.Query.Split(DataParsingExtension.QuerySplitter)[0] + DataParsingExtension.QuerySplitter +
            modifyMe.Login + DataParsingExtension.ValueSplitter + modifyMe.Password,
            TCPCommandType.SendSingleValue));
    }

    public void RequestDelete(ACommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client,
            $"ad;{data.Query};login;{DataParsingExtension.ATableName}",
            DBCommandType.ValueDelete,
            TCPConnectorService.Instance.GenericResult));
    }

// TODO Абсолютно небезопасно, ладно.
    public Client? GetClient(String login)
    {
        return _authorizedClients.FirstOrDefault(x => x.Login == login) == null
            ? null
            : _authorizedClients.FirstOrDefault(x => x.Login == login);
    }

    public Client? GetClient(TcpClient tcpClient)
    {
        return _authorizedClients.FirstOrDefault(x => x.Cocket == tcpClient) == null
            ? null
            : _authorizedClients.FirstOrDefault(x => x.Cocket == tcpClient);
    }

    public void RequestGetAllAccounts(ACommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                $"la{DataParsingExtension.QuerySplitter}*{DataParsingExtension.QuerySplitter}{DataParsingExtension.ATableName}",
                DBCommandType.ValueGetAll, TCPConnectorService.Instance.GenericGetAllResult));
    }

    public void RequestAddAccount(ACommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                "aa;" + data.Query + $";login,password,adminKey;{DataParsingExtension.ATableName}",
                DBCommandType.ValueAdd,
                TCPConnectorService.Instance.GenericResult));
    }

    public void RequestGetAllAdminKeys(ACommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                $"lk{DataParsingExtension.QuerySplitter}*{DataParsingExtension.QuerySplitter}{DataParsingExtension.AKTableName}",
                DBCommandType.ValueGetAll, TCPConnectorService.Instance.GenericGetAllResult));
    }

    public void RequestAddAdminKey(ACommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                "aka;" + data.Query + $";adminKey;{DataParsingExtension.AKTableName}",
                DBCommandType.ValueAdd,
                TCPConnectorService.Instance.GenericResult));
    }

    public void RequestAdminKeyDelete(ACommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client,
            $"akd;{data.Query};adminKey;{DataParsingExtension.AKTableName}",
            DBCommandType.ValueDelete,
            TCPConnectorService.Instance.GenericResult));
    }

    public void RequestAdminKeyModify(ACommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                "akm;" + data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                $";adminKey;{DataParsingExtension.AKTableName};" +
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[1], DBCommandType.ValueModify,
                TCPConnectorService.Instance.GenericResult));
    }
}