// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Threading.Tasks;
using Boron.Extensions;
using CSE.Boron.DataAccessLayer;
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
        public async Task<IActionResult> GetMoviesAsync([FromQuery] MovieQueryParameters movieQueryParameters)
        {
            if (movieQueryParameters == null)
            {
                throw new ArgumentNullException(nameof(movieQueryParameters));
            }

            // return await ResultHandler.Handle(
            //     Task<List<Movie>>.FromResult(movies), movieQueryParameters.GetMethodText(HttpContext), Constants.MoviesControllerException, logger)
            //     .ConfigureAwait(false);
            return await ResultHandler.Handle(
                dal.GetMovies2Async(movieQueryParameters), movieQueryParameters.GetMethodText(HttpContext), Constants.MoviesControllerException, logger)
                .ConfigureAwait(false);
        }
    }
}
