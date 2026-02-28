using ProcurementSystem.API.DTOs;

namespace ProcurementSystem.API.Interfaces
{
    public interface ISupplierService
    {
        Task<List<SupplierDTO>> GetAllSuppliersAsync();
        Task<SupplierDTO> GetSupplierByIdAsync(int id);
        Task<int> CreateSupplierAsync(CreateSupplierDTO dto);
        Task UpdateSupplierAsync(int id, UpdateSupplierDTO dto);
        Task DeleteSupplierAsync(int id);
        Task<List<SupplierDTO>> GetSuppliersByCountryAsync(string country);
        Task<List<SupplierDTO>> GetTopRatedSuppliersAsync(int count);

    }
}
