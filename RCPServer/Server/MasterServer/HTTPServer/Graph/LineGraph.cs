using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Design;

namespace Graph
{
    /// <summary>
    /// Line Graph drawing class
    /// </summary>
    public class LineGraph
    {
        #region SubClasses
        /// <summary>
        /// Plotted pointon a graph
        /// </summary>
        public class PlotPoint
        {
            decimal x = 0;
            decimal y = 0;

            /// <summary>
            /// X Position
            /// </summary>
            public decimal X
            {
                get { return x; }
                set { x = value; }
            }

            /// <summary>
            /// Y Position
            /// </summary>
            public decimal Y
            {
                get { return y; }
                set { y = value; }
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="inX"></param>
            /// <param name="inY"></param>
            public PlotPoint(decimal inX, decimal inY)
            {
                x = inX;
                y = inY;
            }
        }

        private class GraphSort : IComparer<PlotPoint>
        {
            int IComparer<PlotPoint>.Compare(PlotPoint x, PlotPoint y)
            {
                if (x.X < y.X)
                    return -1;
                if (x.X > y.X)
                    return 1;
                return 0;
            }
        }
        #endregion

        #region Member Variables
        // Member variables
        int width = 640, height = 480;
        decimal xDiff = 1;
        decimal yDiff = 10;
        string xAxisLabel = "";
        string yAxisLabel = "";
        string title = "";
        bool drawCurve = false;

        decimal minX = 0;
        decimal maxX = 1;
        decimal minY = 0;
        decimal maxY = 1;

        List<PlotPoint>[] dataSet = new List<PlotPoint>[5];
        Pen[] LinePens = new Pen[] { Pens.Blue, Pens.Red, Pens.Green, Pens.Magenta, Pens.Purple };
        string[] lineLabel = new string[] { "", "", "", "", "" };
        #endregion

        #region Constructors
        public LineGraph()
        {
            dataSet[0] = new List<PlotPoint>();
            dataSet[1] = new List<PlotPoint>();
            dataSet[2] = new List<PlotPoint>();
            dataSet[3] = new List<PlotPoint>();
            dataSet[4] = new List<PlotPoint>();
        }

        public LineGraph(int inWidth, int inHeight)
        {
            width = inWidth;
            height = inHeight;
            dataSet[0] = new List<PlotPoint>();
            dataSet[1] = new List<PlotPoint>();
            dataSet[2] = new List<PlotPoint>();
            dataSet[3] = new List<PlotPoint>();
            dataSet[4] = new List<PlotPoint>();
        }
        #endregion

        /// <summary>
        /// Set graph data for a specific line index.
        /// </summary>
        /// <param name="idx">Line number to set.</param>
        /// <param name="newData">Data to set</param>
        public void SetData(int idx, List<PlotPoint> newData)
        {
            // Put data into internal list and sort.
            dataSet[idx].Clear();
            dataSet[idx].AddRange(newData);
            dataSet[idx].Sort(new GraphSort());

            // Recalculate bounding values.
            minX = 0;
            maxX = 0;
            minY = 0;
            maxY = 0;

            bool SetInitial = false;

            for (int i = 0; i < dataSet.Length; ++i)
            {
                foreach (PlotPoint P in dataSet[i])
                {
                    if (!SetInitial)
                    {
                        minX = maxX = P.X;
                        minY = maxY = P.Y;
                        SetInitial = true;
                    }

                    if (P.X < minX)
                        minX = P.X;
                    if (P.X > maxX)
                        maxX = P.X;
                    if (P.Y < minY)
                        minY = P.Y;
                    if (P.Y > maxY)
                        maxY = P.Y;
                }
            }
        }

        /// <summary>
        /// Set the label/name of a line.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="text"></param>
        public void SetLabel(int idx, string text)
        {
            lineLabel[idx] = text;
        }

