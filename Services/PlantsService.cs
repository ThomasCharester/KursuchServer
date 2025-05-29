namespace KursuchServer.Services;

public class PlantsService
{
    private static PlantsService instance = null;

    public static PlantsService Instance
    {
        get
        {
            if (instance == null) instance = new();
            return instance;
        }
        private set { instance = value; }
    }
    
    public void RequestAddPlant(PCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                $"ppa;{data.Query};plantName;{DataParsingExtension.PlantsTable}",
                DBCommandType.ValueAdd,
                TCPConnectorService.Instance.GenericResult));
    }

    public void RequestAddDisease(PCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, $"pda;{data.Query};diseaseName;{DataParsingExtension.DiseasesTable}",
                DBCommandType.ValueAdd,
                TCPConnectorService.Instance.GenericResult));
    }

    public void RequestAddMedicine(PCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, $"pma;{data.Query};medicineName,concentration;{DataParsingExtension.MedicinesTable}",
                DBCommandType.ValueAdd,
                TCPConnectorService.Instance.GenericResult));
    }
    public void RequestAddPlantDisease(PCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                $"ppda;{data.Query};plantId,diseaseId;{DataParsingExtension.PlantsDiseasesTable}",
                DBCommandType.ValueAdd,
                TCPConnectorService.Instance.GenericResult));
    }

    public void RequestAddDiseaseMedicine(PCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, $"pmda;{data.Query};diseaseId,medicineId,minDosage,maxDosage;{DataParsingExtension.MedicineDiseasesTable}",
                DBCommandType.ValueAdd,
                TCPConnectorService.Instance.GenericResult));
    }

    public void RequestAddPlantMedicine(PCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, $"ppma;{data.Query};plantId,medicineId;{DataParsingExtension.PlantsMedicinesTable}",
                DBCommandType.ValueAdd,
                TCPConnectorService.Instance.GenericResult));
    }
    
    public void RequestDeletePlant(PCommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client,
            $"ppd;{data.Query};plantId;{DataParsingExtension.PlantsTable}",
            DBCommandType.ValueDelete,
            TCPConnectorService.Instance.GenericResult));
    }

    public void RequestDeleteMedicine(PCommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client,
            $"pmd;{data.Query};medicineId;{DataParsingExtension.MedicinesTable}",
            DBCommandType.ValueDelete,
            TCPConnectorService.Instance.GenericResult));
    }

    public void RequestDeleteDisease(PCommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client,
            $"pdd;{data.Query};diseaseId;{DataParsingExtension.DiseasesTable}",
            DBCommandType.ValueDelete,
            TCPConnectorService.Instance.GenericResult));
    }

    public void RequestDeletePlantDisease(PCommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client,
            $"ppdd;{data.Query};plantId,diseaseId;{DataParsingExtension.PlantsDiseasesTable}",
            DBCommandType.ValueDelete,
            TCPConnectorService.Instance.GenericResult));
    }

    public void RequestDeletePlantMedicine(PCommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client,
            $"ppmd;{data.Query};plantId,medicineId;{DataParsingExtension.PlantsMedicinesTable}",
            DBCommandType.ValueDelete,
            TCPConnectorService.Instance.GenericResult));
    }

    public void RequestDeleteDiseaseMedicine(PCommand data) //
    {
        ServerApp.Instance.AddCommand(new DBCommand(data.Client,
            $"pmdd;{data.Query};diseaseId,medicineId;{DataParsingExtension.MedicineDiseasesTable}",
            DBCommandType.ValueDelete,
            TCPConnectorService.Instance.GenericResult));
    }

    public void RequestModifyPlant(PCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, "ppm;" +
                                       data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                                       $";plantName;{DataParsingExtension.PlantsTable};" +
                                       data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[1],
                DBCommandType.ValueModify, TCPConnectorService.Instance.GenericResult));
    }

    public void RequestModifyMedicine(PCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, "pmm;" +
                                       data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                                       $";medicineName,concentration;{DataParsingExtension.MedicinesTable};" +
                                       data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[1],
                DBCommandType.ValueModify, TCPConnectorService.Instance.GenericResult));
    }

    public void RequestModifyDisease(PCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, "pdm;" +
                                       data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                                       $";diseaseName;{DataParsingExtension.DiseasesTable};" +
                                       data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[1],
                DBCommandType.ValueModify, TCPConnectorService.Instance.GenericResult));
    }

    public void RequestModifyDiseaseMedicine(PCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, "pmdm;" +
                                       data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[0] +
                                       $";diseaseId,medicineId,minDosage,maxDosage;{DataParsingExtension.MedicineDiseasesTable};" +
                                       data.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[1],
                DBCommandType.ValueModify, TCPConnectorService.Instance.GenericResult));
    }

    public void RequestGetPlant(PCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, "ppg;" +
                                       data.Query.Split(DataParsingExtension.QuerySplitter)[0] +
                                       $";plantId;{DataParsingExtension.PlantsTable}",
                DBCommandType.ValueGet, TCPConnectorService.Instance.GenericResult));
    }

    public void RequestGetAllPlants(PCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                $"lp{DataParsingExtension.QuerySplitter}*{DataParsingExtension.QuerySplitter}{DataParsingExtension.PlantsTable}",
                DBCommandType.ValueGetAll, TCPConnectorService.Instance.GenericGetAllResult));
    }

    public void RequestGetAllDiseases(PCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                $"ld{DataParsingExtension.QuerySplitter}*{DataParsingExtension.QuerySplitter}{DataParsingExtension.DiseasesTable}",
                DBCommandType.ValueGetAll, TCPConnectorService.Instance.GenericGetAllResult));
    }

    public void RequestGetAllMedicines(PCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                $"lm{DataParsingExtension.QuerySplitter}*{DataParsingExtension.QuerySplitter}{DataParsingExtension.MedicinesTable}",
                DBCommandType.ValueGetAll, TCPConnectorService.Instance.GenericGetAllResult));
    }
    public void RequestGetAllPlantsMedicines(PCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                $"lpm{DataParsingExtension.QuerySplitter}*{DataParsingExtension.QuerySplitter}{DataParsingExtension.PlantsMedicinesTable}",
                DBCommandType.ValueGetAll, TCPConnectorService.Instance.GenericGetAllResult));
    }
    public void RequestGetAllMedicinesDiseasePlant(PCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client, 
$"lmdp{DataParsingExtension.QuerySplitter}{data.Query.Split(DataParsingExtension.QuerySplitter)[0]}{DataParsingExtension.QuerySplitter}*{DataParsingExtension.QuerySplitter}{DataParsingExtension.PlantsMedicinesTable}",
                DBCommandType.ValueGetAll, TCPConnectorService.Instance.GenericGetAllResult));
    }

    public void RequestGetAllPlantsDiseases(PCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                $"lpd{DataParsingExtension.QuerySplitter}*{DataParsingExtension.QuerySplitter}{DataParsingExtension.PlantsDiseasesTable}",
                DBCommandType.ValueGetAll, TCPConnectorService.Instance.GenericGetAllResult));
    }

    public void RequestGetAllDiseasesMedicines(PCommand data) //
    {
        ServerApp.Instance.AddCommand(
            new DBCommand(data.Client,
                $"lmd{DataParsingExtension.QuerySplitter}*{DataParsingExtension.QuerySplitter}{DataParsingExtension.MedicineDiseasesTable}",
                DBCommandType.ValueGetAll, TCPConnectorService.Instance.GenericGetAllResult));
    }

    // public void RequestGetAllAquireType(PCommand data) //
    // {
    //     ServerApp.Instance.AddCommand(
    //         new DBCommand(data.Client,
    //             $"*,{DataParsingExtension.ATTableName}",
    //             DBCommandType.ValueGetAll, GenericGetAllResult));
    // }
    // public void RequestGetAllGoodsSeller(PCommand data) //
    // {
    //     ServerApp.Instance.AddCommand(
    //         new DBCommand(data.Client,
    //             data.Query +
    //             $";accountLogin;{DataParsingExtension.STableName}",
    //             DBCommandType.ValueGet,
    //             ProcessGetAllGoodsSeller));
    // }
    //
    // public void ProcessGetAllGoodsSeller(Object dataObj) //
    // {
    //     DBCommand data = (DBCommand)dataObj;
    //     var seller = (string)data.Output;
    //     if (seller == "ERR") return;
    //
    //     ServerApp.Instance.AddCommand(
    //         new DBCommand(data.Client,
    //             $"lts{DataParsingExtension.QuerySplitter}*{DataParsingExtension.QuerySplitter}{DataParsingExtension.GOTableName}{DataParsingExtension.QuerySplitter}{seller.Split(DataParsingExtension.ValueSplitter)[0].DBReadable()}{DataParsingExtension.QuerySplitter}sellerName",
    //             DBCommandType.ValueGetAllCondition, TCPConnectorService.Instance.GenericGetAllResult));
    // }
}