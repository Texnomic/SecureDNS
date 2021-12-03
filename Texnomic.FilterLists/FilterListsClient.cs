using System.Collections.Generic;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;
using RestSharp;
using Texnomic.FilterLists.Models;

namespace Texnomic.FilterLists
{
    public class FilterListsClient
    {
        private readonly RestClient RestClient;
        private readonly AsyncRetryPolicy<IRestResponse<List<FilterList>>> RetryPolicy;

        public FilterListsClient()
        {
            RestClient = new RestClient
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36",
                FollowRedirects = true
            };

            RetryPolicy = Policy.HandleResult<IRestResponse<List<FilterList>>>(ResultPredicate)
                                .RetryAsync(3);
        }

        private static bool ResultPredicate(IRestResponse Response)
        {
            return Response.IsSuccessful;
        }

        public async Task<List<FilterList>> GetListsAsync()
        {
            var RestRequest = new RestRequest("https://filterlists.com/api/directory/lists", Method.GET);

            RestClient.UseSerializer<NewtonsoftJsonSerializer>();

            var Response = await RetryPolicy.ExecuteAsync(() => RestClient.ExecuteGetAsync<List<FilterList>>(RestRequest));

            if (Response.ErrorException != null) throw Response.ErrorException;

            return Response.Data;
        }
    }
}
