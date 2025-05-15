using System.Net.Sockets;
using System.Xml;
using KursuchServer.DataStructures;

namespace KursuchServer.Services;

public class AccountService
{
    private List<Client> _authorizedClients = new();

    private static AccountService instance = null;

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

    public void CheckAdminKey(Object resultObj)
    {
        var result = (DBCommand)resultObj;

        var client = GetClient(result.Client);

        if (result.Query == "ERR")
        {
            client.SV_Cheats = false;

            return;
        }

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
                $"lov{DataParsingExtension.QuerySplitter} {result.Query}",
                TCPCommandType.SendSingleValue));

            return;
        }

        GetClient(result.Client).SV_Cheats = true;
        ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
            $"loa{DataParsingExtension.QuerySplitter} {result.Query}",
            TCPCommandType.SendSingleValue));
    }

    public void RequestLogin(ACommand data) //
    {
        // var one = data.Query.Split(DataParsingExtension.ValueSplitter)[0];
        // var two = data.Query.Split(DataParsingExtension.ValueSplitter)[1];

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
                CheckAdminKey));
    }

    public void LoginResult(Object resultObj)
    {
        var result = (DBCommand)resultObj;

        if (result.Query == "ERR")
        {
            ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
                $"lof{DataParsingExtension.QuerySplitter}LE1{DataParsingExtension.QuerySplitter}Неправильный логин или пароль",
                TCPCommandType.SendSingleValue));
            return;
        }

        result.Query = new String(result.Query.Where(c => c != '\'').ToArray());

        _authorizedClients.Add(new(result.Query.Split(DataParsingExtension.QuerySplitter)[0].StringToAccountLP(),
            result.Client));

        ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
            $"los{DataParsingExtension.QuerySplitter} {result.Query}",
            TCPCommandType.SendSingleValue));
    }

    public void Logout(ACommand data) //
    {
        Client client = new(data.Query.StringToAccountLP(), data.Client);
        String login = new String(client.Login.Where(c => c != '\'').ToArray());

        _authorizedClients.Remove(_authorizedClients.First(x => x.Login == login));

        ServerApp.Instance.AddCommand(new TCPCommand(data.Client, $"lo{DataParsingExtension.QuerySplitter}(",
            TCPCommandType.DisconnectClient));
    }

    public void RequestRegister(ACommand data) //
    {
        if (data.Query.Split(DataParsingExtension.ValueSplitter)[2] == "\'NAN\'")
            ServerApp.Instance.AddCommand(new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.ValueSplitter)[0] + DataParsingExtension.ValueSplitter +
                data.Query.Split(DataParsingExtension.ValueSplitter)[1] +
                $";login,password;{DataParsingExtension.ATableName}",
                DBCommandType.ValueAdd, RegisterResult));
        else
            ServerApp.Instance.AddCommand(new DBCommand(data.Client,
                data.Query + $";login,password,adminKey;{DataParsingExtension.ATableName}",
                DBCommandType.ValueAdd, RegisterResult));
    }

    public void RegisterResult(Object resultObj)
    {
        var result = (DBCommand)resultObj;

        if (result.Query == "ERR")
        {
            ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
                $"rf{DataParsingExtension.QuerySplitter}re1{DataParsingExtension.QuerySplitter}Аккаунт уже существует",
                TCPCommandType.SendSingleValue));
            return;
        }

        //result.Query = new String(result.Query.Where(c => c != '\'').ToArray());

        //_authorizedClients.Add(new(result.Query.StringToAccount(), result.Client));

        ServerApp.Instance.AddCommand(new TCPCommand(result.Client, $"rs{DataParsingExtension.QuerySplitter})",
            TCPCommandType.SendSingleValue));
    }

    public void RequestModify(ACommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                $";login,password,adminKey;{DataParsingExtension.ATableName};" +
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[1], DBCommandType.ValueModify,
                RegisterModify));
    }

    public void RequestModifySelf(ACommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query + $";login,password,adminKey;{DataParsingExtension.ATableName};" +
                GetClient(data.Client).ClientToStringDB(),
                DBCommandType.ValueModify, RegisterModify));
    }

    public void RegisterModify(Object resultObj)
    {
        var result = (DBCommand)resultObj;

        if (result.Query == "ERR")
        {
            ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
                $"rf{DataParsingExtension.QuerySplitter}am1{DataParsingExtension.QuerySplitter}Аккаунта не существует",
                TCPCommandType.SendSingleValue));
            return;
        }

        Client modifyMe = GetClient(result.Query
            .Split(DataParsingExtension.QuerySplitter)[3]
            .Split(DataParsingExtension.ValueSplitter)[0]);

        Account modifiers = result.Query.Split(DataParsingExtension.QuerySplitter)[0].StringToAccount();

        modifyMe.Login = modifiers.Login;
        modifyMe.Password = modifiers.Password;
        modifyMe.AdminKey = modifiers.AdminKey;

        ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
            $"ams{DataParsingExtension.QuerySplitter}Данные изменены успешно)",
            TCPCommandType.SendSingleValue));
    }

    public void RequestDelete(ACommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client,
            data.Query + $";login;{DataParsingExtension.ATableName}",
            DBCommandType.ValueDelete,
            DeleteAccountResult));
    }

    public void DeleteAccountResult(Object resultObj)
    {
        var result = (DBCommand)resultObj;

        if (result.Query == "ERR")
            ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
                $"df{DataParsingExtension.QuerySplitter}f{DataParsingExtension.QuerySplitter}Ошибка во время удаления",
                TCPCommandType.SendSingleValue));
        else
            ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
                $"ds{DataParsingExtension.QuerySplitter}s{DataParsingExtension.QuerySplitter}Запись удалена",
                TCPCommandType.SendSingleValue));
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
}