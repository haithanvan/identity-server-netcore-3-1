using Elasticsearch.Net;
using Nest;
using System;

namespace Nmb.Shared.Elastic
{
    public class ElasticSearch
    {
        public ElasticClient Client
        {
            get
            {
                var nodes = new Uri[]
                {
                    new Uri("http://elasticsearch:9200/"),
                };

                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming();
                var elasticClient = new ElasticClient(connectionSettings);

                return elasticClient;
            }
        }
    }
}
