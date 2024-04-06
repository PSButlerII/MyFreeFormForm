using Elasticsearch.Net;
using Nest;
using MyFreeFormForm.Models;
using System;

namespace MyFreeFormForm.Services
{
    public class ElasticsearchService
    {
        public ElasticClient Client { get; private set; }

        public ElasticsearchService(string uri)
        {
            var settings = new ConnectionSettings(new Uri(uri))
                .DefaultIndex("forms");
            Client = new ElasticClient(settings);

            EnsureIndexWithMapping();
        }

        public void EnsureIndexWithMapping()
        {
            var existsResponse = Client.Indices.Exists("forms");
            if (!existsResponse.Exists)
            {
                var createIndexResponse = Client.Indices.Create("forms", c => c
                    .Settings(s => s
                        .NumberOfShards(1)
                        .NumberOfReplicas(1))
                    .Map<Form>(m => m
                        .AutoMap()
                        .Properties(ps => ps
                            .Text(t => t
                                .Name(n => n.FormName)
                                .Fielddata(true))
                            .Date(d => d
                                .Name(n => n.CreatedDate)
                                .Format("strict_date_optional_time||epoch_millis"))
                            .Date(d => d
                                .Name(n => n.UpdatedDate)
                                .Format("strict_date_optional_time||epoch_millis"))
                            .Nested<FormField>(n => n
                                .Name(nn => nn.FormFields)
                                .AutoMap()
                                .Properties(pps => pps
                                    .Text(t => t
                                        .Name(nf => nf.FieldName))
                                    .Text(t => t
                                        .Name(nf => nf.FieldValue))
                                // Define other properties for FormField if necessary
                                )
                            )
                        )
                        )
                );
                // Optionally log success or failure of index creation
            }
        }
    }
}
