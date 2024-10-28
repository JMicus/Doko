using Doppelkopf.Core.App;
using Doppelkopf.Core.App.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DokoTelegramService.Tools
{
    internal static class CardVisualizer
    {
        public const string CROSS = "♣️";
        public const string SPADES = "♠️";
        public const string HEARTHS = "♥️";
        public const string DIAMOND = "♦️";

        public const string PIG = "🐷";
        public const string ACE = "🅰";
        public const string KING = "\U0001f934";
        public const string QUEEN = "👸";
        public const string JACK = "\U0001f935‍♂";
        public const string TEN = "🔟";
        public const string NINE = "9⃣";

        public const string E = " ";
        public const string EMPTY = "⬜️";

        public const string FOX = "\U0001f98a";

        private static readonly Dictionary<char, string> _iconOfCode = new Dictionary<char, string>()
        {
            {'S', PIG },
            {'A', ACE },
            {'K', KING },
            {'D', QUEEN },
            {'B', JACK },
            {'1', TEN },
            {'9', NINE }
        };

        private static readonly Dictionary<char, string> _iconOfCode2 = new Dictionary<char, string>()
        {
            {'S', PIG },
            {'A', "A" },
            {'K', "K" },
            {'D', "D" },
            {'B', "B" },
            {'1', "10" },
            {'9', "9" }
        };

        private static readonly Dictionary<char, string> _iconOfColorCode = new Dictionary<char, string>()
        {
            {'K', CROSS },
            {'P', SPADES },
            {'H', HEARTHS },
            {'C', DIAMOND },
            {'S', DIAMOND }
        };

        public static string CardMessage(this Card? card)
        {
            if (card == null)
            {
                return EMPTY + "\r\n" + EMPTY;
            }

            var code = card.NameCode.ToUpper();

            var r0 = "";
            var r1 = "";

            if (_iconOfColorCode.TryGetValue(code[0], out var colorIcon)
                && _iconOfCode2.TryGetValue(code[1], out var icon))
            {
                r0 = colorIcon;
                r1 = icon;
            }

            if (card.Name == ECard.SA)
            {
                r0 = DIAMOND;
                r1 = PIG;
            }

            return r0 + "\r\n" + r1;
        }
    }
}