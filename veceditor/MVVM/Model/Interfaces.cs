using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media;

namespace veceditor.MVVM.Model
{
   public class Point
   {
      public double x, y;

      // Вычисляет расстояние между текущей точкой и другой точкой
      public double DistanceTo(Point other)
      {
         double dx = other.x - x;
         double dy = other.y - y;
         return Math.Sqrt(dx * dx + dy * dy);
      }

      // Перемещение точки на заданные расстояния по осям
      public void Translate(double dx, double dy)
      {
         x += dx;
         y += dy;
      }

      // Проверка на равенство точек
      public override bool Equals(object obj)
      {
         if (obj is Point other)
         {
            return Math.Abs(x - other.x) < double.Epsilon && Math.Abs(y - other.y) < double.Epsilon;
         }
         return false;
      }
   }
   public interface IFigureGraphicProperties
   {
      // Цвет заливки фигуры
      Color SolidColor { get; }

      // Цвет границы
      Color BorderColor { get; }

      // Толщина границы
      float BorderWidth { get; }

      // Стиль границы
      DashStyle BorderDashStyle { get; }
   }
   public class FigureGraphicProperties : IFigureGraphicProperties
   {
      public Color SolidColor { get; set; }
      public Color BorderColor { get; set; }
      public float BorderWidth { get; set; }
      public DashStyle BorderDashStyle { get; set; }

      public FigureGraphicProperties(Color solidColor, Color borderColor, float borderWidth, DashStyle borderDashStyle)
      {
         SolidColor = solidColor;
         BorderColor = borderColor;
         BorderWidth = borderWidth;
         BorderDashStyle = borderDashStyle;
      }
   }
   public interface IGraphicInterface
   {
      void DrawCircle(Point center, double radius);
      void DrawLine(Point start, Point end);
      void DrawRectangle(Point topLeft, double width, double height);
      void DrawPolygon(IEnumerable<Point> points);
   }
   public interface IDrawableFigure
   {
      void Draw(IGraphicInterface graphic);
   }
   public interface IFigure
   {
      bool IsInternal(Point p, double eps);
      IFigure Intersect(IFigure other);
      IFigure Union(IFigure other);
      IFigure Subtract(IFigure other);
      void Move(Point vector);
      void Rotate(Point Center, double angle);
      void Scale(double x, double y);
      void Scale(Point Center, double rad);
      void Reflection(Point ax1, Point ax2);
      IEnumerable<IDrawableFigure> GetDrawFigures();
      bool IsClosed { get; }
      string Name { get; set; }
   }
   public abstract class BaseFigure : IFigure, IDrawableFigure
   {
      protected IFigureGraphicProperties graphics;
      public string Name { get; set; }

      public BaseFigure(string name, IFigureGraphicProperties graphics)
      {
         Name = name;
         this.graphics = graphics;
      }

      public abstract bool IsInternal(Point p, double eps);
      public abstract IFigure Intersect(IFigure other);
      public abstract IFigure Union(IFigure other);
      public abstract IFigure Subtract(IFigure other);
      public abstract void Move(Point vector);
      public abstract void Rotate(Point center, double angle);
      public abstract void Scale(double x, double y);
      public abstract void Scale(Point center, double rad);
      public abstract void Reflection(Point ax1, Point ax2);
      public abstract bool IsClosed { get; }
      public abstract void Draw(IGraphicInterface graphic);

      public virtual IEnumerable<IDrawableFigure> GetDrawFigures()
      {
         return new List<IDrawableFigure> { this };
      }
   }
}