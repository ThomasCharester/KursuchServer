namespace KursuchServer.DataStructures;

public struct Account
{
    public String Login;
    public String Password;
    public String AdminKey;
    
    public Account(){}
    public Account(String login, String password, String adminKey = "NAN")
    {
        Login = login;
        Password = password;
        AdminKey = adminKey;
    }
}