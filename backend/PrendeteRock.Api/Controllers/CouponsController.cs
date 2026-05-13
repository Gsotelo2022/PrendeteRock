using Microsoft.AspNetCore.Mvc;
using PrendeteRock.API.Services;

namespace PrendeteRock.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CouponsController : ControllerBase
    {
        private readonly CouponService _couponService;

        public CouponsController(CouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var coupons = await _couponService.GetAllAsync();
            return Ok(coupons);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var coupon = await _couponService.GetByIdAsync(id);
            if (coupon == null)
                return NotFound();
            return Ok(coupon);
        }

        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var coupon = await _couponService.GetByCodeAsync(code);
            if (coupon == null)
                return NotFound(new { message = "Cupón no encontrado o inactivo" });
            return Ok(coupon);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCouponRequest request)
        {
            try
            {
                var coupon = await _couponService.CreateAsync(
                    request.Code,
                    request.Percentage,
                    request.ExpiryDate,
                    request.UsageLimit);
                return Ok(coupon);
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    message = ex.Message, 
                    innerException = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCouponRequest request)
        {
            try
            {
                await _couponService.UpdateAsync(
                    id,
                    request.Code,
                    request.Percentage,
                    request.ExpiryDate,
                    request.UsageLimit,
                    request.IsActive);
                return Ok(new { message = "Cupón actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                await _couponService.ToggleStatusAsync(id);
                return Ok(new { message = "Estado del cupón actualizado" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _couponService.DeleteAsync(id);
                return Ok(new { message = "Cupón eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class CreateCouponRequest
    {
        public string Code { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? UsageLimit { get; set; }
    }

    public class UpdateCouponRequest
    {
        public string Code { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? UsageLimit { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
