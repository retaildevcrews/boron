// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using Azure.Search.Documents.Indexes;

namespace CSE.Boron.Model
{
    public class Movie
    {
        public string Id { get; set; }
        public string PartitionKey { get; set; }
        [SimpleField(IsKey = true, IsFilterable = true, IsSortable = true)]
        public string MovieId { get; set; }
        [SimpleField(IsFilterable = true, IsSortable = true)]
        public string Type { get; set; }
        [SimpleField(IsFilterable = true, IsSortable = true)]
        public string Title { get; set; }
        [SimpleField(IsFilterable = true, IsSortable = true)]
        public int Year { get; set; }
        public int Runtime { get; set; }
        public double Rating { get; set; }
        public long Votes { get; set; }
        public long TotalScore { get; set; }
        [SimpleField(IsFilterable = true, IsSortable = true)]
        public string TextSearch { get; set; }
        [SimpleField(IsFilterable = true, IsSortable = true)]
        public List<string> Genres { get; set; }
        public List<Role> Roles { get; set; }

        /// <summary>
        /// Compute the partition key based on the movieId or actorId
        ///
        /// For this sample, the partitionkey is the id mod 10
        ///
        /// In a full implementation, you would update the logic to determine the partition key
        /// </summary>
        /// <param name="id">document id</param>
        /// <returns>the partition key</returns>
        public static string ComputePartitionKey(string id)
        {
            // validate id
            if (!string.IsNullOrEmpty(id) &&
                id.Length > 5 &&
                id.StartsWith("tt", StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(id.Substring(2), out int idInt))
            {
                return (idInt % 10).ToString(CultureInfo.InvariantCulture);
            }

            throw new ArgumentException("Invalid Partition Key");
        }
    }
}
