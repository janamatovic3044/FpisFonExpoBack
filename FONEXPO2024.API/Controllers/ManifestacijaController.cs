using FONEXPO2024.Domain.Model.DTO;
using FONEXPO2024.Interfaces.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FONEXPO2024.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManifestacijaController : ControllerBase
    {
        private readonly IManifestacijaService _service;
        public ManifestacijaController(IManifestacijaService service) => _service = service;

        [HttpGet("ManifestacijaInfo")]
        public async Task<ActionResult<ManifestacijaInfoDto>> Get()
        {
            var info = await _service.GetManifestacijaInfo();
            if (info == null) return NotFound();
            return Ok(info);
        }
    }
}
