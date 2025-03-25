﻿using Avalonia;
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
      public IFigure CreateFromJson(Point pt1, Point pt2, FigureType type,
      Avalonia.Media.Color color, double _strokeThickness)
      {
         IFigure fig_obj = null;
         switch (type)
         {
            case FigureType.Line:
               fig_obj = new Line(pt1, pt2, color, _strokeThickness);
               break;
            case FigureType.Circle:
               fig_obj = new Circle(pt1, pt2, color, _strokeThickness);
               break;
            case FigureType.Triangle:
               fig_obj = new Triangle(pt1, pt2, color, _strokeThickness);
               break;
            case FigureType.Rectangle:
               fig_obj = new Rectangle(pt1, pt2, color, _strokeThickness);
               break;
         }
         return fig_obj;
      }
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
      public Line(Point start, Point end, Avalonia.Media.Color color, double _strokeThickness)
      {
         this.start = start;
         this.end = end;
         ColorFigure = color;
         this._strokeThickness = strokeThickness;
      }
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
      private Point center;
      private Point radPoint;
      public Circle(Point center, Point radPoint, Avalonia.Media.Color color, double strokeThickness)
      {
         this.center = center;
         this.radPoint = radPoint;
         this.rad = Math.Sqrt(Math.Pow(center.x - radPoint.x, 2) + Math.Pow(center.y - radPoint.y, 2));
         this.WhenAnyValue(x => x.Center).Subscribe(_=> UpdateRadius());
         this.WhenAnyValue(x => x.RadPoint).Subscribe(_=> UpdateRadius());
         ColorFigure = color;
         this._strokeThickness = _strokeThickness;
      }

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

      public Circle(Point center, Point radPoint)
      {
         this.center = center;
         this.radPoint = radPoint;
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

   public class Triangle : ReactiveObject, IFigure
   {
      public Triangle(Point xPoint, Point yPoint, Avalonia.Media.Color color, double _strokeThickness)
      {
         this.firstPoint = xPoint;
         this.secondPoint = new Point(yPoint.x, xPoint.y);
         this.thirdPoint = new Point((yPoint.x + xPoint.x) / 2, yPoint.y);
         ColorFigure = color;
         this._strokeThickness = _strokeThickness;
         this.WhenAnyValue(x => x.FirstPoint).Subscribe(_ => UpdateSides());
         this.WhenAnyValue(x => x.FourthPoint).Subscribe(_ => UpdateSides());
      }

      public bool _isSelected;
      private Avalonia.Media.Color color;
      private double _strokeThickness = 2;
      private Point firstPoint;
      private Point secondPoint;
      private Point thirdPoint;
      private Point fourthPoint;

      public Point FirstPoint
      {
          get => firstPoint;
          set
          {             
               this.RaiseAndSetIfChanged(ref firstPoint, value);              
          }
      }

      public Point FourthPoint
      {
          get => fourthPoint;
          set
          {              
               this.RaiseAndSetIfChanged(ref fourthPoint, value);              
          }
      }
      
      //public Ellipse? figure;
      public Triangle(Point xPoint, Point yPoint)
      {
         this.firstPoint = xPoint;
         this.secondPoint = new Point(yPoint.x, xPoint.y);
         this.thirdPoint = new Point((yPoint.x + xPoint.x) / 2, yPoint.y);
         this.fourthPoint = yPoint;
         ColorFigure = Avalonia.Media.Color.FromRgb(0, 0, 0);  
         this.WhenAnyValue(x => x.FirstPoint).Subscribe(_=> UpdateSides());
         this.WhenAnyValue(x => x.FourthPoint).Subscribe(_=> UpdateSides());
         
      }

      private void UpdateSides()
      {
           this.secondPoint = new Point(fourthPoint.x, firstPoint.y);
           this.thirdPoint = new Point((firstPoint.x + fourthPoint.x) / 2, fourthPoint.y);
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
      private Point firstPoint;
      private Point secondPoint;
      private Point thirdPoint;
      private Point fourthPoint;

      public Rectangle(Point xPoint, Point yPoint, Avalonia.Media.Color color, double _strokeThickness)
      {
         this.firstPoint = xPoint;
         this.secondPoint = new Point(yPoint.x, xPoint.y);
         this.thirdPoint = new Point(xPoint.x, yPoint.y);
         this.fourthPoint = yPoint;
         ColorFigure = color;
         this._strokeThickness = _strokeThickness;
         this.WhenAnyValue(x => x.FirstPoint).Subscribe(_ => UpdateSides());
         this.WhenAnyValue(x => x.FourthPoint).Subscribe(_ => UpdateSides());
      }
      public Point FirstPoint
      {
          get => firstPoint;
          set
          {             
               this.RaiseAndSetIfChanged(ref firstPoint, value);              
          }
      }

      public Point FourthPoint
      {
          get => fourthPoint;
          set
          {              
               this.RaiseAndSetIfChanged(ref fourthPoint, value);              
          }
      }

      //public Ellipse? figure;
      public Rectangle(Point xPoint, Point yPoint)
      {
         this.firstPoint = xPoint;
         this.secondPoint = new Point(yPoint.x, xPoint.y);
         this.thirdPoint = new Point(xPoint.x, yPoint.y);
         this.fourthPoint = yPoint;
         ColorFigure = Avalonia.Media.Color.FromRgb(0, 0, 0);
         this.WhenAnyValue(x => x.FirstPoint).Subscribe(_=> UpdateSides());
         this.WhenAnyValue(x => x.FourthPoint).Subscribe(_=> UpdateSides());
      }

      private void UpdateSides()
      {
           this.secondPoint = new Point(fourthPoint.x, firstPoint.y);
           this.thirdPoint = new Point(firstPoint.x, fourthPoint.y);
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
            Fill = Brushes.Red
         };

         Canvas.SetLeft(circle, circleObj.Center.x - 3);
         Canvas.SetTop(circle, circleObj.Center.y - 3);

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

         Canvas.SetLeft(circle, circleObj.Center.x - circleObj.rad);
         Canvas.SetTop(circle, circleObj.Center.y - circleObj.rad);

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
