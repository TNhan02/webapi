using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Context;
using webapi.Models;
using webapi.Models.DTO;

namespace webapi.Services
{
    public interface IProduct
    {
        Task<IEnumerable<object>> GetProducts(PaginationFilter filter);
        Task<object> GetProduct(int id);
        Task<object> PatchProduct(int id, [FromBody] DTOProduct editProduct);
        Task<object> PostProduct([FromBody] DTOProduct newProduct);
        Task<object> DeleteProduct(int id);
    }

    public class ProductServices : IProduct
    {
        private ServerContext _context;
        private readonly IMapper _mapper;

        public ProductServices(ServerContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // apply pagination filter to GET all Products
        public async Task<IEnumerable<object>> GetProducts(PaginationFilter filter)
        {
            try
            {
                var products = await _context.Products.Skip((filter.pageNumber - 1) * filter.pageSize)
                                                .Take(filter.pageSize)
                                                .ToListAsync();

                var productDTOs = products.Select(product => new
                {
                    Id = product.Id,
                    Name = product.Name,
                    Quantity = product.Quantity,
                    Description = product.Description,
                    AdditionalInfo = product.AdditionalInfo
                }).ToList();

                return productDTOs;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<object> GetProduct(int id)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(product => product.Id == id);

                if (product != null)
                {
                    var productDTO = new
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Quantity = product.Quantity,
                        Description = product.Description,
                        AdditionalInfo = product.AdditionalInfo
                    };

                    return productDTO;
                }
                else
                {
                    return new NotFoundObjectResult($"Product {id} not found");
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<object> PatchProduct(int id, [FromBody] DTOProduct editProduct)
        {
            var product = await _context.Products.FirstOrDefaultAsync(product => product.Id == id);

            if (product == null)
            {
                return new NotFoundObjectResult($"Product {id} not found to edit");
            }

            try
            {
                AdjustProductProperties(product, editProduct);
                await _context.SaveChangesAsync();

                return editProduct;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<object> PostProduct([FromBody] DTOProduct newProduct)
        {
            if (newProduct == null)
            {
                return new BadRequestObjectResult("Missing input data to create a new product");
            }

            try
            {
                var product = _mapper.Map<Product>(newProduct);

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return newProduct;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<object> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product {id} not found to delete");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return null;
        }

        private void AdjustProductProperties(Product product, DTOProduct editProduct)
        {
            var properties = typeof(DTOProduct).GetProperties();

            foreach (var property in properties)
            {
                var newValue = property.GetValue(editProduct);

                if (newValue != null)
                {
                    var productProperty = product.GetType().GetProperty(property.Name);
                    productProperty.SetValue(product, newValue);
                }
            }
        }
    }
}
