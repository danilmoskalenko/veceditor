using Avalonia.Controls;
using ReactiveUI;
using veceditor.MVVM.ViewModel;

namespace veceditor.MVVM.View
{
   public partial class LineView : UserControl
   {
      public LineView()
      {
         InitializeComponent();
         DataContext = new LineViewModel(); // Привязываем ViewModel
      }
   }
}