        /// <summary>
        /// Draw graph and obtain image.
        /// </summary>
        /// <returns>Bitmap containing graph data.</returns>
        public Image Draw()
        {
            // Create output image
            Bitmap Output = new Bitmap(width, height);

            // Create pens and fonts
            Pen LinePen = Pens.Black;
            Font TextFont = new Font(FontFamily.GenericSansSerif, 8.0f);
            StringFormat Centered = new StringFormat();
            Centered.Alignment = StringAlignment.Center;
            StringFormat Right = new StringFormat();
            Right.Alignment = StringAlignment.Far;

            int EndOff = 50;
            for (int i = 0; i < lineLabel.Length; ++i)
                if (lineLabel[i].Length > 0)
                    EndOff = 150;

            // Lock min/max into displayable ranges
            decimal Val = 0;
            while (true)
            {
                if (Val > minY)
                {
                    minY = Val - yDiff;

                    if (minY - yDiff > 0)
                        minY = minY - yDiff;
                    break;
                }

                Val += yDiff;
            }

            Val = 0;
            while (true)
            {
                if (Val > maxY)
                {
                    maxY = Val + yDiff;
                    break;
                }

                Val += yDiff;
            }

            // Draw outlines
            int GraphStartX = 50;
            int GraphStartY = 50;
            int GraphEndX = width - EndOff;
            int GraphEndY = height - 50;

            Graphics G = Graphics.FromImage(Output);
            G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            G.FillRectangle(Brushes.White, 0, 0, width, height);
            G.DrawRectangle(LinePen, 0, 0, width - 1, height - 1);
            G.DrawLine(LinePen, GraphStartX, GraphStartY, GraphStartX, GraphEndY);
            G.DrawLine(LinePen, GraphStartX, GraphEndY, GraphEndX, GraphEndY);

            // Draw tabs and numbers
            int XTabs = (int)((maxX - minX) / xDiff);
            int YTabs = (int)((maxY - minY) / yDiff);
            if (XTabs == 0 || YTabs == 0)
                return null;

            int XTabDistance = (width - (EndOff + 50)) / XTabs;
            int YTabDistance = (height - 100) / YTabs;

            decimal XCount = minX;
            decimal YCount = minY;

            for (int i = 0; i < width - (EndOff + 50); i += XTabDistance)
            {
                G.DrawLine(LinePen, GraphStartX + i, GraphEndY, GraphStartX + i, GraphEndY + 5);
                G.DrawString(XCount.ToString(), TextFont, Brushes.Black, GraphStartX + i, GraphEndY + 10, Centered);

                XCount += xDiff;
            }

            for (int i = 0; i <= height - 100; i += YTabDistance)
            {
                G.DrawLine(LinePen, GraphStartX, GraphEndY - i, GraphStartX - 5, GraphEndY - i);
                G.DrawString(YCount.ToString(), TextFont, Brushes.Black, new RectangleF(GraphStartX - 40, (GraphEndY - i) - 6, 30.0f, 14.0f), Right);

                YCount += yDiff;
            }

            // Draw Lines
            int LineCnt = 0;
            for (int i = 0; i < dataSet.Length; ++i)
            {
                if (dataSet[i].Count == 0)
                    continue;

                int LastX = 0;
                int LastY = 0;
                bool DrewLast = false;

                Point[] BezierPoints = null;
                int PointIdx = 0;

                if (drawCurve)
                    BezierPoints = new Point[dataSet[i].Count];


                foreach (PlotPoint P in dataSet[i])
                {
                    int PosX = (int)(((P.X - minX) / xDiff) * ((decimal)XTabDistance));
                    int PosY = (int)(((P.Y - minY) / yDiff) * ((decimal)YTabDistance));

                    PosX += GraphStartX;
                    PosY = GraphEndY - PosY;

                    G.DrawLine(LinePens[i], PosX - 2, PosY - 2, PosX + 2, PosY + 2);
                    G.DrawLine(LinePens[i], PosX - 2, PosY + 2, PosX + 2, PosY - 2);

                    if (DrewLast && !drawCurve)
                    {
                        G.DrawLine(LinePens[i], LastX, LastY, PosX, PosY);
                    }
                    else
                        DrewLast = true;

                    LastX = PosX;
                    LastY = PosY;

                    if (drawCurve)
                    {
                        BezierPoints[PointIdx] = new Point(PosX, PosY);
                        ++PointIdx;
                    }
                }
                if (drawCurve)
                    G.DrawCurve(LinePens[i], BezierPoints);

                if (lineLabel[i].Length > 0)
                {
                    G.FillRectangle(new SolidBrush(LinePens[i].Color), new Rectangle(width - 110, 160 + (LineCnt * 20), 4, 4));
                    G.DrawString(lineLabel[i], TextFont, Brushes.Black, width - 103, 155 + (LineCnt * 20));
                    ++LineCnt;
                }
            }

            // Draw labels
            G.DrawString(xAxisLabel, TextFont, Brushes.Black, new RectangleF(10, height - 20, width - 20, 20), Centered);
            G.DrawString(title, TextFont, Brushes.Black, new RectangleF(10, 10, width - 20, 20), Centered);

            G.TranslateTransform(5, height / 2);
            G.RotateTransform(270.0f);
            G.DrawString(yAxisLabel, TextFont, Brushes.Black, 0, 0, Centered);

            G.TranslateTransform(0, 0);
            G.RotateTransform(0);

            // Done
            return Output;
        }

        public string XAxisLabel
        {
            get { return xAxisLabel; }
            set { xAxisLabel = value; }
        }

        public string YAxisLabel
        {
            get { return yAxisLabel; }
            set { yAxisLabel = value; }
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public decimal XDifference
        {
            get { return xDiff; }
            set { xDiff = value; }
        }

        public decimal YDifference
        {
            get { return yDiff; }
            set { yDiff = value; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public bool DrawCurve
        {
            get { return drawCurve; }
            set { drawCurve = value; }
        }
    }
}
