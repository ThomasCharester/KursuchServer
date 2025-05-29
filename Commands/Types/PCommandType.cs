namespace KursuchServer;

// Создаем PCommandType.cs
public enum PCommandType
{
    // Основные сущности
    DiseaseAdd,
    DiseaseDelete,
    DiseaseModify,
    DiseaseGet,
    DiseaseGetAll,
    
    MedicineAdd,
    MedicineDelete,
    MedicineModify,
    MedicineGet,
    MedicineGetAll,
    
    PlantAdd,
    PlantDelete,
    PlantModify,
    PlantGet,
    PlantGetAll,
    
    // Связующие таблицы
    MedicineDiseaseAdd,
    MedicineDiseaseDelete,
    MedicineDiseaseGetAll,
    
    PlantMedicineAdd,
    PlantMedicineDelete,
    PlantMedicineGetAll,
    
    PlantDiseaseAdd,
    PlantDiseaseDelete,
    PlantDiseaseGetAll,

    CalculateZones,
    LoadAll
}
