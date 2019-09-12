using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;
using Texnomic.FilterLists.Models;

namespace Texnomic.FilterLists
{
    public class FilterListsClient
    {
        private readonly RestClient RestClient;
        private readonly RestRequest RestRequest;

        public FilterListsClient()
        {
            RestClient = new RestClient($"https://filterlists.com/api/v1/lists/");

            RestRequest = new RestRequest();

            RestClient.UseSerializer<NewtonsoftJsonSerializer>();
        }

        public async Task<List<FilterList>> GetLists()
        {
            var Response = await RestClient.ExecuteGetTaskAsync<List<FilterList>>(RestRequest);

            return Response.Data;
        }
    }
}
