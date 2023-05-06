using System;
using System.Text;
using System.Threading;
using PaymentSystem.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PaymentSystem.Controllers
{
    public class PaymentsController
    {
        public void main()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "payment_requests",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.QueueDeclare(queue: "payment_responses",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);
                    var paymentRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<PaymentRequest>(message);

                    // simulate payment processing
                    Thread.Sleep(5000);

                    bool paymentSucceeded = true;
                    if (new Random().Next(10) == 0) // 1 in 10 transactions fail
                    {
                        paymentSucceeded = false;
                    }

                    var paymentResponse = new PaymentResponse()
                    {
                        TransactionId = Guid.NewGuid().ToString(),
                        Amount = paymentRequest.Amount,
                        Timestamp = DateTime.UtcNow,
                        Success = paymentSucceeded
                    };

                    var responseJson = Newtonsoft.Json.JsonConvert.SerializeObject(paymentResponse);
                    var responseBytes = Encoding.UTF8.GetBytes(responseJson);
                    channel.BasicPublish(exchange: "",
                                         routingKey: "payment_responses",
                                         basicProperties: null,
                                         body: responseBytes);

                    Console.WriteLine(" [x] Sent {0}", responseJson);
                };
                channel.BasicConsume(queue: "payment_requests",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine("Payment service is listening for payment requests...");
                Console.ReadLine();
            }
        }
    }
}