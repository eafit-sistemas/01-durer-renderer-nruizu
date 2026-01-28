using System;
using SkiaSharp;

public static class Program
{
    public static void Main()
    {
        InputData data = InputData.LoadFromJson("input.json");
        Shape2D projected = ProjectShape(data.Model);
        projected.Print (); // The tests check for the correct projected data to be printed
        Render(projected, data.Parameters, "output.jpg");
    }

    private static void Render(Shape2D shape, RenderParameters parameters, string outputPath)
    {
        float[][] projectedScreenPoints = new float[shape.Points.Length][];

        Console.WriteLine("Screen points:");
        for (int i = 0; i < shape.Points.Length; i++){
            float x = shape.Points[i][0];
            float y = shape.Points[i][1];

            float xScreen = ((x -  parameters.XMin) / (parameters.XMax - parameters.XMin)) * parameters.Resolution;
            float yScreen = (1 - ((y -  parameters.YMin) / (parameters.YMax - parameters.YMin)))* parameters.Resolution;

            projectedScreenPoints[i] = [xScreen, yScreen];
            Console.WriteLine(xScreen + ", " + yScreen);
        }

        int resolution = parameters.Resolution;
        using SKBitmap bmp = new(resolution, resolution);
        using SKCanvas canvas = new(bmp);

        canvas.Clear(SKColor.Parse("#310579"));

        using SKPaint p1 = new() { Color = SKColors.FloralWhite, StrokeWidth = 1 };

        for (int i=0; i < shape.Lines.Length; i++)
        {
            int line1 = shape.Lines[i][0];
            int line2 = shape.Lines[i][1];
            float x1 = projectedScreenPoints[line1][0];
            float y1 = projectedScreenPoints[line1][1];
            float x2 = projectedScreenPoints[line2][0];
            float y2 = projectedScreenPoints[line2][1];
            canvas.DrawLine(x1, y1, x2, y2, p1);
        }

        using SKFileWStream fs = new("projection.png");
        bmp.Encode(fs, SKEncodedImageFormat.Png, 100);
    }

    private static Shape2D ProjectShape(Model3D model)
    {
        int vertexCount = model.VertexTable.Length;
        float[][] projectedPoints = new float[vertexCount][];
        
        for (int i = 0; i < vertexCount; i++)
        {
            float x = model.VertexTable[i][0];
            float y = model.VertexTable[i][1];
            float z = model.VertexTable[i][2];
            
            float x_proj = x / z;
            float y_proj = y / z;
            
            projectedPoints[i] = [x_proj, y_proj];
        }

        return new Shape2D 
        { 
            Points = projectedPoints, 
            Lines = model.EdgeTable 
        };
    }
}

