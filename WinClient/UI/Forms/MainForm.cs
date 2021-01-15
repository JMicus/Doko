using Doko.Helper;
using Doko.Properties;
using Doko.UI;
using Doko.UI.Controls;
using Doko.UI.Forms;
using Doppelkopf.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

// This is the code for your desktop app.
// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

namespace Doko
{
    public partial class MainForm : Form
    {
        //private Game game;

        private DokoClient client;
        private HandControl handControl;
        private TrickControl trickControl;

        

        private static int _gameNo = -1;
        private static int gameNo
        {
            get
            {
                if (_gameNo < 0)
                {
                    _gameNo = new Random().Next(1000);
                }
                return _gameNo;
            }
        }

        public MainForm(int playerNo = -1)
        {
            InitializeComponent();

            this.BackgroundImage = Resources.green;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            trickControl = new TrickControl();
            trickControl.Location = new Geo(0, -150);



            handControl = new HandControl();

            var card = new Card("kd");
            handControl.Location = new Geo(0, 200);
            card.Rotation = 90f;
            card.Movable = true;


            drawPanel.AddObject(trickControl);
            drawPanel.AddObject(handControl);
            //drawPanel.AddObject(card);


            this.ResizeBegin += ((object o, EventArgs e) => this.SuspendLayout());
            this.ResizeEnd += ((object o, EventArgs e) => this.ResumeLayout());

            //Telegram = new Helper.Telegram();

            // DEBUG //////////////////
            if (true && playerNo < 0)
            {
                playerNo = 1;

                if (true)
                {
                    for (int i = 2; i <= 4; i++)
                    {
                        new MainForm(i).Show();
                    }
                }
            }
            ///////////////////////


            //game = new Game(this);

            var playerName = "";
            var gameName = "";

            if (playerNo < 0)
            {
                var login = new LoginForm();
                gameName = login.GameNameTextBox.Text;
                playerNo = Convert.ToInt32(login.playerNoComboBox.Text);
                playerName = login.playerNameTextBox.Text;
            }
            else
            {
                gameName = "doko";// + gameNo;
                playerName = "Spieler " + playerNo;
                
            }

            this.Text = $"{playerName} ({playerNo})";

            client = new DokoClient(gameName, playerNo);


            // hand control
            client.GetHand += (string msg) =>
            {
                handControl.Set(msg);
                drawPanel.Repaint();
            };

            handControl.PutCard += (string cardCode) =>
            {
                client.PutCard(cardCode);
                drawPanel.Repaint();
            };


            // trick control
            client.GetTrick += (int startPlayerNo, string cardCode) =>
            {
                trickControl.Set(startPlayerNo, cardCode);
                drawPanel.Repaint();
            }; 
            
            trickControl.Rotation = 90f * (1 - playerNo);







            client.Start();
            client.SayHello(playerName);


            //handControl.Set("h1.kd.kd.pd.kb.cb.ck.ka.pa.ha");
            //trickControl.Set(1, "ka.k1.kk.ca");


            this.ResizeEnd += MainForm_ResizeEnd;
        }

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            //drawPanel
        }

        /*private void MainForm_Shown(object sender, EventArgs e)
        {
            this.Location = Screen.AllScreens[Telegram.PlayerNo % Screen.AllScreens.Count()].Bounds.Location;
        }*/

        private void dealButton_Click(object sender, EventArgs e)
        {
            client.Deal();
            //drawPanel.Test();
        }
    }
}
