using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace @interface
{
    /// <summary>
    /// Control personalizat pentru redarea graficului de utilizare
    /// </summary>
    public class UsageGraph : Control
    {
        /// <summary>
        /// Colectie de puncte de date temporale
        /// </summary>
        private List<KeyValuePair<DateTime, double>> _timeData;

        /// <summary>
        /// Intervalul selectat de timp (zi, saptamana, luna)
        /// </summary>
        private string _timeRange;

        /// <summary>
        /// Titlul graficului
        /// </summary>
        private string _title;

        /// <summary>
        /// Obtine sau seteaza datele temporale pentru grafic
        /// </summary>
        public List<KeyValuePair<DateTime, double>> TimeData
        {
            get { return _timeData; }
            set
            {
                _timeData = value;
                Invalidate(); // Redesenare la modificarea datelor
            }
        }

        /// <summary>
        /// Obtine sau seteaza intervalul de timp pentru grafic
        /// </summary>
        public string TimeRange
        {
            get { return _timeRange; }
            set
            {
                _timeRange = value;
                Invalidate(); // Redesenare la modificarea intervalului
            }
        }

        /// <summary>
        /// Obtine sau seteaza titlul graficului
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                Invalidate(); // Redesenare la modificarea titlului
            }
        }

        /// <summary>
        /// Constructor pentru UsageGraph
        /// </summary>
        public UsageGraph()
        {
            _timeData = new List<KeyValuePair<DateTime, double>>();
            _timeRange = "1 month";
            _title = "Usage Over Time";

            // Seteaza stiluri de control pentru redare mai buna
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);
        }

        /// <summary>
        /// Actualizeaza intervalul de timp afisat
        /// </summary>
        /// <param name="range">Intervalul nou de timp</param>
        public void UpdateTimeRange(string range)
        {
            TimeRange = range;
        }

        /// <summary>
        /// Gestioneaza redarea controlului
        /// </summary>
        /// <param name="e">Argumentele evenimentului de pictare</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Deseneaza chenarul
            using (Pen borderPen = new Pen(Color.Black, 2))
            {
                g.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);
            }

            // Deseneaza titlul
            using (Font titleFont = new Font("Arial", 12, FontStyle.Bold))
            {
                g.DrawString(_title, titleFont, Brushes.Black, 10, 10);
            }

            // Deseneaza intervalul de timp
            using (Font rangeFont = new Font("Arial", 10))
            {
                g.DrawString(_timeRange, rangeFont, Brushes.Black, Width - 100, 10);
            }

            // Daca nu exista date, afiseaza mesaj
            if (_timeData == null || _timeData.Count == 0)
            {
                using (Font messageFont = new Font("Arial", 12))
                {
                    g.DrawString("No data available", messageFont, Brushes.Gray,
                        Width / 2 - 50, Height / 2);
                }
                return;
            }

            // Deseneaza graficul
            DrawGraph(g);
        }

        /// <summary>
        /// Deseneaza graficul pe baza datelor curente
        /// </summary>
        /// <param name="g">Obiectul Graphics</param>
        private void DrawGraph(Graphics g)
        {
            if (_timeData.Count == 0) return;

            // Dimensiuni graf
            int margin = 40;
            int graphWidth = Width - 2 * margin;
            int graphHeight = Height - 2 * margin;
            int graphBottom = Height - margin;
            int graphLeft = margin;

            // Valoare maxima
            double maxValue = _timeData.Max(d => d.Value);
            if (maxValue < 1) maxValue = 1;

            // Axa Y
            using (Pen axisPen = new Pen(Color.Black, 1))
            {
                g.DrawLine(axisPen, graphLeft, margin, graphLeft, graphBottom);


                using (Font axisFont = new Font("Arial", 8))
                {
                    g.DrawString(maxValue.ToString("0.0") + "h", axisFont, Brushes.Black,
                        graphLeft - 30, margin);

                    g.DrawString((maxValue / 2).ToString("0.0") + "h", axisFont, Brushes.Black,
                        graphLeft - 30, margin + graphHeight / 2);

                    g.DrawString("0h", axisFont, Brushes.Black,
                        graphLeft - 30, graphBottom);
                }

                g.DrawLine(axisPen, graphLeft, graphBottom, Width - margin, graphBottom);

                // Axa X
                if (_timeData.Count > 0)
                {
                    using (Font axisFont = new Font("Arial", 8))
                    {
                        int labelCount = Math.Min(5, _timeData.Count); // Afiseaza 5 dati
                        int step = _timeData.Count / labelCount;

                        for (int i = 0; i < _timeData.Count; i += step)
                        {
                            if (i < _timeData.Count)
                            {
                                float xPos = graphLeft + (i * graphWidth / (_timeData.Count - 1));
                                g.DrawString(_timeData[i].Key.ToString("MM/dd"), axisFont, Brushes.Black,
                                    xPos - 10, graphBottom + 5);
                            }
                        }

                        // Afiseaza ultima data mereu
                        if (_timeData.Count > 1)
                        {
                            float xPos = graphLeft + graphWidth;
                            g.DrawString(_timeData.Last().Key.ToString("MM/dd"), axisFont, Brushes.Black,
                                xPos - 10, graphBottom + 5);
                        }
                    }
                }
            }

            if (_timeData.Count > 1)
            {
                using (Pen linePen = new Pen(Color.Blue, 2))
                {
                    // Calculare puncte
                    Point[] points = new Point[_timeData.Count];

                    for (int i = 0; i < _timeData.Count; i++)
                    {
                        // X
                        float xPos = graphLeft + (i * graphWidth / (_timeData.Count - 1));

                        // Y
                        float yPos = graphBottom - (float)(_timeData[i].Value / maxValue * graphHeight);

                        points[i] = new Point((int)xPos, (int)yPos);
                    }

                    g.DrawLines(linePen, points);

                    using (Brush pointBrush = new SolidBrush(Color.Blue))
                    {
                        foreach (Point p in points)
                        {
                            g.FillEllipse(pointBrush, p.X - 3, p.Y - 3, 6, 6);
                        }
                    }

                    using (Font valueFont = new Font("Arial", 7))
                    {
                        int labelStep = Math.Max(1, _timeData.Count / 10);
                        for (int i = 0; i < _timeData.Count; i += labelStep)
                        {
                            if (_timeData[i].Value > 0.1) // Doar valorile importante
                            {
                                string valueText = _timeData[i].Value.ToString("0.0") + "h";
                                g.DrawString(valueText, valueFont, Brushes.DarkBlue,
                                    points[i].X - 8, points[i].Y - 15);
                            }
                        }

                        if (_timeData.Last().Value > 0.1)
                        {
                            int lastIndex = _timeData.Count - 1;
                            string valueText = _timeData[lastIndex].Value.ToString("0.0") + "h";
                            g.DrawString(valueText, valueFont, Brushes.DarkBlue,
                                points[lastIndex].X - 8, points[lastIndex].Y - 15);
                        }
                    }
                }
            }
        }
    }
}
