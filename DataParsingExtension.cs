using System.Text;
using KursuchServer.DataStructures;

namespace KursuchServer;

public static class DataParsingExtension
{
    public static readonly char ValueSplitter = ',';
    public static readonly char QuerySplitter = ';';
    public static readonly char AdditionalQuerySplitter = '|';

    public static readonly String ATableName = "Accounts";
    public static readonly String AKTableName = "AdminKeys";
    
    public static readonly String ATTableName = "AquireTypes";
    public static readonly String GATableName = "Games";
    public static readonly String PMTableName = "PaymentMethods";
    public static readonly String STableName = "Sellers";
    public static readonly String GOTableName = "Goods";
    
    public const string DiseasesTable = "Diseases";
    public const string MedicinesTable = "Medicines";
    public const string PlantsTable = "Plants";
    public const string MedicineDiseasesTable = "MedicineDiseases";
    public const string PlantMedicinesTable = "PlantMedicines";
    public const string DosagesTable = "Dosages";

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
        return $"{account.Login}{ValueSplitter}{account.Password}{ValueSplitter}{account.AdminKey}";
    }
    public static String AccountToStringDB(this Account account)
    {
        return $"\'{account.Login}\'{ValueSplitter}\'{account.Password}\'{ValueSplitter}\'{account.AdminKey}\'";
    }
    public static String ClientToString(this Client client)
    {
        return $"{client.Login}{ValueSplitter}{client.Password}{ValueSplitter}{client.AdminKey}";
    }
    public static String ClientToStringDB(this Client client)
    {
        return $"\'{client.Login}\'{ValueSplitter}\'{client.Password}\'{ValueSplitter}\'{client.AdminKey}\'";
    }

    public static Account StringToAccount(this String account)
    {
        return new Account(account.Split(ValueSplitter)[0], account.Split(ValueSplitter)[1],
            account.Split(ValueSplitter)[2]);
    }
    public static Account StringToAccountLP(this String account)
    {
        return new Account(account.Split(ValueSplitter)[0], account.Split(ValueSplitter)[1]);
    }

    public static String AccountsToString(this List<Account> accounts)
    {
        StringBuilder builder = new();
        foreach (Account account in accounts)
            builder.Append(account.AccountToString() + '\n');
        return builder.ToString();
    }
    
}