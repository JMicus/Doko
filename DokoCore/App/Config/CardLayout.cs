using System;
using System.Collections.Generic;
using System.Text;

namespace Doppelkopf.Core.App.Config
{
    public class CardLayout
    {
        public Setting<string> Layout { get; set; } = new Setting<string>("Basic");
        public Setting<string> ImageType { get; set; } = new Setting<string>("png");
        public Setting<int> Height { get; set; } = new Setting<int>(128);
        public Setting<int> Width { get; set; } = new Setting<int>(95);
        public Setting<bool> Border { get; set; } = new Setting<bool>(false);

        public void ChangeLayoutToHP()
        {
            Layout.Value = "HP";
            ImageType.Value = "gif";
            Height.Value = 240;
            Width.Value = (int)(Height.Value * 0.605);
            Border.Value = true;
        }
    }
}
