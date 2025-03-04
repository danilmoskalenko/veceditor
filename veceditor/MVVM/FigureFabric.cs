using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using veceditor.MVVM.Model;
using Point = veceditor.MVVM.Model.Point;

namespace veceditor.MVVM
{
   public class Line : IFigure
   {
      Point start;
      Point end;
      public Line(Point start, Point end)
      {
         this.start = start;
         this.end = end;
      }
      public bool IsClosed => throw new NotImplementedException();
      public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
      public IEnumerable<IDrawableFigure> GetDrawFigures() => throw new NotImplementedException();
      public IFigure Intersect(IFigure other) => throw new NotImplementedException();
      public bool IsInternal(Point p, double eps) => throw new NotImplementedException();
      public void Move(Point vector) => throw new NotImplementedException();
      public void Reflection(Point ax1, Point ax2) => throw new NotImplementedException();
      public void Rotate(Point Center, double angle) => throw new NotImplementedException();
      public void Scale(double x, double y) => throw new NotImplementedException();
      public void Scale(Point Center, double rad) => throw new NotImplementedException();
      public IFigure Subtract(IFigure other) { throw new NotImplementedException(); }
      public IFigure Union(IFigure other) { throw new NotImplementedException(); }
   }

   public class Circle : IFigure
   {
      public Point center;
      public Point radPoint;
      public double rad;
      public Circle(Point center, Point radPoint)
      {
         this.center = center;
         this.radPoint = radPoint;
         this.rad = Math.Sqrt(Math.Pow(center.x - radPoint.x, 2) + Math.Pow(center.y - radPoint.y, 2));
      }

      public bool IsClosed => throw new NotImplementedException();
      public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
      public IEnumerable<IDrawableFigure> GetDrawFigures() => throw new NotImplementedException();
      public IFigure Intersect(IFigure other) => throw new NotImplementedException();
      public bool IsInternal(Point p, double eps) => throw new NotImplementedException();
      public void Move(Point vector) => throw new NotImplementedException();
      public void Reflection(Point ax1, Point ax2) => throw new NotImplementedException();
      public void Rotate(Point Center, double angle) => throw new NotImplementedException();
      public void Scale(double x, double y) => throw new NotImplementedException();
      public void Scale(Point Center, double rad) => throw new NotImplementedException();
      public IFigure Subtract(IFigure other) { throw new NotImplementedException(); }
      public IFigure Union(IFigure other) { throw new NotImplementedException(); }
   }

   public class DrawingRenderer : IGraphicInterface
   {
      Canvas canvas;
      public DrawingRenderer(Canvas canvas)
      {
         this.canvas = canvas;
      }
      public void DrawCircle(Point Center, double rad)
      {
         var circle = new Ellipse
         {
            Width = rad * 2,
            Height = rad * 2,
            Stroke = Brushes.Black,
            StrokeThickness = 2
         };
         Canvas.SetLeft(circle, Center.x - rad);
         Canvas.SetTop(circle, Center.y - rad);
         canvas.Children.Add(circle);
      }

      public void DrawLine(Point start, Point end)
      {
         var lineGeom = new LineGeometry
         {
            StartPoint = new Avalonia.Point(start.x, start.y),
            EndPoint = new Avalonia.Point(end.x, end.y)
         };
         var lineShape = new Path
         {
            Stroke = Brushes.Black,
            StrokeThickness = 2,
            Data = lineGeom
         };
         canvas.Children.Add(lineShape);
      }
   }
}
