using ProcurementSystem.API.DTOs;
using ProcurementSystem.API.Interfaces;
using ProcurementSystem.API.Models;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
using static ProcurementSystem.API.Exceptions.CustomExceptions;

namespace ProcurementSystem.API.Services
{
    public class SupplierService:ISupplierService
    {
        private readonly ISupplierRepository _repository;
        private readonly ILogger<SupplierService> _logger;

        public SupplierService(ISupplierRepository repository, ILogger<SupplierService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<List<SupplierDTO>> GetAllSuppliersAsync() {

            try
            {
                _logger.LogInformation("Fetching all Suppliers from the database.");
                var suppliers = await _repository.GetAllAsync();
                return suppliers.Select(s => MapToDTO(s)).ToList();

            }catch(SqlException ex)
            {
                _logger.LogError(ex, "An error occurred while fetching suppliers.");
                throw new DatabaseException("An error occurred while fetching suppliers. Please try again later.",ex);
            }
        }
        public async Task<SupplierDTO> GetSupplierByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Fetching Supplier with ID {SupplierId} from the database.", id);
                var supplier = await _repository.GetByIdAsync(id);
                if (supplier == null)
                {
                    throw new NotFoundException($"Employee with {id} not found");
                }
                return MapToDTO(supplier);
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "An error occurred while fetching supplier with ID {SupplierId}.", id);
                throw new DatabaseException($"An error occurred while fetching supplier with ID {id}. Please try again later.", ex);
            }
        }

        public async Task<int> CreateSupplierAsync(CreateSupplierDTO dto)
        {
            try
            {
                _logger.LogInformation("Creating a new Supplier in the database.");
                var supplier = new Supplier
                {
                    SupplierName = dto.SupplierName,
                    ContactEmail = dto.ContactEmail,
                    ContactPhone = dto.ContactPhone,
                    Country = dto.Country,
                    Rating = dto.Rating,
                };
                var supplierId = await _repository.CreateAsync(supplier);
                if (supplierId < 0)
                {
                    throw new DuplicateException("Supplier with Email already exists");
                }
                return supplierId;
            }
            catch (DuplicateException)
            {
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new supplier.");
                throw new DatabaseException("An error occurred while creating a new supplier. Please try again later.", ex);
            }
        }

        public async Task UpdateSupplierAsync(int id, UpdateSupplierDTO dto)
        {
            try
            {
                _logger.LogInformation($"Updating supplier with id: {id}");
                var supplier = new Supplier
                {
                    SupplierId = id,
                    SupplierName = dto.SupplierName,
                    ContactEmail = dto.ContactEmail,
                    ContactPhone = dto.ContactPhone,
                    Country = dto.Country,
                    Rating = dto.Rating,
                };
                var isUpdated = await _repository.UpdateAsync(supplier);
                if (!isUpdated)
                {
                    throw new NotFoundException($"Supplier with id{id} does not exist");
                }
                _logger.LogInformation($"Supplier with id {id} updated successfully.");
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, $"An error occurred while updating supplier with id {id}.");
                throw new DatabaseException($"An error occurred while updating supplier with id {id}. Please try again later.", ex);
            }
        }

        public async Task DeleteSupplierAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting supplier with id: {id}");
                var deleted = await _repository.DeleteAsync(id);
                if (!deleted)
                {
                    throw new NotFoundException($"Supplier with id{id} does not exist");
                }
                _logger.LogInformation($"Supplier with id {id} deleted successfully.");
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting supplier with id {id}.");
                throw new DatabaseException($"An error occurred while deleting supplier with id {id}. Please try again later.", ex);
            }
        }

        public async Task<List<SupplierDTO>> GetSuppliersByCountryAsync(string country)
        {
            try
            {
                _logger.LogInformation($"Fetching suppliers from country: {country}");
                var suppliers = await _repository.GetByCountryAsync(country);
                return suppliers.Select(s => MapToDTO(s)).ToList();
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching suppliers from country {country}.");
                throw new DatabaseException($"An error occurred while fetching suppliers from country {country}. Please try again later.", ex);
            }
        }

        public async Task<List<SupplierDTO>> GetTopRatedSuppliersAsync(int count)
        {
            try
            {
                _logger.LogInformation($"Fetching top {count} rated suppliers.");
                var suppliers = await _repository.GetTopRatedAsync(count);
                return suppliers.Select(s => MapToDTO(s)).ToList();
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching top {count} rated suppliers.");
                throw new DatabaseException($"An error occurred while fetching top {count} rated suppliers. Please try again later.", ex);
            }
        }

        private SupplierDTO MapToDTO(Supplier supplier)
        {
            return new SupplierDTO
            {
                SupplierId = supplier.SupplierId,
                SupplierName = supplier.SupplierName,
                ContactEmail = supplier.ContactEmail,
                ContactPhone = supplier.ContactPhone,
                Country = supplier.Country,
                Rating = supplier.Rating
            };
        }

    }
}
