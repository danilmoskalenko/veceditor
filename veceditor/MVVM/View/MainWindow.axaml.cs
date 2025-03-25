using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using DynamicData;
using DynamicData.Binding;
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
      Edit,
      Point,
      Line,
      Circle,
      Triangle,
      Rectangle
   }
   public partial class MainWindow : Window
   {
      public FigureType _selectedFigure;
      public TextBlock SelText;
      private Canvas? _canvas;
      private List<Shape> _shapes = new();
      private MainWindowViewModel viewModel;
      public List<Point> _points = new();
      public List<IFigure> tempFigure = new();
      public DrawingRenderer? renderer;
      //Имитация выбранной фигуры
      public FigureFabric CreatorFigures;
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
         this.KeyDown += OnKeyDown;
         _selectedFigure = FigureType.Line;
         Subscribes();
      
      }
      void Subscribes()
      {
         viewModel.WhenAnyValue(x => x._SelectedFigure)
         .Subscribe(type => SelectGUI(type));
         viewModel.FigureRemoved += DeleteFigure;
      }
      void SelectGUI(FigureType type)
      {
         _points.Clear();
         while (tempFigure.Count > 0)
         {
            renderer?.Erase(tempFigure[0]);
            tempFigure.RemoveAt(0);
         }
         _selectedFigure = type;
         SelText.Text = $"{_selectedFigure}";
      }
      void DeleteFigure(object sender, IFigure figure)
      {
         renderer.Erase(figure);
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

         while (tempFigure.Count > 0) { renderer.Erase(tempFigure[0]); tempFigure.RemoveAt(0); }

         var Apoint = e.GetPosition(_canvas);
         if (Apoint.X < 0 || Apoint.Y < 0 || Apoint.X > _canvas.Bounds.Width || Apoint.Y > _canvas.Bounds.Height)
         {
            return;
         }
         Point point = new Point(Apoint.X, Apoint.Y);
         _points.Add(point);

         // Режим рисования точки
         if (_selectedFigure == FigureType.Point)
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
         else if (_selectedFigure == FigureType.Line && _points.Count % 2 == 0)
         {
            var figure = viewModel.FigureCreate(_points[^2], _points[^1]);
            renderer.DrawLine((Line)figure);            
            _points.Clear();
         }

         // Режим рисования круга
         else if (_selectedFigure == FigureType.Circle && _points.Count % 2 == 0)
         {
            var figure = viewModel.FigureCreate(_points[^2], _points[^1]);
            //double rad = circle.rad;
            renderer.DrawCircle((Circle)figure);           
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
            if (_selectedFigure == FigureType.Line)
            {
               Line line = new(start, end);
               renderer.DrawLine(line);
               tempFigure.Add(line);
            }
            if (_selectedFigure == FigureType.Circle)
            {
               Circle circle = new(start, end);
               renderer.DrawCircle(circle);
               tempFigure.Add(circle);
            }
            while (tempFigure.Count > 1) { renderer.Erase(tempFigure[0]); tempFigure.RemoveAt(0); }
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

      public void OnKeyDown(object sender, KeyEventArgs e)
      {
         if (e.Key == Key.D)
         {
            viewModel.DeleteFigure();
         }
         if (e.Key == Key.C)
         {
            for (int i = 0; i < _canvas.Children.Count; i++)
            {
               if (!(_canvas.Children[i] is TextBlock))
               {
                  _canvas.Children.Remove(_canvas.Children[i]);
                  i--;
               }
            }
         }
      }
   }
}
