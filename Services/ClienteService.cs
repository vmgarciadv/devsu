using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public async Task<ClienteDto> PatchClienteAsync(int id, ClientePatchDto clientePatchDto)
        {
            var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);
            if (cliente == null)
            {
                throw new KeyNotFoundException("Cliente no encontrado");
            }

            // Validar que no exista otro cliente con la misma Identificación (en caso de ser necesario)
            if (clientePatchDto.Identificacion != null && clientePatchDto.Identificacion != cliente.Identificacion)
            {
                var clienteExistente = await _unitOfWork.Clientes.GetByIdentificacionAsync(clientePatchDto.Identificacion);
                if (clienteExistente != null && clienteExistente.Id != id)
                {
                    throw new InvalidOperationException("Ya existe un cliente con esa Identificacion");
                }
            }

            // Usar reflection para actualizar solo las propiedades no nulas
            var patchProperties = typeof(ClientePatchDto).GetProperties();
            var clienteType = typeof(Cliente);

            foreach (var patchProp in patchProperties)
            {
                var value = patchProp.GetValue(clientePatchDto);
                
                // Omitir valores nulos y cadenas vacías
                if (value == null || (value is string str && string.IsNullOrEmpty(str)))
                    continue;

                var clienteProp = clienteType.GetProperty(patchProp.Name);
                if (clienteProp != null && clienteProp.CanWrite)
                {
                    clienteProp.SetValue(cliente, value);
                }
            }

            await _unitOfWork.CompleteAsync();
            
            return _mapper.Map<ClienteDto>(cliente);
        }

        public async Task<bool> DeleteClienteAsync(int id)
        {
            var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);
            if (cliente == null)
            {
                throw new KeyNotFoundException("Cliente no encontrado");
            }

            // Soft delete - cambiar Estado a false
            cliente.Estado = false;
            
            await _unitOfWork.CompleteAsync();
            
            return true;
        }
    }
}