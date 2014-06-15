using System;
using System.Text;
using RabbitMQ.Client;

namespace Demo2_Sender
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

        private static void Main(string[] args)
        {
            // Build a new channel to Rabbit.
            IModel channel = BuildChannel(RabbitConfiguration.Server, RabbitConfiguration.Username, RabbitConfiguration.Password);

            Console.WriteLine("Listening for input...");

//            while (true)
//            {
                // Capture input from user
//                string msg = Console.ReadLine();

                // Declare a new queue called 'hello'
                channel.QueueDeclare(RabbitConfiguration.QueueName, false, false, false, null);

                for (int i = 0; i < 100000; i++)
                {
                    // Convert input to byte array
//                    byte[] body = Encoding.UTF8.GetBytes(msg);
                    byte[] body = Encoding.UTF8.GetBytes(i.ToString());

                    // Write the byte array to the 'hello' queue.
                    channel.BasicPublish("", RabbitConfiguration.QueueName, null, body);

                    // Notify user that message was sent
                    string notify = String.Format(" [x] Sent {0}", i);

                    Console.WriteLine(notify);
                }

//            }
        }
    }
}