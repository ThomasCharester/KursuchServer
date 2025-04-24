using System.Net.Sockets;

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

    public void RequestLogin(ACommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, data.Data, DBCommandType.CheckAccountData, LoginResult));
    }

    public void LoginResult(Object resultObj)
    {
        var result = (DBCommand)resultObj;

        if (result.Data == "ERR")
        {
            ServerApp.Instance.AddCommand(new TCPCommand(result.Client, "lf;LE1;Неправильный логин или пароль",
                TCPCommandType.SendDefaultMessage));
            return;
        }

        _authorizedClients.Add(new(result.Data.StringToAccount(), result.Client));

        ServerApp.Instance.AddCommand(new TCPCommand(result.Client, "ls;)",
            TCPCommandType.SendDefaultMessage));
    }

    public void Logout(ACommand data) //
    {
        Client client = data.Data.StringToAccount().AccountToClient();
        client.Cocket = data.Client;
        _authorizedClients.Remove(client); // TODO Эффективность

        ServerApp.Instance.AddCommand(new TCPCommand(data.Client, "lo;(",
            TCPCommandType.DisconnectClient));
    }

    public void RequestRegister(ACommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client, data.Data, DBCommandType.AccountAdd, RegisterResult));
    }

    public void RegisterResult(Object resultObj)
    {
        var result = (DBCommand)resultObj;

        if (result.Data == "ERR")
        {
            ServerApp.Instance.AddCommand(new TCPCommand(result.Client, "rf;re1;Аккаунт уже существует",
                TCPCommandType.SendDefaultMessage));
            return;
        }

        _authorizedClients.Add(new(result.Data.StringToAccount(), result.Client));

        ServerApp.Instance.AddCommand(new TCPCommand(result.Client, "rs;)",
            TCPCommandType.SendDefaultMessage));
    }
    public void RequestModify(ACommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client, data.Data, DBCommandType.AccountModify, RegisterModify));
    }

    public void RegisterModify(Object resultObj)
    {
        var result = (DBCommand)resultObj;

        if (result.Data == "ERR")
        {
            ServerApp.Instance.AddCommand(new TCPCommand(result.Client, "rf;am1;Аккаунта не существует",
                TCPCommandType.SendDefaultMessage));
            return;
        }

        ServerApp.Instance.AddCommand(new TCPCommand(result.Client, "am;s;Данные изменены успешно)",
            TCPCommandType.SendDefaultMessage));
    }
    public void RequestDelete(ACommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client, data.Data, DBCommandType.AccountDelete, DeleteAccountResult));
    }

    public void DeleteAccountResult(Object resultObj)
    {
        var result = (DBCommand)resultObj;

        if (result.Data == "ERR")
            ServerApp.Instance.AddCommand(new TCPCommand(result.Client, "df;f;Ошибка во время удаления",
                TCPCommandType.SendDefaultMessage));
        else
            ServerApp.Instance.AddCommand(new TCPCommand(result.Client, "ds;s;Запись удалена",
                TCPCommandType.SendDefaultMessage));
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