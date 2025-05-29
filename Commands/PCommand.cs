using System.Net.Sockets;
using KursuchServer.Services;

namespace KursuchServer;

public class PCommand: Command
{
    public PCommandType SubType;

    public PCommand()
    {
        Type = CommandType.GCommand;
    }

    public PCommand(TcpClient tcpClient, String query, PCommandType subType, Action<Object> outputFunc = null)
    {
        OutputFunc = outputFunc;
        Type = CommandType.PCommand;
        Client = tcpClient;
        Query = query;
        SubType = subType;
    }

    public override void Execute()
    {
        try
        {
            switch (SubType)
            {
                case PCommandType.PlantGetAll:
                    PlantsService.Instance.RequestGetAllPlants(this);
                    break;
                case PCommandType.DiseaseGetAll:
                    PlantsService.Instance.RequestGetAllDiseases(this);
                    break;
                case PCommandType.MedicineGetAll:
                    PlantsService.Instance.RequestGetAllMedicines(this);
                    break;
                case PCommandType.PlantMedicineGetAll:
                    PlantsService.Instance.RequestGetAllPlantsMedicines(this);
                    break;
                case PCommandType.MedicineDiseaseGetAll:
                    PlantsService.Instance.RequestGetAllDiseasesMedicines(this);
                    break;
                case PCommandType.PlantDiseaseGetAll:
                    PlantsService.Instance.RequestGetAllPlantsDiseases(this);
                    break;
                case PCommandType.CalculateZones:
                    ZoneCalculator.Instance.Calculate(this);
                    break;
                case PCommandType.LoadAll:
                    PlantsService.Instance.RequestGetAllPlants(this); 
                    PlantsService.Instance.RequestGetAllDiseases(this); 
                    PlantsService.Instance.RequestGetAllMedicines(this); 
                    PlantsService.Instance.RequestGetAllPlantsMedicines(this); 
                    PlantsService.Instance.RequestGetAllDiseasesMedicines(this);
                    PlantsService.Instance.RequestGetAllPlantsDiseases(this);
                    break;
            }

            var client = AccountService.Instance.GetClient(Client);
            if (client is not { SV_Cheats: true }) return;
            
            switch (SubType)
            {
                case PCommandType.PlantAdd:
                    PlantsService.Instance.RequestAddPlant(this);
                    break;
                case PCommandType.PlantDelete:
                    PlantsService.Instance.RequestDeletePlant(this);
                    break;
                case PCommandType.PlantModify:
                    PlantsService.Instance.RequestModifyPlant(this);
                    break;
                case PCommandType.DiseaseAdd:
                    PlantsService.Instance.RequestAddDisease(this);
                    break;
                case PCommandType.DiseaseDelete:
                    PlantsService.Instance.RequestDeleteDisease(this);
                    break;
                case PCommandType.DiseaseModify:
                    PlantsService.Instance.RequestModifyDisease(this);
                    break;
                case PCommandType.MedicineAdd:
                    PlantsService.Instance.RequestAddMedicine(this);
                    break;
                case PCommandType.MedicineDelete:
                    PlantsService.Instance.RequestDeleteMedicine(this);
                    break;
                case PCommandType.MedicineModify:
                    PlantsService.Instance.RequestModifyMedicine(this);
                    break;
                case PCommandType.PlantMedicineAdd:
                    PlantsService.Instance.RequestAddPlantMedicine(this);
                    break;
                case PCommandType.PlantMedicineDelete:
                    PlantsService.Instance.RequestDeletePlantMedicine(this);
                    break;
                case PCommandType.PlantDiseaseAdd:
                    PlantsService.Instance.RequestAddPlantDisease(this);
                    break;
                case PCommandType.PlantDiseaseDelete:
                    PlantsService.Instance.RequestDeletePlantDisease(this);
                    break;
                case PCommandType.MedicineDiseaseAdd:
                    PlantsService.Instance.RequestAddDiseaseMedicine(this);
                    break;
                case PCommandType.MedicineDiseaseDelete:
                    PlantsService.Instance.RequestDeleteDiseaseMedicine(this);
                    break;
            }
        }
        catch (Exception ex)
        {
            ServerApp.Instance.AddCommand(new TCPCommand(Client, $"ew{DataParsingExtension.QuerySplitter}{ex.Message}",
                TCPCommandType.SendSingleValue));
        }
    }

    public override void Undo()
    {
    }
}