using System;
using System.Collections.Generic;

namespace veceditor.MVVM.Model
{
   public class Circle : BaseFigure
   {
      public Point Center { get; set; }
      public double Radius { get; set; }

      // Конструктор
      public Circle(string name, IFigureGraphicProperties graphics, Point center, double radius)
          : base(name, graphics)
      {
         Center = center;
         Radius = radius;
      }

      // Проверка, находится ли точка внутри круга
      public override bool IsInternal(Point p, double eps)
      {
         return Center.DistanceTo(p) <= Radius + eps;
      }

      // Пересечение с другой фигурой
      public override IFigure Intersect(IFigure other)
      {
         if (other is Circle otherCircle)
         {
            double distance = Center.DistanceTo(otherCircle.Center);
            double sumOfRadii = Radius + otherCircle.Radius;
            double diffOfRadii = Math.Abs(Radius - otherCircle.Radius);

            // Если круги не пересекаются
            if (distance > sumOfRadii)
               return null;

            // Если один круг полностью внутри другого
            if (distance <= diffOfRadii)
               return Radius < otherCircle.Radius ? this : otherCircle;

            // Вычисляем пересечение (пока возвращаем null, так как это сложная операция)
            return null;
         }
         return null;
      }

      // Объединение с другой фигурой
      public override IFigure Union(IFigure other)
      {
         if (other is Circle otherCircle)
         {
            double distance = Center.DistanceTo(otherCircle.Center);
            double sumOfRadii = Radius + otherCircle.Radius;

            // Если круги пересекаются или касаются
            if (distance <= sumOfRadii)
            {
               // Возвращаем новый круг, охватывающий оба круга
               double newRadius = (distance + sumOfRadii) / 2;
               Point newCenter = new Point
               {
                  x = (Center.x + otherCircle.Center.x) / 2,
                  y = (Center.y + otherCircle.Center.y) / 2
               };
               return new Circle("Union Circle", graphics, newCenter, newRadius);
            }

            // Если круги не пересекаются, возвращаем составную фигуру
            //return new CompositeFigure("Composite", graphics, new List<IFigure> { this, otherCircle });
         }
         return null;
      }

      // Вычитание другой фигуры
      public override IFigure Subtract(IFigure other)
      {
         if (other is Circle otherCircle)
         {
            double distance = Center.DistanceTo(otherCircle.Center);
            double diffOfRadii = Math.Abs(Radius - otherCircle.Radius);

            // Если другой круг полностью внутри текущего
            if (distance + otherCircle.Radius <= Radius)
            {
               // Возвращаем кольцо (пока возвращаем null, так как это сложная операция)
               return null;
            }

            // Если круги пересекаются
            if (distance <= Radius + otherCircle.Radius)
            {
               // Возвращаем составную фигуру (пока возвращаем null)
               return null;
            }

            // Если круги не пересекаются, возвращаем текущий круг
            return this;
         }
         return null;
      }

      // Перемещение круга
      public override void Move(Point vector)
      {
         Center.Translate(vector.x, vector.y);
      }

      // Поворот круга вокруг центра
      public override void Rotate(Point center, double angle)
      {
         double radians = angle * Math.PI / 180;
         double cos = Math.Cos(radians);
         double sin = Math.Sin(radians);

         double dx = Center.x - center.x;
         double dy = Center.y - center.y;

         Center.x = center.x + (dx * cos - dy * sin);
         Center.y = center.y + (dx * sin + dy * cos);
      }

      // Масштабирование круга
      public override void Scale(double x, double y)
      {
         Radius *= x; // Масштабируем радиус
      }

      // Масштабирование круга относительно центра
      public override void Scale(Point center, double rad)
      {
         double scaleFactor = rad / Radius;
         Radius = rad;
         Center.x = center.x + (Center.x - center.x) * scaleFactor;
         Center.y = center.y + (Center.y - center.y) * scaleFactor;
      }

      // Отражение круга
      public override void Reflection(Point ax1, Point ax2)
      {
         // Вычисляем коэффициенты уравнения прямой ax1-ax2
         double A = ax2.y - ax1.y;
         double B = ax1.x - ax2.x;
         double C = ax2.x * ax1.y - ax1.x * ax2.y;

         // Вычисляем отражение центра круга
         double denominator = A * A + B * B;
         double x = Center.x;
         double y = Center.y;

         Center.x = x - 2 * A * (A * x + B * y + C) / denominator;
         Center.y = y - 2 * B * (A * x + B * y + C) / denominator;
      }

      // Отрисовка круга
      public override void Draw(IGraphicInterface graphic)
      {
         graphic.DrawCircle(Center, Radius);
      }

      // Круг является замкнутой фигурой
      public override bool IsClosed => true;
   }
}