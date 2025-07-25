using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using devsu.DTOs;
using devsu.Services;

namespace devsu.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly IReporteService _reporteService;

        public ReportesController(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        [HttpGet]
        public async Task<IActionResult> GenerarReporte([FromQuery] DateTime fecha, [FromQuery] string cliente)
        {
            var request = new ReporteRequestDto
            {
                Cliente = cliente,
                FechaInicio = fecha.Date,
                FechaFin = fecha.Date.AddDays(1).AddSeconds(-1)
            };

            var response = await _reporteService.GenerarReporteEstadoCuentaAsync(request);
            return Ok(response);
        }

        [HttpGet("rango")]
        public async Task<IActionResult> GenerarReporteRango([FromQuery] ReporteRequestDto request)
        {
            var response = await _reporteService.GenerarReporteEstadoCuentaAsync(request);
            return Ok(response);
        }
    }
}