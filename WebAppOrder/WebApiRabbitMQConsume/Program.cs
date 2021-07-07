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
                            channel.BasicNack(ea.DeliveryTag, false, true); //Informa um N�o OK para o RabbitMQ em caso de exce��es e coloca a mensagem de volta pra fila.
                        }
                    };

                    channel.BasicConsume(
                        queue: "orderQueue",
                        autoAck: false, //AutoAck informa que recebeu a mensagem e em seguida ela � apagada no RabbitMQ. Se houver uma exe��o ap�s informar que recebeu a mensagem voc� vai perder a mensagem pra sempre.
                        consumer: consumer);

                    Console.WriteLine("Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}