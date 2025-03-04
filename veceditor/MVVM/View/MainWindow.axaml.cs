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

      /*
       * ����� 0 - ��������� ������ �����
       * ����� 1 - ��������� �����
       * ����� 2 - ��������� �����
      */
      private int mode = 2;

      public MainWindow()
      {
         InitializeComponent();
         _canvas = this.FindControl<Canvas>("DrawingCanvas");
         PointerPressed += OnPointerPressed;
      }

      private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
      {
         if (_canvas == null) return;

         var point = e.GetPosition(_canvas); //���������� ������������ �������
         _points.Add(point);

         // ������ ����� � ���� �������
         var ellipse = new Ellipse
         {
            Width = 6,
            Height = 6,
            Fill = Brushes.Black
         };

         // ������������� ����������
         Canvas.SetLeft(ellipse, point.X - 3); //-3 ��� ������
         Canvas.SetTop(ellipse, point.Y - 3);

         // ��������� ����� �� Canvas
         _canvas.Children.Add(ellipse);

         //��������� ����� (����� ������� ����)
         if(_points.Count % 2 == 0 && mode == 1)
         {
            var lineGeom = new LineGeometry
            {
               StartPoint = _points[^2], //������ � �����
               EndPoint = _points[^1] //���������
            };
            var lineShape = new Path
            {
               Stroke = Brushes.Black,
               StrokeThickness = 2,
               Data = lineGeom
            };
            _canvas.Children.Add(lineShape);
         }
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
         }

      }
   }
}
