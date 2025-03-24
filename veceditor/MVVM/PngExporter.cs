using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.IO;
using System.Threading.Tasks;

namespace veceditor.MVVM
{
    public class PngExporter
    {
        /// <summary>
        /// Exports the given canvas to a PNG file
        /// </summary>
        /// <param name="canvas">The canvas to export</param>
        /// <param name="filePath">The file path to save the PNG to</param>
        /// <returns>True if successful, false otherwise</returns>
        public static async Task<bool> ExportToPng(Canvas canvas, string filePath)
        {
            try
            {
                // Make sure the canvas has a valid size
                if (canvas.Bounds.Width <= 0 || canvas.Bounds.Height <= 0)
                {
                    return false;
                }

                // Create a RenderTargetBitmap with the canvas size
                var pixelSize = new PixelSize((int)canvas.Bounds.Width, (int)canvas.Bounds.Height);
                var size = new Size(canvas.Bounds.Width, canvas.Bounds.Height);
                
                using (var renderBitmap = new RenderTargetBitmap(pixelSize, new Vector(96, 96)))
                {
                    // Render the canvas to the bitmap
                    renderBitmap.Render(canvas);
                    
                    // Save the bitmap to a file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        renderBitmap.Save(fileStream);
                        await Task.CompletedTask; // Just to keep the async signature
                    }
                }
                
                return true;
            }
            catch (Exception ex)
            {
                // Log the error if needed
                Console.WriteLine($"Error exporting to PNG: {ex.Message}");
                return false;
            }
        }
    }
} 