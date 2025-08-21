using FONEXPO2024.Domain.Model.DTO;
using FONEXPO2024.Interfaces.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FONEXPO2024.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegistracijaController : ControllerBase
{
    private readonly IRegistracijaService _service;
    public RegistracijaController(IRegistracijaService service) => _service = service;

    [HttpPost("Registracija")]
    public async Task<ActionResult<RegistrationResponseDto>> Get([FromBody] RegistracijaRequestDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new CancelPrijavaResponseDto
            {
                Error = new ErrorDto { Details = "Neispravan unos. Token i email su obavezni." }
            });
        }


        var result = await _service.RegisterAsync(dto);

        // Ako je doslo do neke predvidjene greske (npr. prijava nije pronadjena ili je vec otkazana)
        if (!string.IsNullOrEmpty(result.Error!.Details))
            return BadRequest(result);

        // Uspesno otkazano
        return Ok(result);
    }

    [HttpPut("CancelPrijava")]

    public async Task<ActionResult<CancelPrijavaResponseDto>> CancelPrijava([FromBody] LoginPrijava dto)
    {
        // Model validation: obavezni polja + email format
        if (!ModelState.IsValid)
        {
            return BadRequest(new CancelPrijavaResponseDto
            {
                Error = new ErrorDto { Details = "Neispravan unos. Token i email su obavezni." }
            });
        }


        // Poziv servisa
        var result = await _service.CancelAsync(dto);

        // Ako je doslo do neke predvidjene greske (npr. prijava nije pronadjena ili je vec otkazana)
        if (!string.IsNullOrEmpty(result.Error!.Details))
            return BadRequest(result);

        // Uspesno otkazano
        return Ok(result);
    }

    [HttpPut("Login")]

    public async Task<ActionResult<PrijavaResponseDto>> Login([FromBody] LoginPrijava dto)
    {
        // Model validation: obavezni polja + email format
        if (!ModelState.IsValid)
        {
            return BadRequest(new CancelPrijavaResponseDto
            {
                Error = new ErrorDto { Details = "Neispravan unos. Token i email su obavezni." }
            });
        }


        // Poziv servisa
        var result = await _service.LoginAsync(dto);

        if (result.Error != null)
        {
            // Ako je doslo do neke predvidjene greske (npr. prijava nije pronadjena ili je vec otkazana)
            if (!string.IsNullOrEmpty(result?.Error?.Details))
                return BadRequest(result);
        }
        // Uspesno otkazano
        return Ok(result);
    }

    [HttpPut("IzracunajCenu")]
    public async Task<ActionResult<decimal>> IzracunajCenu([FromBody] CenaRacunanjeDTO dto)
    {
        // Model validation: obavezni polja + email format
        if (!ModelState.IsValid)
        {
            return BadRequest();
            
        }


        // Poziv servisa
        var result = await _service.IzracunajCenu(dto);

     
        // Uspesno otkazano
        return Ok(result);
    }


    [HttpPut("UpdatePrijava")]
    public async Task<ActionResult<PrijavaResponseDto>> UpdatePrijava([FromBody] UpdatePrijavaDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new PrijavaResponseDto
            {
                Error = new ErrorDto { Details = "Neispravan unos." }
            });
        }

        var result = await _service.UpdateAsync(dto);

        if (result.Error != null && !string.IsNullOrWhiteSpace(result.Error.Details))
            return BadRequest(result);

        return Ok(result);
    }
}

