using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
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
   public class FigureFabric
   {
      public IFigure Create(Point pt1, Point pt2, FigureType type)
      {
         IFigure fig_obj = null;
         switch (type)
         {
            case FigureType.Line:
               fig_obj = new Line(pt1, pt2);
               break;
            case FigureType.Circle:
               fig_obj = new Circle(pt1, pt2);
               break;
            case FigureType.Triangle:
               fig_obj = new Triangle(pt1, pt2);
               break;
            case FigureType.Rectangle:
               fig_obj = new Rectangle(pt1, pt2);
               break;
         }
         return fig_obj;
      }
   }

   
   public class Line : IFigure
   {
      public Point start;
      public Point end;
      public Path? figure;
      public Line (Point start, Point end)
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
      public Ellipse? figure;
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

   public class Triangle : IFigure
   {
      public Point firstPoint;
      public Point secondPoint;
      public Point thirdPoint;
      
      //public Ellipse? figure;
      public Triangle(Point xPoint, Point yPoint)
      {
         this.firstPoint = xPoint;
         this.secondPoint = new Point(yPoint.x, xPoint.y);
         this.thirdPoint = new Point((yPoint.x + xPoint.x) / 2, yPoint.y);          
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

   public class Rectangle : IFigure
   {
      public Point firstPoint;
      public Point secondPoint;
      public Point thirdPoint;
      public Point fourthPoint;

      //public Ellipse? figure;
      public Rectangle(Point xPoint, Point yPoint)
      {
         this.firstPoint = xPoint;
         this.secondPoint = new Point(yPoint.x, xPoint.y);
         this.thirdPoint = new Point(xPoint.x, yPoint.y);
         this.fourthPoint = yPoint;
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
      public void DrawCircle(Circle circleObj)
      {
         var circle = new Ellipse
         {
            Width = circleObj.rad * 2,
            Height = circleObj.rad * 2,
            Stroke = Brushes.Black,
            StrokeThickness = 2
         };

         Canvas.SetLeft(circle, circleObj.center.x - circleObj.rad);
         Canvas.SetTop(circle, circleObj.center.y - circleObj.rad);

         canvas.Children.Add(circle);
         circleObj.figure = circle;
      }
      public void DrawLine(Line line)
      {
         var lineGeom = new LineGeometry
         {
            StartPoint = new Avalonia.Point(line.start.x, line.start.y),
            EndPoint = new Avalonia.Point(line.end.x, line.end.y)
         };
         var lineShape = new Path
         {
            Stroke = Brushes.Black,
            StrokeThickness = 2,
            Data = lineGeom
         };
         line.figure = lineShape;
         canvas.Children.Add(lineShape);
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
         var clip = new RectangleGeometry
         {
            Rect = new Avalonia.Rect(0, 0, canvas.Width, canvas.Height)
         };
         circle.Clip = clip;
         Canvas.SetLeft(circle, Center.x - rad);
            Canvas.SetTop(circle, Center.y - rad);
         
         canvas.Children.Add(circle);
      }

      public void Erase(IFigure figure)
      {
         if (figure is Line)
         {
            var line = figure as Line;
            if (line.figure != null)
            {
               canvas.Children.Remove(line.figure);
               line.figure = null;
            }
         }
         else if (figure is Circle)
         {
            var circle = figure as Circle;
            if (circle.figure != null)
            {
               canvas.Children.Remove(circle.figure);
               circle.figure = null;
            }
         }
      }
   }
}
