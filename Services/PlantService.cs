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

    public void RequestAddDisease(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, data.Query + $";diseaseName;{DataParsingExtension.DiseasesTable}",
                DBCommandType.ValueAdd,
                GenericResult));
    }

    public void RequestAddMedicine(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, data.Query + $";medicineName;{DataParsingExtension.MedicinesTable}",
                DBCommandType.ValueAdd,
                GenericResult));
    }

    public void RequestAddPlant(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, data.Query + $";plantName;{DataParsingExtension.PlantsTable}",
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

    public void RequestAddDosage(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query +
                $";medicineName,diseaseName,minDosage,maxDosage;{DataParsingExtension.MedicineDosagesTable}",
                DBCommandType.ValueAdd,
                GenericResult));
    }

// Delete methods
    public void RequestDeleteDisease(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, data.Query + $";diseaseName;{DataParsingExtension.DiseasesTable}",
                DBCommandType.ValueDelete,
                GenericResult));
    }

    public void RequestDeleteMedicine(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, data.Query + $";medicineName;{DataParsingExtension.MedicinesTable}",
                DBCommandType.ValueDelete,
                GenericResult));
    }

    public void RequestDeletePlant(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, data.Query + $";plantName;{DataParsingExtension.PlantsTable}",
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

    public void RequestDeleteDosage(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query + $";medicineName,diseaseName;{DataParsingExtension.MedicineDosagesTable}",
                DBCommandType.ValueDelete,
                GenericResult));
    }

// Modify methods
    public void RequestModifyDisease(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                $";diseaseName;{DataParsingExtension.DiseasesTable};" +
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[1],
                DBCommandType.ValueModify, GenericResult));
    }

    public void RequestModifyMedicine(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                $";medicineName;{DataParsingExtension.MedicinesTable};" +
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[1],
                DBCommandType.ValueModify, GenericResult));
    }

    public void RequestModifyPlant(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                $";plantName;{DataParsingExtension.PlantsTable};" +
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[1],
                DBCommandType.ValueModify, GenericResult));
    }

// Get methods
    public void RequestGetDisease(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                $";diseaseName;{DataParsingExtension.DiseasesTable}",
                DBCommandType.ValueGet, GenericGetResult));
    }

    public void RequestGetMedicine(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                $";medicineName;{DataParsingExtension.MedicinesTable}",
                DBCommandType.ValueGet, GenericGetResult));
    }

    public void RequestGetPlant(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                $";plantName;{DataParsingExtension.PlantsTable}",
                DBCommandType.ValueGet, GenericGetResult));
    }

// GetAll methods
    public void RequestGetAllDiseases(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                $"*,{DataParsingExtension.DiseasesTable}",
                DBCommandType.ValueGetAll, GenericGetAllResult));
    }

    public void RequestGetAllMedicines(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                $"*,{DataParsingExtension.MedicinesTable}",
                DBCommandType.ValueGetAll, GenericGetAllResult));
    }

    public void RequestGetAllPlants(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                $"*,{DataParsingExtension.PlantsTable}",
                DBCommandType.ValueGetAll, GenericGetAllResult));
    }

    public void RequestGetDosage(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                $";medicineName,diseaseName;{DataParsingExtension.MedicineDosagesTable}",
                DBCommandType.ValueGet, GenericGetResult));
    }

    public void RequestGetAllDosages(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                $"*,{DataParsingExtension.MedicineDosagesTable}",
                DBCommandType.ValueGetAll, GenericGetAllResult));
    }

// Методы для работы с растениями-лекарствами
    public void RequestGetPlantMedicine(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                $";plantName,medicineName;{DataParsingExtension.PlantMedicinesTable}",
                DBCommandType.ValueGet, GenericGetResult));
    }

    public void RequestGetAllPlantMedicines(GCommand data)
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                $"*,{DataParsingExtension.PlantMedicinesTable}",
                DBCommandType.ValueGetAll, GenericGetAllResult));
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