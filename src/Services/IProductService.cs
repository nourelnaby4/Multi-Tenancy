namespace API.Services;

public interface IProductService
{
    Task<Product> CreateAsync(Product product);
    Task<IReadOnlyList<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync (int id);    
}
