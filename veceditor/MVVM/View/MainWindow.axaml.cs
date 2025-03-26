using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using veceditor.MVVM;
using veceditor.MVVM.Model;
using veceditor.MVVM.View;
using veceditor.MVVM.ViewModel;
using Line = veceditor.MVVM.Line;
using Point = veceditor.MVVM.Model.Point;
using Rectangle = veceditor.MVVM.Rectangle;
using Newtonsoft.Json;
using System.Data;
using System.Linq;
using System.IO;

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
         //SelText.Text = $"{_selectedFigure}";
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
         SelText.Text = "";
         SelText.Text += "Delete - удалить\n";
         SelText.Text += "Ctrl+C - очистка\n";
         //SelText.Text = $"{viewModel._SelectedFigure}";
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
               var _point = viewModel.FigureCreate(point, new Point(0, 0)) as Circle;
               renderer.DrawPoint(_point);
               _points.Clear();
               _point.figure.PointerPressed += (sender, e) =>
               {
                  InteractFigure(_point, false);
               };

               InteractFigure(_point, true);
               viewModel.Figures.Add(_point);
               break;


            case FigureType.Line:
               if (_points.Count % 2 != 0) break;
               var line = viewModel.FigureCreate(_points[^2], _points[^1]) as Line;
               renderer.DrawLine(line);
               _points.Clear();
               line.figure.PointerPressed += (sender, e) =>
               {
                  InteractFigure(line, false);
               };

               InteractFigure(line, true);
               viewModel.Figures.Add(line);
               break;

            case FigureType.Circle:
               if (_points.Count % 2 != 0) break;
               var circle = viewModel.FigureCreate(_points[^2], _points[^1]) as Circle;
               //double rad = circle.rad;
               renderer.DrawCircle(circle);
               _points.Clear();
               circle.figure.PointerPressed += (sender, e) =>
               {
                  InteractFigure(circle, false);
               };

               InteractFigure(circle, true);
               viewModel.Figures.Add(circle);
               break;
            case FigureType.Rectangle:
               if (_points.Count % 2 != 0) break;
               var rectangle = viewModel.FigureCreate(_points[^2], _points[^1]) as Rectangle;
               renderer.DrawRectangle(rectangle);
               _points.Clear();
               rectangle.figure.PointerPressed += (sender, e) =>
               {
                  InteractFigure(rectangle, false);
               };

               InteractFigure(rectangle, true);
               viewModel.Figures.Add(rectangle);
               break;
            case FigureType.Triangle:
               if (_points.Count % 2 != 0) break;
               var triangle = viewModel.FigureCreate(_points[^2], _points[^1]) as Triangle;
               renderer.DrawTriangle(triangle);
               _points.Clear();
               triangle.figure.PointerPressed += (sender, e) =>
               {
                  InteractFigure(triangle, false);
               };

               InteractFigure(triangle, true);
               viewModel.Figures.Add(triangle);
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
            switch (_selectedFigure)
            {
               case FigureType.Line:
                  Line line = new(start, end);
                  renderer.DrawLine(line);
                  tempFigure.Add(line);
                  break;
               case FigureType.Circle:
                  Circle circle = new(start, end, false);
                  renderer.DrawCircle(circle);
                  tempFigure.Add(circle);
                  break;
               case FigureType.Rectangle:
                  Rectangle rectangle = new(start, end);
                  renderer.DrawRectangle(rectangle);
                  tempFigure.Add(rectangle);
                  break;
               case FigureType.Triangle:
                  Triangle triangle = new(start, end);
                  renderer.DrawTriangle(triangle);
                  tempFigure.Add(triangle);
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
            if (figure.isSelected) { SelectFigure(line); }
         }
         else if (figure is Circle)
         {
            var circle = figure as Circle;
            if (circle.isPoint)
               renderer.DrawPoint(circle);
            else
               renderer.DrawCircle(circle);

            circle.figure.PointerPressed += (sender, e) =>
            {
               InteractFigure(circle, false);
            };
            if (figure.isSelected) { SelectFigure(circle); }
         }
         else if (figure is Rectangle)
         {
            var rectangle = figure as Rectangle;
            renderer.DrawRectangle(rectangle);

            rectangle.figure.PointerPressed += (sender, e) =>
            {
               InteractFigure(rectangle, false);
            };
            if (figure.isSelected) { SelectFigure(rectangle); }
         }
         else if (figure is Triangle)
         {
            var triangle = figure as Triangle;
            renderer.DrawTriangle(triangle);

            triangle.figure.PointerPressed += (sender, e) =>
            {
               InteractFigure(triangle, false);
            };
            if (figure.isSelected) { SelectFigure(triangle); }
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
         bool drawFlag = true;
         if ((figure is Circle circle))
         {
            if (circle.isPoint) drawFlag = false;
         }
         if (drawFlag)
            DrawPoints(figure);
         ChangeColor(figure, new SolidColorBrush(Colors.Blue));
         LineView.viewModel.mw = this;
         LineView.viewModel.currentFigure = figure;
         LineView.instance.FillInfo(figure);
         //renderer.ReDraw(figure);
      }

      private void DrawPoints(IFigure figure)
      {
         if (figure is Line line)
         {
            Circle point = new(line.start, new Point(0, 0), true)
            {
               ColorFigure = Colors.Red
            };
            renderer.DrawPoint(point);
            selectPointList.Add(point);
            point = new(line.end, new Point(0, 0), true)
            {
               ColorFigure = Colors.Red
            };
            renderer.DrawPoint(point);
            selectPointList.Add(point);
         }
         else if (figure is Circle circle)
         {
            Circle point = new(circle.center, new Point(0, 0), true)
            {
               ColorFigure = Colors.Red
            };
            renderer.DrawPoint(point);
            selectPointList.Add(point);
            point = new(circle.radPoint, new Point(0, 0), true)
            {
               ColorFigure = Colors.Red
            };
            renderer.DrawPoint(point);
            selectPointList.Add(point);
         }
         else if (figure is Rectangle rectangle)
         {
            Circle point = new(rectangle.TopLeft, new Point(0, 0), true)
            {
               ColorFigure = Colors.Red
            };
            renderer.DrawPoint(point);
            selectPointList.Add(point);
            point = new(rectangle.BottomRight, new Point(0, 0), true)
            {
               ColorFigure = Colors.Red
            };
            renderer.DrawPoint(point);
            selectPointList.Add(point);
         }
         else if (figure is Triangle triangle)
         {
            Circle point = new(triangle.topPoint, new Point(0, 0), true)
            {
               ColorFigure = Colors.Red
            };
            renderer.DrawPoint(point);
            selectPointList.Add(point);
            point = new(triangle.bottomPoint1, new Point(0, 0), true)
            {
               ColorFigure = Colors.Red
            };
            renderer.DrawPoint(point);
            selectPointList.Add(point);
         }
      }

      private async void OnSavePngClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
      {
         if (_canvas == null) return;

         UnselectFigure(viewModel.CurFigure);

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

      private async void OnSaveSvgClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
      {
         if (_canvas == null) return;

         UnselectFigure(viewModel.CurFigure);

         var saveFileDialog = new SaveFileDialog
         {
            Title = "Сохранить как SVG",
            Filters = new List<FileDialogFilter>
                  {
                     new FileDialogFilter { Name = "SVG Files", Extensions = new List<string> { "svg" } }
                  },
            DefaultExtension = "svg"
         };

         var filePath = await saveFileDialog.ShowAsync(this);
         if (string.IsNullOrEmpty(filePath))
            return;

         // Временно скрываем TextBlock с текущим режимом рисования
         bool selTextWasVisible = SelText.IsVisible;
         SelText.IsVisible = false;

         try
         {
            var success = await SvgExporter.ExportToSvg(_canvas, filePath);
            if (success)
            {
               // Сообщение об успехе
               Console.WriteLine("SVG сохранен успешно!");
            }
            else
            {
               // Сообщение об ошибке
               Console.WriteLine("Не удалось сохранить SVG.");
            }
         }
         finally
         {
            // Восстанавливаем видимость
            SelText.IsVisible = selTextWasVisible;
         }
      }
      // Поворот на 15° влево
      private void OnRotateLeftClick(object sender, RoutedEventArgs e)
      {
         if (viewModel.CurFigure is null) return;
         RotateFigure(viewModel.CurFigure, -15);
      }

      // Поворот на 15° вправо
      private void OnRotateRightClick(object sender, RoutedEventArgs e)
      {
         if (viewModel.CurFigure is null) return;
         RotateFigure(viewModel.CurFigure, 15);
      }

      // Уменьшение масштаба на 10%
      private void OnScaleDownClick(object sender, RoutedEventArgs e)
      {
         if (viewModel.CurFigure is null) return;
         ScaleFigure(viewModel.CurFigure, 0.9);
      }

      // Увеличение масштаба на 10%
      private void OnScaleUpClick(object sender, RoutedEventArgs e)
      {
         if (viewModel.CurFigure is null) return;
         ScaleFigure(viewModel.CurFigure, 1.1);
      }

      // Метод перемещения
      private void MoveFigure(IFigure figure, Point vector)
      {
         figure.Move(vector);
         ReDraw(figure);
         bool needControlPoint = true;
         if (figure is Circle circle)
            if (circle.isPoint)
               needControlPoint = false;
         if(needControlPoint)
            UpdateSelectionPoints();
      }

      // Методы перемещения
      private void OnMoveLeftClick(object sender, RoutedEventArgs e)
      {
         if (viewModel.CurFigure == null) return;
         MoveFigure(viewModel.CurFigure, new Point(-10, 0));
      }

      private void OnMoveRightClick(object sender, RoutedEventArgs e)
      {
         if (viewModel.CurFigure == null) return;
         MoveFigure(viewModel.CurFigure, new Point(10, 0));
      }

      private void OnMoveUpClick(object sender, RoutedEventArgs e)
      {
         if (viewModel.CurFigure == null) return;
         MoveFigure(viewModel.CurFigure, new Point(0, -10));
      }

      private void OnMoveDownClick(object sender, RoutedEventArgs e)
      {
         if (viewModel.CurFigure == null) return;
         MoveFigure(viewModel.CurFigure, new Point(0, 10));
      }

      private async void OnSaveStateClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
      {
         try
         {
            if (_canvas == null) return;

            var saveFileDialog = new SaveFileDialog
            {
               Title = "Save State",
               Filters = new List<FileDialogFilter>
               {
                  new FileDialogFilter { Name = "JSON Files", Extensions = new List<string> { "json" } }
               },
               DefaultExtension = "json"
            };

            var filePath = await saveFileDialog.ShowAsync(this);
            if (string.IsNullOrEmpty(filePath))
               return;
            var list = viewModel.Figures.ToList();
            for (int i = 0; i< viewModel.Figures.ToList().Count; i++)
            {
               if (viewModel.Figures.ToList()[i] == null) { viewModel.Figures.ToList().RemoveAt(i); i--; }
            }
            var state = new ProgramState
            {
               Figures = viewModel.Figures.ToList().Select(figure => figure.getFigureData()).ToList()
            };
            
            var json = JsonConvert.SerializeObject(state, Formatting.Indented);
            await File.WriteAllTextAsync(filePath, json);
         }
         catch (Exception ex)
         {
            Console.WriteLine($"Error saving state: {ex.Message}");
         }
      }

      private async void OnLoadStateClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
      {
         try
         {
            if (_canvas == null) return;

            var openFileDialog = new OpenFileDialog
            {
               Title = "Load State",
               Filters = new List<FileDialogFilter>
               {
                  new FileDialogFilter { Name = "JSON Files", Extensions = new List<string> { "json" } }
               }
            };

            var filePaths = await openFileDialog.ShowAsync(this);
            if (filePaths == null || filePaths.Length == 0)
               return;
            
            viewModel.ClearFigures();
            UnselectFigure(null);
            
            var json = await File.ReadAllTextAsync(filePaths[0]);
            var state = JsonConvert.DeserializeObject<ProgramState>(json);

            foreach (var figureData in state.Figures)
            {
               FigureType type = figureData.Type switch
               {
                  "Line" => FigureType.Line,
                  "Circle" => FigureType.Circle,
                  "Rectangle" => FigureType.Rectangle,
                  "Point" => FigureType.Point,
                  "Triangle" => FigureType.Triangle,
                  _ => FigureType.Edit
               };
               
               if (type == FigureType.Edit) throw new DataException($"Invalid figure type: {figureData.Type}");

               var p1 = figureData.Start.ToPoint();
               var p2 = figureData.End.ToPoint();
               var color =  figureData.Color;
               var thickness = figureData.StrokeThickness;
               FigureFabric figureFabric = new FigureFabric();
               
               IFigure figure =  figureFabric.CreateFromJson(p1, p2, type, color, thickness);
               
               ReDraw(figure);
               viewModel.Figures.Add(figure);
            }
            
         }
         catch (Exception ex)
         {
            Console.WriteLine($"Error saving state: {ex.Message}");
         }
      }

      public void ChangeColor(IFigure figure, Brush newColor)
      {
         if (figure is Circle circle)
         {
            if (circle.isPoint)
               circle.figure.Fill = newColor;
            else
               circle.figure.Stroke = newColor;
         }
         else if (figure is Line line)
            line.figure.Stroke = newColor;
         else if (figure is Rectangle rectangle)
            rectangle.figure.Stroke = newColor;
         else if (figure is Triangle triangle)
            triangle.figure.Stroke = newColor;
      }

      public void OnKeyDown(object sender, KeyEventArgs e)
      {

         base.OnKeyDown(e);

         // Удаление фигуры
         if (e.Key == Key.Delete)
         {
            viewModel.DeleteFigure();
            InteractFigure(viewModel.CurFigure, true);
         }

         // Удаление холста
         if (e.Key == Key.C && e.KeyModifiers == KeyModifiers.Control)
         {
            viewModel.ClearFigures();
            UnselectFigure(null);
         }

         if (e.KeyModifiers == KeyModifiers.None)
         {
            switch (e.Key)
            {
               case Key.R: // Поворот направо
                  RotateFigure(viewModel.CurFigure, 15);
                  break;
               case Key.L: // Поворот налево
                  RotateFigure(viewModel.CurFigure, -15);
                  break;
               case Key.B: // Увеличение
                  ScaleFigure(viewModel.CurFigure, 1.1);
                  break;
               case Key.S: // Уменьшение
                  ScaleFigure(viewModel.CurFigure, 0.9);
                  break;
            }
         }
         // Перемещение
         if (e.KeyModifiers == KeyModifiers.None || e.KeyModifiers == KeyModifiers.Alt)
         {
            switch (e.Key)
            {
               case Key.Left:
                  MoveFigure(viewModel.CurFigure, new Point(-10, 0));
                  break;
               case Key.Right:
                  MoveFigure(viewModel.CurFigure, new Point(10, 0));
                  break;
               case Key.Up:
                  MoveFigure(viewModel.CurFigure, new Point(0, -10));
                  break;
               case Key.Down:
                  MoveFigure(viewModel.CurFigure, new Point(0, 10));
                  break;
            }
         }
      }

      public void RotateFigure(IFigure figure, double angleDegrees)
      {
         if (figure == null) return;

         if (figure is Rectangle) return;

         // Конвертируем градусы в радианы
         double angleRadians = angleDegrees * Math.PI / 180;

         // Получаем центр фигуры и выполняем поворот
         Point center = figure.GetCenter();
         figure.Rotate(center, angleRadians);

         // Обновляем отображение
         ReDraw(figure);
         bool needControlPoint = true;
         if (figure is Circle circle)
            if (circle.isPoint)
               needControlPoint = false;
         if (needControlPoint)
            UpdateSelectionPoints();
      }

      public void ScaleFigure(IFigure figure, double factor)
      {
         if (figure == null) return;
         bool isPoint = false;
         if (figure is Circle circle)
            if (circle.isPoint)
               isPoint = true;
         if (isPoint) return;

         // Получаем центр фигуры и выполняем масштабирование
         Point center = figure.GetCenter();
         figure.Scale(center, factor);

         // Обновляем отображение
         ReDraw(figure);
         UpdateSelectionPoints();
      }

      private void UpdateSelectionPoints()
      {
         if (viewModel.CurFigure != null)
         {
            ClearPointList();
            DrawPoints(viewModel.CurFigure);
         }
      }
   }

}
