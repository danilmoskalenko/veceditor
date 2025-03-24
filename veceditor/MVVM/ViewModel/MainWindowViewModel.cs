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
      public FigureType _selectedFigure;
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
         SelectFigure = ReactiveCommand.Create<FigureType>(Select);

         ForwardCommand = ReactiveCommand.Create(ForwardAction);
         BackwardCommand = ReactiveCommand.Create(BackwardAction);
         EditModeCommand = ReactiveCommand.Create(ToggleEditMode);

         Subscribes();
         
      }
      private void Subscribes()
      {
         this.WhenAnyValue(x => x._SelectedFigure)
            .Subscribe(type => Select(type));
      }

      void Select(FigureType type)
      {
         if (type == FigureType.None && !_isEditMode ||
             type != FigureType.None && _isEditMode)
         {
            ToggleEditMode();
         }
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

      public FigureType GetTypeSelectedFigure()
      {
         return _selectedFigure;
      }
   }
}
