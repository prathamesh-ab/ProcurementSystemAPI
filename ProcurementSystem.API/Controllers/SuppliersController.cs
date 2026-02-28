using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProcurementSystem.API.DTOs;
using ProcurementSystem.API.Interfaces;

namespace ProcurementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _service;

        public SuppliersController(ISupplierService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var suppliers = await _service.GetAllSuppliersAsync();
            return Ok(new ApiResponse<List<SupplierDTO>>
            {
                Success = true,
                Data = suppliers
            });
        }

        [HttpGet("{id")]
        public async Task<IActionResult> GetById(int id)
        {
            var supplier = await _service.GetSupplierByIdAsync(id);
            return Ok(new ApiResponse<SupplierDTO>
            {
                Success = true,
                Data = supplier
            });
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSupplierDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponse<int>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = errors
                });
            }

            var supplierId = await _service.CreateSupplierAsync(dto);

            return CreatedAtAction(nameof(GetById),
                new { id = supplierId },
                new ApiResponse<int>
                {
                    Success = true,
                    Message = "Supplier created successfully",
                    Data = supplierId
                });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSupplierDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = errors
                });
            }

            await _service.UpdateSupplierAsync(id, dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Supplier updated successfully"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteSupplierAsync(id);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Supplier deleted successfully"
            });
        }

        [HttpGet("country/{country}")]
        public async Task<IActionResult> GetByCountry([FromRoute] string country)
        {
            var suppliers = await _service.GetSuppliersByCountryAsync(country);
            return Ok(new ApiResponse<List<SupplierDTO>>
            {
                Success = true,
                Data = suppliers
            });
        }

        [HttpGet("top-rated")]
        public async Task<IActionResult> GetTopRated([FromQuery] int count)
        {
            var suppliers = await _service.GetTopRatedSuppliersAsync(count);
            return Ok(new ApiResponse<List<SupplierDTO>>
            {
                Success = true,
                Data = suppliers
            });
        }

    }
}
