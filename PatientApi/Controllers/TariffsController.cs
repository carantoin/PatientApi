using Microsoft.AspNetCore.Mvc;
using PatientApi.Models;
using PatientApi.Services;

namespace PatientApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TariffsController : ControllerBase
{
    private readonly TariffService _tariffService;

    public TariffsController(TariffService tariffService) =>
        _tariffService = tariffService;

    [HttpGet]
    public async Task<ActionResult<List<Tariff>>> GetAll() =>
        Ok(await _tariffService.GetAllAsync());

    [HttpGet("{diagnosisCode}")]
    public async Task<ActionResult<Tariff>> GetByCode(string diagnosisCode)
    {
        var tariff = await _tariffService.GetByDiagnosisCodeAsync(diagnosisCode);
        return tariff is null ? NotFound() : Ok(tariff);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Tariff tariff)
    {
        await _tariffService.CreateAsync(tariff);
        return CreatedAtAction(nameof(GetByCode),
            new { diagnosisCode = tariff.DiagnosisCode }, tariff);
    }
}