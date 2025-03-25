using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using veceditor.MVVM.Model;
using veceditor.MVVM.ViewModel;

namespace veceditor.MVVM.View
{
   public partial class LineView : UserControl
   {
      public static LineViewModel viewModel;
      public static LineView instance;

      public LineView()
      {
         InitializeComponent();
         instance = this;
         viewModel = new LineViewModel();
         DataContext = viewModel;
         //viewModel.X1 = 10;
      }
      public void FillInfo(IFigure figure)
      {
         viewModel.notReDraw = true;
         if (figure is Circle circle)
         {
            viewModel.X1 = circle.Center.x;
            viewModel.Y1 = circle.Center.y;

            viewModel.X2 = circle.RadPoint.x;
            viewModel.Y2 = circle.RadPoint.y;

            viewModel.StrokeThickness = circle.strokeThickness;

            viewModel.Color_A = circle.ColorFigure.A;
            viewModel.Color_R = circle.ColorFigure.R;
            viewModel.Color_G = circle.ColorFigure.G;
            viewModel.Color_B = circle.ColorFigure.B;
         }
         else if (figure is Line line)
         {
            viewModel.X1 = line.start.x;
            viewModel.Y1 = line.start.y;

            viewModel.X2 = line.end.x;
            viewModel.Y2 = line.end.y;

            viewModel.StrokeThickness = line.strokeThickness;

            viewModel.Color_A = line.ColorFigure.A;
            viewModel.Color_R = line.ColorFigure.R;
            viewModel.Color_G = line.ColorFigure.G;
            viewModel.Color_B = line.ColorFigure.B;
         }
         viewModel.notReDraw = false;
      }
   }
}
