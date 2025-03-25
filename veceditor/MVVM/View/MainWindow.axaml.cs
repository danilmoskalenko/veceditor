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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reactive.Linq;
using System.Threading;
using veceditor.MVVM;
using veceditor.MVVM.Model;
using veceditor.MVVM.View;
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
         switch (_selectedFigure)
         {
            case FigureType.Point:
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
               break;

            case FigureType.Line:
               if (_points.Count % 2 != 0) break;
               var line = new Line(_points[^2], _points[^1]);
               renderer.DrawLine(line);
               _points.Clear();
               line.figure.PointerPressed += (sender, e) =>
               {
                  InteractFigure(line, false);
               };

               InteractFigure(line, true);
               viewModel.FigureCreate(line);
               break;

            case FigureType.Circle:
               if (_points.Count % 2 != 0) break;
               var circle = new Circle(_points[^2], _points[^1]);
               //double rad = circle.rad;
               renderer.DrawCircle(circle);
               _points.Clear();
               circle.figure.PointerPressed += (sender, e) =>
               {
                  InteractFigure(circle, false);
               };

               InteractFigure(circle, true);
               viewModel.FigureCreate(circle);

               break;
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
            switch(_selectedFigure)
            {
               case FigureType.Line:
                  Line line = new(start, end);
                  renderer.DrawLine(line);
                  tempFigure.Add(line);
                  break;
               case FigureType.Circle:
                  Circle circle = new(start, end);
                  renderer.DrawCircle(circle);
                  tempFigure.Add(circle);
                  break;
            }
            while (tempFigure.Count > 1) { renderer.Erase(tempFigure[0]); tempFigure.RemoveAt(0); }
         }
      }

      public void ReDraw(IFigure figure)
      {
         ClearPointList();

         renderer.Erase(figure);
         if (figure is Line)
         {
            var line = figure as Line;
            renderer.DrawLine(line);

            line.figure.PointerPressed += (sender, e) =>
            {
               InteractFigure(line, false);
            };
            if(figure.isSelected) { SelectFigure(line); }
         }
         else if (figure is Circle)
         {
            var circle = figure as Circle;
            renderer.DrawCircle(circle);

            circle.figure.PointerPressed += (sender, e) =>
            {
               InteractFigure(circle, false);
            };
            if (figure.isSelected) { SelectFigure(circle); }
         }
      }

      private void InteractFigure(IFigure figure, bool ignoreMode)
      {
         if (_selectedFigure != FigureType.Edit && !ignoreMode) return;

         // Убираем выделение (если есть)
         UnselectFigure(viewModel.CurFigure);

         viewModel.CurFigure = figure;

         // Работа с выделением
         SelectFigure(viewModel.CurFigure);
      }

      public List<Circle> selectPointList = new();
      private void ClearPointList()
      {
         foreach (var ell in selectPointList)
         {
            renderer.Erase(ell);
         }
         selectPointList.Clear();
      }
      private void UnselectFigure(IFigure figure)
      {
         ClearPointList();
         if (figure != null)
         {
            figure.isSelected = false;            
            ChangeColor(figure, new SolidColorBrush(figure.ColorFigure));
         }
         //renderer.ReDraw(figure);
      }
      private void SelectFigure(IFigure figure)
      {
         if (figure == null) return;
         figure.isSelected = true;
         DrawPoints(figure);
         ChangeColor(figure, new SolidColorBrush(Colors.Blue));
         LineView.viewModel.mw = this;
         LineView.viewModel.currentFigure = figure;
         LineView.instance.FillInfo(figure);
         //renderer.ReDraw(figure);
      }

      private void DrawPoints(IFigure figure)
      {
         if(figure is Line line)
         {
            Circle point = new(line.start, new Point(0,0));
            renderer.DrawPoint(point);
            selectPointList.Add(point);
            point = new(line.end, new Point(0, 0));
            renderer.DrawPoint(point);
            selectPointList.Add(point);
         }
         else if(figure is Circle circle)
         {
            Circle point = new(circle.center, new Point(0, 0));
            renderer.DrawPoint(point);
            selectPointList.Add(point);
            point = new(circle.radPoint, new Point(0, 0));
            renderer.DrawPoint(point);
            selectPointList.Add(point);
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

      public void ChangeColor(IFigure figure, Brush newColor)
      {
         if (figure is Circle circle)
            circle.figure.Stroke = newColor;
         else if (figure is Line line)
            line.figure.Stroke = newColor;
      }

      public void OnKeyDown(object sender, KeyEventArgs e)
      {
         if (e.Key == Key.D)
         {
            viewModel.DeleteFigure();
            InteractFigure(viewModel.CurFigure, true);
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
            while(viewModel.Figures.Count != 0) viewModel.DeleteFigure();
            UnselectFigure(null);
         }
      }
   }
}
