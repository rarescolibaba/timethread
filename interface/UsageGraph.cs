using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace @interface
{
    /// <summary>
    /// Custom control for rendering the usage graph
    /// </summary>
    public class UsageGraph : Control
    {
        /// <summary>
        /// Collection of time data points
        /// </summary>
        private List<KeyValuePair<DateTime, double>> _timeData;

        /// <summary>
        /// Selected time range (day, week, month)
        /// </summary>
        private string _timeRange;

        /// <summary>
        /// Title of the graph
        /// </summary>
        private string _title;

        /// <summary>
        /// Gets or sets the time data for the graph
        /// </summary>
        public List<KeyValuePair<DateTime, double>> TimeData
        {
            get { return _timeData; }
            set
            {
                _timeData = value;
                Invalidate(); // Redraw when data changes
            }
        }

        /// <summary>
        /// Gets or sets the time range for the graph
        /// </summary>
        public string TimeRange
        {
            get { return _timeRange; }
            set
            {
                _timeRange = value;
                Invalidate(); // Redraw when range changes
            }
        }

        /// <summary>
        /// Gets or sets the title of the graph
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                Invalidate(); // Redraw when title changes
            }
        }

        /// <summary>
        /// Constructor for UsageGraph
        /// </summary>
        public UsageGraph()
        {
            _timeData = new List<KeyValuePair<DateTime, double>>();
            _timeRange = "1 month";
            _title = "Usage Over Time";
            
            // Set control styles for better rendering
            SetStyle(ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.OptimizedDoubleBuffer | 
                     ControlStyles.ResizeRedraw | 
                     ControlStyles.UserPaint, true);
        }

        /// <summary>
        /// Updates the time range of displayed data
        /// </summary>
        /// <param name="range">New time range</param>
        public void UpdateTimeRange(string range)
        {
            TimeRange = range;
        }

        /// <summary>
        /// Handles the painting of the control
        /// </summary>
        /// <param name="e">Paint event arguments</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            // Draw border
            using (Pen borderPen = new Pen(Color.Black, 2))
            {
                g.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);
            }
            
            // Draw title
            using (Font titleFont = new Font("Arial", 12, FontStyle.Bold))
            {
                g.DrawString(_title, titleFont, Brushes.Black, 10, 10);
            }
            
            // Draw time range
            using (Font rangeFont = new Font("Arial", 10))
            {
                g.DrawString(_timeRange, rangeFont, Brushes.Black, Width - 100, 10);
            }
            
            // If no data, show message
            if (_timeData == null || _timeData.Count == 0)
            {
                using (Font messageFont = new Font("Arial", 12))
                {
                    g.DrawString("No data available", messageFont, Brushes.Gray, 
                        Width / 2 - 50, Height / 2);
                }
                return;
            }
            
            // Draw graph
            DrawGraph(g);
        }

        /// <summary>
        /// Draws the graph based on current data
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawGraph(Graphics g)
        {
            if (_timeData.Count == 0) return;

            // Graph dimensions
            int margin = 40;
            int graphWidth = Width - 2 * margin;
            int graphHeight = Height - 2 * margin;
            int graphBottom = Height - margin;
            int graphLeft = margin;
            
            // Find max value for scaling
            double maxValue = _timeData.Max(d => d.Value);
            if (maxValue == 0) maxValue = 1; // Avoid division by zero
            
            // Draw Y-axis
            using (Pen axisPen = new Pen(Color.Black, 1))
            {
                g.DrawLine(axisPen, graphLeft, margin, graphLeft, graphBottom);
                
                // Draw Y-axis labels
                using (Font axisFont = new Font("Arial", 8))
                {
                    // Draw max value
                    g.DrawString(maxValue.ToString("0.0") + "h", axisFont, Brushes.Black, 
                        graphLeft - 30, margin);
                    
                    // Draw middle value
                    g.DrawString((maxValue / 2).ToString("0.0") + "h", axisFont, Brushes.Black, 
                        graphLeft - 30, margin + graphHeight / 2);
                    
                    // Draw zero
                    g.DrawString("0h", axisFont, Brushes.Black, 
                        graphLeft - 30, graphBottom);
                }
                
                // Draw X-axis
                g.DrawLine(axisPen, graphLeft, graphBottom, Width - margin, graphBottom);
                
                // Draw X-axis labels
                if (_timeData.Count > 0)
                {
                    using (Font axisFont = new Font("Arial", 8))
                    {
                        // Draw first date
                        g.DrawString(_timeData.First().Key.ToString("MM/dd"), axisFont, Brushes.Black, 
                            graphLeft, graphBottom + 5);
                        
                        // Draw last date
                        g.DrawString(_timeData.Last().Key.ToString("MM/dd"), axisFont, Brushes.Black, 
                            Width - margin - 30, graphBottom + 5);
                    }
                }
            }
            
            // Draw data points and lines
            if (_timeData.Count > 1)
            {
                using (Pen linePen = new Pen(Color.Blue, 2))
                {
                    // Calculate points
                    Point[] points = new Point[_timeData.Count];
                    
                    for (int i = 0; i < _timeData.Count; i++)
                    {
                        // X position based on date range
                        float xPos = graphLeft + (i * graphWidth / (_timeData.Count - 1));
                        
                        // Y position based on value (inverted, since 0,0 is top-left)
                        float yPos = graphBottom - (float)(_timeData[i].Value / maxValue * graphHeight);
                        
                        points[i] = new Point((int)xPos, (int)yPos);
                    }
                    
                    // Draw lines between points
                    g.DrawLines(linePen, points);
                    
                    // Draw points
                    using (Brush pointBrush = new SolidBrush(Color.Blue))
                    {
                        foreach (Point p in points)
                        {
                            g.FillEllipse(pointBrush, p.X - 3, p.Y - 3, 6, 6);
                        }
                    }
                }
            }
        }
    }
} 