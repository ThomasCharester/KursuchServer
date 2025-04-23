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

    public void Login(String login, String password) //
    {
        
    }

    public void Logout(String login) //
    {
        
    }

    public void RegisterAccount(String login, String password) //
    {
        
    }
    public void DeleteAccount(String adminKey, String login) //
    {
        
    }

    public void SVCheats(String adminKey, String login, bool cheats) //
    {
        
    }
    
    // TODO Абсолютно небезопасно, придумай метод защиты.
    public Client GetClient(String login)
    {
        return _authorizedClients.Find(x => x.Login == login);
    }

    public Client GetClient(TcpClient tcpClient)
    {
        return _authorizedClients.Find(x => x.Cocket == tcpClient);
    }
}