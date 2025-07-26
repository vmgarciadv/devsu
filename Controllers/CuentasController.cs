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
        public async Task<ActionResult<IEnumerable<CuentaDto>>> GetCuentas([FromQuery] PaginationParameters paginationParameters)
        {
            if (paginationParameters == null || (paginationParameters.PageNumber == 1 && paginationParameters.PageSize == 10))
            {
                var cuentas = await _cuentaService.GetAllCuentasAsync();
                return Ok(cuentas);
            }
            else
            {
                var paginatedCuentas = await _cuentaService.GetAllCuentasPaginatedAsync(paginationParameters);
                return Ok(paginatedCuentas);
            }
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<CuentaDto>> GetCuenta(int id)
        {
            var cuenta = await _cuentaService.GetCuentaByNumeroCuentaAsync(id);
            return Ok(cuenta);
        }
        
        [HttpPost]
        public async Task<ActionResult<CuentaDto>> CreateCuenta(CuentaDto cuentaDto)
        {
            var cuenta = await _cuentaService.CreateCuentaAsync(cuentaDto);
            return CreatedAtAction(nameof(GetCuenta), new { id = cuenta.NumeroCuenta }, cuenta);
        }

        [HttpPut("{numeroCuenta}")]
        public async Task<ActionResult<CuentaDto>> UpdateCuenta(int numeroCuenta, CuentaDto cuentaDto)
        {
            var cuenta = await _cuentaService.UpdateCuentaAsync(numeroCuenta, cuentaDto);
            return Ok(cuenta);
        }

        [HttpPatch("{numeroCuenta}")]
        public async Task<ActionResult<CuentaDto>> PatchCuenta(int numeroCuenta, [FromBody] CuentaPatchDto cuentaPatchDto)
        {
            var cuenta = await _cuentaService.PatchCuentaAsync(numeroCuenta, cuentaPatchDto);
            return Ok(cuenta);
        }

        [HttpDelete("{numeroCuenta}")]
        public async Task<ActionResult> DeleteCuenta(int numeroCuenta)
        {
            await _cuentaService.DeleteCuentaAsync(numeroCuenta);
            return Ok(new { mensaje = "Cuenta eliminada exitosamente" });
        }
    }
}