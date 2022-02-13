namespace Texnomic.FilterLists;

public class FilterListsClient
{
    private readonly HttpClient HttpClient;

    public FilterListsClient()
    {
        var Handler = new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip
        };

        HttpClient = new HttpClient(Handler)
        {
            BaseAddress = new Uri("https://filterlists.com/api/directory/lists")
        };
    }

    public async ValueTask<List<FilterList>> GetListsAsync()
        => await HttpClient.GetFromJsonAsync<List<FilterList>>("");
}