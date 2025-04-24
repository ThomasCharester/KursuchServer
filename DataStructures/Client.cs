using System.Net.Sockets;

namespace KursuchServer;

public struct Client
{
    public String Login;
    public String Password;
    public String AdminKey;
    public TcpClient Cocket;
    
    public Client(){}

    public Client(String login, String password, String adminKey, TcpClient cocket = null)
    {
        Login = login;
        Password = password;
        AdminKey = adminKey;
        Cocket = cocket;
    }
    public Client(Account account, TcpClient cocket = null)
    {
        Login = account.Login;
        Password = account.Password;
        AdminKey = account.AdminKey;
        Cocket = cocket;
    }
}