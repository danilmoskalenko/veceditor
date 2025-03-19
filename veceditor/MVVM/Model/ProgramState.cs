using System.Collections.Generic;
using veceditor.MVVM;

namespace veceditor.MVVM.Model
{
    public class ProgramState
    {
        public List<FigureData> Figures { get; set; } = new();
        public FigureType CurrentFigureType { get; set; }
    }

    public class FigureData
    {
        public string Type { get; set; }
        public PointData Start { get; set; }
        public PointData End { get; set; }
        public double? Radius { get; set; }
        public List<PointData> Points { get; set; } = new(); // For triangle and rectangle
        public double? Width { get; set; }  // For point
        public double? Height { get; set; } // For point
    }

    public class PointData
    {
        public double X { get; set; }
        public double Y { get; set; }

        public PointData(double x, double y)
        {
            X = x;
            Y = y;
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
} 