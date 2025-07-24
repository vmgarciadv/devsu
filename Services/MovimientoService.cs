using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using devsu.DTOs;
using devsu.Models;
using devsu.Repositories;
using devsu.Configuration;

namespace devsu.Services
{
    public class MovimientoService : IMovimientoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly BusinessRulesOptions _businessRules;

        public MovimientoService(IUnitOfWork unitOfWork, IMapper mapper, IOptions<BusinessRulesOptions> businessRulesOptions)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _businessRules = businessRulesOptions.Value;
        }

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

        public async Task<MovimientoDto> CreateMovimientoAsync(CreateMovimientoDto createMovimientoDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Obtener la cuenta por número de cuenta
                var cuenta = await _unitOfWork.Cuentas.GetByNumeroCuentaAsync(createMovimientoDto.NumeroCuenta);
                if (cuenta == null)
                {
                    throw new KeyNotFoundException($"Cuenta con número {createMovimientoDto.NumeroCuenta} no encontrada");
                }

                // Obtener el último movimiento para calcular el saldo actual
                var ultimoMovimiento = await _unitOfWork.Movimientos.GetLastMovimientoByCuentaAsync(cuenta.CuentaId);
                decimal saldoActual = ultimoMovimiento?.Saldo ?? cuenta.SaldoInicial;

                // Validar tipo de movimiento
                var tipoMovimientoNormalizado = createMovimientoDto.TipoMovimiento.ToLower();
                if (tipoMovimientoNormalizado != "debito" && tipoMovimientoNormalizado != "credito")
                {
                    throw new ArgumentException($"TipoMovimiento debe ser 'Debito' o 'Credito'. Valor recibido: '{createMovimientoDto.TipoMovimiento}'");
                }

                var esDebito = tipoMovimientoNormalizado == "debito";
                decimal valorMovimiento = esDebito ? -Math.Abs(createMovimientoDto.Valor) : Math.Abs(createMovimientoDto.Valor);

                // Validaciones para débitos
                if (esDebito)
                {
                    // Validar saldo disponible
                    if (saldoActual + valorMovimiento < 0)
                    {
                        throw new InvalidOperationException("Saldo no disponible");
                    }
                    
                    // Validar límite diario de retiros (débitos)
                    var limiteDiario = _businessRules.LimiteDiarioRetiro;
                    var totalDebitosHoy = await _unitOfWork.Movimientos.GetTotalDebitosDiarioAsync(cuenta.CuentaId, DateTime.Now);
                    var nuevoTotalDebitos = totalDebitosHoy + Math.Abs(createMovimientoDto.Valor);
                    
                    if (nuevoTotalDebitos > limiteDiario)
                    {
                        throw new InvalidOperationException($"Cupo diario excedido. Débitos hoy: ${totalDebitosHoy:F2}, Intento: ${Math.Abs(createMovimientoDto.Valor):F2}, Límite: ${limiteDiario:F2}");
                    }
                }

                // Crear el movimiento
                var movimiento = new Movimiento
                {
                    CuentaId = cuenta.CuentaId,
                    Fecha = DateTime.Now,
                    TipoMovimiento = createMovimientoDto.TipoMovimiento,
                    Valor = valorMovimiento,
                    Saldo = saldoActual + valorMovimiento
                };

                await _unitOfWork.Movimientos.AddAsync(movimiento);

                // Commit transaction (this will also save changes)
                await _unitOfWork.CommitTransactionAsync();

                // Mapear después del commit
                var result = new MovimientoDto
                {
                    Fecha = movimiento.Fecha,
                    TipoMovimiento = movimiento.TipoMovimiento,
                    Valor = movimiento.Valor,
                    Saldo = movimiento.Saldo,
                    NumeroCuenta = cuenta.NumeroCuenta
                };

                return result;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }


    }
}