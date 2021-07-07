using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppOrder.Domain;

namespace WebAppOrder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {

        public ILogger<OrderController> logger;

        public OrderController(ILogger<OrderController> logger)
        {
            this.logger = logger;
        }

        public IActionResult InsertOrder(Order order)
        {
            try
            {
                return Accepted(order);
            }
            catch (Exception ex)
            {
                logger.LogError("Erro ao tentar criar uma novo pedido", ex);
                return new StatusCodeResult(500);
            }
        }
    }
}
