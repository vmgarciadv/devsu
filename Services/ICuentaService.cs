using System.Collections.Generic;
using System.Threading.Tasks;
using devsu.DTOs;

namespace devsu.Services
{
    public interface ICuentaService
    {
        Task<IEnumerable<CuentaDto>> GetAllCuentasAsync();
        Task<CuentaDto> GetCuentaByNumeroCuentaAsync(int numeroCuenta);
        Task<CuentaDto> CreateCuentaAsync(CuentaDto cuentaDto);
        Task<CuentaDto> UpdateCuentaAsync(int numeroCuenta, CuentaDto cuentaDto);
        Task<CuentaDto> PatchCuentaAsync(int numeroCuenta, CuentaPatchDto cuentaPatchDto);
    }
}