using BookStore.BLL.Services.ViewModels;
using BookStore.BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.PL.Controllers
{
    public class PaymentController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public PaymentController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("process-payment/{orderId}")]
        public async Task<IActionResult> ProcessPayment(int orderId, [FromBody] PaymentVm paymentVm)
        {
            var result = await _orderService.ProcessPaymentAsync(orderId, paymentVm);
            if (result)
            {
                return Ok(new { Message = "Thanh toán được xử lý thành công.", OrderId = orderId });
            }
            return BadRequest("Xử lý thanh toán không thành công.");
        }

        [HttpPost("complete-order/{orderId}")]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            var order = await _orderService.CompleteOrderAsync(orderId);
            if (order != null)
            {
                return Ok(order);
            }
            return NotFound($"Order với ID {orderId} không tìm thấy.");
        }
    }
}
