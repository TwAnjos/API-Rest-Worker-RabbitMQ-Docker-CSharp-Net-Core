using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;
using WebAppOrderTestRabbitMQ.Domain;

namespace WebAppOrderTestRabbitMQ.Controllers
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
                #region This project was created as RabbitMQ tests only, without any architecture, rules or best practices implemented.

                var factory = new ConnectionFactory() { HostName = "localhost" };

                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: "orderQueue",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    string message = JsonSerializer.Serialize(order);

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: "orderQueue",
                        basicProperties: null,
                        body: body);
                }

                #endregion This project was created as RabbitMQ tests only, without any architecture, rules or best practices implemented.

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