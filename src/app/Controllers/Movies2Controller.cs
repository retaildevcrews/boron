// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using CSE.Boron.DataAccessLayer;
using CSE.Boron.Model;
using Microsoft.AspNetCore.Mvc;
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

        /// <summary>
        ///  Constructor
        /// </summary>
        /// <param name="logger">log instance</param>
        /// <param name="dal">data access layer instance</param>
        public Movies2Controller(ILogger<MoviesController> logger, IDAL dal)
        {
            this.logger = logger;
            this.dal = dal;
        }

        /// <summary>
        /// Returns a JSON array of Movie objects
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public async Task<IActionResult> GetMoviesAsync()
        {
            var endpoint = new Uri("https://<YOUR SEARCH NAME>.search.windows.net");
            var credential = new AzureKeyCredential("<YOUR API KEY>");
            var indexClient = new SearchIndexClient(endpoint, credential);
            var srchclient = indexClient.GetSearchClient("<YOUR INDEX NAME>");

            var options = new SearchOptions()
            {
                IncludeTotalCount = true, // If not set the property TotalCount is set as null
                Size = 100, // PageSize
            };

            var response = await srchclient
                .SearchAsync<Movie>("*", options)
                .ConfigureAwait(false);

            // Below is the raw code to re-read the raw response
            // response.GetRawResponse().ContentStream.Position = 0;
            // var byteArray = new byte[response.GetRawResponse().ContentStream.Length];
            // var count = response.GetRawResponse().ContentStream.Read(byteArray, 0, (int)response.GetRawResponse().ContentStream.Length - 1);
            // string converted = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

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

            return Ok(movies);

            // search=tt0385887,tt0326965,tt0069049&searchFields=movieId
            // TODO - add Azure Search query here

            // if (movieQueryParameters == null)
            // {
            //     throw new ArgumentNullException(nameof(movieQueryParameters));
            // }

            // return await ResultHandler.Handle(
            //     dal.GetMoviesAsync(movieQueryParameters), movieQueryParameters.GetMethodText(HttpContext), Constants.MoviesControllerException, logger)
            //     .ConfigureAwait(false);
        }
    }
}
