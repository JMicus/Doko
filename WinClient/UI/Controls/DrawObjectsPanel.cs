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
using Doko.Properties;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Doko.UI.Controls
{
    public partial class DrawObjectsPanel : UserControl
    {
        private AllContainerDrawObject Objects;

        private Geo dragStartLocation;
        private AbstractDrawObject dragObject;
        private Matrix dragTransform;
        
        private Image dragBackground;
        private Image dragObjectImage;
        private Image dragForeground;

        public DrawObjectsPanel()
        {
            InitializeComponent();

            

            Objects = new AllContainerDrawObject();
            DrawObjectsPanel_Resize(null, null);

            this.Paint += DrawObjectsPanel_Paint;

            this.MouseClick += DrawObjectsPanel_MouseClick;

            this.MouseDown += DrawObjectsPanel_MouseDown;
            this.MouseMove += DrawObjectsPanel_MouseMove;
            this.MouseUp += DrawObjectsPanel_MouseUp;

            this.SizeChanged += DrawObjectsPanel_Resize;
        }

        private void DrawObjectsPanel_Resize(object sender, EventArgs e)
        {
            Objects.Size = new Geo(this.Size);
            Objects.Location = new Geo(this.Size).Half;
        }

        public void AddObject(AbstractDrawObject obj)
        {
            Objects.Objects.Add(obj);

        }

        public void Test()
        {
            Console.WriteLine("//////////////////");
            foreach (var o in Objects.GetChildsRec())
            {
                Console.WriteLine(o);
                o.Paint(CreateGraphics());
            }

        }

        private void DrawObjectsPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (dragObject != null)
                {
                    dragObject.NewPosition();

                    //dragObject.Transform = dragObject.NoDragTransform;

                    //dragDraw();

                    //dragObject.IsSingleDrawModeObject = false;

                    //AbstractDrawObject.SingleItemMode = false;


                    dragObject = null;
                    //repaint();
                }
            }
        }

        private void DrawObjectsPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (dragObject != null)
                {
                    

                    var shift = new Geo(e.Location).Sub(dragStartLocation);

                    dragObject.Transform = dragObject.NoDragTransform.Clone();
                    dragObject.Transform.Translate(shift.X, shift.Y);
                    //dragObject.Location = dragObject.LocationFix.Sub(dragStartLocation).Add(e.Location);

                    dragDraw();

                }
            }
        }

        private void dragDraw()
        {
            this.Paint -= DrawObjectsPanel_Paint;
            this.SuspendLayout();

            var iTemp = new Bitmap(900, 900);
            var gTemp = Graphics.FromImage(iTemp);
            gTemp.Clear(Color.Green);

            gTemp.DrawImage(dragBackground, 0, 0, Width, Height);

            //g.Transform.Translate(shift.X, shift.Y);
            dragObject.DrawTransformed(gTemp);

            gTemp.Transform = new Matrix();
            gTemp.DrawImage(dragForeground, 0, 0, Width, Height);

            var g = this.CreateGraphics();
            //g.Clear(Color.Transparent);
            g.DrawImage(iTemp, 0, 0);

            this.ResumeLayout();
            this.Paint += DrawObjectsPanel_Paint;
        }

        private void DrawObjectsPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var hit = Objects.GetObject(e.Location);
                if (hit != null && hit.Movable && hit.Visible)
                {
                    dragObject = hit;
                    dragStartLocation = new Geo(e.Location);

                    dragObject.NoDragTransform = dragObject.Transform.Clone();


                    dragBackground = new Bitmap(Width, Height);
                    var gBack = Graphics.FromImage(dragBackground);

                    dragObjectImage = new Bitmap(Width, Height);
                    var gObj = Graphics.FromImage(dragObjectImage);

                    dragForeground = new Bitmap(Width, Height);
                    var gFore = Graphics.FromImage(dragForeground);

                    var g = gBack;
                    foreach (var obj in Objects.GetChildsRec())
                    {
                        Console.WriteLine(obj);
                        //obj.PaintedObject = null;
                        if (obj != hit)
                        {
                            g.DrawImage(obj.PaintedObject, 0, 0);
                        }
                        else
                        {
                            gObj.DrawImage(obj.PaintedObject, 0, 0);
                            g = gFore;
                        }
                    }


                    //dragBackground.Save(@"C:\Users\acer\Documents\ProgrammeCode\Doppelkopf\out\dragBack.png", ImageFormat.Png);
                    //dragObjectImage.Save(@"C:\Users\acer\Documents\ProgrammeCode\Doppelkopf\out\dragObj.png", ImageFormat.Png);
                    //dragForeground.Save(@"C:\Users\acer\Documents\ProgrammeCode\Doppelkopf\out\dragFore.png", ImageFormat.Png);
                    //dragObject.Visible = true;

                    //dragObject.IsSingleDrawModeObject = true;

                    //AbstractDrawObject.SingleItemMode = true;
                }
            }
        }

        private void DrawObjectsPanel_MouseClick(object sender, MouseEventArgs e)
        {
            Console.WriteLine(Objects.GetObject(e.Location));
        }

        public void Repaint()
        {
            DrawObjectsPanel_Paint(this, null);
        }

        public void DrawObjectsPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = this.CreateGraphics();

            g.DrawImage(Resources.green, Bounds);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            drawObjects(g);
        }

        private void drawObjects(Graphics g)
        {
            Objects.Paint(g);
        }
    }
}
