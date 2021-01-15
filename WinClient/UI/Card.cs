using Doko.Helper;
using Doko.Properties;
using Doko.UI;
using Doko.UI.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doko.UI
{
    public class Card : AbstractDrawObject
    {
        public event Action<Card, Geo> PositionChanged;


        public static Geo DefaultSize = new Geo(150, 204);
        public static Image ImageFromName(string name)
        {
            try
            {
                return (Image)Resources.ResourceManager.GetObject(name.Substring(0, 2).ToLower());
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        private Image image;

        private string _cardCode = "";

        public string CardCode
        {
            get
            {
                return _cardCode;
            }
            set
            {
                _cardCode = value;
                image = ImageFromName(_cardCode);
            }
        }

        public Card(string code = "")
        {
            Size = DefaultSize;
            CardCode = code;
        }

        internal override void Draw(Graphics g)
        {
            if (image != null)
            {
                g.DrawImage(image, Bounds);
            }
        }

        public override string ToString()
        {
            return "Card " + CardCode;
        }

        public override void NewPosition()
        {
            var pointsA = new[] { new Point(0, 0) };
            var pointsB = new[] { new Point(0, 0) };

            Transform.TransformPoints(pointsA);
            NoDragTransform.TransformPoints(pointsB);

            var diff = new Geo(pointsA[0]).Sub(pointsB[0]);

            Console.WriteLine(diff);

            PositionChanged(this, diff);
        }
    }
}
