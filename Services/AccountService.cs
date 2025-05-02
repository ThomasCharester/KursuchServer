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

    public void ThatKey(Object resultObj)
    {
        var result = (DBCommand)resultObj;
        
        if (result.Query == "ERR")
        {
            ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
                $"lf{DataParsingExtension.QuerySplitter}LE1{DataParsingExtension.QuerySplitter}Неправильный логин или пароль",
                TCPCommandType.SendSingleValue));
            return;
        }

        result.Query = new String(result.Query.Where(c => c != '\'').ToArray());

        ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
            $"ls{DataParsingExtension.QuerySplitter} {result.Query}",
            TCPCommandType.SendSingleValue));
    }
    public void RequestLogin(ACommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, data.Query + $";login,password;{DataParsingExtension.ATableName}", DBCommandType.CheckData,
                LoginResult));
    }

    public void LoginResult(Object resultObj)
    {
        var result = (DBCommand)resultObj;

        if (result.Query == "ERR")
        {
            ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
                $"lf{DataParsingExtension.QuerySplitter}LE1{DataParsingExtension.QuerySplitter}Неправильный логин или пароль",
                TCPCommandType.SendSingleValue));
            return;
        }

        result.Query = new String(result.Query.Where(c => c != '\'').ToArray());

        _authorizedClients.Add(new(result.Query.Split(DataParsingExtension.QuerySplitter)[0].StringToAccount(), result.Client));

        ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
            $"ls{DataParsingExtension.QuerySplitter} {result.Query}",
            TCPCommandType.SendSingleValue));
    }

    public void Logout(ACommand data) //
    {
        Client client = new(data.Query.StringToAccount(), data.Client);

        _authorizedClients.Remove(_authorizedClients.First(x => x.Login == client.Login));

        ServerApp.Instance.AddCommand(new TCPCommand(data.Client, $"lo{DataParsingExtension.QuerySplitter}(",
            TCPCommandType.DisconnectClient));
    }

    public void RequestRegister(ACommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client, data.Query + $";login,password,adminKey;{DataParsingExtension.ATableName}",
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

        result.Query = new String(result.Query.Where(c => c != '\'').ToArray());

        _authorizedClients.Add(new(result.Query.StringToAccount(), result.Client));

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
                data.Query + $";login,password,adminKey;{DataParsingExtension.ATableName};" + GetClient(data.Client).Value.ClientToStringDB(),
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

        Client modifyMe = GetClient(result.Query.Split(DataParsingExtension.QuerySplitter)[3]
            .Split(DataParsingExtension.ValueSplitter)[0]).Value;
        
        Account modifiers = result.Query.Split(DataParsingExtension.QuerySplitter)[0].StringToAccount();
        
        modifyMe.Login = modifiers.Login;
        modifyMe.Password = modifiers.Password;
        modifyMe.AdminKey = modifiers.AdminKey;
        
        ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
            $"am{DataParsingExtension.QuerySplitter}s{DataParsingExtension.QuerySplitter}Данные изменены успешно)",
            TCPCommandType.SendSingleValue));
    }

    public void RequestDelete(ACommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client, data.Query + $";login;{DataParsingExtension.ATableName}",
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

    public void SVCheats(ACommand data) //
    {
    }

    // TODO Абсолютно небезопасно, ладно.
    public Client? GetClient(String login)
    {
        Client temp = _authorizedClients.FirstOrDefault(x => x.Login == login);
        return _authorizedClients.FirstOrDefault(x => x.Login == login).Login == null ? null : temp;
    }

    public Client? GetClient(TcpClient tcpClient)
    {
        Client temp = _authorizedClients.FirstOrDefault(x => x.Cocket == tcpClient);
        return _authorizedClients.FirstOrDefault(x => x.Cocket == tcpClient).Login == null ? null : temp;
    }
}