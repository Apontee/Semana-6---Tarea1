using Backend.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Services
{
    public interface IProductoService
    {
        Task<IEnumerable<ProductoDto>> GetAllAsync(int usuarioId);
        Task<ProductoDto?> GetByIdAsync(int id, int usuarioId);
        Task<ProductoDto> CreateAsync(CreateProductoDto datos, int usuarioId);
        Task UpdateAsync(int id, UpdateProductoDto datos, int usuarioId);
        Task DeleteAsync(int id, int usuarioId);
    }
}
