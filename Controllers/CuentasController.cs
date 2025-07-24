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
    public class CuentasController : ControllerBase
    {
        private readonly ICuentaService _cuentaService;
        
        public CuentasController(ICuentaService cuentaService)
        {
            _cuentaService = cuentaService;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CuentaDto>>> GetCuentas()
        {
            var cuentas = await _cuentaService.GetAllCuentasAsync();
            return Ok(cuentas);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<CuentaDto>> GetCuenta(int id)
        {
            try
            {
                var cuenta = await _cuentaService.GetCuentaByNumeroCuentaAsync(id);
                if (cuenta == null)
                    return NotFound();
                    
                return Ok(cuenta);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
        
        [HttpPost]
        public async Task<ActionResult<CuentaDto>> CreateCuenta(CuentaDto cuentaDto)
        {
            try
            {
                var cuenta = await _cuentaService.CreateCuentaAsync(cuentaDto);
                return CreatedAtAction(nameof(GetCuenta), new { id = cuenta.CuentaId }, cuenta);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{numeroCuenta}")]
        public async Task<ActionResult<CuentaDto>> UpdateCuenta(int numeroCuenta, CuentaDto cuentaDto)
        {
            try
            {
                var cuenta = await _cuentaService.UpdateCuentaAsync(numeroCuenta, cuentaDto);
                return Ok(cuenta);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPatch("{numeroCuenta}")]
        public async Task<ActionResult<CuentaDto>> PatchCuenta(int numeroCuenta, [FromBody] CuentaPatchDto cuentaPatchDto)
        {
            try
            {
                var cuenta = await _cuentaService.PatchCuentaAsync(numeroCuenta, cuentaPatchDto);
                return Ok(cuenta);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{numeroCuenta}")]
        public async Task<ActionResult> DeleteCuenta(int numeroCuenta)
        {
            try
            {
                await _cuentaService.DeleteCuentaAsync(numeroCuenta);
                return Ok(new { mensaje = "Cuenta eliminada exitosamente" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}