using System.Globalization;
using System.Numerics;
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
    
    public static readonly string DiseasesTable = "Diseases";
    public static readonly string MedicinesTable = "Medicines";
    public static readonly string PlantsTable = "Plants";
    public static readonly string MedicineDiseasesTable = "MedicineDiseases";
    public static readonly string PlantsMedicinesTable = "PlantsMedicines";
    public static readonly string PlantsDiseasesTable = "PlantsDiseases";

    public static Vector2 StringToVector2(this string vector)
    {
        return new Vector2(float.Parse(vector.Split(ValueSplitter)[0], CultureInfo.InvariantCulture),
            float.Parse(vector.Split(ValueSplitter)[1], CultureInfo.InvariantCulture));
    }

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
        return str.Replace("\'", "");
    }

    public static String ClientToStringVS(this Client client)
    {
        return client.Login.DBReadable() + ValueSplitter + client.Password.DBReadable();
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