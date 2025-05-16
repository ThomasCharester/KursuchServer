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

    public static readonly String GATableName = "Games";
    public static readonly String PMTableName = "PaymentMethod";
    public static readonly String STableName = "Sellers";
    public static readonly String GOTableName = "Goods";

    public const string DiseasesTable = "Diseases";
    public const string MedicinesTable = "Medicines";
    public const string PlantsTable = "Plants";
    public const string MedicineDiseasesTable = "MedicineDiseases";
    public const string PlantMedicinesTable = "PlantMedicines";
    public const string DosagesTable = "Dosages";

    public static String DBReadable(this String str)
    {
        return '\'' + str + '\'';
    }
    public static String NANIsNULL(this String str)
    {
        return str == " = 'NAN'" ? " IS NULL" : str;
    }
    public static String NANToNULL(this String str)
    {
        return str == "'NAN'" ? "NULL" : str;
    }
    public static String HumanReadable(this String str)
    {
        return str.Replace("\'","");
    }
    public static String ClientToStringVS(this Client client)
    {
        return client.Login.DBReadable()+ValueSplitter+client.Password.DBReadable();
    }
    public static Account StringToAccount(this String account)
    {
        return new Account(account.Split(ValueSplitter)[0], account.Split(ValueSplitter)[1],
            account.Split(ValueSplitter)[2]);
    }
    public static Account StringToAccountS(this String account)
    {
        return new Account(account.Split(ValueSplitter)[0], account.Split(ValueSplitter)[1]);
    }
}