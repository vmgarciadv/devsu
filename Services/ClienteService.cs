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
    }
}