using ProcurementSystem.API.Models;

namespace ProcurementSystem.API.Interfaces
{
    public interface ISupplierRepository
    {
        Task<List<Supplier>> GetAllAsync();
        Task<Supplier> GetByIdAsync(int id);
        Task<int> CreateAsync(Supplier supplier);
        Task<bool> UpdateAsync(Supplier supplier);
        Task<bool> DeleteAsync(int id);
        Task<List<Supplier>> GetByCountryAsync(string country);
        Task<List<Supplier>> GetTopRatedAsync(int count);

    }
}
