using Npgsql.Replication;

namespace KursuchServer;

public struct Account
{
    public String Login;
    public String Password;
    public String AdminKey;
}