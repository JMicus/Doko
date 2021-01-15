using Doko.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doko.UI.Controls
{
    public abstract class AbstractDrawObject
    {
        public static bool SingleItemMode;

        public Geo Location = new Geo(0, 0);

        public Geo LocationFix = new Geo(0, 0);

        public Image PaintedObject;

        public void SetFixedLocation(Geo geo)
        {
            LocationFix = geo;
            Location = geo;
        }

        public void SetFixedLocationX(int x)
        {
            LocationFix.X = x;
            Location.X = x;
        }

        public void SetFixedLocationY(int y)
        {
            LocationFix.Y = y;
            Location.Y = y;
        }

        //public Geo ShiftLocation = new Geo(0, 0);

        public Geo Size = new Geo(0, 0);

        public Color? FrameColor = null;

        public bool Movable = false;
        public bool Visible = true;
        public bool IsSingleDrawModeObject = false;

        public Matrix Transform;
        public Matrix NoDragTransform;

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(Size.Half.Invert.Point, Size.Size);
            }
        }

        public float Rotation = 0;

        internal abstract void Draw(Graphics g);

        public virtual void NewPosition()
        {
            // to be overridden
        }

        internal virtual List<AbstractDrawObject> GetChilds()
        {
            return new List<AbstractDrawObject>();
        }

        internal List<AbstractDrawObject> GetChildsRec()
        {
            var childs = new List<AbstractDrawObject>();

            childs.Add(this);
            
            foreach (var c in GetChilds())
            {
                childs.AddRange(c.GetChildsRec());
            }

            return childs;
        }

        public abstract override string ToString();

        /*public void Paint(Graphics gBack, Graphics gObj, Graphics gFore, AbstractDrawObject obj)
        {
            Paint(null, gBack, gBack, gFore, obj);
        }*/


        public void DrawTransformed(Graphics g)
        {
            var tTemp = g.Transform;
            g.Transform = Transform;
            Draw(g);
            g.Transform = tTemp;
        }

        public void Paint(Graphics g)
        {
            
            if (Visible == false)
            {
                return;
            }

            var t = g.Transform;


            var newImage = PaintedObject == null;

            //if (Transform != null)
            g.TranslateTransform(Location.X, Location.Y);
            g.RotateTransform(Rotation);
            
            if (newImage)
            {
                
                
                
                //g.TranslateTransform(ShiftLocation.X, ShiftLocation.Y);


                //Draw(g, gBack, gObj, gFore, obj);

                PaintedObject = new Bitmap(700, 700);
                var gObject = Graphics.FromImage(PaintedObject);
                gObject.Transform = g.Transform;

                //dragObject.Visible = false;

                Draw(gObject);

                
                /*foreach (var c in GetChilds())
                {
                    c.Paint(gObject, gBack, gObj, gFore, obj);
                }*/

                // TODO debugging
                if (FrameColor.HasValue)
                {
                    gObject.DrawRectangle(new Pen(new SolidBrush(FrameColor.Value), 10f), Bounds);
                }

                PaintedObject.Save($@"C:\Users\acer\Documents\ProgrammeCode\Doppelkopf\out\Object{this}.png", ImageFormat.Png);

                Transform = g.Transform;
                
            }

            g.Transform = new Matrix();
            g.DrawImage(PaintedObject, 0, 0);


            g.Transform = Transform;
            foreach (var c in GetChilds())
            {
                c.Paint(g);
            }

            g.Transform = t;
        }

        public AbstractDrawObject GetObject(Point location)
        {
            if (Transform == null)
            {
                return null;
            }

            var childs = GetChilds();
            for (int i = childs.Count - 1; i >= 0; i--)
            {
                var hit = childs[i].GetObject(location);
                if (hit != null)
                {
                    return hit;
                }
            }

            var points = new[] { location };
            var tInverted = Transform.Clone();
            tInverted.Invert();
            tInverted.TransformPoints(points);

            if (Bounds.Contains(points[0]))
            {
                return this;
            }

            return null;
        }



    }
}
