using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Collections.Generic;

namespace veceditor
{
   public partial class MainWindow : Window
   {
      private Canvas? _canvas;
      private List<Point> _points = new();
      private List<Shape> _shapes = new();

      /*
       * Режим 0 - рисование только точек
       * Режим 1 - рисование линий
       * Режим 2 - рисование круга
      */
      private int mode = 0;

      public MainWindow()
      {
         InitializeComponent();
         _canvas = this.FindControl<Canvas>("DrawingCanvas");
         PointerPressed += OnPointerPressed;
      }

      private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
      {
         if (_canvas == null) return;

         var point = e.GetPosition(_canvas);
         _points.Add(point);

         // Режим рисования точки
         if (mode == 0)
         {
            var ellipse = new Ellipse
            {
               Width = 6,
               Height = 6,
               Fill = Brushes.Black
            };
            Canvas.SetLeft(ellipse, point.X - 3);
            Canvas.SetTop(ellipse, point.Y - 3);
            _canvas.Children.Add(ellipse);
            _shapes.Add(ellipse);

            //Пример изменения цвета
            if (_shapes.Count > 1) ChangeColor(_shapes[^2], new SolidColorBrush(Colors.Red));
         }

         // Режим рисования линии
         else if (mode == 1 && _points.Count % 2 == 0)
         {
            var lineGeom = new LineGeometry
            {
               StartPoint = _points[^2],
               EndPoint = _points[^1]
            };
            var lineShape = new Path
            {
               Stroke = Brushes.Black,
               StrokeThickness = 2,
               Data = lineGeom
            };
            _canvas.Children.Add(lineShape);
            _shapes.Add(lineShape);
         }

         // Режим рисования круга
         else if (mode == 2 && _points.Count % 2 == 0)
         {
            var center = _points[^2];
            var radiusPoint = _points[^1];
            var radius = Math.Sqrt(Math.Pow(radiusPoint.X - center.X, 2) + Math.Pow(radiusPoint.Y - center.Y, 2));

            var circle = new Ellipse
            {
               Width = radius * 2,
               Height = radius * 2,
               Stroke = Brushes.Black,
               StrokeThickness = 2
            };
            Canvas.SetLeft(circle, center.X - radius);
            Canvas.SetTop(circle, center.Y - radius);
            _canvas.Children.Add(circle);
            _shapes.Add(circle);
         }
      }

      public void ChangeColor(Shape shape, Brush newColor)
      {
         if (shape is Ellipse ellipse)
            ellipse.Fill = newColor;
         else if (shape is Path path)
            path.Stroke = newColor;
      }
   }
}
