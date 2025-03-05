﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using veceditor.MVVM;
using veceditor.MVVM.Model;
using veceditor.MVVM.ViewModel;
using static System.Net.Mime.MediaTypeNames;
using Line = veceditor.MVVM.Line;
using Point = veceditor.MVVM.Model.Point;

namespace veceditor
{
   public enum FigureType
   {
      Point,
      Circle,
      Rectangle,
      Triangle,
      Line
   }
   public partial class MainWindow : Window
   {
      private Canvas? _canvas;
      private List<Point> _points = new();
      private List<Shape> _shapes = new();

      private DrawingRenderer renderer;
      private MainWindowViewModel viewModel;

      //Имитация выбранной фигуры
      TextBlock SelText;
      /*
       * Режим 0 - рисование только точек
       * Режим 1 - рисование линий
       * Режим 2 - рисование круга
      */
      private int mode = 0;

      public MainWindow(MainWindowViewModel viewModel)
      {
         InitializeComponent();
         DataContext = viewModel;
         this.viewModel = viewModel;
         _canvas = this.FindControl<Canvas>("DrawingCanvas");
         if (_canvas != null)
         {
            viewModel._logic = new Logic(_canvas);
            TextBlock();
            renderer = new DrawingRenderer(_canvas);
         }
         PointerPressed += OnPointerPressed;
      }
      void TextBlock()
      {
         SelText = new TextBlock
         {
            FontSize = 20,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
            Margin = new Thickness(0, 10, 10, 0),
            Foreground = Brushes.Red,
         };
         SelText.Text = $"{viewModel._figureType}";
         viewModel.SelText = SelText;
         _canvas.Children.Add(SelText); 
      }
      private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
      {
         if (_canvas == null) return;

         var Apoint = e.GetPosition(_canvas);
         Point point = new Point(Apoint.X, Apoint.Y);
         _points.Add(point);

         // Режим рисования точки
         if (viewModel._figureType == FigureType.Point)
         {
            var ellipse = new Ellipse
            {
               Width = 6,
               Height = 6,
               Fill = Brushes.Black
            };
            Canvas.SetLeft(ellipse, point.x - 3);
            Canvas.SetTop(ellipse, point.y - 3);
            _canvas.Children.Add(ellipse);
            _shapes.Add(ellipse);

            //Пример изменения цвета
            if (_shapes.Count > 1) ChangeColor(_shapes[^2], new SolidColorBrush(Colors.Red));
         }

         // Режим рисования линии
         else if (viewModel._figureType == FigureType.Line && _points.Count % 2 == 0)
         {
            //var lineGeom = new LineGeometry
            //{
            //   StartPoint = _points[^2],
            //   EndPoint = _points[^1]
            //};
            //var lineShape = new Path
            //{
            //   Stroke = Brushes.Black,
            //   StrokeThickness = 2,
            //   Data = lineGeom
            //};
            //_canvas.Children.Add(lineShape);
            //_shapes.Add(lineShape);
            var line = new Line(_points[^2], _points[^1]);
            renderer.DrawLine(line);
         }

         // Режим рисования круга
         else if (viewModel._figureType == FigureType.Circle && _points.Count % 2 == 0)
         {
            //var center = _points[^2];
            //var radiusPoint = _points[^1];
            //var radius = Math.Sqrt(Math.Pow(radiusPoint.x - center.x, 2) + Math.Pow(radiusPoint.y - center.y, 2));
            //var circle = new Ellipse
            //{
            //   Width = radius * 2,
            //   Height = radius * 2,
            //   Stroke = Brushes.Black,
            //   StrokeThickness = 2
            //};
            //Canvas.SetLeft(circle, center.x - radius);
            //Canvas.SetTop(circle, center.y - radius);
            //_canvas.Children.Add(circle);
            //_shapes.Add(circle);
            var circle = new Circle(_points[^2], _points[^1]);
            double rad = circle.rad;
            renderer.DrawCircle(_points[^2], rad);
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
