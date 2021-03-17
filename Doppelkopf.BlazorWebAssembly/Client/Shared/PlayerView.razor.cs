using Microsoft.AspNetCore.Components;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using C = Doppelkopf.Core.App;

namespace Doppelkopf.BlazorWebAssembly.Client.Shared
{
    public partial class PlayerView : ComponentBase
    {
        [Parameter]
        public C.Player Player { get; set; }

        [Parameter]
        public bool MsgDirUp { get; set; } = false;

        [Parameter]
        public string Width { get; set; } = null;

        private PlayerMessageView _msgView;

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            //Console.WriteLine("PlayerView - player: " + Player.NameLabel);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            Console.WriteLine("PlayerView render " + (firstRender ? "(first)" : ""));
        }

        public void Refresh()
        {
            StateHasChanged();
        }

        public void RefreshMsg(List<string> messages)
        {
            _msgView.Refresh(messages);
        }
    }
}
