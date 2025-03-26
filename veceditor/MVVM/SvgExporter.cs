using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using veceditor.MVVM.Model;
using Point = veceditor.MVVM.Model.Point;
using System.Linq;
using Path = Avalonia.Controls.Shapes.Path;
using System.Globalization;

namespace veceditor.MVVM
{
    public class SvgExporter
    {
        /// <summary>
        /// Экспортирует содержимое Canvas в файл SVG
        /// </summary>
        /// <param name="canvas">Canvas для экспорта</param>
        /// <param name="filePath">Путь для сохранения SVG файла</param>
        /// <returns>True в случае успеха, False в случае ошибки</returns>
        public static async Task<bool> ExportToSvg(Canvas canvas, string filePath)
        {
            try
            {
                // Проверяем размеры Canvas
                if (canvas.Bounds.Width <= 0 || canvas.Bounds.Height <= 0)
                {
                    return false;
                }

                // Используем инвариантную культуру для чисел с точкой вместо запятой
                CultureInfo invariantCulture = CultureInfo.InvariantCulture;
                
                // Создаем SVG разметку
                StringBuilder svgBuilder = new StringBuilder();
                
                // Заголовок SVG документа
                svgBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>");
                svgBuilder.AppendLine($"<svg width=\"{canvas.Bounds.Width.ToString(invariantCulture)}\" height=\"{canvas.Bounds.Height.ToString(invariantCulture)}\" xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\">");
                
                // Добавляем белый фон размером с рабочую область
                svgBuilder.AppendLine($"<rect width=\"{canvas.Bounds.Width.ToString(invariantCulture)}\" height=\"{canvas.Bounds.Height.ToString(invariantCulture)}\" fill=\"white\"/>");
                
                // Обрабатываем все дочерние элементы Canvas
                foreach (var child in canvas.Children)
                {
                    // Пропускаем TextBlock (это текст с выбранной фигурой)
                    if (child is TextBlock)
                        continue;
                    
                    // Добавляем SVG представление каждой фигуры
                    if (child is Ellipse ellipse)
                    {
                        // Определяем, является ли это кругом или точкой
                        bool isPoint = ellipse.Width <= 6 && ellipse.Height <= 6;
                        
                        if (isPoint)
                        {
                            double x = Canvas.GetLeft(ellipse) + ellipse.Width / 2;
                            double y = Canvas.GetTop(ellipse) + ellipse.Height / 2;
                            var color = (ellipse.Fill as SolidColorBrush)?.Color;
                            string fillColor = color != null ? $"rgb({color.Value.R},{color.Value.G},{color.Value.B})" : "black";
                            svgBuilder.AppendLine($"<circle cx=\"{x.ToString(invariantCulture)}\" cy=\"{y.ToString(invariantCulture)}\" r=\"3\" fill=\"{fillColor}\"/>");
                        }
                        else
                        {
                            double cx = Canvas.GetLeft(ellipse) + ellipse.Width / 2;
                            double cy = Canvas.GetTop(ellipse) + ellipse.Height / 2;
                            double rx = ellipse.Width / 2;
                            double ry = ellipse.Height / 2;
                            var color = (ellipse.Stroke as SolidColorBrush)?.Color;
                            string strokeColor = color != null ? $"rgb({color.Value.R},{color.Value.G},{color.Value.B})" : "black";
                            svgBuilder.AppendLine($"<ellipse cx=\"{cx.ToString(invariantCulture)}\" cy=\"{cy.ToString(invariantCulture)}\" rx=\"{rx.ToString(invariantCulture)}\" ry=\"{ry.ToString(invariantCulture)}\" fill=\"none\" stroke=\"{strokeColor}\" stroke-width=\"{ellipse.StrokeThickness}\"/>");
                        }
                    }
                    else if (child is Path path)
                    {
                        // Обрабатываем разные типы фигур на основе их Data
                        if (path.Data is LineGeometry lineGeom)
                        {
                            var color = (path.Stroke as SolidColorBrush)?.Color;
                            string strokeColor = color != null ? $"rgb({color.Value.R},{color.Value.G},{color.Value.B})" : "black";
                            svgBuilder.AppendLine($"<line x1=\"{lineGeom.StartPoint.X.ToString(invariantCulture)}\" y1=\"{lineGeom.StartPoint.Y.ToString(invariantCulture)}\" " +
                                                 $"x2=\"{lineGeom.EndPoint.X.ToString(invariantCulture)}\" y2=\"{lineGeom.EndPoint.Y.ToString(invariantCulture)}\" " +
                                                 $"stroke=\"{strokeColor}\" stroke-width=\"{path.StrokeThickness}\"/>");
                        }
                        else if (path.Data is RectangleGeometry rectGeom)
                        {
                            var color = (path.Stroke as SolidColorBrush)?.Color;
                            string strokeColor = color != null ? $"rgb({color.Value.R},{color.Value.G},{color.Value.B})" : "black";
                            svgBuilder.AppendLine($"<rect x=\"{rectGeom.Rect.X.ToString(invariantCulture)}\" y=\"{rectGeom.Rect.Y.ToString(invariantCulture)}\" " +
                                                 $"width=\"{rectGeom.Rect.Width.ToString(invariantCulture)}\" height=\"{rectGeom.Rect.Height.ToString(invariantCulture)}\" " +
                                                 $"style=\"fill:none; stroke-width:{path.StrokeThickness}; stroke:{strokeColor}\"/>");
                        }
                        else if (path.Data is PathGeometry pathGeom && pathGeom.Figures.Count > 0)
                        {
                            // Обрабатываем треугольник и другие пути
                            var pathFigure = pathGeom.Figures[0];
                            var points = new StringBuilder();
                            
                            // Добавляем начальную точку
                            points.Append($"{pathFigure.StartPoint.X.ToString(invariantCulture)},{pathFigure.StartPoint.Y.ToString(invariantCulture)} ");
                            
                            // Добавляем все точки из сегментов
                            foreach (var segment in pathFigure.Segments)
                            {
                                if (segment is LineSegment lineSegment)
                                {
                                    points.Append($"{lineSegment.Point.X.ToString(invariantCulture)},{lineSegment.Point.Y.ToString(invariantCulture)} ");
                                }
                            }
                            
                            // Используем polygon для замкнутых фигур (треугольник)
                            svgBuilder.AppendLine($"<polygon points=\"{points.ToString().Trim()}\" " +
                                                 $"style=\"fill:none; stroke-width:2; stroke:rgb(0,0,0)\"/>");
                        }
                    }
                    else if (child is Polygon polygon)
                    {
                        // Обрабатываем треугольник
                        var points = new StringBuilder();
                        foreach (var point in polygon.Points)
                        {
                            points.Append($"{point.X.ToString(invariantCulture)},{point.Y.ToString(invariantCulture)} ");
                        }
                        
                        var color = (polygon.Stroke as SolidColorBrush)?.Color;
                        string strokeColor = color != null ? $"rgb({color.Value.R},{color.Value.G},{color.Value.B})" : "black";
                        svgBuilder.AppendLine($"<polygon points=\"{points.ToString().Trim()}\" " +
                                             $"style=\"fill:none; stroke-width:{polygon.StrokeThickness}; stroke:{strokeColor}\"/>");
                    }
                }
                
                // Закрываем SVG документ
                svgBuilder.AppendLine("</svg>");
                
                // Записываем SVG в файл
                await File.WriteAllTextAsync(filePath, svgBuilder.ToString());
                
                return true;
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                Console.WriteLine($"Ошибка экспорта в SVG: {ex.Message}");
                return false;
            }
        }
    }
} 