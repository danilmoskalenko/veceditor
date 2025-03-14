using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using DynamicData;
using DynamicData.Alias;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using vecedidor.MVVM.ViewModel;
using veceditor.MVVM.Model;
using MyPoint = veceditor.MVVM.Model.Point; // Псевдоним для Point из interfaces

namespace veceditor
{
   public partial class MainWindow : Window
   {
      ViewModelBase vm = new();
      ReadOnlyObservableCollection<string> figures;
      public ReadOnlyObservableCollection<string> Figures => figures;

      public MainWindow()
      {
         InitializeComponent();

         // Инициализация ViewModel и привязка данных
         vm.Figures.Connect().Select(f => f.Name).SortAndBind(out figures);
         vm.Figures.CountChanged.Subscribe(c => { });

         // Вызов метода отрисовки
         Draw();
      }

      void Draw()
      {
         DrawingGroup dGroup = new DrawingGroup();


         using (DrawingContext dc = dGroup.Open())
         {

            var graphicInterface = new AvaloniaGraphicInterface(dc);


            var circle = new Circle(
                name: "My Circle",
                graphics: new FigureGraphicProperties(
                    solidColor: Colors.Blue,
                    borderColor: Colors.Black,
                    borderWidth: 2.0f,
                    borderDashStyle: new DashStyle(new double[] { 2, 2 }, 0) // Пунктирная линия
                ),
                center: new MyPoint { x = 100, y = 100 },
                radius: 50
            );


            circle.Draw(graphicInterface);
         }


         DrawingImage drawing = new(dGroup);
         Picture.Source = drawing;
      }
   }

   // Реализация IGraphicInterface для Avalonia
   public class AvaloniaGraphicInterface : IGraphicInterface
   {
      private readonly DrawingContext _context;
      private Color _fillColor = Colors.Blue;
      private Color _borderColor = Colors.Black;
      private float _borderWidth = 2.0f;
      private DashStyle _borderDashStyle = new DashStyle(new double[] { 2, 2 }, 0);

      public AvaloniaGraphicInterface(DrawingContext context)
      {
         _context = context;
      }

      public void DrawCircle(MyPoint center, double radius)
      {
         var brush = new SolidColorBrush(_fillColor);
         var pen = new Pen(new SolidColorBrush(_borderColor), _borderWidth, dashStyle: _borderDashStyle);
         _context.DrawEllipse(brush, pen, new Avalonia.Point(center.x, center.y), radius, radius);
      }

      public void DrawLine(MyPoint start, MyPoint end)
      {
         var pen = new Pen(new SolidColorBrush(_borderColor), _borderWidth, dashStyle: _borderDashStyle);
         _context.DrawLine(pen, new Avalonia.Point(start.x, start.y), new Avalonia.Point(end.x, end.y));
      }

      public void DrawRectangle(MyPoint topLeft, double width, double height)
      {
         var brush = new SolidColorBrush(_fillColor);
         var pen = new Pen(new SolidColorBrush(_borderColor), _borderWidth, dashStyle: _borderDashStyle);
         _context.DrawRectangle(brush, pen, new Rect(topLeft.x, topLeft.y, width, height));
      }

      public void DrawPolygon(IEnumerable<MyPoint> points)
      {
         var brush = new SolidColorBrush(_fillColor);
         var pen = new Pen(new SolidColorBrush(_borderColor), _borderWidth, dashStyle: _borderDashStyle);
         var avaloniaPoints = points.Select(p => new Avalonia.Point(p.x, p.y)).ToList();
         _context.DrawGeometry(brush, pen, new PolylineGeometry(avaloniaPoints, true));
      }
   }
}