using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace Texnomic.Blazor.Components
{
    public class DataTableBase : ComponentBase
    {
        [Parameter] public IEnumerable<dynamic> Records { get; set; }

        [Parameter] public bool Pageable { get; set; } = false;

        [Parameter] public bool Sortable { get; set; } = false;

        [Parameter] public bool Filterable { get; set; } = false;

        public PropertyInfo[] TypeProperties => Records.GetType().GenericTypeArguments[0].GetProperties();
    }
}
