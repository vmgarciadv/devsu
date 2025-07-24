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
            try
            {
                var movimiento = await _movimientoService.GetMovimientoByIdAsync(id);
                if (movimiento == null)
                    return NotFound();
                    
                return Ok(movimiento);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
        
        [HttpPost]
        public async Task<ActionResult<MovimientoDto>> CreateMovimiento(CreateMovimientoDto createMovimientoDto)
        {
            try
            {
                var movimiento = await _movimientoService.CreateMovimientoAsync(createMovimientoDto);
                // Return 201 Created with the movimiento in the body
                return Created("", movimiento);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}