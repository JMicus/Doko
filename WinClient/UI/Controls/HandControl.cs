using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Doko.Properties;
using System.Windows.Threading;
using Doko.Helper;

namespace Doko.UI.Controls
{
    public partial class HandControl : AbstractDrawObject
    {
        public Action<string> PutCard;

        //public string Cards;


        private List<Card> cards = new List<Card>();

        //private Control parent;

        /*public HandControl(Control parent)
        {
            //this.parent = parent;

            parent.Paint += HandControl_Paint;
        }*/

        public HandControl()
        {
            Size = new Geo(500, 200);
            #if DEBUG
                FrameColor = Color.Blue;
            #endif
        }

        internal override List<AbstractDrawObject> GetChilds()
        {
            return cards.Select(x => (AbstractDrawObject)x).ToList();
        }

        public void Set(string cardsCode)
        {
            cards.Clear();
            var cardsArray = cardsCode.Split('.');

            for (int i = 0; i < cardsArray.Length; i++)
            {
                var card = new Card(cardsArray[i]);
                card.PositionChanged += CardPositionChanged;
                card.SetFixedLocationY(0);
                card.Movable = true;
                cards.Add(card);
            }

            Arrange();


        }

        private void CardPositionChanged(Card card, Geo shift)
        {
            if (shift.Y < -100)
            {
                cards.Remove(card);
                PutCard(card.CardCode);
            }
        }

        public void Arrange()
        {
            if (cards.Count == 0)
            {
                return;
            }

            int width = cards[0].Size.X;

            int step = Math.Min(width, (this.Size.X - width) / (Math.Max(cards.Count - 1, 1)));

            int start = Size.Half.Invert.X + (this.Size.X - (step * (cards.Count - 1))) / 2;

            for (int i = 0; i < cards.Count; i++)
            {
                float fromCenter = (i - cards.Count / 2);

                cards[i].SetFixedLocationX(start + i * step);
                //cards[i].SetFixedLocationY(Math.Abs((int)(fromCenter * 5)));

                //cards[i].Rotation = fromCenter * 3;

                
            }
        }

        internal override void Draw(Graphics g)
        {
            // draw nothing
        }

        public override string ToString()
        {
            return "HandControl";
        }
    }
}
