using System.Net.Sockets;
using KursuchServer.DataStructures;

namespace KursuchServer;

public record struct Client
{
    public String Login;
    public String Password;
    public String AdminKey;
    public TcpClient Cocket;
    public bool SV_Cheats = false;
    public Client(){}

    public Client(String login, String password, String adminKey, TcpClient cocket = null, bool svCheats = false)
    {
        Login = login;
        Password = password;
        AdminKey = adminKey;
        Cocket = cocket;
        SV_Cheats = svCheats;
    }
    public Client(Account account, TcpClient cocket = null, bool svCheats = false)
    {
        Login = account.Login;
        Password = account.Password;
        AdminKey = account.AdminKey;
        Cocket = cocket;
        SV_Cheats = svCheats;
    }
}