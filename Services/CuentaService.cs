using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using devsu.DTOs;
using devsu.Models;
using devsu.Repositories;
using devsu.Exceptions;

namespace devsu.Services
{
    public class CuentaService : ICuentaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        
        public CuentaService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        
        public async Task<IEnumerable<CuentaDto>> GetAllCuentasAsync()
        {
            var cuentas = await _unitOfWork.Cuentas.GetAllWithClienteAsync();
            return _mapper.Map<IEnumerable<CuentaDto>>(cuentas);
        }
        
        public async Task<CuentaDto> GetCuentaByNumeroCuentaAsync(int id)
        {
            var cuenta = await _unitOfWork.Cuentas.GetByNumeroCuentaAsync(id);
            if (cuenta == null)
            {
                throw new NotFoundException($"Cuenta con número {id} no encontrada");
            }
            return _mapper.Map<CuentaDto>(cuenta);
        }
        
        public async Task<CuentaDto> CreateCuentaAsync(CuentaDto cuentaDto)
        {
            // Buscar cliente por nombre
            var cliente = await _unitOfWork.Clientes.GetByNombreAsync(cuentaDto.NombreCliente);
            if (cliente == null)
            {
                throw new NotFoundException($"Cliente con nombre '{cuentaDto.NombreCliente}' no encontrado");
            }
            
            // Generar número de cuenta único
            var numeroCuenta = await GenerateUniqueAccountNumberAsync();
            
            var cuenta = _mapper.Map<Cuenta>(cuentaDto);
            cuenta.NumeroCuenta = numeroCuenta;
            cuenta.ClienteId = cliente.Id; // Asignar el ClienteId encontrado
            
            // Validar y normalizar TipoCuenta
            if (!string.IsNullOrEmpty(cuenta.TipoCuenta))
            {
                string normalized = cuenta.TipoCuenta.Trim().ToLower();
                if (normalized == "ahorro")
                {
                    cuenta.TipoCuenta = "Ahorro";
                }
                else if (normalized == "corriente")
                {
                    cuenta.TipoCuenta = "Corriente";
                }
                else
                {
                    throw new InvalidOperationException($"TipoCuenta debe ser 'Ahorro' o 'Corriente'. Valor recibido: '{cuenta.TipoCuenta}'");
                }
            }
            
            await _unitOfWork.Cuentas.AddAsync(cuenta);
            await _unitOfWork.CompleteAsync();
            
            // Retornar el DTO con la información completa
            var cuentaCreada = await _unitOfWork.Cuentas.GetByNumeroCuentaAsync(numeroCuenta);
            return _mapper.Map<CuentaDto>(cuentaCreada);
        }
        
        private async Task<int> GenerateUniqueAccountNumberAsync()
        {
            // Generar número aleatorio de 6 dígitos
            var random = new Random();
            int numeroCuenta;
            bool exists;
            
            do
            {
                // Genera un número entre 100000 y 999999 (6 dígitos)
                numeroCuenta = random.Next(100000, 1000000);
                
                // Verificar si ya existe
                var cuentaExistente = await _unitOfWork.Cuentas.GetByNumeroCuentaAsync(numeroCuenta);
                exists = cuentaExistente != null;
            }
            while (exists);
            
            return numeroCuenta;
        }

        public async Task<CuentaDto> UpdateCuentaAsync(int numeroCuenta, CuentaDto cuentaDto)
        {
            var cuenta = await _unitOfWork.Cuentas.GetByNumeroCuentaAsync(numeroCuenta);
            if (cuenta == null)
            {
                throw new NotFoundException("Cuenta no encontrada");
            }
            
            // Actualizar manualmente las propiedades para evitar problemas con el Id
            cuenta.TipoCuenta = cuentaDto.TipoCuenta;
            cuenta.Estado = cuentaDto.Estado;
            
            // Validar y normalizar TipoCuenta
            if (!string.IsNullOrEmpty(cuenta.TipoCuenta))
            {
                string normalized = cuenta.TipoCuenta.Trim().ToLower();
                if (normalized == "ahorro")
                {
                    cuenta.TipoCuenta = "Ahorro";
                }
                else if (normalized == "corriente")
                {
                    cuenta.TipoCuenta = "Corriente";
                }
                else
                {
                    throw new InvalidOperationException($"TipoCuenta debe ser 'Ahorro' o 'Corriente'. Valor recibido: '{cuenta.TipoCuenta}'");
                }
            }
            
            await _unitOfWork.CompleteAsync();
            
            return _mapper.Map<CuentaDto>(cuenta);
        }

        public async Task<CuentaDto> PatchCuentaAsync(int numeroCuenta, CuentaPatchDto cuentaPatchDto)
        {
            var cuenta = await _unitOfWork.Cuentas.GetByNumeroCuentaAsync(numeroCuenta);
            if (cuenta == null)
            {
                throw new NotFoundException("Cuenta no encontrada");
            }
            
            // Actualizar solo las propiedades que se proporcionan
            if (cuentaPatchDto.NumeroCuenta.HasValue)
                cuenta.NumeroCuenta = cuentaPatchDto.NumeroCuenta.Value;
            
            if (cuentaPatchDto.TipoCuenta != null)
            {
                string normalized = cuentaPatchDto.TipoCuenta.Trim().ToLower();
                if (normalized == "ahorro")
                {
                    cuenta.TipoCuenta = "Ahorro";
                }
                else if (normalized == "corriente")
                {
                    cuenta.TipoCuenta = "Corriente";
                }
                else
                {
                    throw new InvalidOperationException($"TipoCuenta debe ser 'Ahorro' o 'Corriente'. Valor recibido: '{cuentaPatchDto.TipoCuenta}'");
                }
            }
            
            if (cuentaPatchDto.SaldoInicial.HasValue)
                cuenta.SaldoInicial = cuentaPatchDto.SaldoInicial.Value;
            
            if (cuentaPatchDto.Estado.HasValue)
                cuenta.Estado = cuentaPatchDto.Estado.Value;
            
            await _unitOfWork.CompleteAsync();
            
            return _mapper.Map<CuentaDto>(cuenta);
        }

        public async Task<bool> DeleteCuentaAsync(int numeroCuenta)
        {
            var cuenta = await _unitOfWork.Cuentas.GetByNumeroCuentaAsync(numeroCuenta);
            if (cuenta == null)
            {
                throw new NotFoundException("Cuenta no encontrada");
            }

            // Soft delete - cambiar Estado a false
            cuenta.Estado = false;
            
            await _unitOfWork.CompleteAsync();
            
            return true;
        }
    }
}