using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using DynamicData;
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
      None,
      Point,
      Line,
      Circle,
      Triangle,
      Rectangle
   }
   public partial class MainWindow : Window
   {
      public TextBlock SelText;
      private Canvas? _canvas;
      private List<Shape> _shapes = new();
      private MainWindowViewModel viewModel;
      public List<Point> _points = new();
      public List<IFigure> tempFigure = new();
      public DrawingRenderer? renderer;
      //Имитация выбранной фигуры

      public MainWindow(MainWindowViewModel viewModel)
      {
         
         InitializeComponent();
         DataContext = viewModel;
         this.viewModel = viewModel;
         _canvas = this.FindControl<Canvas>("DrawingCanvas");
         if (_canvas != null)
         {
            _canvas.ClipToBounds = true;
            TextBlock();
            renderer = new DrawingRenderer(_canvas);
         }
         _canvas.PointerPressed += OnPointerPressed;
         _canvas.PointerMoved += OnPointerMoved;
         viewModel.WhenAnyValue(x => x._SelectedFigure)
         .Subscribe(_ => SelectGUI());
      
      }
      void SelectGUI()
      {
         Console.WriteLine("SelectGUI called");
         _points.Clear();
         while (tempFigure.Count > 0)
         {
            renderer?.Erase(tempFigure[0]);
            tempFigure.RemoveAt(0);
         }
         SelText.Text = $"{viewModel._selectedFigure}";
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
         SelText.Text = $"{viewModel._SelectedFigure}";
         _canvas.Children.Add(SelText); 
      }
      private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
      {
         if (_canvas == null) return;

         var Apoint = e.GetPosition(_canvas);
         if (Apoint.X < 0 || Apoint.Y < 0 || Apoint.X > _canvas.Bounds.Width || Apoint.Y > _canvas.Bounds.Height)
         {
            return;
         }
         Point point = new Point(Apoint.X, Apoint.Y);
         _points.Add(point);

         // Режим рисования точки
         if (viewModel._SelectedFigure == FigureType.Point)
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
            _points.Clear();

            //Пример изменения цвета
            //if (_shapes.Count > 1) ChangeColor(_shapes[^2], new SolidColorBrush(Colors.Red));
         }

         // Режим рисования линии
         else if (viewModel._SelectedFigure == FigureType.Line && _points.Count % 2 == 0)
         {
            var line = new Line(_points[^2], _points[^1]);
            renderer.DrawLine(line);
            _points.Clear();
         }

         // Режим рисования круга
         else if (viewModel._SelectedFigure == FigureType.Circle && _points.Count % 2 == 0)
         {
            var circle = new Circle(_points[^2], _points[^1]);
            //double rad = circle.rad;
            renderer.DrawCircle(circle);
            _points.Clear();
         }
      }

     private void OnPointerMoved(object? sender, PointerEventArgs e)
      {
         if (_points.Count == 1)
         {
            Point start = _points[^1];
            Avalonia.Point Aend = e.GetPosition(_canvas);
            if (_canvas == null) return;
            if (Aend.X < 0 || Aend.Y < 0 || Aend.X > _canvas.Bounds.Width || Aend.Y > _canvas.Bounds.Height) return;
            Point end = new(Aend.X, Aend.Y);
            if (viewModel._SelectedFigure == FigureType.Line)
            {
               Line line = new(start, end);
               renderer.DrawLine(line);
               tempFigure.Add(line);
            }
            if (viewModel._SelectedFigure == FigureType.Circle)
            {
               Circle circle = new(start, end);
               renderer.DrawCircle(circle);
               tempFigure.Add(circle);
            }
            while (tempFigure.Count > 1) { renderer.Erase(tempFigure[0]); tempFigure.RemoveAt(0); }
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
