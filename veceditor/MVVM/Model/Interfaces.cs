﻿using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veceditor.MVVM.Model
{
    public class Point
    {
        public double x, y;
        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }
    public interface IFigureGraphicProperties
    {
        Color SolidColor { get; }
        Color BorderColor { get; }

    }
    public interface IGraphicInterface
    {
        void DrawCircle(Point Center, double rad);
        void DrawLine(Line line);
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
        void RemoveFigureFromCanvas(Canvas _canvas);
   }
    public interface ILogic
    {
      ObservableCollection<IFigure> Figures { get; }
      void AddFigure(IFigure figure);
      void RemoveFigure(IFigure figure);
      void ClearFigures();
      public ObservableCollection<IFigure> GetFigures();


   }
}