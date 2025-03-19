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

namespace veceditor.MVVM
{
   public class Line : IFigure
   {
      public Model.Point start;
      public Model.Point end;
      public Path? figure;
      public Line(Model.Point start, Model.Point end)
      {
         this.start = start;
         this.end = end;
      }
      public bool IsClosed => false;
      public string Name { get; set; } = "Line";
      public IEnumerable<IDrawableFigure> GetDrawFigures() => throw new NotImplementedException();
      public IFigure Intersect(IFigure other) => throw new NotImplementedException();
      public bool IsInternal(Model.Point p, double eps) => throw new NotImplementedException();
      public void Move(Model.Point vector) => throw new NotImplementedException();
      public void Reflection(Model.Point ax1, Model.Point ax2) => throw new NotImplementedException();
      public void Rotate(Model.Point Center, double angle) => throw new NotImplementedException();
      public void Scale(double x, double y) => throw new NotImplementedException();
      public void Scale(Model.Point Center, double rad) => throw new NotImplementedException();
      public IFigure Subtract(IFigure other) { throw new NotImplementedException(); }
      public IFigure Union(IFigure other) { throw new NotImplementedException(); }
   }

   public class Circle : IFigure
   {
      public Model.Point center;
      public Model.Point radPoint;
      public double rad;
      public Ellipse? figure;
      public Circle(Model.Point center, Model.Point radPoint)
      {
         this.center = center;
         this.radPoint = radPoint;
         this.rad = Math.Sqrt(Math.Pow(center.x - radPoint.x, 2) + Math.Pow(center.y - radPoint.y, 2));
      }

      public bool IsClosed => true;
      public string Name { get; set; } = "Circle";
      public IEnumerable<IDrawableFigure> GetDrawFigures() => throw new NotImplementedException();
      public IFigure Intersect(IFigure other) => throw new NotImplementedException();
      public bool IsInternal(Model.Point p, double eps) => throw new NotImplementedException();
      public void Move(Model.Point vector) => throw new NotImplementedException();
      public void Reflection(Model.Point ax1, Model.Point ax2) => throw new NotImplementedException();
      public void Rotate(Model.Point Center, double angle) => throw new NotImplementedException();
      public void Scale(double x, double y) => throw new NotImplementedException();
      public void Scale(Model.Point Center, double rad) => throw new NotImplementedException();
      public IFigure Subtract(IFigure other) { throw new NotImplementedException(); }
      public IFigure Union(IFigure other) { throw new NotImplementedException(); }
   }

   public class Rectangle : IFigure
   {
      public Model.Point topLeft;
      public Model.Point bottomRight;
      public Path? figure;
      public Rectangle(Model.Point topLeft, Model.Point bottomRight)
      {
         this.topLeft = topLeft;
         this.bottomRight = bottomRight;
      }

      public bool IsClosed => true;
      public string Name { get; set; } = "Rectangle";
      public IEnumerable<IDrawableFigure> GetDrawFigures() => throw new NotImplementedException();
      public IFigure Intersect(IFigure other) => throw new NotImplementedException();
      public bool IsInternal(Model.Point p, double eps) => throw new NotImplementedException();
      public void Move(Model.Point vector) => throw new NotImplementedException();
      public void Reflection(Model.Point ax1, Model.Point ax2) => throw new NotImplementedException();
      public void Rotate(Model.Point Center, double angle) => throw new NotImplementedException();
      public void Scale(double x, double y) => throw new NotImplementedException();
      public void Scale(Model.Point Center, double rad) => throw new NotImplementedException();
      public IFigure Subtract(IFigure other) { throw new NotImplementedException(); }
      public IFigure Union(IFigure other) { throw new NotImplementedException(); }
   }

   public class Triangle : IFigure
   {
      public Model.Point point1;
      public Model.Point point2;
      public Model.Point point3;
      public Path? figure;
      public Triangle(Model.Point point1, Model.Point point2, Model.Point point3)
      {
         this.point1 = point1;
         this.point2 = point2;
         this.point3 = point3;
      }

      public bool IsClosed => true;
      public string Name { get; set; } = "Triangle";
      public IEnumerable<IDrawableFigure> GetDrawFigures() => throw new NotImplementedException();
      public IFigure Intersect(IFigure other) => throw new NotImplementedException();
      public bool IsInternal(Model.Point p, double eps) => throw new NotImplementedException();
      public void Move(Model.Point vector) => throw new NotImplementedException();
      public void Reflection(Model.Point ax1, Model.Point ax2) => throw new NotImplementedException();
      public void Rotate(Model.Point Center, double angle) => throw new NotImplementedException();
      public void Scale(double x, double y) => throw new NotImplementedException();
      public void Scale(Model.Point Center, double rad) => throw new NotImplementedException();
      public IFigure Subtract(IFigure other) { throw new NotImplementedException(); }
      public IFigure Union(IFigure other) { throw new NotImplementedException(); }
   }

   public class DrawingRenderer : IGraphicInterface
   {
      public Canvas canvas;
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

      public void DrawRectangle(Rectangle rectangle)
      {
         var rectGeom = new RectangleGeometry
         {
            Rect = new Avalonia.Rect(
               rectangle.topLeft.x,
               rectangle.topLeft.y,
               rectangle.bottomRight.x - rectangle.topLeft.x,
               rectangle.bottomRight.y - rectangle.topLeft.y
            )
         };
         var rectShape = new Path
         {
            Stroke = Brushes.Black,
            StrokeThickness = 2,
            Data = rectGeom
         };
         rectangle.figure = rectShape;
         canvas.Children.Add(rectShape);
      }

      public void DrawTriangle(Triangle triangle)
      {
         var figure = new PathFigure
         {
            StartPoint = new Avalonia.Point(triangle.point1.x, triangle.point1.y),
            Segments = new PathSegments
            {
               new LineSegment { Point = new Avalonia.Point(triangle.point2.x, triangle.point2.y) },
               new LineSegment { Point = new Avalonia.Point(triangle.point3.x, triangle.point3.y) },
               new LineSegment { Point = new Avalonia.Point(triangle.point1.x, triangle.point1.y) }
            }
         };
         var pathGeom = new PathGeometry { Figures = new PathFigures { figure } };
         var triangleShape = new Path
         {
            Stroke = Brushes.Black,
            StrokeThickness = 2,
            Data = pathGeom
         };
         triangle.figure = triangleShape;
         canvas.Children.Add(triangleShape);
      }

      public void DrawCircle(Model.Point Center, double rad)
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
         if (figure is Line line)
         {
            if (line.figure != null)
            {
               canvas.Children.Remove(line.figure);
               line.figure = null;
            }
         }
         else if (figure is Circle circle)
         {
            if (circle.figure != null)
            {
               canvas.Children.Remove(circle.figure);
               circle.figure = null;
            }
         }
         else if (figure is Rectangle rectangle)
         {
            if (rectangle.figure != null)
            {
               canvas.Children.Remove(rectangle.figure);
               rectangle.figure = null;
            }
         }
         else if (figure is Triangle triangle)
         {
            if (triangle.figure != null)
            {
               canvas.Children.Remove(triangle.figure);
               triangle.figure = null;
            }
         }
      }
   }
}
