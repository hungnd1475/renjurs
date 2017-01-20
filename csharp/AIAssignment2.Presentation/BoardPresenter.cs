using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using AIAssignment2.Foundations;

namespace AIAssignment2.Presentation
{
    class MovePlayedEventArgs : EventArgs
    {
        public Move Move { get; private set; }
        public MovePlayedEventArgs(Move move)
        {
            this.Move = move;
        }
    }

    static class CanvasDrawer
    {
        private static double stoneRadius;
        private static double unitSize;
        private static Thickness margin;
        private static double indexSize = 20;
        private static bool[,] hasStone;
        private static Rect[,] inputRects;
        private static int size;
        private static Border lastBorder;

        public static void DrawLine(this Canvas canvas, Point p1, Point p2)
        {
            var line = new Line();
            line.X1 = p1.X;
            line.X2 = p2.X;
            line.Y1 = p1.Y;
            line.Y2 = p2.Y;
            line.Stroke = Brushes.Black;
            line.StrokeThickness = 1.5;
            canvas.Children.Add(line);
        }

        public static void DrawBoard(this Canvas canvas, int size)
        {
            canvas.Children.Clear();
            hasStone = new bool[size, size];

            unitSize = (canvas.Width - indexSize) / size;
            stoneRadius = unitSize / 2;
            margin = new Thickness(unitSize / 2 + indexSize, unitSize / 2 + indexSize, unitSize / 2, unitSize / 2);
            CanvasDrawer.size = size;

            var border = new Border();
            Canvas.SetTop(border, indexSize);
            Canvas.SetLeft(border, indexSize);
            border.Width = canvas.Width - indexSize;
            border.Height = canvas.Height - indexSize;
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = Brushes.Black;
            border.Background = Brushes.SaddleBrown;
            canvas.Children.Add(border);

            var i = 0;
            for (double x = margin.Top; i < size; x = (++i) * unitSize + margin.Top)
            {
                var p1 = new Point(x, margin.Left);
                var p2 = new Point(x, canvas.Width - margin.Right);
                canvas.DrawLine(p1, p2);
            }

            i = 0;
            for (double y = margin.Left; i < size; y = (++i) * unitSize + margin.Left)
            {
                var p1 = new Point(margin.Top, y);
                var p2 = new Point(canvas.Height - margin.Bottom, y);
                canvas.DrawLine(p1, p2);
            }

            for (i = 1; i <= size; i++)
            {
                var textTop = new TextBlock();
                textTop.Text = "" + i;
                textTop.FontSize = 14;
                textTop.Foreground = Brushes.Black;

                var textLeft = new TextBlock();
                textLeft.Text = "" + i;
                textLeft.FontSize = 14;
                textLeft.Foreground = Brushes.Black;

                canvas.DrawText(textTop, indexSize, indexSize, 0, (i - 1) * unitSize + margin.Left - indexSize / 2);
                canvas.DrawText(textLeft, indexSize, indexSize, (i - 1) * unitSize + margin.Top - indexSize / 2, 0);
            }

            inputRects = new Rect[size, size];
            for (i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    var pos = new Point();
                    pos.X = j * unitSize + margin.Top - 5;
                    pos.Y = i * unitSize + margin.Left - 5;
                    inputRects[j, i] = new Rect(pos, new Size(10, 10));
                }
        }

        public static Move? PlayAt(this Canvas canvas, Point mousePos)
        {
            int x = 0, y = 0;
            var found = false;

            for (x = 0; x < size; x++)
            {
                for (y = 0; y < size; y++)
                {
                    if (inputRects[x, y].Contains(mousePos))
                    {
                        found = true;
                        break;
                    }
                }
                if (found) break;
            }

            if (found && !hasStone[x, y])
            {
                canvas.DrawStone(y, x, 2, true);
                return new Move(y, x);
            }

            return null;
        }

        public static Border DrawText(this Canvas canvas, TextBlock text, double width, double height, double top, double left)
        {
            var border = new Border();
            border.Width = width;
            border.Height = height;
            Canvas.SetTop(border, top);
            Canvas.SetLeft(border, left);

            text.HorizontalAlignment = HorizontalAlignment.Center;
            text.VerticalAlignment = VerticalAlignment.Center;

            border.Child = text;
            canvas.Children.Add(border);

            return border;
        }

        public static void DrawStone(this Canvas canvas, int x, int y, int index, bool black)
        {
            var stone = new Ellipse();
            Canvas.SetLeft(stone, y * unitSize + margin.Left - stoneRadius);
            Canvas.SetTop(stone, x * unitSize + margin.Top - stoneRadius);
            stone.Width = stoneRadius * 2;
            stone.Height = stoneRadius * 2;
            stone.Fill = black ? Brushes.Black : Brushes.White;
            canvas.Children.Add(stone);

            var text = new TextBlock();
            text.Text = "" + index;
            text.FontWeight = FontWeights.Bold;
            text.FontSize = stoneRadius;
            text.Foreground = black ? Brushes.White : Brushes.Black;

            if (lastBorder != null)
            {
                lastBorder.BorderThickness = new Thickness(0);
            }

            lastBorder = canvas.DrawText(text, stoneRadius * 2, stoneRadius * 2, x * unitSize + margin.Top - stoneRadius,
                 y * unitSize + margin.Left - stoneRadius);
            lastBorder.BorderThickness = new Thickness(2);
            lastBorder.BorderBrush = Brushes.Red;

            hasStone[y, x] = true;
        }
    }
}
