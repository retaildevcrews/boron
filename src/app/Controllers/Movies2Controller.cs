// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using Boron.Extensions;
using CSE.Boron.DataAccessLayer;
using CSE.Boron.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CSE.Boron.Controllers
{
    /// <summary>
    /// Handle all of the /api/movies requests
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class Movies2Controller : Controller
    {
        private readonly ILogger logger;
        private readonly IDAL dal;
        private IConfigurationRoot config;

        /// <summary>
        ///  Constructor
        /// </summary>
        /// <param name="logger">log instance</param>
        /// <param name="dal">data access layer instance</param>
        public Movies2Controller(ILogger<MoviesController> logger, IDAL dal, IConfiguration config)
        {
            this.logger = logger;
            this.dal = dal;
            this.config = config as IConfigurationRoot;
        }

        /// <summary>
        /// Returns a JSON array of Movie objects
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public async Task<IActionResult> GetMoviesAsync([FromQuery] MovieQueryParameters movieQueryParameters)
        {
            if (movieQueryParameters == null)
            {
                throw new ArgumentNullException(nameof(movieQueryParameters));
            }

            string endpointUrl = config.GetValue<string>(Constants.SearchEndpoint);
            string key = config.GetValue<string>(Constants.SearchKey);
            string index = config.GetValue<string>(Constants.SearchIndex);
            Uri endpoint = new Uri(endpointUrl);
            AzureKeyCredential credential = new AzureKeyCredential(key);
            SearchIndexClient indexClient = new SearchIndexClient(endpoint, credential);
            SearchClient searchclient = indexClient.GetSearchClient(index);

            var options = new SearchOptions()
            {
                IncludeTotalCount = true, // If not set the property TotalCount is set as null
                Size = 100, // PageSize
            };

            var response = await searchclient
                .SearchAsync<Movie>("*", options)
                .ConfigureAwait(false);
            Console.WriteLine("Total count: {response.Value.TotalCount}");

            // Below is the raw code to re-read the raw response
            // response.GetRawResponse().ContentStream.Position = 0;
            // var byteArray = new byte[response.GetRawResponse().ContentStream.Length];
            // var count = response.GetRawResponse().ContentStream.Read(byteArray, 0, (int)response.GetRawResponse().ContentStream.Length - 1);
            // string converted = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            // Iterate over the index result
            var searchResults = response.Value.GetResultsAsync();
            int count = 1;
            var movies = new List<Movie>();
            await foreach (SearchResult<Movie> resp in searchResults)
            {
                Movie doc = resp.Document;
                var score = resp.Score;
                movies.Add(resp.Document);
                Console.WriteLine($"Count: {count}, Rid: {doc.Rid}, MovieId: {doc.MovieId}, Id: {doc.Id}, Score: {score}");
                if (count++ == 50)
                {
                    break;
                }
            }

            // Create an array of movie ids from the index search
            var movieIds = movies.Select(movie => movie.MovieId);

            // return await ResultHandler.Handle(movies, );
            return await ResultHandler.Handle(
                Task<List<Movie>>.FromResult(movies), movieQueryParameters.GetMethodText(HttpContext), Constants.MoviesControllerException, logger)
                .ConfigureAwait(false);
        }
    }
}
