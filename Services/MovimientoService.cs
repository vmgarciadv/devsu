using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using devsu.DTOs;
using devsu.Models;
using devsu.Repositories;

namespace devsu.Services
{
    public class MovimientoService : IMovimientoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        
        public async Task<IEnumerable<MovimientoDto>> GetAllMovimientosAsync()
        {
            var movimientos = await _unitOfWork.Movimientos.GetAllAsync();
            return _mapper.Map<IEnumerable<MovimientoDto>>(movimientos);
        }
        
        public async Task<MovimientoDto> GetMovimientoByIdAsync(int id)
        {
            var movimiento = await _unitOfWork.Movimientos.GetByIdAsync(id);
            return _mapper.Map<MovimientoDto>(movimiento);
        }
        
        public async Task<MovimientoDto> CreateMovimientoAsync(MovimientoDto movimientoDto)
        {
            // Obtener la cuenta con sus movimientos
            var cuenta = await _unitOfWork.Cuentas.GetCuentaWithMovimientosAsync(movimientoDto.CuentaId);
            if (cuenta == null)
            {
                throw new KeyNotFoundException("Cuenta no encontrada");
            }
            
            // Obtener el último movimiento para calcular el saldo actual
            var ultimoMovimiento = await _unitOfWork.Movimientos.GetLastMovimientoByCuentaAsync(movimientoDto.CuentaId);
            decimal saldoActual = ultimoMovimiento?.Saldo ?? cuenta.SaldoInicial;
            
            // Validar tipo de movimiento
            var esDebito = movimientoDto.TipoMovimiento.ToLower() == "debito";
            decimal valorMovimiento = esDebito ? -Math.Abs(movimientoDto.Valor) : Math.Abs(movimientoDto.Valor);
            
            // Validaciones para débitos
            if (esDebito)
            {
                // Validar saldo disponible
                if (saldoActual + valorMovimiento < 0)
                {
                    throw new InvalidOperationException("Saldo no disponible");
                }
            }
            
            // Crear el movimiento
            var movimiento = new Movimiento
            {
                CuentaId = movimientoDto.CuentaId,
                Fecha = DateTime.Now,
                TipoMovimiento = movimientoDto.TipoMovimiento,
                Valor = valorMovimiento,
                Saldo = saldoActual + valorMovimiento
            };
            
            await _unitOfWork.Movimientos.AddAsync(movimiento);
            await _unitOfWork.CompleteAsync();
            
            var result = _mapper.Map<MovimientoDto>(movimiento);
            result.NumeroCuenta = cuenta.NumeroCuenta;
            
            return result;
        }
        

    }
}