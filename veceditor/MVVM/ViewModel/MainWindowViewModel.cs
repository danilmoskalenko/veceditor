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

      public MainWindowViewModel()
      {
         _figureType = FigureType.Line;
         SelText = new TextBlock();
         SelectFigure = ReactiveCommand.Create<FigureType>(Select);
         SelectFigure.ObserveOn(RxApp.MainThreadScheduler);
      }
      void Select(FigureType type)
      {
         _points.Clear();
         while(tempFigure.Count > 0)
         {
           renderer.Erase(tempFigure[0]); tempFigure.RemoveAt(0);
         }
         SelText.Text = $"{type}";
         switch (type)
         {
            case FigureType.Point:
               _figureType = type;
               break;
            case FigureType.Line:
               _figureType = type;
               break;
            case FigureType.Circle:
               _figureType = type;
               break;
            default:
               break;
         }
      }
   }
}
