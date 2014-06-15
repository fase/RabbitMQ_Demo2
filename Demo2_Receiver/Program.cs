using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Demo2_Receiver
{
    class Program
    {
        private readonly static dynamic RabbitConfiguration = new
        {
            Server = "192.168.100.86",
            Username = "jfasenmyer",
            Password = "password",
            QueueName = "hello"
        };

        public static IModel BuildChannel(string hostName, string username, string password, int port = 5672)
        {
            ConnectionFactory factory = new ConnectionFactory { HostName = hostName, Port = port, UserName = username, Password = password };

            IConnection connection = factory.CreateConnection();

            return connection.CreateModel();
        }

        static void Main(string[] args)
        {
            // Build a new channel to Rabbit.
            IModel channel = BuildChannel(RabbitConfiguration.Server, RabbitConfiguration.Username, RabbitConfiguration.Password);

            channel.QueueDeclare(RabbitConfiguration.QueueName, false, false, false, null);

            //const bool noAck = true;

            const bool noAck = false;
            channel.BasicQos(0, 1, false);

            var consumer = new QueueingBasicConsumer(channel);
            channel.BasicConsume(RabbitConfiguration.QueueName, noAck, consumer);

            Console.WriteLine(" [*] Waiting for messages...");

            while (true)
            {
                var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                var body = ea.Body;

                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine(" [x] Received {0}", message);

                Thread.Sleep(5000);

                channel.BasicAck(ea.DeliveryTag, false);
            }
        }
    }
}