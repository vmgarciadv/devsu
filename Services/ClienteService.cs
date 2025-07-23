using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using devsu.DTOs;
using devsu.Models;
using devsu.Repositories;

namespace devsu.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        
        public ClienteService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ClienteDto> GetClienteByIdAsync(int id)
        {
            var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);
            return _mapper.Map<ClienteDto>(cliente);
        }
        
        public async Task<ClienteDto> CreateClienteAsync(ClienteDto clienteDto)
        {
            // Verificar si ya existe un cliente con la misma identificación
            var clienteExistente = await _unitOfWork.Clientes.GetByIdentificacionAsync(clienteDto.Identificacion);
            if (clienteExistente != null)
            {
                throw new InvalidOperationException("Ya existe un cliente con esa Identificacion.");
            }
            
            var cliente = _mapper.Map<Cliente>(clienteDto);
            await _unitOfWork.Clientes.AddAsync(cliente);
            await _unitOfWork.CompleteAsync();
            
            return _mapper.Map<ClienteDto>(cliente);
        }

        public async Task<IEnumerable<ClienteDto>> GetAllClientesAsync()
        {
            var clientes = await _unitOfWork.Clientes.GetAllAsync();
            return _mapper.Map<IEnumerable<ClienteDto>>(clientes);
        }

        public async Task<ClienteDto> UpdateClienteAsync(int id, ClienteDto clienteDto)
        {
            var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);
            if (cliente == null)
            {
                throw new KeyNotFoundException("Cliente no encontrado");
            }

            // Verificar si la identificación ya está en uso por otro cliente
            if (clienteDto.Identificacion != cliente.Identificacion)
            {
                var clienteExistente = await _unitOfWork.Clientes.GetByIdentificacionAsync(clienteDto.Identificacion);
                if (clienteExistente != null && clienteExistente.Id != id)
                {
                    throw new InvalidOperationException("Ya existe un cliente con esa Identificacion.");
                }
            }

            // Actualizar manualmente las propiedades para evitar problemas con el Id
            cliente.Nombre = clienteDto.Nombre;
            cliente.Genero = clienteDto.Genero;
            cliente.Edad = clienteDto.Edad;
            cliente.Identificacion = clienteDto.Identificacion;
            cliente.Direccion = clienteDto.Direccion;
            cliente.Telefono = clienteDto.Telefono;
            cliente.Contrasena = clienteDto.Contrasena;
            cliente.Estado = clienteDto.Estado;
            
            await _unitOfWork.CompleteAsync();
            
            return _mapper.Map<ClienteDto>(cliente);
        }
    }
}