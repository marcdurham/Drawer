﻿using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace DumbCad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DrawMode mode = DrawMode.Ready;
        float zoomFactor = 1f;

        SKPaint paintCircleFinished = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1f,
            Color = SKColors.Brown
        };

        SKPaint paintCircleStarting = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2f,
            Color = SKColors.Blue
        };

        List<Circle> Circles = new List<Circle>();
        Circle circleStarting = new Circle();
        SKPoint circleFinishingPoint = new SKPoint();

        public MainWindow()
        {
            InitializeComponent();

            SetMode(DrawMode.Ready);
        }

        private void goButton_Click(object sender, RoutedEventArgs e)
        {
            viewPort.InvalidateVisual();
        }

        private void viewPort_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            var surface = e.Surface;
            var canvas = surface.Canvas;

            canvas.Clear(SKColors.Beige);
            canvas.Scale(zoomFactor);

            foreach (var circle in Circles)
            {
                canvas.DrawCircle(circle.Location, circle.Radius, paintCircleFinished);
            }

            if(mode == DrawMode.CircleFinish && circleStarting != null)
            {
                canvas.DrawCircle(circleStarting.Location, circleStarting.Radius, paintCircleStarting);
                canvas.DrawRect(
                    x: circleStarting.Location.X,
                    y: circleStarting.Location.Y,
                    w: 2f,
                    h: 2f,
                    paintCircleStarting);

                canvas.DrawLine(
                    circleStarting.Location,
                    circleFinishingPoint,
                    paintCircleStarting);
            }
        }

        private void viewPort_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void viewPort_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClickAt(e.GetPosition(viewPort));
        }

        private void ClickAt(Point point)
        {
            if (mode == DrawMode.CircleStart)
            {
                circleStarting = new Circle()
                {
                    Location = new SKPoint((float)point.X, (float)point.Y)
                };

                SetMode(DrawMode.CircleFinish);
                Cursor = Cursors.Cross;
            }
            else if (mode == DrawMode.CircleFinish)
            {
                if (circleStarting.Radius > 0)
                {
                    Circles.Add(circleStarting);
                }

                circleStarting = null;

                SetMode(DrawMode.CircleStart);
                Cursor = Cursors.Cross;
                viewPort.InvalidateVisual();
            }
        }

        private void drawCircleButton_Click(object sender, RoutedEventArgs e)
        {
            SetMode(DrawMode.CircleStart);
        }

        void SetMode(DrawMode mode)
        {
            this.mode = mode;
            this.modeLabel.Content = mode.ToString();
        }

        private void viewPort_MouseMove(object sender, MouseEventArgs e)
        {
            if(mode == DrawMode.CircleFinish && circleStarting != null)
            {
                var point = e.GetPosition(viewPort);
                circleFinishingPoint = new SKPoint((float)point.X, (float)point.Y);
                float radius = (float)Math.Sqrt(
                    Math.Pow(circleStarting.Location.X - point.X, 2) +
                    Math.Pow(circleStarting.Location.Y - point.Y, 2));


                circleStarting.Radius = radius;
                viewPort.InvalidateVisual();
            }

            var cursorLocation = e.GetPosition(viewPort);
            cursorLocationLabel.Content = $"Cursor: {cursorLocation.X:F3},{cursorLocation.Y:F3}";
        }

        private void viewPort_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(mode == DrawMode.CircleFinish)
            {
                SetMode(DrawMode.CircleStart);
                viewPort.InvalidateVisual();
            }
        }

        private void selectButton_Click(object sender, RoutedEventArgs e)
        {
            SetMode(DrawMode.Select);
        }

        private void zoomInButton_Click(object sender, RoutedEventArgs e)
        {
            zoomFactor *= 2f;
            viewPort.InvalidateVisual();
        }

        private void zoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            zoomFactor *= 0.5f;
            viewPort.InvalidateVisual();
        }
    }

    public class Circle
    {
        public SKPoint Location { get; set; }
        public float Radius { get; set; }
    }
}
