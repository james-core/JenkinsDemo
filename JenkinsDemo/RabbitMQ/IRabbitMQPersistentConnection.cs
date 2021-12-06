using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JenkinsDemo.RabbitMQ
{
    /// <summary>
    /// RabbitMQ持久连接
    /// 接口
    /// </summary>
    public interface IRabbitMQPersistentConnection
        : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}
