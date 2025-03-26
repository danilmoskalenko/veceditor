using System.Collections.Generic;
using System.Text.Json.Serialization;
using veceditor.MVVM;
using Newtonsoft.Json;

namespace veceditor.MVVM.Model;

public class PointData
{
   public double X { get; set; }
   public double Y { get; set; }

   [Newtonsoft.Json.JsonConstructor]
   public PointData(double x, double y)
   {
      X = x;
      Y = y;
   }

   public PointData(Point p)
   {
      X = p.x;
      Y = p.y;
   }

   public static PointData FromPoint(Point point)
   {
      return new PointData(point.x, point.y);
   }

   public Point ToPoint()
   {
      return new Point(X, Y);
   }
   
}

public class FigureData
{
   public string Type { get; set; }
   public PointData Start { get; set; }
   public PointData End { get; set; }
   public Avalonia.Media.Color Color { get; set; }
   public double StrokeThickness { get; set; }
}

public class ProgramState
{
   public List<FigureData> Figures { get; set; } = new();
}