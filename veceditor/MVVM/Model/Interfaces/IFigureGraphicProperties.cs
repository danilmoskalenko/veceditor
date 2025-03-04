using System.Drawing;

namespace veceditor.MVVM.Model;

public interface IFigureGraphicProperties
{
   Color SolidColor { get; set; }
   Color BorderColor { get; set; }
   double BorderThickness { get; set; }
}