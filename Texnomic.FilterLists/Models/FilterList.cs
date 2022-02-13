namespace Texnomic.FilterLists.Models;

public class FilterList
{
    [JsonPropertyName("id")] 
    public int ID { get; set; }

    [JsonPropertyName("description")] 
    public string Description { get; set; }

    [JsonPropertyName("homeUrl")] 
    public string HomeUrl { get; set; }

    [JsonPropertyName("languageIds")]
    public List<Languages> Languages { get; set; }

    [JsonPropertyName("licenseId")] 
    public int LicenseId { get; set; }

    [JsonPropertyName("maintainerIds")]
    public List<Maintainers> Maintainers { get; set; }

    [JsonPropertyName("name")] 
    public string Name { get; set; }

    [JsonPropertyName("submissionUrl")] 
    public string SubmissionUrl { get; set; }

    [JsonPropertyName("syntaxId")]
    public Syntax? Syntax { get; set; }

    [JsonPropertyName("tagIds")]
    public List<Tags> Tags { get; set; }

    [JsonPropertyName("viewUrl")] 
    public string ViewUrl { get; set; }

    [JsonPropertyName("descriptionSourceUrl")] 
    public string DescriptionSourceUrl { get; set; }

    [JsonPropertyName("issuesUrl")] 
    public string IssuesUrl { get; set; }

    [JsonPropertyName("forumUrl")] 
    public string ForumUrl { get; set; }

    [JsonPropertyName("publishedDate")] 
    public DateTime? PublishedDate { get; set; }

    [JsonPropertyName("viewUrlMirrors")] 
    public List<string> ViewUrlMirrors { get; set; }

    [JsonPropertyName("donateUrl")] 
    public string DonateUrl { get; set; }

    [JsonPropertyName("emailAddress")] 
    public string EmailAddress { get; set; }

    [JsonPropertyName("chatUrl")] 
    public string ChatUrl { get; set; }

    [JsonPropertyName("policyUrl")] 
    public string PolicyUrl { get; set; }
}