using JenkinsDemo.RabbitMQ;
using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace JenkinsDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class RabbitMQController : ControllerBase
    {
        string queue = "send.msg.queues";
        string exchange = "send.msg";
        string eventname = "eventname";
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private IModel _consumerChannel;

        public RabbitMQController(IRabbitMQPersistentConnection persistentConnection)
        {
            //_persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));

            //_consumerChannel = CreateConsumerChannel();

            //SubsManager_OnEventRemoved();

        }

        [HttpGet("{message}")]
        public string pushtwo(string message)
        {
            IConnectionFactory connFactory = new ConnectionFactory//创建连接工厂对象
            {
                HostName = "114.55.27.56",
                Port = 5672,
                UserName = "admin",
                Password = "admin",
            };
            using (IConnection conn = connFactory.CreateConnection())
            {
                using (IModel channel = conn.CreateModel())
                {
                    string queueName = queue;
                    //声明一个队列
                    channel.QueueDeclare(
                      queue: queueName,//消息队列名称
                      durable: true,//是否缓存
                      exclusive: false,
                      autoDelete: false,
                      arguments: null
                       );

                    //消息内容
                    byte[] body = Encoding.UTF8.GetBytes(message);
                    //发送消息
                    channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
                }
            }

            return message;
        }

        [HttpGet]
        [Route("/api/RabbitMQ/receive")]
        public string receive()
        {
            string res = "";
            IConnectionFactory connFactory = new ConnectionFactory//创建连接工厂对象
            {
                HostName = "114.55.27.56",
                Port = 5672,
                UserName = "admin",
                Password = "admin",
            };
            using (IConnection conn = connFactory.CreateConnection())
            {
                using (IModel channel = conn.CreateModel())
                {
                    string queueName = queue;
                    //声明一个队列
                    channel.QueueDeclare(
                      queue: queueName,//消息队列名称
                      durable: true,//是否缓存
                      exclusive: false,
                      autoDelete: false,
                      arguments: null
                       );
                    //创建消费者对象
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        res = "接收到信息为:"+ Encoding.UTF8.GetString(ea.Body.ToArray());
                    };
                    //消费者开启监听
                    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

                }
            }

            return res;
        }


        [HttpGet]
        public string push(string message)
        {
            string res = string.Empty;
            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
              .Or<SocketException>()
              .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
              {
                  res = string.Format("{0},{1}", ex.Message, $"{time.TotalSeconds:n1}");
              });

            var body = Encoding.UTF8.GetBytes(message);

            policy.Execute(() =>
            {
                var properties = _consumerChannel.CreateBasicProperties();
                properties.DeliveryMode = 2; // persistent

                res = "Publishing event to RabbitMQ";

                _consumerChannel.BasicPublish(
                    exchange: exchange,
                    routingKey: eventname,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
            });

            return "push success";
        }

        /// <summary>
        /// 订阅管理器事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventName"></param>
        private void SubsManager_OnEventRemoved()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            using (var channel = _persistentConnection.CreateModel())
            {
                channel.QueueUnbind(queue: queue,
                    exchange: exchange,
                    routingKey: eventname);
            }
        }

        /// <summary>
        /// 创造消费通道
        /// </summary>
        /// <returns></returns>
        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            //_logger.LogTrace("Creating RabbitMQ consumer channel");

            var channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: exchange,
                                    type: "direct", durable: true);

            channel.QueueDeclare(queue: queue,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
                StartBasicConsume();
            };

            return channel;
        }

        /// <summary>
        /// 开始基本消费
        /// </summary>
        private void StartBasicConsume()
        {
            if (_consumerChannel != null)
            {
                var consumer = new AsyncEventingBasicConsumer(_consumerChannel);

                //consumer.Received += Consumer_Received;

                _consumerChannel.BasicConsume(
                    queue: queue,
                    autoAck: false,
                    consumer: consumer);
            }
            else
            {
                /* _logger.LogError("StartBasicConsume can't call on _consumerChannel == null");*/
            }
        }

    }
}
