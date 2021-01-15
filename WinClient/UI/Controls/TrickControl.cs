using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Doko.Helper;

namespace Doko.UI.Controls
{
    public partial class TrickControl : AbstractDrawObject
    {
        private Card[] Cards = new Card[4];

        private int StartPlayerNo = 1;

        public TrickControl()
        {
            Size = new Geo(300, 300);

            #if DEBUG
                FrameColor = Color.Blue;
            #endif
        }

        public void Set(int startPlayerNo, string cards)
        {
            var cardArray = cards.Split('.');
            int xShift = 1;
            int yShift = 1;

            for (int i = 0; i < 4; i++)
            {
                Cards[i] = new Card(cardArray[i]);
                Cards[i].Rotation = 90 * i;

                Cards[i].Location = new Geo(20 * xShift, 50 * yShift);

                var xShiftTemp = xShift;
                xShift = -yShift;
                yShift = xShiftTemp;
                //Cards[i].ShiftLocation = new Geo(0, 0);
            }
            
            StartPlayerNo = startPlayerNo;

            /*parent.Invoke(new Action(() =>
            {
                TrickControl_Paint(this, new PaintEventArgs(this.CreateGraphics(), this.Bounds));
            }));*/
        }

        internal override List<AbstractDrawObject> GetChilds()
        {
            var childs = new List<AbstractDrawObject>();

            for (int i = 0; i < 4; i++)
            {
                var card = Cards[(i + StartPlayerNo - 1) % 4];
                if (card != null)
                {
                    childs.Add(card);
                }
            }

            return childs;
        }

        public override string ToString()
        {
            return "Trick";
        }

        internal override void Draw(Graphics g)
        {
            // do nothing
        }

    }
}
