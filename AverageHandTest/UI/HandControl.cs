using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Remoting.Channels;
using AverageHandTest.Properties;
using Doppelkopf.Core.App;

namespace AverageHandTest.UI
{
    public partial class HandControl : UserControl
    {


        private List<Card> _cards = new List<Card>();

        public List<Card> Cards
        {
            get
            {
                return _cards;
            }
            set
            {
                _cards.Clear();
                foreach (var card in value)
                {
                    _cards.Add(new Card(card.Name));
                }
                //this.Refresh();
            }
        }

        public HandControl()
        {
            InitializeComponent();

            this.Paint += HandControl_Paint;
        }

        private void HandControl_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            var width = 45;
            var height = width * 128 / 95;

            this.Width = width * _cards.Count + 20;
            this.Height = height + 20;

            for (int i = 0; i < _cards.Count; i++)
            {
                g.DrawImage((Image)Resources.ResourceManager.GetObject(_cards[i].NameCode), i * width, 0, width, height);
            }
        }


    }
}
