using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using devsu.DTOs;

namespace devsu.Services
{
    public interface IMovimientoService
    {
        Task<IEnumerable<MovimientoDto>> GetAllMovimientosAsync();
        Task<MovimientoDto> GetMovimientoByIdAsync(int id);
        Task<MovimientoDto> CreateMovimientoAsync(MovimientoDto movimientoDto);
    }
}