using Doko.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doko.UI.Controls
{
    class AllContainerDrawObject : AbstractDrawObject
    {
        public List<AbstractDrawObject> Objects = new List<AbstractDrawObject>();

        public override string ToString()
        {
            return "AllContainer";
        }

        internal override void Draw(Graphics g)
        {
            //g.DrawImage(Resources.green, Bounds);
        }

        internal override List<AbstractDrawObject> GetChilds()
        {
            return Objects;
        }
    }
}
