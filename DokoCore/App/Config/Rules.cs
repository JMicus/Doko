using System;
using System.Collections.Generic;
using System.Text;

namespace Doppelkopf.Core.App.Config
{
    public class Rules
    {
        public Setting<int> Pigs { get; set; } = new Setting<int>(1, true, "Schweine", new List<(int Value, string DisplayValue)>
                                                                                        {
                                                                                            (0, "Ohne"),
                                                                                            (1, "Ein Schwein, ein Fuchs"),
                                                                                            (2, "Zwei Schweine")
                                                                                        });

        public Setting<bool> Nines { get; set; } = new Setting<bool>(false, true, "Neunen", new List<(bool Value, string DisplayValue)>
                                                                                        {
                                                                                            (true, "Mit"),
                                                                                            (false, "Ohne")
                                                                                        });
    }
}
