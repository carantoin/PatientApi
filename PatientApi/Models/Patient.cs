using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PatientApi.Models;

[BsonIgnoreExtraElements]
public class Patient
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("fullName")]
    public string FullName { get; set; } = null!;

    [BsonElement("gender")]
    public string Gender { get; set; } = null!;

    [BsonElement("age")]
    public int Age { get; set; }

    [BsonElement("residence")]
    public string Residence { get; set; } = null!;

    [BsonElement("diagnosisCode")]
    public string DiagnosisCode { get; set; } = null!;

    [BsonElement("diagnosisName")]
    public string DiagnosisName { get; set; } = null!;

    [BsonElement("daysInClinic")]
    public int DaysInClinic { get; set; }

    [BsonElement("treatmentCost")]
    public decimal TreatmentCost { get; set; }
}