using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly ApplicationDbContext _contexto;

        public ProductoRepository(ApplicationDbContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<IEnumerable<Producto>> GetAllAsync(int usuarioId)
        {
            return await _contexto.Productos.Where(p => p.UsuarioId == usuarioId).ToListAsync();
        }

        public async Task<Producto?> GetByIdAsync(int id, int usuarioId)
        {
            return await _contexto.Productos.FirstOrDefaultAsync(p => p.Id == id && p.UsuarioId == usuarioId);
        }

        public async Task<Producto> AddAsync(Producto entidad)
        {
            _contexto.Productos.Add(entidad);
            await _contexto.SaveChangesAsync();
            return entidad;
        }

        public async Task UpdateAsync(Producto entidad)
        {
            _contexto.Productos.Update(entidad);
            await _contexto.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entidad = await _contexto.Productos.FindAsync(id);
            if (entidad != null)
            {
                _contexto.Productos.Remove(entidad);
                await _contexto.SaveChangesAsync();
            }
        }
    }
}
