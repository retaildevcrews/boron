// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace CSE.Boron.Model
{
    public class Role
    {
        public int Order { get; set; }
        [JsonPropertyName("actorId")]
        public string ActorId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        public int? BirthYear { get; set; }
        public int? DeathYear { get; set; }
        public string Category { get; set; }
        public List<string> Characters { get; set; }
    }
}
