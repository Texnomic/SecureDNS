using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;
using Texnomic.FilterLists.Models;

namespace Texnomic.FilterLists
{
    public static class FilterListsClient
    {
        private static readonly RestClient RestClient = new RestClient($"https://filterlists.com/api/v1/lists/");
        
        private static readonly RestRequest RestRequest = new RestRequest();

        public static async Task<List<FilterList>> GetLists()
        {
            RestClient.UseSerializer<NewtonsoftJsonSerializer>();

            var Response = await RestClient.ExecuteGetTaskAsync<List<FilterList>>(RestRequest);

            return Response.Data;
        }
    }
}
