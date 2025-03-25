using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veceditor.MVVM.View
{
   public class FigureTypeToImageConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         if (value is FigureType figureType)
         {
            string imagePath = $"avares://veceditor/Icons/{figureType}.png";
            return new Bitmap(AssetLoader.Open(new System.Uri(imagePath)));
         }
         return null;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         throw new NotImplementedException();
      }
   }
}