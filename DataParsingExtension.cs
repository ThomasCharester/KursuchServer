using System.Text;

namespace KursuchServer;

public static class DataParsingExtension
{
    public static Account ClientToAccount(this Client client)
    {
        return new Account(client.Login, client.Password, client.AdminKey);
    }

    public static Client AccountToClient(this Account account)
    {
        return new Client(account.Login, account.Password, account.AdminKey);
    }

    public static String AccountToString(this Account account)
    {
        return $"{account.Login}|{account.Password}|{account.AdminKey}";
    }

    public static Account StringToAccount(this String account)
    {
        return new Account(account.Split('|')[0], account.Split('|')[1], account.Split('|')[2]);
    }

    public static String AccountsToString(this List<Account> accounts)
    {
        StringBuilder builder = new();
        foreach (Account account in accounts)
            builder.Append(account.AccountToString()+'\n');
        return builder.ToString();
    }
}