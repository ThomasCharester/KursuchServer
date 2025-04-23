using System.Net.Sockets;

namespace KursuchServer.Services;

public class AccountService
{
    List<Client> clients = new();
    
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

    public void RegisterAccount(String adminKey, String login, String password) //
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
        return clients.Find(x => x.Login == login);
    }

    public Client GetClient(TcpClient tcpClient)
    {
        return clients.Find(x => x.Cocket == tcpClient);
    }
}