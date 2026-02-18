using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        // Servicio para manejar la lógica de negocio
        private readonly IProductoService _servicio;

        public ProductosController(IProductoService servicio)
        {
            _servicio = servicio;
        }

        // Helper para obtener el ID del usuario logueado
        private int ObtenerUsuarioId()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            if (idClaim != null && int.TryParse(idClaim.Value, out int id))
            {
                return id;
            }
            // Log this specific failure if you have a logger
            throw new UnauthorizedAccessException("Usuario no autenticado o ID inválido en Claims.");
        }

        // GET: api/Productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> ObtenerTodos()
        {
            try 
            {
                var usuarioId = ObtenerUsuarioId();
                return Ok(await _servicio.GetAllAsync(usuarioId));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        // GET: api/Productos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDto>> ObtenerPorId(int id)
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                var producto = await _servicio.GetByIdAsync(id, usuarioId);
                if (producto == null) return NotFound();
                return Ok(producto);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        // POST: api/Productos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<ProductoDto>> Crear(CreateProductoDto datos)
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                var creado = await _servicio.CreateAsync(datos, usuarioId);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                 var innerMessage = ex.InnerException?.Message ?? ex.Message;
                 return StatusCode(500, new { error = "Error al crear producto: " + innerMessage });
            }
        }

        // PUT: api/Productos/5
        [HttpPut("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, UpdateProductoDto datos)
        {
            if (id <= 0) return BadRequest();
            
            try
            {
                var usuarioId = ObtenerUsuarioId();
                var existente = await _servicio.GetByIdAsync(id, usuarioId);
                if (existente == null) return NotFound();

                await _servicio.UpdateAsync(id, datos, usuarioId);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        // DELETE: api/Productos/5
        [HttpDelete("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                var existente = await _servicio.GetByIdAsync(id, usuarioId);
                if (existente == null) return NotFound();

                await _servicio.DeleteAsync(id, usuarioId);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }
    }
}
