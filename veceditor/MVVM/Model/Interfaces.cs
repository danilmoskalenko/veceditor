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
using System.Collections.Immutable;

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

        public static Point operator +(Point a, Point b) => new(a.x + b.x, a.y + b.y);
        public static Point operator -(Point a, Point b) => new(a.x - b.x, a.y - b.y);

        public static Point Origin(params Point[] points)
        {
           Point output = new(0.0, 0.0);
           foreach (Point p in points)
             output += p;
           return new Point(output.x / points.Length, output.y / points.Length);
        }
    }

   public class FigureTransform
   {
      private double a = 1.0, b = 0.0, c = 0.0, d = 1.0, e = 0.0, f = 0.0;
      private double scaleX = 1.0, scaleY = 1.0, mirrorX = 1.0, mirrorY = 1.0, angleRad = 0.0, skewX = 0.0, skewY = 0.0;
      private Point origin;

      public FigureTransform(Point origin)
      {
         this.origin = origin;
      }
      private void Transform()
      {
         a = scaleX * mirrorX * Math.Cos(angleRad) + scaleX * mirrorX * skewX * Math.Sin(angleRad);
         b = -scaleY * mirrorY * Math.Sin(angleRad) + scaleY * mirrorY * skewX * Math.Cos(angleRad);
         c = scaleX * mirrorX * skewY * Math.Cos(angleRad) + scaleX * mirrorX * Math.Sin(angleRad);
         d = -scaleY * mirrorY * skewY * Math.Sin(angleRad) + scaleY * mirrorY * Math.Cos(angleRad);
      }
      public void Resize(double xScale, double yScale)
      {
         this.scaleX = xScale;
         this.scaleY = yScale;
         Transform();
      }
      public void Scale(double xFactor, double yFactor) => Resize(xFactor * this.scaleX, yFactor * this.scaleY);
      public void MirrorX()
      {
         mirrorX = mirrorX * -1.0;
         Transform();
      }
      public void MirrorY()
      {
         mirrorY = mirrorY * -1.0;
         Transform();
      }
      public void Translate(double xOffset, double yOffset)
      {
         e = xOffset; f = yOffset;
         origin = new Point(origin.x + xOffset, origin.y + yOffset);
      }
      public void Translate(Point p) => Translate(p.x, p.y);
      public void Move(double xOffset, double yOffset) => Translate(xOffset + e, yOffset + f);
      public void Move(Point p) => Translate(p + origin);

      public void Rotate(double angle, bool rad = false)
      {
         angleRad = rad ? angle % (2 * Math.PI) : (angle % 360) * 0.01745329251;
         Transform();
      }
      public void SkewX(double xFactor)
      {
         skewX = xFactor;
         Transform();
      }
      public void SkewY(double yFactor)
      {
         skewY = yFactor;
         Transform();
      }
      public ImmutableDictionary<string, double> GetTransform => ImmutableDictionary.CreateRange(
          [
             KeyValuePair.Create("Scale X", scaleX),
               KeyValuePair.Create("Scale Y", scaleY),
               KeyValuePair.Create("Mirror X", mirrorX),
               KeyValuePair.Create("Mirror Y", mirrorY),
               KeyValuePair.Create("Rotation", angleRad),
               KeyValuePair.Create("Skew X", skewX),
               KeyValuePair.Create("Skew Y", skewY),
            ]);
      public Point Apply(Point p) => new Point(a * p.x + c * p.y + e, b * p.x + d * p.y + f);
      public Point ApplyGlobal(Point p) => Apply(p - origin);

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
}