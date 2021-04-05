using Doppelkopf.BlazorWebAssembly.Client.Enums;
using Doppelkopf.Core.App.Helper;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doppelkopf.BlazorWebAssembly.Client.Services
{
    public class MenuService
    {
        public event Func<EMenuAction, bool> OnClick;

        public Watch<EMenuAction> OpenTab = new Watch<EMenuAction>(EMenuAction.PageTable);

        public void Click(EMenuAction click, bool tabChanged = false)
        {
            if (tabChanged)
            {
                Console.WriteLine("Open Tab: " + click);
                OpenTab.Value = click;
            }

            OnClick?.Invoke(click);
        }

         
    }
}
