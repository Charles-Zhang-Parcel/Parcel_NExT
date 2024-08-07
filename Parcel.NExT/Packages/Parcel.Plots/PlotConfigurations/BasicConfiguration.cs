﻿/*Try avoiding complex inheritance hierarchy; Keep it single-level*/

using Parcel.Types;

namespace Parcel.Graphing.PlotConfigurations
{
    /// <summary>
    /// Provides most common configurations for most if not all plot types
    /// </summary>
    /// <remarks>
    /// Notice all those values will be provided on the GUI, likely as primitive inputs, so they cannot be null.
    /// </remarks>
    /// <references>
    /// Default colors: https://scottplot.net/cookbook/4.1/colors/
    /// </references>
    public class BasicConfiguration
    {
        public int ImageWidth { get; set; } = 600;
        public int ImageHeight { get; set; } = 400;
    }

    #region Standard
    public sealed class DrawVector2DConfiguration : BasicConfiguration
    {
        public int ArrowThickness { get; set; } = 2;
        public Color ArrowColor { get; set; } = Colors.Red;
    }
    public sealed class NumberDisplayConfiguration : BasicConfiguration
    {
        public int DecimalPlaces { get; set; } = 2;
        public Color NumberColor { get; set; } = Color.Parse("#1F77B4FF");
        public Color TitleColor { get; set; } = Color.Parse("#192a39FF");
    }
    public sealed class ScatterPlotConfiguration : BasicConfiguration
    {
        public string Title { get; set; } = string.Empty;
        public string XAxis { get; set; } = string.Empty;
        public string YAxis { get; set; } = string.Empty;
        public string Legend { get; set; } = string.Empty;
        public Color Color { get; set; } = Color.Parse("#1F77B4FF");
    }
    public sealed class ScatterPlotMultiSeriesConfiguration : BasicConfiguration
    {
        public string Title { get; set; } = string.Empty;
        public string XAxis { get; set; } = string.Empty;
        public string YAxis { get; set; } = string.Empty;
        public string[]? Legends { get; set; } = null;
        public Color[]? Colors { get; set; } = null;
    }
    public sealed class LinePlotConfiguration : BasicConfiguration
    {
        public string Title { get; set; } = string.Empty;
        public string XAxis { get; set; } = string.Empty;
        public string YAxis { get; set; } = string.Empty;
        public string Legend { get; set; } = string.Empty;
    }
    public sealed class ScatterPlotTwoAxesConfiguration : BasicConfiguration
    {
        public string Title { get; set; } = string.Empty;
        public string XAxis { get; set; } = string.Empty;
        public string YAxis1 { get; set; } = string.Empty;
        public string YAxis2 { get; set; } = string.Empty;
        public string Legend1 { get; set; } = string.Empty;
        public string Legend2 { get; set; } = string.Empty;
    }
    public sealed class BarChartConfiguration : BasicConfiguration
    {
        public string Title { get; set; } = string.Empty;
        public string XAxis { get; set; } = string.Empty;
        public string YAxis { get; set; } = string.Empty;
    }
    public sealed class HisogramConfiguration : BasicConfiguration
    {
        public string Title { get; set; } = string.Empty;
        public string XAxis { get; set; } = string.Empty;
        public string YAxis { get; set; } = string.Empty;
        public int HisogramBars { get; set; } = 10;
    }
    public sealed class BubbleChartConfiguration : BasicConfiguration
    {
        public string Title { get; set; } = string.Empty;
        public string XAxis { get; set; } = string.Empty;
        public string YAxis { get; set; } = string.Empty;
    }
    public sealed class BubbleChartMultiSeriesConfiguration : BasicConfiguration
    {
        public string Title { get; set; } = string.Empty;
        public string XAxis { get; set; } = string.Empty;
        public string YAxis { get; set; } = string.Empty;
        public string[]? Legends { get; set; } = null;
    }
    public sealed class FunnelChartConfiguration : BasicConfiguration
    {
        public string Title { get; set; } = string.Empty;
        public string XAxis { get; set; } = string.Empty;
        public string YAxis { get; set; } = string.Empty;
        public string[]? Labels { get; set; } = null;
    }
    public sealed class PopulationPyramidConfiguration : BasicConfiguration
    {
        public string Title { get; set; } = string.Empty;
        public double BarSize { get; set; } = 0.8;
        public int LabelFontSize { get; set; } = 12;
        public Color MaleBarColor { get; set; } = Color.Parse("#1F77B4FF");
        public Color FemaleBarColor { get; set; } = Color.Parse("#FF7F0EFF");
        public double BarGap { get; set; } = 0.1;
    }
    #endregion

    #region Data Grid Integration
    public sealed class TableDisplayConfiguration : BasicConfiguration
    {
        public Color Color { get; set; } = Color.Parse("#1F77B4FF");
    }
    #endregion
}
