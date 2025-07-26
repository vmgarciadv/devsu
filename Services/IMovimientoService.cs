using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using devsu.DTOs;

namespace devsu.Services
{
    public interface IMovimientoService
    {
        Task<IEnumerable<MovimientoDto>> GetAllMovimientosAsync();
        Task<PaginatedResponse<MovimientoDto>> GetAllMovimientosPaginatedAsync(PaginationParameters paginationParameters);
        Task<MovimientoDto> GetMovimientoByIdAsync(int id);
        Task<MovimientoDto> CreateMovimientoAsync(CreateMovimientoDto createMovimientoDto);
    }
}