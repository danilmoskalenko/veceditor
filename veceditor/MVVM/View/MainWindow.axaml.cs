using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using System.Collections.Generic;

namespace veceditor
{
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
      }
   }
}
