using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Texnomic.Blazor.Components
{
    public class JsFunctions
    {
        public static Task<bool> ToDataTable(IJSRuntime JsRuntime, string ID)
        {
            return JsRuntime.InvokeAsync<bool>("JsFunctions.ToDataTable", ID);
        }
    }
}
