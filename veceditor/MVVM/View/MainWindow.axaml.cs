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
using System.Threading.Tasks;

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
      private List<Shape> _shapes = new();
      private ILogic _logic;
      private MainWindowViewModel viewModel;

      //Имитация выбранной фигуры
      TextBlock SelText;

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
            _logic = new Logic(_canvas);
            viewModel.renderer = new DrawingRenderer(_canvas);
         }
         PointerPressed += OnPointerPressed;
         PointerMoved += OnPointerMoved;
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
         if (Apoint.X < 0 || Apoint.Y < 0 || Apoint.X > _canvas.Bounds.Width || Apoint.Y > _canvas.Bounds.Height)
         {
            return;
         }
         Point point = new Point(Apoint.X, Apoint.Y);
         viewModel._points.Add(point);

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
            viewModel._points.Clear();

            //Пример изменения цвета
            if (_shapes.Count > 1) ChangeColor(_shapes[^2], new SolidColorBrush(Colors.Red));
         }

         // Режим рисования линии
         else if (viewModel._figureType == FigureType.Line && viewModel._points.Count % 2 == 0)
         {
            var line = new Line(viewModel._points[^2], viewModel._points[^1]);
            _logic.AddFigure(line);
            viewModel.renderer.DrawLine(line);
            viewModel._points.Clear();
         }

         // Режим рисования круга
         else if (viewModel._figureType == FigureType.Circle && viewModel._points.Count % 2 == 0)
         {
            var circle = new Circle(viewModel._points[^2], viewModel._points[^1]);
            //double rad = circle.rad;
            viewModel.renderer.DrawCircle(circle);
            viewModel._points.Clear();
         }
      }

     private void OnPointerMoved(object? sender, PointerEventArgs e)
      {
         if (viewModel._points.Count == 1)
         {
            Point start = viewModel._points[^1];
            Avalonia.Point Aend = e.GetPosition(_canvas);
            if (_canvas == null) return;
            if (Aend.X < 0 || Aend.Y < 0 || Aend.X > _canvas.Bounds.Width || Aend.Y > _canvas.Bounds.Height) return;
            Point end = new(Aend.X, Aend.Y);
            if (viewModel._figureType == FigureType.Line)
            {
               Line line = new(start, end);
               viewModel.renderer.DrawLine(line);
               viewModel.tempFigure.Add(line);
            }
            if (viewModel._figureType == FigureType.Circle)
            {
               Circle circle = new(start, end);
               viewModel.renderer.DrawCircle(circle);
               viewModel.tempFigure.Add(circle);
            }
            while (viewModel.tempFigure.Count > 1) { viewModel.renderer.Erase(viewModel.tempFigure[0]); viewModel.tempFigure.RemoveAt(0); }
         }
      }

      private async void OnSavePngClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
      {
         if (_canvas == null) return;

         var saveFileDialog = new SaveFileDialog
         {
            Title = "Save as PNG",
            Filters = new List<FileDialogFilter>
            {
               new FileDialogFilter { Name = "PNG Files", Extensions = new List<string> { "png" } }
            },
            DefaultExtension = "png"
         };

         var filePath = await saveFileDialog.ShowAsync(this);
         if (string.IsNullOrEmpty(filePath))
            return;

         // Hide the SelText temporarily for the export
         bool selTextWasVisible = SelText.IsVisible;
         SelText.IsVisible = false;

         try
         {
            var success = await PngExporter.ExportToPng(_canvas, filePath);
            if (success)
            {
               // Success message
               Console.WriteLine("Image saved successfully!");
            }
            else
            {
               // Error message
               Console.WriteLine("Failed to save the image.");
            }
         }
         finally
         {
            // Restore visibility
            SelText.IsVisible = selTextWasVisible;
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
