using Doppelkopf.BlazorWebAssembly.Client.Enums;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doppelkopf.BlazorWebAssembly.Client.Services
{
    public class StateService
    {
        public event Action OnChange;

        private bool _inGame = false;

        public bool InGame
        {
            get { 
                return _inGame;
            }
            set {
                _inGame = value;
                NotifyStateChanged();
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
