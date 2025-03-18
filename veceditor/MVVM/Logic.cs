using Avalonia.Controls;
using Avalonia.LogicalTree;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using veceditor.MVVM.Model;

namespace veceditor.MVVM
{
   public class Logic : ILogic
   {
      Canvas canvas;
      // Хранилище фигур
      public ObservableCollection<IFigure> Figures { get; set; } = new ObservableCollection<IFigure>();
      public Logic(Canvas canvas)
      {
         this.canvas = canvas;
      }
      // Добавление фигур
      public void AddFigure(IFigure figure)
      {
         Figures.Add(figure);
      }
      // Удаление фигур
      public void RemoveFigure(IFigure figure)
      {
         figure.RemoveFigureFromCanvas(canvas);
         Figures.Remove(figure);
      }
      // Очистка фигур
      public void ClearFigures()
      {
         foreach (var elem in Figures)
         {
            elem.RemoveFigureFromCanvas(canvas);
         }
         Figures.Clear();
      }
      public ObservableCollection<IFigure> GetFigures()
      {
         return Figures;
      }
   }
}