using ReactiveUI;
using System.Reactive;
using System.Windows.Input;
using System.Reactive.Linq;
using Avalonia.Threading;
using System;
using Avalonia.Controls;
using System.Collections.Generic;
using Point = veceditor.MVVM.Model.Point;
using veceditor.MVVM.Model;
using Avalonia.Rendering;
using System.Collections.ObjectModel;
using vecedidor.MVVM.ViewModel;

namespace veceditor.MVVM.ViewModel
{
   public partial class MainWindowViewModel : ViewModelBase
   {
      public ReactiveCommand<FigureType, Unit> SelectFigure { get; }
      public TextBlock SelText;
      public List<Point> _points = new();
      public List<IFigure> tempFigure = new();
      public DrawingRenderer? renderer;

      private bool _isEditMode;
      public ICommand ForwardCommand { get; }
      public ICommand BackwardCommand { get; }
      public ICommand EditModeCommand { get; }

      // Коллекция типов фигур
      public ObservableCollection<FigureType> FigureTypes { get; } = new()
        {
            FigureType.None, FigureType.Point, FigureType.Line, FigureType.Circle, FigureType.Rectangle, FigureType.Triangle
        };

      // Выбранная фигура
      private FigureType _selectedFigure;
      public FigureType _SelectedFigure
      {
         get => _selectedFigure;
         set
         {
            if (_selectedFigure != value)
            {
               this.RaiseAndSetIfChanged(ref _selectedFigure, value);
               Select(value);
            }
         }
      }

      // Текущая фигура и её параметры
      private object? _currentFigureSettings;
      public object? CurrentFigureSettings
      {
         get => _currentFigureSettings;
         set => this.RaiseAndSetIfChanged(ref _currentFigureSettings, value);
      }

      public MainWindowViewModel()
      {
         _selectedFigure = FigureType.Line;
         SelText = new TextBlock();
         SelectFigure = ReactiveCommand.Create<FigureType>(Select);
         SelectFigure.ObserveOn(RxApp.MainThreadScheduler);

         ForwardCommand = ReactiveCommand.Create(ForwardAction);
         BackwardCommand = ReactiveCommand.Create(BackwardAction);
         EditModeCommand = ReactiveCommand.Create(ToggleEditMode);
      }

      void Select(FigureType type)
      {
         if (type == FigureType.None && !_isEditMode ||
             type != FigureType.None && _isEditMode)
         {
            ToggleEditMode();
         }

         _points.Clear();
         while (tempFigure.Count > 0)
         {
            renderer?.Erase(tempFigure[0]);
            tempFigure.RemoveAt(0);
         }

         SelText.Text = $"{type}";
         _selectedFigure = type;

         // Выбор соответствующих параметров
         switch (type)
         {
            case FigureType.Line:
               CurrentFigureSettings = new LineViewModel();
               break;
            /*case FigureType.Rectangle:
               CurrentFigureSettings = new RectangleViewModel();
               break;
            case FigureType.Circle:
               CurrentFigureSettings = new CircleViewModel();
               break;
            case FigureType.Triangle:
               CurrentFigureSettings = new TriangleViewModel();
               break;*/
            default:
               CurrentFigureSettings = null;
               break;
         }
      }

      private void ForwardAction()
      {
         // Логика для "Вперёд" (Ctrl+R)
      }

      private void BackwardAction()
      {
         // Логика для "Назад" (Ctrl+Z)
      }

      private void ToggleEditMode()
      {
         _isEditMode = !_isEditMode;
         if (_isEditMode)
         {
            _SelectedFigure = FigureType.None;
         }
         else
         {
            _SelectedFigure = FigureType.Point;
         }
      }
   }
}
