using ReactiveUI;
using System.Reactive;
using System.Windows.Input;
using vecedidor.MVVM.ViewModel;
using System.Reactive.Linq;
using Avalonia.Threading;
using System;
using Avalonia.Controls.Documents;
using static System.Net.Mime.MediaTypeNames;
using Avalonia.Controls;
using System.Collections.Generic;
using Point = veceditor.MVVM.Model.Point;
using veceditor.MVVM.Model;
using Avalonia.Rendering;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using DynamicData;
using ReactiveUI;
using System.Linq;
using veceditor.MVVM;

namespace veceditor.MVVM.ViewModel
{
    public partial class MainWindowViewModel : ViewModelBase
    {
      public ReactiveCommand<FigureType, Unit> SelectFigure { get; }
      public TextBlock SelText;
      public FigureType _figureType;
      public List<Point> _points = new();
      public List<IFigure> tempFigure = new();
      public DrawingRenderer? renderer;
      public ILogic? Logic { get; set; }

      public MainWindowViewModel()
      {
         _figureType = FigureType.Line;
         SelText = new TextBlock();
         SelectFigure = ReactiveCommand.Create<FigureType>(Select);
         SelectFigure.ObserveOn(RxApp.MainThreadScheduler);
      }

      public async Task SaveState(string filePath)
      {
         try
         {
            var state = new ProgramState
            {
               Figures = Logic.Figures.ToList().Select(figure =>
               {
                  var figureData = new FigureData
                  {
                     Type = figure.GetType().Name
                  };

                  switch (figure)
                  {
                     case Line line:
                        figureData.Start = new PointData(line.start.x, line.start.y);
                        figureData.End = new PointData(line.end.x, line.end.y);
                        break;
                     case Circle circle:
                        figureData.Start = new PointData(circle.center.x, circle.center.y);
                        figureData.End = new PointData(circle.radPoint.x, circle.radPoint.y);
                        break;
                     case Rectangle rectangle:
                        figureData.Start = new PointData(rectangle.topLeft.x, rectangle.topLeft.y);
                        figureData.End = new PointData(rectangle.bottomRight.x, rectangle.bottomRight.y);
                        break;
                     case Triangle triangle:
                        figureData.Points = new List<PointData>
                        {
                           new PointData(triangle.point1.x, triangle.point1.y),
                           new PointData(triangle.point2.x, triangle.point2.y),
                           new PointData(triangle.point3.x, triangle.point3.y)
                        };
                        break;
                  }

                  return figureData;
               }).ToList(),
               CurrentFigureType = _figureType
            };

            var json = JsonConvert.SerializeObject(state, Formatting.Indented);
            await File.WriteAllTextAsync(filePath, json);
         }
         catch (Exception ex)
         {
            Console.WriteLine($"Error saving state: {ex.Message}");
         }
      }

      public async Task LoadState(string filePath)
      {
         try
         {
            var json = await File.ReadAllTextAsync(filePath);
            var state = JsonConvert.DeserializeObject<ProgramState>(json);

            // Clear existing figures
            var figures = Logic.Figures.ToList();
            foreach (var figure in figures)
            {
               renderer.Erase(figure);
               Logic.RemoveFigure(figure);
            }

            // Restore figures
            foreach (var figureData in state.Figures)
            {
               IFigure figure = null;

               switch (figureData.Type)
               {
                  case "Line":
                     figure = new Line(
                        new Point(figureData.Start.X, figureData.Start.Y),
                        new Point(figureData.End.X, figureData.End.Y)
                     );
                     break;

                  case "Circle":
                     figure = new Circle(
                        new Point(figureData.Start.X, figureData.Start.Y),
                        new Point(figureData.End.X, figureData.End.Y)
                     );
                     break;

                  case "Rectangle":
                     figure = new Rectangle(
                        new Point(figureData.Start.X, figureData.Start.Y),
                        new Point(figureData.End.X, figureData.End.Y)
                     );
                     break;

                  case "Triangle":
                     if (figureData.Points.Count == 3)
                     {
                        figure = new Triangle(
                           new Point(figureData.Points[0].X, figureData.Points[0].Y),
                           new Point(figureData.Points[1].X, figureData.Points[1].Y),
                           new Point(figureData.Points[2].X, figureData.Points[2].Y)
                        );
                     }
                     break;
               }

               if (figure != null)
               {
                  Logic.AddFigure(figure);
                  switch (figure)
                  {
                     case Line line:
                        renderer.DrawLine(line);
                        break;
                     case Circle circle:
                        renderer.DrawCircle(circle);
                        break;
                     case Rectangle rectangle:
                        renderer.DrawRectangle(rectangle);
                        break;
                     case Triangle triangle:
                        renderer.DrawTriangle(triangle);
                        break;
                  }
               }
            }

            // Restore current figure type
            _figureType = state.CurrentFigureType;
            SelText.Text = _figureType.ToString();
         }
         catch (Exception ex)
         {
            Console.WriteLine($"Error loading state: {ex.Message}");
         }
      }

      public ObservableCollection<FigureType> FigureTypes { get; } = new()
    {
        FigureType.Point, FigureType.Line, FigureType.Circle, FigureType.Rectangle, FigureType.Triangle
    };
      public FigureType _SelectedFigure
      {
         get => _figureType;
         set
         {
            if (_figureType != value)
            {
               this.RaiseAndSetIfChanged(ref _figureType, value);
               Select(value); // Теперь вызываем Select после обновления
            }
         }
      }

      void Select(FigureType type)
      {
         _points.Clear();
         while (tempFigure.Count > 0)
         {
            renderer?.Erase(tempFigure[0]); // Защита от `null`
            tempFigure.RemoveAt(0);
         }
         SelText.Text = $"{type}";

         _figureType = type; // Теперь свойство корректно обновляется
      }

   }
}
