﻿using Parcel.Graphing.PlotConfigurations;
using Parcel.Neo.Base.DataTypes;
using Parcel.Types;
using Parcel.Math.Types;

namespace Parcel.Graphing
{
    internal class CustomPalette : ScottPlot.IPalette
    {
        public string Name { get; } = "A Custom Palette";
        public string Description { get; } = "Used when customizing plot colors.";

        public ScottPlot.Color[] Colors { get; set; } = new ScottPlot.Palettes.Category10().Colors; // Default

        public ScottPlot.Color GetColor(int index)
            => Colors[index % Colors.Length];
    }

    public static class Plot
    {
        #region Constants
        internal const int DefaultWidth = 400;
        internal const int DefaultHeight = 300;
        #endregion

        #region Illustrational
        /// <summary>
        /// PlotVector(s)2D: Draws Vector2D in coordinate grid. Great for educational purpose (when comoaring before and after manipulation).
        /// </summary>
        public static Image DrawVector2D(Vector2D vector, DrawVector2DConfiguration? configurations = null)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Numerical Analytics
        public static Image NumberDisplay(string label, double number, NumberDisplayConfiguration? configurations = null)
        {
            configurations ??= new NumberDisplayConfiguration()
            {
                ImageWidth = 200,
                ImageHeight = 160,
            };

            ScottPlot.Plot plot = new();
            plot.Layout.Frameless();
            plot.HideGrid();

            const float padding = 40;

            ScottPlot.Plottables.Text titleText = plot.Add.Text($"{label}\n\n", configurations.ImageWidth + padding * 2, configurations.ImageHeight + padding * 2);
            titleText.LabelFontSize = 22;
            titleText.LabelBold = false;
            titleText.LabelFontColor = Convert(configurations.TitleColor);
            titleText.LabelPadding = padding;
            titleText.LabelAlignment = ScottPlot.Alignment.MiddleCenter;

            ScottPlot.Plottables.Text numberText = plot.Add.Text($"\n{number.ToString($"F{configurations.DecimalPlaces}")}", configurations.ImageWidth + padding * 2, configurations.ImageHeight + padding * 2);
            numberText.LabelFontSize = 26;
            numberText.LabelBold = true;
            numberText.LabelFontColor = Convert(configurations.NumberColor);
            numberText.LabelBackgroundColor = ScottPlot.Colors.DarkSlateGray.WithAlpha(.1);
            numberText.LabelBorderColor = ScottPlot.Colors.Black;
            numberText.LabelBorderWidth = 3;
            numberText.LabelPadding = padding;
            numberText.LabelAlignment = ScottPlot.Alignment.MiddleCenter;

            string path = Image.GetTempImagePath();
            plot.SavePng(path, configurations.ImageWidth == 0 ? 60 : configurations.ImageWidth, configurations.ImageHeight == 0 ? 60 : configurations.ImageHeight);
            return new Image(path);
        }
        #endregion

