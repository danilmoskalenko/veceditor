using ReactiveUI;
using System;
using veceditor.MVVM.Model;

namespace veceditor.MVVM.ViewModel
{
   public class LineViewModel : ReactiveObject
   {
      private double _x1, _y1, _x2, _y2, _strokeThickness;
      private byte _color_a, _color_r, _color_g, _color_b;
      private bool _isLineVisible;

      public IFigure currentFigure;
      public MainWindow? mw;
      public bool notReDraw = false;

      public double X1
      {
         get => _x1;
         set
         {
            if (value != _x1)
            {
               this.RaiseAndSetIfChanged(ref _x1, value);
               OnChanged();
            }
         }
      }

      public double Y1
      {
         get => _y1;
         set
         {
            if (value != _y1)
            {
               this.RaiseAndSetIfChanged(ref _y1, value);
               OnChanged();
            }
         }
      }

      public double X2
      {
         get => _x2;
         set
         {
            if (value != _x2)
            {
               this.RaiseAndSetIfChanged(ref _x2, value);
               OnChanged();
            }
         }
      }

      public double Y2
      {
         get => _y2;
         set
         {
            if (value != _y2)
            {
               this.RaiseAndSetIfChanged(ref _y2, value);
               OnChanged();
            }
         }
      }

      public double StrokeThickness
      {
         get => _strokeThickness;
         set
         {
            if (value != _strokeThickness)
            {
               this.RaiseAndSetIfChanged(ref _strokeThickness, value);
               OnChanged();
            }
         }
      }
      public byte Color_A
      {
         get => _color_a;
         set
         {
            if (value != _color_a)
            {
               this.RaiseAndSetIfChanged(ref _color_a, value);
               OnChanged();
            }
         }
      }

      public byte Color_R
      {
         get => _color_r;
         set
         {
            if (value != _color_r)
            {
               this.RaiseAndSetIfChanged(ref _color_r, value);
               OnChanged();
            }
         }
      }

      public byte Color_G
      {
         get => _color_g;
         set
         {
            if (value != _color_g)
            {
               this.RaiseAndSetIfChanged(ref _color_g, value);
               OnChanged();
            }
         }
      }

      public byte Color_B
      {
         get => _color_b;
         set
         {
            if (value != _color_b)
            {
               this.RaiseAndSetIfChanged(ref _color_b, value);
               OnChanged();
            }
         }
      }

      public bool IsLineVisible
      {
         get => _isLineVisible;
         set
         {
            if (value != _isLineVisible)
            {
               this.RaiseAndSetIfChanged(ref _isLineVisible, value);
               OnChanged();
            }
         }
      }

      private void OnChanged()
      {
         if (notReDraw) return;
         if (currentFigure is Circle circle)
         {
            circle.Center = new Point(X1,Y1);
            circle.RadPoint = new Point(X2,Y2);
            
            circle.strokeThickness = StrokeThickness;
            circle.ColorFigure = new Avalonia.Media.Color(Color_A, Color_R, Color_G, Color_B);

            mw.ReDraw(circle);
         }
         else if (currentFigure is Line line)
         {
            line.start = new Point(X1, Y1);
            line.end = new Point(X2, Y2);

            line.strokeThickness = StrokeThickness;
            line.ColorFigure = new Avalonia.Media.Color(Color_A, Color_R, Color_G, Color_B);

            mw.ReDraw(line);
         }
      }
   }
}
