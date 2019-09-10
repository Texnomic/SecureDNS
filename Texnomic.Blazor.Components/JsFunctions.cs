using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Texnomic.Blazor.Components
{
    public class JsFunctions
    {
        public static ValueTask<bool> ToDataTable(IJSRuntime JsRuntime, string ID)
        {
            return JsRuntime.InvokeAsync<bool>("JsFunctions.ToDataTable", ID);
        }
    }
}
