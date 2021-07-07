using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using WebAppOrderTestRabbitMQ.Domain;

namespace WebApiRabbitMQConsume
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: "orderQueue",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                        );

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (model, ea) =>
                    {
                        try
                        {
                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);
                            var order = JsonSerializer.Deserialize<Order>(message);

                            Console.WriteLine($"Order {order.OrderNumber}|{order.ItemName}|{order.Price:N2}");

                            channel.BasicAck(ea.DeliveryTag, false);
                        }
                        catch (Exception ex)
                        {
                            //Logger
                            Console.WriteLine("Erro: "+ex);
                            channel.BasicNack(ea.DeliveryTag, false, true); //Informa um Não OK para o RabbitMQ em caso de exceções e coloca a mensagem de volta pra fila.
                        }
                    };

                    channel.BasicConsume(
                        queue: "orderQueue",
                        autoAck: false, //AutoAck informa que recebeu a mensagem e em seguida ela é apagada no RabbitMQ. Se houver uma exeção após informar que recebeu a mensagem você vai perder a mensagem pra sempre.
                        consumer: consumer);

                    Console.WriteLine("Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}