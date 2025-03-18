using ReactiveUI;

namespace veceditor.MVVM.ViewModel
{
   public class LineViewModel : ReactiveObject
   {
      private double _x1, _y1, _x2, _y2, _strokeThickness;
      private string _color;
      private bool _isLineVisible;

      public double X1 { get => _x1; set => this.RaiseAndSetIfChanged(ref _x1, value); }
      public double Y1 { get => _y1; set => this.RaiseAndSetIfChanged(ref _y1, value); }
      public double X2 { get => _x2; set => this.RaiseAndSetIfChanged(ref _x2, value); }
      public double Y2 { get => _y2; set => this.RaiseAndSetIfChanged(ref _y2, value); }
      public double StrokeThickness { get => _strokeThickness; set => this.RaiseAndSetIfChanged(ref _strokeThickness, value); }
      public string Color { get => _color; set => this.RaiseAndSetIfChanged(ref _color, value); }

      public bool IsLineVisible
      {
         get => _isLineVisible;
         set => this.RaiseAndSetIfChanged(ref _isLineVisible, value);
      }
   }
}
