using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doppelkopf.BlazorWebAssembly.Client.Services
{
    public class MenuService
    {
        public enum MenuClick
        {
            Debug,
            Deal,
            SpecialGame,
        }

        public class MenuEntry
        {
            public string Label { get; set; }
            public string Text { get; set; }
        }

        public event Action OnChange;

        public event Action<MenuClick> OnClick;

        private List<MenuEntry> _entries = new List<MenuEntry>();

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

        public void Click(MenuClick click)
        {
            OnClick?.Invoke(click);
        }


        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
