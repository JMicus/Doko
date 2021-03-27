using Doppelkopf.BlazorWebAssembly.Client.Enums;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doppelkopf.BlazorWebAssembly.Client.Services
{
    public class MenuService
    {
        public event Action<EMenuAction> OnClick;

        public void Click(EMenuAction click)
        {
            OnClick?.Invoke(click);
        }

         
    }
}
