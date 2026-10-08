namespace PatientApi.Models;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string TariffsCollectionName { get; set; } = null!;
    public string PatientsCollectionName { get; set; } = null!;
}