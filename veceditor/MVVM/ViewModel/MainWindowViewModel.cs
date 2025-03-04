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
      public ReactiveCommand<Unit, Unit> SelectFigure { get; }

      public FigureType _figureType;

      public MainWindowViewModel()
      {
         _figureType = FigureType.Line;
         SelectFigure = ReactiveCommand.Create(Select);
         SelectFigure.ObserveOn(RxApp.MainThreadScheduler);
      }
      void Select()
      {
               //_figureType = FigureType.Line;
               _figureType = FigureType.Circle;
      }
   }
}
