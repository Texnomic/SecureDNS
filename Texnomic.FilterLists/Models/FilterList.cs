using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Texnomic.FilterLists.Enums;

namespace Texnomic.FilterLists.Models
{
    public class FilterList
    {
        [JsonProperty("id")] 
        public int ID { get; set; }

        [JsonProperty("description")] 
        public string Description { get; set; }

        [JsonProperty("homeUrl")] 
        public string HomeUrl { get; set; }

        [JsonProperty("languageIds")]
        public List<Languages> Languages { get; set; }

        [JsonProperty("licenseId")] 
        public int LicenseId { get; set; }

        [JsonProperty("maintainerIds")]
        public List<Maintainers> Maintainers { get; set; }

        [JsonProperty("name")] 
        public string Name { get; set; }

        [JsonProperty("submissionUrl")] 
        public string SubmissionUrl { get; set; }

        [JsonProperty("syntaxId")]
        public Syntax? Syntax { get; set; }

        [JsonProperty("tagIds")]
        public List<Tags> Tags { get; set; }

        [JsonProperty("viewUrl")] 
        public string ViewUrl { get; set; }

        [JsonProperty("descriptionSourceUrl")] 
        public string DescriptionSourceUrl { get; set; }

        [JsonProperty("issuesUrl")] 
        public string IssuesUrl { get; set; }

        [JsonProperty("forumUrl")] 
        public string ForumUrl { get; set; }

        [JsonProperty("publishedDate")] 
        public DateTime? PublishedDate { get; set; }

        [JsonProperty("viewUrlMirrors")] 
        public List<string> ViewUrlMirrors { get; set; }

        [JsonProperty("donateUrl")] 
        public string DonateUrl { get; set; }

        [JsonProperty("emailAddress")] 
        public string EmailAddress { get; set; }

        [JsonProperty("chatUrl")] 
        public string ChatUrl { get; set; }

        [JsonProperty("policyUrl")] 
        public string PolicyUrl { get; set; }
    }
}
