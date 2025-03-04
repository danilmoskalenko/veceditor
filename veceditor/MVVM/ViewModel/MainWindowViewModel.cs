using ReactiveUI;
using System.Reactive;
using System.Windows.Input;
using vecedidor.MVVM.ViewModel;
using System.Reactive.Linq;
namespace veceditor.MVVM.ViewModel
{
    public partial class MainWindowViewModel : ViewModelBase
    {
      public ReactiveCommand<FigureType, Unit> SelectLine { get; }
      public ReactiveCommand<FigureType, Unit> SelectCircle { get; }

      public FigureType _figureType;
      public MainWindowViewModel()
      {
         _figureType = FigureType.Line;
         SelectLine = ReactiveCommand.Create<FigureType>(SelectFigure);
         SelectCircle = ReactiveCommand.Create<FigureType>(SelectFigure);
      }
      void SelectFigure(FigureType type)
      {
         switch (type)
         {
            case FigureType.Line:
               _figureType = FigureType.Line;
               break;
            case FigureType.Circle:
               _figureType = FigureType.Circle;
               break;
         }
      }
   }
}