        #region Standard Plots
        public static Image ScatterPlot(Vector x, Vector[] ys, ScatterPlotMultiSeriesConfiguration? configurations = null)
        {
            configurations ??= new();

            ScottPlot.Plot plot = new();
            if (configurations.Colors != null)
                plot.Add.Palette = new CustomPalette()
                {
                    Colors = [.. configurations.Colors.Select(Convert)]
                };

            for (int i = 0; i < ys.Length; i++)
            {
                double[] y = ys[i];

                ScottPlot.Plottables.Scatter s = plot.Add.Scatter(x, y);
                if (configurations.Legends != null)
                {
                    plot.Legend.IsVisible = true;
                    s.LegendText = configurations.Legends[i];
                }
            }

            if (!string.IsNullOrEmpty(configurations.Title))
                plot.Title(configurations.Title);
            if (!string.IsNullOrEmpty(configurations.XAxis))
                plot.Axes.Bottom.Label.Text = configurations.XAxis;
            if (!string.IsNullOrEmpty(configurations.YAxis))
                plot.Axes.Left.Label.Text = configurations.YAxis;

            string path = Image.GetTempImagePath();
            plot.SavePng(path, configurations.ImageWidth == 0 ? DefaultWidth : configurations.ImageWidth, configurations.ImageHeight == 0 ? DefaultHeight : configurations.ImageHeight);
            return new Image(path);
        }
        public static Image ScatterPlot(Vector x, Vector y, ScatterPlotConfiguration? configurations = null)
        {
            configurations ??= new();

            ScottPlot.Plot plot = new();
            if (configurations.Color != null)
                plot.Add.Palette = new CustomPalette()
                {
                    Colors = [Convert(configurations.Color)]
                };

            ScottPlot.Plottables.Scatter s = plot.Add.Scatter(x, y);
            if (!string.IsNullOrEmpty(configurations.Legend))
            {
                plot.Legend.IsVisible = true;
                s.LegendText = configurations.Legend;
            }

            if (!string.IsNullOrEmpty(configurations.Title))
                plot.Title(configurations.Title);
            if (!string.IsNullOrEmpty(configurations.XAxis))
                plot.Axes.Bottom.Label.Text = configurations.XAxis;
            if (!string.IsNullOrEmpty(configurations.YAxis))
                plot.Axes.Left.Label.Text = configurations.YAxis;

            string path = Image.GetTempImagePath();
            plot.SavePng(path, configurations.ImageWidth == 0 ? DefaultWidth : configurations.ImageWidth, configurations.ImageHeight == 0 ? DefaultHeight : configurations.ImageHeight);
            return new Image(path);
        }
        public static Image ScatterPlotTwoAxes(Vector x, Vector y1, Vector y2, ScatterPlotTwoAxesConfiguration? configurations = null)
        {
            configurations ??= new();

            ScottPlot.Plot plot = new();
            var sig1 = plot.Add.Scatter(x, y1);
            var sig2 = plot.Add.Scatter(x, y2);
            if (!string.IsNullOrEmpty(configurations.Legend1))
            {
                plot.Legend.IsVisible = true;
                sig1.LegendText = configurations.Legend1;
            }
            if (!string.IsNullOrEmpty(configurations.Legend2))
            {
                plot.Legend.IsVisible = true;
                sig2.LegendText = configurations.Legend2;
            }

            if (!string.IsNullOrEmpty(configurations.Title))
                plot.Title(configurations.Title);
            if (!string.IsNullOrEmpty(configurations.XAxis))
                plot.Axes.Bottom.Label.Text = configurations.XAxis;
            if (!string.IsNullOrEmpty(configurations.YAxis1))
            {
                sig1.Axes.YAxis = plot.Axes.Left;
                plot.Axes.Right.Label.Text = configurations.YAxis1;
            }
            if (!string.IsNullOrEmpty(configurations.YAxis2))
            {
                sig2.Axes.YAxis = plot.Axes.Right;
                plot.Axes.Left.Label.Text = configurations.YAxis2;
            }

            string path = Image.GetTempImagePath();
            plot.SavePng(path, configurations.ImageWidth == 0 ? DefaultWidth : configurations.ImageWidth, configurations.ImageHeight == 0 ? DefaultHeight : configurations.ImageHeight);
            return new Image(path);
        }
        public static Image LinePlot(Vector x, Vector y, LinePlotConfiguration? configurations = null)
        {
            configurations ??= new();

            ScottPlot.Plot plot = new();
            if (!string.IsNullOrEmpty(configurations.Legend))
            {
                var s = plot.Add.Scatter(x, y);
                s.LegendText = configurations.Legend;
            }

            if (!string.IsNullOrEmpty(configurations.Title))
                plot.Title(configurations.Title);
            if (!string.IsNullOrEmpty(configurations.XAxis))
                plot.Axes.Bottom.Label.Text = configurations.XAxis;
            if (!string.IsNullOrEmpty(configurations.YAxis))
                plot.Axes.Left.Label.Text = configurations.YAxis;

            string path = Image.GetTempImagePath();
            plot.SavePng(path, configurations.ImageWidth == 0 ? DefaultWidth : configurations.ImageWidth, configurations.ImageHeight == 0 ? DefaultHeight : configurations.ImageHeight);
            return new Image(path);
        }
        public static Image BarChart(Vector values, BarChartConfiguration? configurations = null)
        {
            configurations ??= new();

            ScottPlot.Plot plot = new();
            plot.Add.Bars(values);
            plot.Axes.Margins(bottom: 0); // Tell the plot to autoscale with no padding beneath the bars

            if(!string.IsNullOrEmpty(configurations.Title))
                plot.Title(configurations.Title);
            if (!string.IsNullOrEmpty(configurations.XAxis))
                plot.Axes.Bottom.Label.Text = configurations.XAxis;
            if (!string.IsNullOrEmpty(configurations.YAxis))
                plot.Axes.Left.Label.Text = configurations.YAxis;

            string path = Image.GetTempImagePath();
            plot.SavePng(path, configurations.ImageWidth == 0 ? DefaultWidth : configurations.ImageWidth, configurations.ImageHeight == 0 ? DefaultHeight : configurations.ImageHeight);
            return new Image(path);
        }
        public static Image Histogram(Vector values, HisogramConfiguration? configurations = null)
        {
            configurations ??= new();

            ScottPlot.Plot plot = new();

            ScottPlot.Statistics.Histogram hist = new(min: values.Min(), max: values.Max(), binCount: configurations.HisogramBars, true, false);
            hist.AddRange(values);
            ScottPlot.Plottables.BarPlot barPlot = plot.Add.Bars(values: hist.Counts, positions: hist.BinCenters);
            foreach (var bar in barPlot.Bars)
                bar.Size = hist.BinSize;

            if (!string.IsNullOrEmpty(configurations.Title))
                plot.Title(configurations.Title);
            if (!string.IsNullOrEmpty(configurations.XAxis))
                plot.Axes.Bottom.Label.Text = configurations.XAxis;
            if (!string.IsNullOrEmpty(configurations.YAxis))
                plot.Axes.Left.Label.Text = configurations.YAxis;

            string path = Image.GetTempImagePath();
            plot.SavePng(path, configurations.ImageWidth == 0 ? DefaultWidth : configurations.ImageWidth, configurations.ImageHeight == 0 ? DefaultHeight : configurations.ImageHeight);
            return new Image(path);
        }
        public static Image BubbleChart(Vector x, Vector y, Vector radius, BubbleChartConfiguration? configurations = null)
        {
            if (x.Length != y.Length || y.Length != radius.Length)
                throw new ArgumentException("Unmatching data length.");
            configurations ??= new();

            ScottPlot.Plot plot = new();

            for (int i = 0; i < x.Length; i++)
                PlotBubble(plot, x[i], y[i], radius[i]);

            // To keep bubbles circular
            ScottPlot.AxisRules.SquareZoomOut squareRule = new(plot.Axes.Bottom, plot.Axes.Left);
            plot.Axes.Rules.Add(squareRule);

            if (!string.IsNullOrEmpty(configurations.Title))
                plot.Title(configurations.Title);
            if (!string.IsNullOrEmpty(configurations.XAxis))
                plot.Axes.Bottom.Label.Text = configurations.XAxis;
            if (!string.IsNullOrEmpty(configurations.YAxis))
                plot.Axes.Left.Label.Text = configurations.YAxis;

            string path = Image.GetTempImagePath();
            plot.SavePng(path, configurations.ImageWidth == 0 ? DefaultWidth : configurations.ImageWidth, configurations.ImageHeight == 0 ? DefaultHeight : configurations.ImageHeight);
            return new Image(path);
        }
        public static Image BubbleChart(Vector[] xs, Vector[] ys, Vector[] radii, BubbleChartMultiSeriesConfiguration? configurations = null)
        {
            if (xs.Length != ys.Length || ys.Length != radii.Length)
                throw new ArgumentException("Unmatching data length.");
            configurations ??= new();

            ScottPlot.Plot plot = new();
            var cmap = new ScottPlot.Colormaps.Jet();

            for (int i = 0; i < xs.Length; i++)
            {
                var x = xs[i];
                var y = ys[i];
                var radius = radii[i];

                if (x.Length != y.Length || y.Length != radius.Length)
                    throw new ArgumentException("Unmatching data length.");

                for (int j = 0; j < x.Length; j++)
                    PlotBubble(plot, x[j], y[j], radius[j], cmap.GetColor(i));

                if (configurations.Legends != null)
                {
                    plot.Legend.IsVisible = true;
                    plot.Legend.ManualItems.Add(new ScottPlot.LegendItem
                    {
                        FillColor = cmap.GetColor(i),
                        LabelText = configurations.Legends[i]
                    });
                }
            }

            // To keep bubbles circular
            ScottPlot.AxisRules.SquareZoomOut squareRule = new(plot.Axes.Bottom, plot.Axes.Left);
            plot.Axes.Rules.Add(squareRule);

            if (!string.IsNullOrEmpty(configurations.Title))
                plot.Title(configurations.Title);
            if (!string.IsNullOrEmpty(configurations.XAxis))
                plot.Axes.Bottom.Label.Text = configurations.XAxis;
            if (!string.IsNullOrEmpty(configurations.YAxis))
                plot.Axes.Left.Label.Text = configurations.YAxis;

            string path = Image.GetTempImagePath();
            plot.SavePng(path, configurations.ImageWidth == 0 ? DefaultWidth : configurations.ImageWidth, configurations.ImageHeight == 0 ? DefaultHeight : configurations.ImageHeight);
            return new Image(path);
        }
        public static Image FunnelChart(Vector values, FunnelChartConfiguration? configurations = null)
        {
            configurations ??= new();

            ScottPlot.Plot plot = new();

            // Derive Y positions to the center of each value segment
            double[] ys = Enumerable.Range(0, values.Length).Reverse().Select(y => y + .5).ToArray();
            foreach (var ((upperX, lowerX), y) in values
                .Zip(values.Append(values.Last()).Skip(1))
                .Zip(ys))
            {
                plot.Add.Polygon(
                [
                    new(-upperX, y+.5), // Upper left
                    new(upperX, y+.5),  // Upper right
                    new(lowerX, y-.5),  // Lower right
                    new(-lowerX, y-.5)  // Lower left
                ]);
                var text = plot.Add.Text(upperX.ToString(), new(0, y));
                text.Alignment = ScottPlot.Alignment.MiddleCenter;
            };

            plot.HideGrid();
            // Hide axes but leave their titles visible
            plot.Axes.GetAxes().ToList().ForEach(x =>
            {
                x.FrameLineStyle.IsVisible = false;
                x.SetTicks([], []);
            });

            if (configurations.Labels != null)
                plot.Axes.Left.SetTicks(ys, configurations.Labels);

            if (!string.IsNullOrEmpty(configurations.Title))
                plot.Title(configurations.Title);
            if (!string.IsNullOrEmpty(configurations.XAxis))
                plot.Axes.Bottom.Label.Text = configurations.XAxis;
            if (!string.IsNullOrEmpty(configurations.YAxis))
                plot.Axes.Left.Label.Text = configurations.YAxis;

            string path = Image.GetTempImagePath();
            plot.SavePng(path, configurations.ImageWidth == 0 ? DefaultWidth : configurations.ImageWidth, configurations.ImageHeight == 0 ? DefaultHeight : configurations.ImageHeight);
            return new Image(path);
        }
        #endregion

