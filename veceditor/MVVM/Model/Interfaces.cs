using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
      Point GetCenter();
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