using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDIDrawer;
using System.Drawing;

namespace Lab3_WonhyukCho
{
    public interface IRender
    {
        // render instance to the supplied drawer
        void Render(CDrawer dr);
    }
    public interface IAnimate
    {
        // cause per-tick state changes to instance (movement, animation, etc...)
        void Tick();
    }

    public abstract class Shape : IRender
    {
        protected PointF sPosition;
        protected Color sClr;
        protected Color lineClr = Color.White;
        protected Shape parentShape;
        
        public Shape(PointF p, Color c, Shape sP )
        {
            sPosition = p;
            sClr = c;
            parentShape = sP;
        }
        public PointF getPotint
        {
            get
            {
                return sPosition;
            }
        }

        public void Render(CDrawer dr)
        {
            vRender(dr);
          
        }
        protected virtual void vRender(CDrawer dr)
        {
            if (parentShape != null)
            {
                dr.AddLine((int)sPosition.X, (int)sPosition.Y, (int)parentShape.sPosition.X, (int)parentShape.sPosition.Y, lineClr);
            }
            //change color? 

            //   dr.Render();
        }

    }
    public class FixedSquare : Shape
    {
        protected int squareSize;
        public FixedSquare(PointF p, Color c, Shape parent, int iSize = 20) : base(p,c, parent)
        {
            squareSize = iSize;
        }

        protected override void vRender(CDrawer dr)
        {
            base.vRender(dr);
            dr.AddCenteredRectangle((int)sPosition.X, (int)sPosition.Y, squareSize, squareSize, sClr,2,Color.Black);
        }


    }
    public abstract class AniShape : Shape, IAnimate
    {
        protected double sequenceValue;
        protected double delta;
        //protected double animation;
        public AniShape(PointF p, Color c, Shape parent = null, double dAniIncrement = 0, double dAniValue = 0) : base(p, c, parent)
        {
            sequenceValue = dAniValue;
            delta = dAniIncrement;
        }
        public void Tick()
        {
            vTick();
        }
        protected virtual void vTick()
        {
            sequenceValue += delta;
        }
    }
    public class AniPoly : AniShape
    {
        protected int side;
        protected int radius;
        public AniPoly(PointF p, Color c, int sides, Shape parent = null, double dAniIncrement = 0, double dAniValue = 0, int iRadius = 25) : base(p, c, parent, dAniIncrement, dAniValue)
        {
            side = sides;
            radius = iRadius;
            if (side < 3) throw new ArgumentException("count provided is less than 3");


        }
        protected override void vRender(CDrawer dr)
        {
            base.vRender(dr);
            dr.AddPolygon((int)this.sPosition.X, (int)this.sPosition.Y, radius, side, sequenceValue, sClr, 3, Color.Red);
        }
       

    }
    public abstract class AniChild : AniShape
    {
        protected double distance;
        public AniChild(Color c, double dDistToParent, Shape parent, double dAniIncrement = 0, double dAniValue = 0) : base(new PointF(parent.getPotint.X,parent.getPotint.Y) ,c,parent, dAniIncrement, dAniValue)
        {
            if (parent is null) throw new ArgumentException("Parent is null Exception happend!");
            distance = dDistToParent;
            this.Tick();
        }

       
    }
    public class AniHighlight : AniChild
    {
        protected double radius = 50;
        protected int temp;
       public AniHighlight(Color c, Shape parent, double dAniIncrement = 0, double dAniValue = 0) : base(c,0, parent, dAniIncrement, dAniValue)
        {
            delta = dAniIncrement;
        }
       /* protected override void vTick()
        {
            base.vTick();

           
        }*/
        protected override void vRender(CDrawer dr)
        {
            /*  dr.AddCenteredEllipse((int)parentShape.getPotint.X, (int)parentShape.getPotint.Y, (int)(50 + 50 * Math.Cos(sequenceValue) / 2)
                  , (int)(50 + 50 * Math.Cos(sequenceValue) / 2), Color.FromArgb((int)(100 + 100 * Math.Cos(sequenceValue) / 2), sClr));*/

            temp = (int)(radius + radius / 2 * Math.Cos(sequenceValue));
            dr.AddCenteredEllipse((int)parentShape.getPotint.X, (int)parentShape.getPotint.Y,
                                    Math.Max(temp, 1), Math.Max(temp, 1), Color.FromArgb(150, sClr));
            base.vRender(dr);

        }
    }
    public abstract class AniBall : AniChild
    {
        public AniBall(Color c, double dDistToParent, Shape parent, double dAniIncrement = 0, double dAniValue = 0) : base(c,dDistToParent, parent,dAniIncrement,dAniValue)
        {
          /*  sPosition.X = (float)(parent.getPotint.X + dDistToParent);
            sPosition.Y = (float)(parent.getPotint.Y + dDistToParent);*/
        }
        protected override void vRender(CDrawer dr)
        {
            dr.AddCenteredEllipse((int)this.getPotint.X, (int)this.getPotint.Y, 20, 20, sClr);
            base.vRender(dr);
        }
    }
    public class VWobbleBall : AniBall
    {
        public VWobbleBall(Color c, double dParentDistance, Shape parent, double dAniIncrement = 0, double dAniValue = 0) : base(c, dParentDistance,parent,dAniIncrement,dAniValue)
        {         
        }
       
        protected override void vTick()
        {
            base.vTick();
            this.sPosition.X = (float)(this.parentShape.getPotint.X );
            this.sPosition.Y = (float)(this.parentShape.getPotint.Y + distance * Math.Cos(sequenceValue));
        }

    }
    public class HWobbleBall : AniBall
    {

        public HWobbleBall(Color c, double dParentDistance, Shape parent, double dAniIncrement = 0, double dAniValue = 0) : base(c, dParentDistance, parent, dAniIncrement, dAniValue)
        {
         // distance = dParentDistance;
        }
        protected override void vTick()
        {
            base.vTick();
            this.sPosition.X = (float)(this.parentShape.getPotint.X + distance * Math.Sin(sequenceValue));
            this.sPosition.Y = (float)(this.parentShape.getPotint.Y );
        }


    }
    public class OrbitBall : AniBall
    {
        protected double ratio;
        public OrbitBall(Color c, double dParentDistance, Shape parent, double dAniIncrement = 0, double dAniValue = 0, double dWHRatio = 1) : base(c, dParentDistance, parent, dAniIncrement, dAniValue)
        {
            ratio = dWHRatio;
            if(ratio!=1)
               lineClr = Color.Transparent;
        }
        protected override void vTick()
        {
            base.vTick();
            this.sPosition.X = (float)(this.parentShape.getPotint.X + distance * Math.Sin(sequenceValue));
            this.sPosition.Y = (float)(this.parentShape.getPotint.Y + distance * Math.Cos(sequenceValue) * ratio);
        }
    }









}
