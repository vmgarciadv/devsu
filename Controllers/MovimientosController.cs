using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using devsu.DTOs;
using devsu.Services;

namespace devsu.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class MovimientosController : ControllerBase
  {
    private readonly IMovimientoService _movimientoService;

    public MovimientosController(IMovimientoService movimientoService)
    {
      _movimientoService = movimientoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovimientoDto>>> GetMovimientos()
    {
      var movimientos = await _movimientoService.GetAllMovimientosAsync();
      return Ok(movimientos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MovimientoDto>> GetMovimiento(int id)
    {
      var movimiento = await _movimientoService.GetMovimientoByIdAsync(id);
      return Ok(movimiento);
    }

    [HttpPost]
    public async Task<ActionResult<MovimientoDto>> CreateMovimiento(CreateMovimientoDto createMovimientoDto)
    {
      var movimiento = await _movimientoService.CreateMovimientoAsync(createMovimientoDto);
      return Created("", movimiento);
    }
  }
}