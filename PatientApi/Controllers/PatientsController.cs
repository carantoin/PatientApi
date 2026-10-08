using Microsoft.AspNetCore.Mvc;
using PatientApi.Models;
using PatientApi.Services;

namespace PatientApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly PatientService _patientService;
    private readonly TariffService _tariffService;

    public PatientsController(PatientService patientService, TariffService tariffService)
    {
        _patientService = patientService;
        _tariffService = tariffService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Patient>>> GetAll() =>
        Ok(await _patientService.GetAllAsync());

    [HttpGet("out-of-town")]
    public async Task<ActionResult<List<Patient>>> GetOutOfTownPatients() =>
        Ok(await _patientService.GetOutOfTownPatientsAsync());

    [HttpGet("revenue-by-diagnosis")]
    public async Task<ActionResult<List<DiagnosisRevenue>>> GetRevenueByDiagnosis() =>
        Ok(await _patientService.GetRevenueByDiagnosisAsync());

    [HttpPost]
    public async Task<IActionResult> Create(Patient patient)
    {
        await _patientService.CreateAsync(patient);
        return CreatedAtAction(nameof(GetAll), new { id = patient.Id }, patient);
    }

    [HttpPost("seed")]
    public async Task<IActionResult> SeedData()
    {
        await _tariffService.SeedDataAsync();
        await _patientService.SeedDataAsync();
        return Ok("Данные инициализированы");
    }
}