using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PatientApi.Models;

namespace PatientApi.Services;

public class TariffService
{
    private readonly IMongoCollection<Tariff> _tariffs;

    public TariffService(IMongoClient client, IOptions<MongoDbSettings> settings)
    {
        var db = client.GetDatabase(settings.Value.DatabaseName);
        _tariffs = db.GetCollection<Tariff>(settings.Value.TariffsCollectionName);
    }

    public async Task<List<Tariff>> GetAllAsync() =>
        await _tariffs.Find(_ => true).ToListAsync();

    public async Task<Tariff?> GetByDiagnosisCodeAsync(string code) =>
        await _tariffs.Find(t => t.DiagnosisCode == code).FirstOrDefaultAsync();

    public async Task CreateAsync(Tariff tariff) =>
        await _tariffs.InsertOneAsync(tariff);

    public async Task SeedDataAsync()
    {
        if (await _tariffs.CountDocumentsAsync(_ => true) > 0) return;

        var tariffs = new List<Tariff>
        {
            new() { DiagnosisCode = "J00", DiagnosisName = "Острый назофарингит",      CostPerDay = 1500 },
            new() { DiagnosisCode = "J01", DiagnosisName = "Острый синусит",           CostPerDay = 2500 },
            new() { DiagnosisCode = "J02", DiagnosisName = "Острый фарингит",          CostPerDay = 1800 },
            new() { DiagnosisCode = "K35", DiagnosisName = "Острый аппендицит",        CostPerDay = 4500 },
            new() { DiagnosisCode = "I10", DiagnosisName = "Эссенциальная гипертензия",CostPerDay = 3200 },
            new() { DiagnosisCode = "E11", DiagnosisName = "Сахарный диабет 2 типа",   CostPerDay = 3800 },
            new() { DiagnosisCode = "M54", DiagnosisName = "Дорсалгия",                CostPerDay = 2200 }
        };

        await _tariffs.InsertManyAsync(tariffs);
    }
}