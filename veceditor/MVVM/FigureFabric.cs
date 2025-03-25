using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
using ReactiveUI;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
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
            case FigureType.Point:
               fig_obj = new Circle(pt1, pt2, true);
               break;
            case FigureType.Line:
               fig_obj = new Line(pt1, pt2);
               break;
            case FigureType.Circle:
               fig_obj = new Circle(pt1, pt2, false);
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

      public bool _isSelected;

      private Avalonia.Media.Color color;
      private double _strokeThickness = 2;
      public Line (Point start, Point end)
      {
         this.start = start;
         this.end = end;
         ColorFigure = Avalonia.Media.Color.FromRgb(0, 0, 0);
      }
      public bool IsClosed => throw new NotImplementedException();
      public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
      public Avalonia.Media.Color ColorFigure { get => color; set => color = value; }
      public double strokeThickness { get => _strokeThickness; set => _strokeThickness = value; }
      bool IFigure.isSelected { get => _isSelected; set => _isSelected = value; }


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

   public class Circle : ReactiveObject, IFigure
   {
      public bool _isSelected;
      private Avalonia.Media.Color color;
      private double _strokeThickness = 2;
      public Point center;
      public Point radPoint;
      public bool isPoint;

      public Point Center
      {
          get => center;
          set
          {             
               this.RaiseAndSetIfChanged(ref center, value);              
          }
      }

      public Point RadPoint
      {
          get => radPoint;
          set
          {              
               this.RaiseAndSetIfChanged(ref radPoint, value);              
          }
      }
      public double rad;
      public Ellipse? figure;

      public Circle(Point center, Point radPoint, bool isPoint)
      {
         this.center = center;
         this.radPoint = radPoint;
         this.isPoint = isPoint;
         this.rad = Math.Sqrt(Math.Pow(center.x - radPoint.x, 2) + Math.Pow(center.y - radPoint.y, 2));
         this.WhenAnyValue(x => x.Center).Subscribe(_=> UpdateRadius());
         this.WhenAnyValue(x => x.RadPoint).Subscribe(_=> UpdateRadius());
         ColorFigure = Avalonia.Media.Color.FromRgb(0, 0, 0);
      }

      private void UpdateRadius()
      {
           this.rad = Math.Sqrt(Math.Pow(center.x - radPoint.x, 2) + Math.Pow(center.y - radPoint.y, 2));
      }

      
      public bool IsClosed => throw new NotImplementedException();
      public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
      public Avalonia.Media.Color ColorFigure { get => color; set => color = value; }
      public double strokeThickness { get => _strokeThickness; set => _strokeThickness = value; }
      bool IFigure.isSelected { get => _isSelected; set => _isSelected = value; }
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
      public bool _isSelected;
      private Avalonia.Media.Color color;
      private double _strokeThickness = 2;
      public Point topPoint;
      public Point bottomPoint1;
      public Point bottomPoint2;
      internal Polygon? figure;

      //public Ellipse? figure;
      public Triangle(Point topPoint, Point bottomPoint1)
      {
         this.topPoint = topPoint;
         this.bottomPoint1 = bottomPoint1;

         // Рассчитываем вторую боковую точку (bottomPoint2)
         CalculateBottomPoint2();

         ColorFigure = Avalonia.Media.Color.FromRgb(0, 0, 0);
      }

      // Метод для вычисления второй боковой точки
      public void CalculateBottomPoint2()
      {
         // Проверяем, где расположена первая точка основания относительно верхней точки
         double deltaX = bottomPoint1.x - topPoint.x;
         double deltaY = bottomPoint1.y - topPoint.y;

         // Если первая точка ниже верхней, то зеркалим по оси X
         if (deltaY > 0)
         {
            double bottomPoint2X = topPoint.x - deltaX; // Симметричное расположение по X
            bottomPoint2 = new Point(bottomPoint2X, bottomPoint1.y);
         }
         // Если первая точка выше верхней, то зеркалим по оси Y
         else
         {
            double bottomPoint2Y = topPoint.y - deltaY; // Симметричное расположение по Y
            bottomPoint2 = new Point(bottomPoint1.x, bottomPoint2Y);
         }
      }

      public bool IsClosed => throw new NotImplementedException();
      public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
      public Avalonia.Media.Color ColorFigure { get => color; set => color = value; }
      public double strokeThickness { get => _strokeThickness; set => _strokeThickness = value; }
      bool IFigure.isSelected { get => _isSelected; set => _isSelected = value; }
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

   public class Rectangle : ReactiveObject, IFigure
   {
      public bool _isSelected;
      private Avalonia.Media.Color color;
      private double _strokeThickness = 2;
      public Point topLeft;
      public Point bottomRight;
      internal Path? figure;

      public Point TopLeft
      {
         get => topLeft;
         set => this.RaiseAndSetIfChanged(ref topLeft, value);
      }

      public Point BottomRight
      {
         get => bottomRight;
         set => this.RaiseAndSetIfChanged(ref bottomRight, value);
      }

      public double Width => Math.Abs(bottomRight.x - topLeft.x);
      public double Height => Math.Abs(bottomRight.y - topLeft.y);

      public Rectangle(Point point1, Point point2)
      {
         this.topLeft = point1;
         this.bottomRight = point2;

         UpdatePoint();
         
         ColorFigure = Avalonia.Media.Color.FromRgb(0, 0, 0);
      }

      public void UpdatePoint()
      {
         Point point1 = topLeft;
         Point point2 = bottomRight;
         if (point1.x < point2.x && point1.y < point2.y)
         {
            this.topLeft = point1;
            this.bottomRight = point2;
         }
         else if (point1.x > point2.x && point1.y > point2.y)
         {
            this.topLeft = point2;
            this.bottomRight = point1;
         }
         else
         {
            // Если одна точка по оси X или Y больше другой, их нужно инвертировать
            this.topLeft = new Point(Math.Min(point1.x, point2.x), Math.Min(point1.y, point2.y));
            this.bottomRight = new Point(Math.Max(point1.x, point2.x), Math.Max(point1.y, point2.y));
         }
      }

      public bool IsClosed => true;
      public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
      public Avalonia.Media.Color ColorFigure { get => color; set => color = value; }
      public double strokeThickness { get => _strokeThickness; set => _strokeThickness = value; }
      bool IFigure.isSelected { get => _isSelected; set => _isSelected = value; }

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
      public void DrawPoint(Circle circleObj)
      {
         var circle = new Ellipse
         {
            Width = 6,
            Height = 6,
            Fill = new SolidColorBrush(circleObj.ColorFigure)
         };

         Canvas.SetLeft(circle, circleObj.center.x - 3);
         Canvas.SetTop(circle, circleObj.center.y - 3);

         canvas.Children.Add(circle);
         circleObj.figure = circle;
      }
      public void DrawCircle(Circle circleObj)
      {
         var circle = new Ellipse
         {
            Width = circleObj.rad * 2,
            Height = circleObj.rad * 2,
            Stroke = new SolidColorBrush(circleObj.ColorFigure),
            StrokeThickness = circleObj.strokeThickness,
            //Fill = Brushes.Transparent
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
            Stroke = new SolidColorBrush(line.ColorFigure),
            StrokeThickness = line.strokeThickness,
            Data = lineGeom
         };
         line.figure = lineShape;
         canvas.Children.Add(lineShape);
      }

      public void DrawRectangle(Rectangle rectangle)
      {
         var rect = new RectangleGeometry
         {
            Rect = new Avalonia.Rect(rectangle.TopLeft.x, rectangle.TopLeft.y, rectangle.Width, rectangle.Height)
         };

         var rectShape = new Path
         {
            Stroke = new SolidColorBrush(rectangle.ColorFigure),
            StrokeThickness = rectangle.strokeThickness,
            Data = rect
         };

         canvas.Children.Add(rectShape);
         rectangle.figure = rectShape;
      }


      public void DrawTriangle(Triangle triangle)
      {
         var triangleShape = new Polygon
         {
            Points = new AvaloniaList<Avalonia.Point>
        {
            new Avalonia.Point(triangle.topPoint.x, triangle.topPoint.y),
            new Avalonia.Point(triangle.bottomPoint1.x, triangle.bottomPoint1.y),
            new Avalonia.Point(triangle.bottomPoint2.x, triangle.bottomPoint2.y)
        },
            Stroke = new SolidColorBrush(triangle.ColorFigure),
            StrokeThickness = triangle.strokeThickness,
            //Fill = new SolidColorBrush(triangle.FillColor) // Можно добавить заливку (если нужно)
         };

         canvas.Children.Add(triangleShape);
         triangle.figure = triangleShape;
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
         else if(figure is Rectangle rectangle)
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
