using JenkinsDemo.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JenkinsDemo.Extensions
{
    public static class RabbitMQSetup
    {
        public static void AddRabbitMQSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<RabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = "114.55.27.56",
                    Port= 5672,
                    UserName = "admin",
                    Password = "admin",
                    
                    //DispatchConsumersAsync = true
                };
                int retryCount = 5;
                return new RabbitMQPersistentConnection(factory, logger, 5);
            });
        }
    }
}
