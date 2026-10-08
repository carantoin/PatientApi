using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PatientApi.Models;

namespace PatientApi.Services;

public class PatientService
{
    private readonly IMongoCollection<Patient> _patients;

    public PatientService(IMongoClient client, IOptions<MongoDbSettings> settings)
    {
        var db = client.GetDatabase(settings.Value.DatabaseName);
        _patients = db.GetCollection<Patient>(settings.Value.PatientsCollectionName);
    }

    public async Task<List<Patient>> GetAllAsync() =>
        await _patients.Find(_ => true).ToListAsync();

    public async Task CreateAsync(Patient patient) =>
        await _patients.InsertOneAsync(patient);

    public async Task<List<Patient>> GetOutOfTownPatientsAsync()
    {
        const string localResidence = "Москва";
        return await _patients
            .Find(p => p.Residence != localResidence)
            .SortBy(p => p.FullName)
            .ToListAsync();
    }

    public async Task<List<DiagnosisRevenue>> GetRevenueByDiagnosisAsync()
    {
        var pipeline = _patients.Aggregate()
            .Group(p => p.DiagnosisCode,
                g => new
                {
                    DiagnosisCode = g.Key,
                    DiagnosisName = g.First().DiagnosisName,
                    TotalRevenue = g.Sum(x => x.TreatmentCost),
                    PatientCount = g.Count()
                })
            .SortByDescending(x => x.TotalRevenue);

        var results = await pipeline.ToListAsync();

        return results.Select(r => new DiagnosisRevenue
        {
            DiagnosisCode = r.DiagnosisCode,
            DiagnosisName = r.DiagnosisName,
            TotalRevenue = r.TotalRevenue,
            PatientCount = r.PatientCount
        }).ToList();
    }

    public async Task SeedDataAsync()
    {
        if (await _patients.CountDocumentsAsync(_ => true) > 0) return;

        var patients = new List<Patient>
        {
            new() { FullName="Иванов И.И.",     Gender="М", Age=25, Residence="Москва",           DiagnosisCode="J00", DiagnosisName="Острый назофарингит",      DaysInClinic=5,  TreatmentCost=7500  },
            new() { FullName="Петрова А.С.",    Gender="Ж", Age=34, Residence="Санкт-Петербург",  DiagnosisCode="J01", DiagnosisName="Острый синусит",           DaysInClinic=7,  TreatmentCost=17500 },
            new() { FullName="Сидоров В.П.",    Gender="М", Age=45, Residence="Казань",           DiagnosisCode="K35", DiagnosisName="Острый аппендицит",        DaysInClinic=10, TreatmentCost=45000 },
            new() { FullName="Кузнецова Е.А.",  Gender="Ж", Age=52, Residence="Новосибирск",      DiagnosisCode="I10", DiagnosisName="Эссенциальная гипертензия",DaysInClinic=14, TreatmentCost=44800 },
            new() { FullName="Смирнов Д.О.",    Gender="М", Age=61, Residence="Екатеринбург",     DiagnosisCode="E11", DiagnosisName="Сахарный диабет 2 типа",   DaysInClinic=12, TreatmentCost=45600 },
            new() { FullName="Фёдорова О.Н.",   Gender="Ж", Age=29, Residence="Москва",           DiagnosisCode="J02", DiagnosisName="Острый фарингит",          DaysInClinic=4,  TreatmentCost=7200  },
            new() { FullName="Морозов А.К.",    Gender="М", Age=38, Residence="Санкт-Петербург",  DiagnosisCode="M54", DiagnosisName="Дорсалгия",                DaysInClinic=6,  TreatmentCost=13200 },
            new() { FullName="Волкова Т.И.",    Gender="Ж", Age=47, Residence="Казань",           DiagnosisCode="J01", DiagnosisName="Острый синусит",           DaysInClinic=9,  TreatmentCost=22500 },
            new() { FullName="Зайцев П.Р.",     Gender="М", Age=55, Residence="Новосибирск",      DiagnosisCode="I10", DiagnosisName="Эссенциальная гипертензия",DaysInClinic=11, TreatmentCost=35200 },
            new() { FullName="Никитина Л.В.",   Gender="Ж", Age=42, Residence="Екатеринбург",     DiagnosisCode="E11", DiagnosisName="Сахарный диабет 2 типа",   DaysInClinic=13, TreatmentCost=49400 },
            new() { FullName="Орлов С.С.",      Gender="М", Age=31, Residence="Москва",           DiagnosisCode="J00", DiagnosisName="Острый назофарингит",      DaysInClinic=3,  TreatmentCost=4500  },
            new() { FullName="Белова Н.А.",     Gender="Ж", Age=26, Residence="Санкт-Петербург",  DiagnosisCode="M54", DiagnosisName="Дорсалгия",                DaysInClinic=8,  TreatmentCost=17600 },
            new() { FullName="Гусев М.Ю.",      Gender="М", Age=49, Residence="Казань",           DiagnosisCode="K35", DiagnosisName="Острый аппендицит",        DaysInClinic=7,  TreatmentCost=31500 },
            new() { FullName="Романова И.П.",   Gender="Ж", Age=58, Residence="Новосибирск",      DiagnosisCode="J02", DiagnosisName="Острый фарингит",          DaysInClinic=5,  TreatmentCost=9000  },
            new() { FullName="Тимофеев А.В.",   Gender="М", Age=36, Residence="Екатеринбург",     DiagnosisCode="J01", DiagnosisName="Острый синусит",           DaysInClinic=6,  TreatmentCost=15000 }
        };

        await _patients.InsertManyAsync(patients);
    }
}

public class DiagnosisRevenue
{
    public string DiagnosisCode { get; set; } = null!;
    public string DiagnosisName { get; set; } = null!;
    public decimal TotalRevenue { get; set; }
    public int PatientCount { get; set; }
}