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
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        
        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetClientes()
        {
            try {
                var clientes = await _clienteService.GetAllClientesAsync();
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDto>> GetCliente(int id)
        {
            try
            {
                var cliente = await _clienteService.GetClienteByIdAsync(id);
                if (cliente == null)
                    return NotFound();
                    
                return Ok(cliente);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<ClienteDto>> CreateCliente(ClienteDto clienteDto)
        {
            try
            {
                var cliente = await _clienteService.CreateClienteAsync(clienteDto);
                return CreatedAtAction(nameof(GetCliente), new { id = cliente.Id }, cliente);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ClienteDto>> UpdateCliente(int id, ClienteDto clienteDto)
        {
            try
            {
                var cliente = await _clienteService.UpdateClienteAsync(id, clienteDto);
                return Ok(cliente);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<ClienteDto>> PatchCliente(int id, [FromBody] ClientePatchDto clientePatchDto)
        {
            try
            {
                var cliente = await _clienteService.PatchClienteAsync(id, clientePatchDto);
                return Ok(cliente);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}