using Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Repositories
{
    public interface IProductoRepository
    {
        Task<IEnumerable<Producto>> GetAllAsync(int usuarioId);
        Task<Producto?> GetByIdAsync(int id, int usuarioId);
        Task<Producto> AddAsync(Producto entidad);
        Task UpdateAsync(Producto entidad);
        Task DeleteAsync(int id);
    }
}
