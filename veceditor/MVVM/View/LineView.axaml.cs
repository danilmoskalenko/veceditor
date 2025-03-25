using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
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
            viewModel.X1 = circle.center.x;
            viewModel.Y1 = circle.center.y;

            viewModel.X2 = circle.radPoint.x;
            viewModel.Y2 = circle.radPoint.y;

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
         else if (figure is Rectangle rectangle)
         {
            viewModel.X1 = rectangle.topLeft.x;
            viewModel.Y1 = rectangle.topLeft.y;

            viewModel.X2 = rectangle.bottomRight.x;
            viewModel.Y2 = rectangle.bottomRight.y;

            viewModel.StrokeThickness = rectangle.strokeThickness;

            viewModel.Color_A = rectangle.ColorFigure.A;
            viewModel.Color_R = rectangle.ColorFigure.R;
            viewModel.Color_G = rectangle.ColorFigure.G;
            viewModel.Color_B = rectangle.ColorFigure.B;
         }
         else if (figure is Triangle triangle)
         {
            viewModel.X1 = triangle.topPoint.x;
            viewModel.Y1 = triangle.topPoint.y;

            viewModel.X2 = triangle.bottomPoint1.x;
            viewModel.Y2 = triangle.bottomPoint1.y;

            viewModel.StrokeThickness = triangle.strokeThickness;

            viewModel.Color_A = triangle.ColorFigure.A;
            viewModel.Color_R = triangle.ColorFigure.R;
            viewModel.Color_G = triangle.ColorFigure.G;
            viewModel.Color_B = triangle.ColorFigure.B;
         }
         viewModel.notReDraw = false;
      }
   }
}
