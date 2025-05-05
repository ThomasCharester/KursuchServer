namespace KursuchServer.Services;

public class PlantService
{
    private static PlantService instance = null;

    public static PlantService Instance
    {
        get
        {
            if (instance == null) instance = new();
            return instance;
        }
        private set { instance = value; }
    }
// Добавление основных сущностей
    public void RequestAddDisease(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, 
                data.Query + $";diseaseName;{DataParsingExtension.DiseasesTable}",
                DBCommandType.ValueAdd, 
                GenericResult));
    }

    public void RequestAddMedicine(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query + $";medicineName;{DataParsingExtension.MedicinesTable}",
                DBCommandType.ValueAdd,
                GenericResult));
    }

    public void RequestAddPlant(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query + $";plantName;{DataParsingExtension.PlantsTable}",
                DBCommandType.ValueAdd,
                GenericResult));
    }

    // Работа со связующими таблицами
    public void RequestAddMedicineDisease(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query + $";medicineName,diseaseName;{DataParsingExtension.MedicineDiseasesTable}",
                DBCommandType.ValueAdd,
                GenericResult));
    }

    public void RequestAddPlantMedicine(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query + $";plantName,medicineName;{DataParsingExtension.PlantMedicinesTable}",
                DBCommandType.ValueAdd,
                GenericResult));
    }

    // Дозировки
    public void RequestAddDosage(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query + $";medicineName,diseaseName,minDosage,maxDosage;{DataParsingExtension.DosagesTable}",
                DBCommandType.ValueAdd,
                GenericResult));
    }

    // Удаление
    public void RequestDeleteDisease(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query + $";diseaseName;{DataParsingExtension.DiseasesTable}",
                DBCommandType.ValueDelete,
                GenericResult));
    }

    public void RequestDeletePlantMedicine(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query + $";plantName,medicineName;{DataParsingExtension.PlantMedicinesTable}",
                DBCommandType.ValueDelete,
                GenericResult));
    }

    // Получение данных
    public void RequestGetDiseasesForMedicine(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                $";medicineName;{DataParsingExtension.MedicineDiseasesTable}",
                DBCommandType.ValueGetAll, 
                GenericGetAllResult));
    }

    public void RequestGetDosages(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                $";medicineName,diseaseName;{DataParsingExtension.DosagesTable}",
                DBCommandType.ValueGet,
                GenericGetResult));
    }

    // Получение всех записей
    public void RequestGetAllPlants(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                $"*,{DataParsingExtension.PlantsTable}",
                DBCommandType.ValueGetAll, 
                GenericGetAllResult));
    }

    
    public void GenericResult(Object resultObj)
    {
        var result = (DBCommand)resultObj;

        if (result.Query == "ERR")
        {
            ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
                $"se{DataParsingExtension.QuerySplitter}TF{DataParsingExtension.QuerySplitter}Ошибка",
                TCPCommandType.SendSingleValue));
            return;
        }

        ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
            $"te{DataParsingExtension.QuerySplitter} {result.Query}",
            TCPCommandType.SendSingleValue));
    }

    public void GenericGetResult(Object resultObj)
    {
        var result = (DBCommand)resultObj;

        if (result.Query == "ERR")
        {
            ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
                $"se{DataParsingExtension.QuerySplitter}TF{DataParsingExtension.QuerySplitter}Ошибка",
                TCPCommandType.SendSingleValue));
            return;
        }

        TCPConnectorService.Instance.SendSingleValue(result);
    }

    public void GenericGetAllResult(Object resultObj)
    {
        var result = (DBCommand)resultObj;

        if (result.Output == null) result.Query = "ERR";

        if (result.Query == "ERR")
        {
            ServerApp.Instance.AddCommand(new TCPCommand(result.Client,
                $"se{DataParsingExtension.QuerySplitter}TF{DataParsingExtension.QuerySplitter}Ошибка",
                TCPCommandType.SendSingleValue));
            return;
        }

        TCPConnectorService.Instance.SendMultipleValue(result);
    }
// Существующие методы GenericResult, GenericGetResult, GenericGetAllResult остаются без изменений
}