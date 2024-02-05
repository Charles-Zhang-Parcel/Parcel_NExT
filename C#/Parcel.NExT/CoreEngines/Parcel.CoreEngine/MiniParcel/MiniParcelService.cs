using Parcel.CoreEngine.Document;
using Parcel.CoreEngine.Helpers;

namespace Parcel.CoreEngine.MiniParcel
{
    public static class MiniParcelService
    {
        #region Interface
        public static ParcelDocument Parse(string scripts)
            => Parse(scripts.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
        public static ParcelDocument Parse(string[] lines)
        {
            ParcelDocument document = new();

            ParcelGraph graph = document.MainGraph;
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                if (trimmedLine.EndsWith(':'))
                {
                    graph = new ParcelGraph(trimmedLine.TrimEnd(':'));
                    document.Graphs.Add(graph);
                }
                else
                {
                    ParcelNode node = ParseNode(document, line);
                    document.AddNode(graph, node, new System.Numerics.Vector2());
                }
            }
            return document;
        }
        /// <summary>
        /// Get raw fully expanded node definition from currently available context
        /// </summary>
        public static ParcelNode ParseAsNode(ParcelDocument document, string miniParcelExpression)
        {
            return ParseNode(document, miniParcelExpression);
        }
        #endregion

        #region Routines
        private static ParcelNode ParseNode(ParcelDocument document, string line)
        {
            // TODO: Add attribute resolution by utilizing current document context
            string[] parts = line.SplitCommandLineArguments();
            return new ParcelNode(parts.First(), Enumerable.Range(0, parts.Length - 1).ToDictionary(i => $"${i}", i => parts[i + 1]));
        }
        #endregion
    }
}
