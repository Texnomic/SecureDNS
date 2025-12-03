namespace Texnomic.SecureDNS.Middlewares;

public class FilterListsMiddleware : IAsyncMiddleware<IMessage, IMessage>
{
    private readonly ILogger Logger;
    private readonly HttpClient HttpClient = new();
    private readonly FastHashSet<string> Filter = [];

    public FilterListsMiddleware(IOptionsMonitor<FilterListsMiddlewareOptions> Options, ILogger Logger) : base()
    {
        this.Logger = Logger;

        Options.OnChange(OptionsOnChange);

        _ = InitializeAsync(Options.CurrentValue.IDs);
    }

    private void OptionsOnChange(FilterListsMiddlewareOptions Options)
    {
        Filter.Clear();

        Filter.TrimExcess();

        _ = InitializeAsync(Options.IDs);

        Logger.Information("FilterLists {@IDs} Updates Applied.", Options.IDs);
    }

    public async Task<IMessage> Run(IMessage Message, Func<IMessage, Task<IMessage>> Next)
    {
        if (Filter.Contains(Message.Questions.First().Domain.Name))
        {
            Logger.Warning("Filtered Query {@ID} To {@Domain}.", Message.ID, Message.Questions.First().Domain.Name);

            return new Message()
            {
                ID = Message.ID,
                MessageType = MessageType.Response,
                ResponseCode = ResponseCode.Refused,
                Questions = Message.Questions
            };
        }
        else
        {
            return await Next(Message);
        }
    }

    private async Task InitializeAsync(int[] IDs)
    {
        try
        {
            Logger.Verbose("FilterLists Initialization Started.");

            var Lists = await GetFilterListsAsync(IDs);

            Logger.Information("FilterLists Initialization Started with {Count} Selected Lists.", $"{Lists.Count:N0}");

            foreach (var List in Lists)
            {
                try
                {
                    var File = await HttpClient.GetStringAsync(List.ViewUrl);

                    var Domains = Parse(File);

                    foreach (var Domain in Domains)
                    {
                        Filter.Add(Domain);
                    }

                    Logger.Verbose("{@FilterList} Download Completed.", List);
                }
                catch (Exception Error)
                {
                    Logger.Error("{@Error} While Downloading {@FilterList}.", Error, List);
                }
            }

            Filter.TrimExcess();

            Logger.Information("FilterLists Initialization Completed with {@Count} Domains.", $"{Filter.Count:n0}");
        }
        catch (Exception Error)
        {
            Logger.Fatal("Fatal {@Error} Occurred While Initializing FilterLists.", Error);
        }
    }

    private static async Task<List<FilterList>> GetFilterListsAsync(int[] IDs)
    {
        var Client = new FilterListsClient();

        var Lists = await Client.GetListsAsync();

        Lists = Lists.Where(List => List.Syntax is Syntax.Hosts or Syntax.Domains)
            .Where(List => IDs.Contains(List.ID))           
            .ToList();

        return Lists;
    }

    private static string[] Parse(string File)
    {
        return File.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(Line => Line.Replace("0.0.0.0", ""))
            .Select(Line => Line.Replace("127.0.0.1", ""))
            .Select(Line => Line.Trim())
            .SkipWhile(Line => Line.StartsWith("#"))
            .SkipWhile(string.IsNullOrEmpty)
            .SkipWhile(string.IsNullOrWhiteSpace)
            .ToArray();
    }
}