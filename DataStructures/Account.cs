using Npgsql.Replication;

namespace KursuchServer;

public struct Account
{
    public String Login;
    public String Password;
    public String AdminKey;

    public Account(){}
    public Account(String login, String password, String adminKey)
    {
        Login = login;
        Password = password;
        AdminKey = adminKey;
    }
}