        #region Standard Charts
        /// <alias>Age Population Chart</alias>
        /// <remarks>Notice a population pyramid devides humans into male and female and might not be a gender neural way to represent diversity within population</remarks>
        public static Image PopulationPyramid(string[] ageGroups, Vector maleData, Vector femaleData, PopulationPyramidConfiguration? configurations = null)
        {
            // TODO: Move age group outside of drawing frame and well aligned on the left side of the drawing area; Might need to use custom text labels for this

            if (ageGroups == null || maleData == null || femaleData == null)
                throw new ArgumentNullException();
            if (ageGroups.Length != maleData.Length || ageGroups.Length != femaleData.Length)
                throw new ArgumentException("Unmatching data length.");
            if (maleData.Any(d => d < 0))
                throw new ArgumentException("Male data contains negative value.");
            if (femaleData.Any(d => d < 0))
                throw new ArgumentException("Male data contains negative value.");
            configurations ??= new();

            // Reverse order
            ageGroups = ageGroups.Reverse().ToArray();
            maleData = maleData.Reverse().ToArray();
            femaleData = femaleData.Reverse().ToArray();

            ScottPlot.Plot plot = new();
            // Male group bar data
            CreateDataBars(maleData, plot, true, configurations);
            // Female group bar data
            CreateDataBars(femaleData, plot, false, configurations);

            // Set display range
            int fontSize = configurations.LabelFontSize;
            float margin = MeasureWidth(ageGroups.OrderByDescending(s => s.Length).First(), fontSize);
            double maleMax = maleData.Max();
            double femaleMax = femaleData.Max();
            double absMax = System.Math.Max(maleMax, femaleMax);
            double additionalPadding = absMax / 4; // Notice charts autoamtically adjusts scale based on actual data being plotted; We use 1/4 for additional padding
            plot.Axes.SetLimitsX(-maleMax - margin - additionalPadding, + femaleMax + margin / 2); // Include extra margin to account for label
            plot.Layout.Frameless();
            plot.HideGrid();
            // Title
            if (!string.IsNullOrEmpty(configurations.Title))
                plot.Title(configurations.Title);
            // Labels
            for (int i = 0; i < ageGroups.Length; i++)
            {
                string item = ageGroups[i];
                ScottPlot.Plottables.Text text = plot.Add.Text(item, -maleMax - additionalPadding, i * (configurations.BarSize + configurations.BarGap));
                text.FontSize = fontSize;
                text.LabelBold = true;
                text.LabelFontColor = ScottPlot.Colors.Black;
                text.LabelAlignment = ScottPlot.Alignment.MiddleRight;
            }
            // Draw division line
            plot.Add.VerticalLine(0, 1, ScottPlot.Colors.Black);

            // Save result
            string path = Image.GetTempImagePath();
            plot.SavePng(path, configurations.ImageWidth, configurations.ImageHeight);
            return new Image(path);

            // Helper
            static void CreateDataBars(Vector data, ScottPlot.Plot plot, bool male, PopulationPyramidConfiguration configuration)
            {
                ScottPlot.Plottables.BarPlot barPlot = plot.Add.Bars(male ? data.Select(d => -d).ToArray() : data);
                int index = 0;
                foreach (ScottPlot.Bar bar in barPlot.Bars)
                {
                    // Set position and width
                    bar.Size = configuration.BarSize;
                    bar.Position = index * (configuration.BarSize + configuration.BarGap);

                    // Set label
                    bar.Label = System.Math.Abs(bar.Value).ToString();
                    index++;
                }
                // Customize label style
                barPlot.ValueLabelStyle.Bold = true;
                barPlot.ValueLabelStyle.FontSize = 18;
                barPlot.Horizontal = true;
                barPlot.LegendText = male ? "Male" : "Female";
            }

            static float MeasureWidth(string text, int fontSize)
            {
                return new ScottPlot.LabelStyle()
                {
                    Text = text,
                    FontSize = fontSize
                }.Measure().Width;
            }
        }
        #endregion

        #region Node Graphs
        public static Image NodeGraph()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Helpers
        private static ScottPlot.Color Convert(Parcel.Types.Color color)
            => ScottPlot.Color.FromHex(color.ToString());

        private static void PlotBubble(ScottPlot.Plot plot, double x, double y, double radius, ScottPlot.Color? color = null)
        {
            var c = plot.Add.Circle(
                xCenter: x,
                yCenter: y,
                radius: radius
            );
            if (color != null)
                c.LineColor = color.Value;
            c.FillStyle.Color = c.LineColor.WithAlpha(.5);
        }
        #endregion
    }
}
