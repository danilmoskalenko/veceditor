using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.IO;
using System.Threading.Tasks;
using Path = Avalonia.Controls.Shapes.Path;

namespace veceditor.MVVM
{
    public class PngExporter
    {

        public static async Task<bool> ExportToPng(Canvas canvas, string filePath)
        {
            try
            {
                if (canvas.Bounds.Width <= 0 || canvas.Bounds.Height <= 0)
                    return false;

                var pixelSize = new PixelSize(
                    (int)Math.Ceiling(canvas.Bounds.Width),
                    (int)Math.Ceiling(canvas.Bounds.Height));

                using (var renderBitmap = new RenderTargetBitmap(pixelSize, new Vector(96, 96)))
                {
                    using (var drawingContext = renderBitmap.CreateDrawingContext())
                    {
                        drawingContext.DrawRectangle(
                            new SolidColorBrush(Colors.White),
                            null,
                            new Rect(0, 0, canvas.Bounds.Width, canvas.Bounds.Height));

                        foreach (var child in canvas.Children)
                        {
                            if (!child.IsVisible || child is TextBlock)
                                continue;

                            double left = Canvas.GetLeft(child);
                            double top = Canvas.GetTop(child);

                            if (double.IsNaN(left)) left = 0;
                            if (double.IsNaN(top)) top = 0;

                            if (child is Ellipse ellipse)
                            {
                                bool isPoint = ellipse.Width <= 6 && ellipse.Height <= 6;
                                
                                if (isPoint)
                                {
                                    double x = left + ellipse.Width / 2;
                                    double y = top + ellipse.Height / 2;
                                    
                                    drawingContext.DrawEllipse(
                                        new SolidColorBrush(Colors.Black),
                                        null,
                                        new Point(x, y),
                                        3, 3);
                                }
                                else
                                {
                                    double cx = left + ellipse.Width / 2;
                                    double cy = top + ellipse.Height / 2;
                                    
                                    drawingContext.DrawEllipse(
                                        null,
                                        new Pen(new SolidColorBrush(Colors.Black), 2),
                                        new Point(cx, cy),
                                        ellipse.Width / 2,
                                        ellipse.Height / 2);
                                }
                            }
                            else if (child.GetType().Name == "Rectangle")
                            {
                                var control = child as Control;
                                if (control != null)
                                {
                                    drawingContext.DrawRectangle(
                                        null,
                                        new Pen(new SolidColorBrush(Colors.Black), 2),
                                        new Rect(left, top, control.Bounds.Width, control.Bounds.Height));
                                }
                            }
                            else if (child is Path path)
                            {
                                if (path.Data is LineGeometry lineGeom)
                                {
                                    drawingContext.DrawLine(
                                        new Pen(new SolidColorBrush(Colors.Black), 2),
                                        lineGeom.StartPoint,
                                        lineGeom.EndPoint);
                                }
                                else if (path.Data is RectangleGeometry rectGeom)
                                {
                                    drawingContext.DrawRectangle(
                                        null,
                                        new Pen(new SolidColorBrush(Colors.Black), 2),
                                        rectGeom.Rect);
                                }
                                else if (path.Data is PathGeometry pathGeom && pathGeom.Figures.Count > 0)
                                {
                                    drawingContext.DrawGeometry(
                                        null,
                                        new Pen(new SolidColorBrush(Colors.Black), 2),
                                        pathGeom);
                                }
                                else
                                {
                                    var transformedPath = new Path
                                    {
                                        Data = path.Data,
                                        Stroke = path.Stroke,
                                        StrokeThickness = path.StrokeThickness,
                                        Fill = path.Fill
                                    };
                                    
                                    transformedPath.RenderTransform = new TranslateTransform(left, top);
                                    transformedPath.Render(drawingContext);
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Handling unknown type: {child.GetType().Name}");
                                var transform = Matrix.CreateTranslation(left, top);
                                drawingContext.PushTransform(transform);
                                child.Render(drawingContext);
                                drawingContext.PushTransform(Matrix.Identity);
                            }
                        }
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        renderBitmap.Save(fileStream);
                        await Task.CompletedTask;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting to PNG: {ex.Message}");
                return false;
            }
        }
    }
} 