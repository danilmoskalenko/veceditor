using Avalonia.Media;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace veceditor.MVVM.Model
{
   public abstract class AbstractBaseFigure(Point origin, String name = "") : ReactiveObject, IFigure
   {
      protected FigureTransform transform = new FigureTransform(origin);
      protected String name = name;
      protected Avalonia.Media.Color color;
      protected double _strokeThickness = 2;
      protected bool _isSelected = false;
      public string Name { get { return name; } set { name = value; } }
      public Color ColorFigure { get { return color; } set { color = value; } }
      public double strokeThickness { get { return _strokeThickness; } set { _strokeThickness = value; } }
      public bool isSelected { get { return _isSelected; } set { _isSelected = value; } }
      bool IFigure.IsClosed => throw new NotImplementedException();
      public abstract IEnumerable<IDrawableFigure> GetDrawFigures();
      public abstract IFigure Intersect(IFigure other);
      public abstract bool IsInternal(Point p, double eps);
      public void Move(Point vector) => transform.Move(vector);
      public void Rotate(double angle) => transform.Rotate(angle, true);
      public void Scale(double x, double y) => transform.Resize(x, y);
      public void MirrorX() => transform.MirrorX();
      public void MirrorY() => transform.MirrorY();
      public void SkewX(double a) => transform.SkewX(a);
      public void SkewY(double a) => transform.SkewY(a);
      public abstract void Reflection(Point ax1, Point ax2);
      public abstract void Scale(Point Center, double rad);
      public abstract IFigure Union(IFigure other);
      public abstract IFigure Subtract(IFigure other);
      public abstract void Rotate(Point Center, double angle);
   }
}
