using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doko.Helper
{
    public class Geo
    {
        public int X;
        public int Y;

        public Size Size
        {
            get
            {
                return new Size(X, Y);
            }
            set
            {
                X = value.Width;
                Y = value.Height;
            }
        }

        public Point Point
        {
            get
            {
                return new Point(X, Y);
            }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }


        public Geo Half => new Geo(X / 2, Y / 2);

        public Geo Invert => new Geo(-X, -Y);

        public Geo(Geo g)
        {
            X = g.X;
            Y = g.Y;
        }

        public Geo(Point p)
        {
            Point = p;
        }

        public Geo(Size s)
        {
            Size = s;
        }

        public Geo(int x, int y)
        {
            Point = new Point(x, y);
        }

        public Geo Add(Geo add)
        {
            X += add.X;
            Y += add.Y;
            return this;
        }

        public Geo Add(Point add)
        {
            X += add.X;
            Y += add.Y;
            return this;
        }

        public Geo Sub(Geo sub)
        {
            X -= sub.X;
            Y -= sub.Y;
            return this;
        }

        public Geo Sub(Point sub)
        {
            X -= sub.X;
            Y -= sub.Y;
            return this;
        }

        public Geo Mult(float x)
        {
            X = (int)(X * x);
            Y = (int)(Y * x);
            return this;
        }


        public override string ToString()
        {
            return $"Geo [{X}, {Y}]";
        }

    }
}
