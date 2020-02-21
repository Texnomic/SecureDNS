using System;
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
            RestClient = new RestClient();

            RetryPolicy = Policy.HandleResult<IRestResponse<List<FilterList>>>(ResultPredicate)
                                .RetryAsync(3);
        }

        private static bool ResultPredicate(IRestResponse Response)
        {
            return Response.ErrorException != null;
        }

        public async Task<List<FilterList>> GetListsAsync()
        {
            var RestRequest = new RestRequest("https://filterlists.com/api/v1/lists/", Method.GET);

            RestClient.UseSerializer<NewtonsoftJsonSerializer>();

            var Response = await RetryPolicy.ExecuteAsync(() => RestClient.ExecuteGetAsync<List<FilterList>>(RestRequest));

            if (Response.ErrorException != null) throw Response.ErrorException;

            return Response.Data;
        }
    }
}
