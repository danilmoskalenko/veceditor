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
using System.Collections.Specialized;
using System.Linq;

namespace veceditor.MVVM.ViewModel
{
   public partial class MainWindowViewModel : ViewModelBase
   {
      int id_changes = -1;
      // 0 - добавление фигуры
      // 1 - удаление фигуры
      public ReactiveCommand<FigureType, Unit> SelectFigure { get; }
      //Текущая фигура 
      private IFigure curFigure;
      public IFigure CurFigure
    {
        get => curFigure;
        set => this.RaiseAndSetIfChanged(ref curFigure, value);
    }
      private bool _isEditMode;
      public ICommand ForwardCommand { get; }
      public ICommand BackwardCommand { get; }
      public ICommand EditModeCommand { get; }

      // Коллекция типов фигур
      public ObservableCollection<FigureType> FigureTypes { get; } = new()
        {
            FigureType.Edit, FigureType.Point, FigureType.Line, FigureType.Circle, FigureType.Rectangle, FigureType.Triangle
        };
      
      public ObservableCollection<IFigure> Figures { get; set;}

      // Выбранная фигура (в Меню выбора)
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

      // Текущая фигура и её параметры (в меню выбора)
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

         Figures = new ObservableCollection<IFigure>();
         //Инициализация всех подписок
         Subscribes();
         
      }
      private void Subscribes()
      {
         this.WhenAnyValue(x => x._SelectedFigure)
            .Subscribe(type => Select(type));
         this.WhenAnyValue(x => x.Figures.Count)
            .Subscribe(_ => UpdateCurFigures());

         Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                 h => Figures.CollectionChanged += h,
                h => Figures.CollectionChanged -= h
             )
             .Where(e => e.EventArgs.Action == NotifyCollectionChangedAction.Remove) // Фильтруем только удаление
             .Select(e => e.EventArgs.OldItems[0] as IFigure)
             .Subscribe(figure => 
            {
               FigureRemoved?.Invoke(this, figure);
               UpdateCurFigures();
            });

           
      }
      public event EventHandler<IFigure> FigureRemoved;
       //При удалении/добавлении фигур указатель на текущую фигуру смещается на последний элемент
      public void UpdateCurFigures()
      {
         curFigure = Figures.LastOrDefault();
      }
      void Select(FigureType type)
      {
         if (type == FigureType.Edit && !_isEditMode ||
             type != FigureType.Edit && _isEditMode)
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
            _SelectedFigure = FigureType.Edit;
         }
         else
         {
            _SelectedFigure = FigureType.Point;
         }
      }
      //Добавление фигур
      public void FigureCreate(IFigure figure)
      {
         Figures.Add(figure);
      }
      //Удаление фигур
      public void DeleteFigure()
      {
         Figures.Remove(curFigure);
      }
   }
}
