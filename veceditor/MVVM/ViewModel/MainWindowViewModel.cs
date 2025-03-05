using ReactiveUI;
using System.Reactive;
using System.Windows.Input;
using vecedidor.MVVM.ViewModel;
using System.Reactive.Linq;
using Avalonia.Threading;
using System;
using Avalonia.Controls.Documents;
using static System.Net.Mime.MediaTypeNames;

namespace veceditor.MVVM.ViewModel
{
    public partial class MainWindowViewModel : ViewModelBase
    {
      public ReactiveCommand<FigureType, Unit> SelectFigure { get; }

      public FigureType _figureType;

      public MainWindowViewModel()
      {
         _figureType = FigureType.Line;
         SelectFigure = ReactiveCommand.Create<FigureType>(Select);
         SelectFigure.ObserveOn(RxApp.MainThreadScheduler);
      }
      void Select(FigureType type)
      {
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
