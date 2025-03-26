using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media;

namespace veceditor.MVVM.Model
{
    public class Point
    {
        public double x, y;
        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }
    public interface IFigureGraphicProperties
    {
        Color SolidColor { get; }
        Color BorderColor { get; }

    }
    public interface IGraphicInterface
    {
        void DrawCircle(Circle circle);
        void DrawLine(Line line);
        void DrawRectangle(Rectangle rectangle);
        void DrawTriangle(Triangle triangle);

    }
    public interface IDrawableFigure
    {
        void Draw(IGraphicInterface graphic);
    }

   public interface IFigure
   {
      bool IsInternal(Point p, double eps);
      IFigure Intersect(IFigure other);
      IFigure Union(IFigure other);
      IFigure Subtract(IFigure other);
      void Move(Point vector);
      void Rotate(Point Center, double angle);
      void Scale(double x, double y);
      void Scale(Point Center, double rad);
      void Reflection(Point ax1, Point ax2);
      IEnumerable<IDrawableFigure> GetDrawFigures();
      bool IsClosed { get; }
      string Name { get; set; }
      Avalonia.Media.Color ColorFigure { get; set; }
      double strokeThickness { get; set; }

      bool isSelected { get; set; }
   }
   public abstract class BaseFigure : IFigure, IDrawableFigure
   {
      protected IFigureGraphicProperties graphics;
      public string Name { get; set; }

      public BaseFigure(string name, IFigureGraphicProperties graphics)
      {
         Name = name;
         this.graphics = graphics;
      }

      public abstract bool IsInternal(Point p, double eps);
      public abstract IFigure Intersect(IFigure other);
      public abstract IFigure Union(IFigure other);
      public abstract IFigure Subtract(IFigure other);
      public abstract void Move(Point vector);
      public abstract void Rotate(Point center, double angle);
      public abstract void Scale(double x, double y);
      public abstract void Scale(Point center, double rad);
      public abstract void Reflection(Point ax1, Point ax2);
      public abstract bool IsClosed { get; }
      public abstract void Draw(IGraphicInterface graphic);

      public virtual IEnumerable<IDrawableFigure> GetDrawFigures()
      {
         return new List<IDrawableFigure> { this };
      }
   }
}