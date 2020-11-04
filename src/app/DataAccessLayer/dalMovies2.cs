// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using CSE.Boron.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSE.Boron.DataAccessLayer
{
    /// <summary>
    /// Data Access Layer for CosmosDB
    /// </summary>
    public partial class DAL
    {
        /// <summary>
        /// Get a list of Movies by search and/or filter terms
        /// </summary>
        /// <param name="movieQueryParameters">movie search parameters</param>
        /// <returns>List of Movies or an empty list</returns>
        public async Task<IEnumerable<Movie>> GetMovies2Async(MovieQueryParameters movieQueryParameters, string[] movieIds = null)
        {
            _ = movieQueryParameters ?? throw new ArgumentNullException(nameof(movieQueryParameters));

            int offset = movieQueryParameters.GetOffset();
            int limit = movieQueryParameters.PageSize;

            if (!string.IsNullOrWhiteSpace(movieQueryParameters.Q))
            {
                movieQueryParameters.Q = movieQueryParameters.Q.Trim();
            }

            Uri endpoint = new Uri(searchEndpoint);
            AzureKeyCredential credential = new AzureKeyCredential(searchKey);
            SearchIndexClient indexClient = new SearchIndexClient(endpoint, credential);
            SearchClient searchclient = indexClient.GetSearchClient(searchIndex);

            var options = new SearchOptions()
            {
                IncludeTotalCount = true, // If not set the property TotalCount is set as null
                Size = 100, // PageSize
            };

            Azure.Response<SearchResults<Movie>> response;
            // We are only searching by the query parameter, this should be expanded
            if (!string.IsNullOrWhiteSpace(movieQueryParameters.Q))
            {
                response = await searchclient
                    .SearchAsync<Movie>(movieQueryParameters.Q.Trim(), options)
                    .ConfigureAwait(false);
            }
            else
            {
                response = await searchclient
                    .SearchAsync<Movie>("*", options)
                    .ConfigureAwait(false);
            }

            // Currently we are only searching by title, this needs to be changed as other search fields are implemented for searching
            options.SearchFields.Add("title");
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

            return await Task<List<Movie>>.FromResult(movies).ConfigureAwait(false);
        }
    }
}