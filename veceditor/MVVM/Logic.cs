using Avalonia.Controls;
using Avalonia.LogicalTree;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using veceditor.MVVM.Model;

namespace veceditor.MVVM
{
   public class Logic : ILogic
   {
      Canvas canvas;
      public ObservableCollection<IFigure> Figures { get; set; } = new ObservableCollection<IFigure>();
      public Logic(Canvas canvas)
      {
         this.canvas = canvas;
      }

      public void AddFigure(IFigure figure)
      {
         Figures.Add(figure);
      }

      public void RemoveFigure(IFigure figure)
      {
         Figures.Remove(figure);
      }
      public void ClearFigures()
      {
         Figures.Clear();
      }
   }
}
