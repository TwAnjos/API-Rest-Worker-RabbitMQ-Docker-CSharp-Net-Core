using System;

namespace WebAppOrderTestRabbitMQ.Domain
{
    public sealed class Order
    {
        public int OrderNumber { get; set; }

        public String ItemName { get; set; }

        public float Price { get; set; }
    }
}