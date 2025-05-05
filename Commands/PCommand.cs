using System.Net.Sockets;
using KursuchServer.Services;

namespace KursuchServer;

public class PCommand : Command
{
    public PCommandType SubType;

    public PCommand()
    {
        Type = CommandType.PCommand;
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
            var client = AccountService.Instance.GetClient(Client);
            bool isAdmin = client?.AdminKey != "NAN";

            switch (SubType)
            {
                // Операции требующие прав администратора
                case PCommandType.DiseaseAdd when isAdmin:
                    PlantService.Instance.RequestAddDisease(this);
                    break;
                case PCommandType.DiseaseDelete when isAdmin:
                    PlantService.Instance.RequestDeleteDisease(this);
                    break;
                case PCommandType.MedicineAdd when isAdmin:
                    PlantService.Instance.RequestAddMedicine(this);
                    break;
                case PCommandType.MedicineDelete when isAdmin:
                    PlantService.Instance.RequestDeleteMedicine(this);
                    break;
                case PCommandType.PlantAdd when isAdmin:
                    PlantService.Instance.RequestAddPlant(this);
                    break;
                case PCommandType.PlantDelete when isAdmin:
                    PlantService.Instance.RequestDeletePlant(this);
                    break;
                case PCommandType.MedicineDiseaseAdd when isAdmin:
                    PlantService.Instance.RequestAddMedicineDisease(this);
                    break;
                case PCommandType.MedicineDiseaseDelete when isAdmin:
                    PlantService.Instance.RequestDeleteMedicineDisease(this);
                    break;
                case PCommandType.PlantMedicineAdd when isAdmin:
                    PlantService.Instance.RequestAddPlantMedicine(this);
                    break;
                case PCommandType.PlantMedicineDelete when isAdmin:
                    PlantService.Instance.RequestDeletePlantMedicine(this);
                    break;
                case PCommandType.DosageAdd when isAdmin:
                    PlantService.Instance.RequestAddDosage(this);
                    break;
                case PCommandType.DosageDelete when isAdmin:
                    // Реализовать при необходимости
                    break;

                // Операции для всех пользователей
                case PCommandType.DiseaseGet:
                    PlantService.Instance.RequestGetDisease(this);
                    break;
                case PCommandType.DiseaseGetAll:
                    PlantService.Instance.RequestGetAllDiseases(this);
                    break;
                case PCommandType.MedicineGet:
                    PlantService.Instance.RequestGetMedicine(this);
                    break;
                case PCommandType.MedicineGetAll:
                    PlantService.Instance.RequestGetAllMedicines(this);
                    break;
                case PCommandType.PlantGet:
                    PlantService.Instance.RequestGetPlant(this);
                    break;
                case PCommandType.PlantGetAll:
                    PlantService.Instance.RequestGetAllPlants(this);
                    break;
                case PCommandType.MedicineDiseaseGetAll:
                    PlantService.Instance.RequestGetAllMedicineDiseases(this);
                    break;
                case PCommandType.PlantMedicineGetAll:
                    PlantService.Instance.RequestGetAllPlantsMedicines(this);
                    break;
                case PCommandType.DosageGet:
                    PlantService.Instance.RequestGetDosage(this);
                    break;
                case PCommandType.DosageGetAll:
                    PlantService.Instance.RequestGetAllDosages(this);
                    break;
            }
        }
        catch (Exception ex)
        {
            ServerApp.Instance.AddCommand(new TCPCommand(Client, 
                $"ew{DataParsingExtension.QuerySplitter}{ex.Message}",
                TCPCommandType.SendSingleValue));
        }
    }
}