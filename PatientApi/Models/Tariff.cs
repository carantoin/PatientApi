using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PatientApi.Models;

[BsonIgnoreExtraElements]
public class Tariff
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("diagnosisCode")]
    public string DiagnosisCode { get; set; } = null!;

    [BsonElement("diagnosisName")]
    public string DiagnosisName { get; set; } = null!;

    [BsonElement("costPerDay")]
    public decimal CostPerDay { get; set; }
}