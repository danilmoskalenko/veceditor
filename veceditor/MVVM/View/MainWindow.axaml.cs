using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Collections.Generic;

namespace veceditor;

public partial class MainWindow : Window
{
   private Canvas? _canvas;
   private List<Point> _points = new();

   public MainWindow()
   {
      InitializeComponent();
      _canvas = this.FindControl<Canvas>("DrawingCanvas");
      PointerPressed += OnPointerPressed;
   }

   private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
   {
      if (_canvas is null) return;
      _canvas.Children.Clear();

      var point = e.GetPosition(_canvas); //Координаты относительно канваса
      _points.Add(point);

      double ellipseSize = 60;

      // Создаём точку в виде элипса
      var ellipse = new Ellipse
      {
         Width = ellipseSize,
         Height = ellipseSize,
         Fill = GetRandomColor()
      };

      // Устанавливаем координаты
      Canvas.SetLeft(ellipse, point.X - ellipseSize / 2.0);
      Canvas.SetTop(ellipse, point.Y - ellipseSize / 2.0);

      // Добавляем точку на Canvas
      _canvas.Children.Add(ellipse);
   }

   public static SolidColorBrush GetRandomColor()
   {
      var random = new Random();
      byte r = (byte)random.Next(0, 256);
      byte g = (byte)random.Next(0, 256);
      byte b = (byte)random.Next(0, 256);
      return new SolidColorBrush(Color.FromArgb(255, r, g, b));
   }
}