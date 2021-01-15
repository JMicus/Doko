using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Doko.UI.Controls
{
    internal class HandCardButton : Button, IRepaitControl
    {
        private string card;

        private Point handLocation;

        private Point mouseStartLocation;

        private bool dragging;

        internal event Action<string> PutCard;

        internal HandCardButton(Point location, string card)
        {
            this.card = card;

            handLocation = location;
            Location = handLocation;

            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            ImageAlign = ContentAlignment.TopLeft;
            Margin = new Padding(0);
            Padding = new Padding(0);
            BackgroundImage = UI.Card.ImageFromName(card);
            ForeColor = Color.Transparent;
            BackColor = Color.Transparent;
            BackgroundImageLayout = ImageLayout.Zoom;
            TextImageRelation = TextImageRelation.ImageAboveText;
            TextAlign = ContentAlignment.BottomCenter;

            /*MouseClick += (object o, MouseEventArgs ea) =>
            {
                PutCard((o as Button).Text);
            };*/

            this.MouseDown += HandCardButton_MouseDown;
            this.MouseMove += HandCardButton_MouseMove;
            this.MouseUp += HandCardButton_MouseUp;

            this.Paint += HandCardButton_Paint;
        }

        private void HandCardButton_Paint(object sender, PaintEventArgs e)
        {
            /*var controls = this.Parent.Controls.OfType;
            foreach (Control c in controls)
            {
                c.re
                g.DrawImage(c.Image, c.Location.X, c.Location.Y, c.Width, c.Height);
            }*/
        }

        private void HandCardButton_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dragging = false;

                if (Location.Y - handLocation.Y < -200)
                {
                    PutCard(card);
                    this.Parent.Controls.Remove(this);
                };
                Location = handLocation;
            }
        }

        private void HandCardButton_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && dragging)
            {
                Location = Point.Add(handLocation, new Size(Point.Subtract(mouseAbs(e), new Size(mouseStartLocation))));
            }
        }

        private void HandCardButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dragging = true;
                mouseStartLocation = mouseAbs(e);
            }
        }

        private Point mouseAbs(MouseEventArgs e)
        {
            return Point.Add(this.Location, new Size(e.Location));
        }

        public void PaintTo(Graphics g)
        {
            throw new NotImplementedException();
        }
    }
}
