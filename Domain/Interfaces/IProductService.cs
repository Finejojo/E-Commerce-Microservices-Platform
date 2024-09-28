using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IProductService
    {

        Task<ProductDto> CreateAsync(CreateProductDto productDto);
        Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto productDto);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<ProductDto>> GetAllAsync();
    }
}
