using Microsoft.AspNetCore.Components;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doppelkopf.BlazorWebAssembly.Client.Shared
{
    public partial class Player : ComponentBase
    {
        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public string Test { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            
        }

    }
}
