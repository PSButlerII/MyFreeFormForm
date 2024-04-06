using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MyFreeFormForm.Models;
using MyFreeFormForm.Services;
using Nest;

namespace MyFreeFormForm.Controllers
{
    [ApiController]
    [Route("api")]
    public class ElasticSearchController : Controller
    {
        private readonly ILogger<ElasticSearchController> _logger;
        private readonly ElasticsearchService _elasticsearchService;

        public ElasticSearchController(ILogger<ElasticSearchController> logger, ElasticsearchService elasticsearchService)
        {
            _logger = logger;
            _elasticsearchService = elasticsearchService;
        }

        public IActionResult Index()
        {
            return View();
        }
     

        [HttpGet]
        public async Task<IActionResult> SearchResults(string searchTerm = "")
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                // No search term provided, just return the view without a model
                return View();
            }
            var response = await _elasticsearchService.Client.SearchAsync<Form>(s => s
                .Query(q => q
                    .Bool(b => b
                        .Should(
                            bs => bs.Match(m => m.Field(f => f.FormName).Query(searchTerm)),
                            bs => bs.Nested(n => n
                                .Path(p => p.FormFields)
                                .Query(nq => nq
                                    .Bool(nb => nb
                                        .Should(
                                            ns => ns.Match(m => m.Field(f => f.FormFields.First().FieldValue).Query(searchTerm))
                                        )
                                    )
                                )
                            )
                        )
                    )
                )
            );

            // Check if the response is valid and has hits
            if (response.IsValid && response.Hits.Any())
            {
                var results = response.Hits.Select(hit => hit.Source).ToList();
                return View(results);
            }
            else
            {
                // Return an empty list to avoid null reference exceptions
                return View(new List<Form>());
            }
        }

        [HttpPost("SearchFormsAsync")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> SearchFormsAsync(string searchTerm)
        {
            // Adjust the query to search across multiple fields
            var response = await _elasticsearchService.Client.SearchAsync<Form>(s => s
                .Query(q => q
                    .Bool(b => b
                        .Should(
                            bs => bs.Match(m => m.Field(f => f.FormName).Query(searchTerm)),
                            bs => bs.Nested(n => n
                                .Path(p => p.FormFields)
                                .Query(nq => nq
                                    .Bool(nb => nb
                                        .Should(
                                            ns => ns.Match(m => m.Field(f => f.FormFields.First().FieldValue).Query(searchTerm))
                                        )
                                    )
                                )
                            )
                        )
                    )
                )
            );


            if (!response.IsValid || !response.Hits.Any())
            {
                // log the error and the response if the search fails.
                _logger.LogInformation("Search failed with response: {response}", response);
                _logger.LogError("No search results found for the term: {searchTerm}", searchTerm);

                return Ok(new List<object>()); // Return an empty list to ensure valid JSON.
            }

            var results = response.Hits.Select(hit => hit.Source).ToList();
            return Ok(results);
        }

        public async Task<IAggregate> GetFormStatisticsAsync()
        {
            var response = await _elasticsearchService.Client.SearchAsync<Form>(s => s
                .Size(0) // Don't return documents
                .Aggregations(a => a
                    .Terms("top_form_names", t => t
                        .Field(f => f.FormName.Suffix("keyword"))
                        .Size(10)
                    )
                )
            );

            return response.Aggregations.Terms("top_form_names");
        }
    }
}
