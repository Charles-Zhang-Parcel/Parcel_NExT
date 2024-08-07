﻿using Parcel.CoreEngine.Document;
using Parcel.CoreEngine.Service.CoreExtensions;

namespace Parcel.CoreEngine.Service.UnitTests
{
    public class BasicProceduralLogicTest
    {
        /// <summary>
        /// This test is not thread safe because Console is globally static per process and xUnit cannot handle parallel testing of functions safely
        /// </summary>
        [Fact]
        public void SimplestProceduralLogicWithCustomType()
        {
            // Creates and executes a graph that writes references the final output in a preview node
            ParcelDocument document = CreateGraph();
            document.Execute();
            ParcelNode previewNode = document.MainGraph.MainLayout.Placements.Single(n => n.Node.Target == "Preview").Node!;
            Assert.Equal(5 + 12 + 15, (double)document.NodePayloads[previewNode].PayloadData.First().Value);
        }

        #region Routines
        private ParcelDocument CreateGraph()
        {
            // Set graph variable
            ParcelNode n1 = new("{SetVariable}", new Dictionary<string, string>
            {
                { "name", "Test Variable" },
                { "value", "5" },
            });

            // Get graph variable
            ParcelNode n2 = new("{GetVariable}", new Dictionary<string, string>
            {
                { "name", "Test Variable" },
            });

            // Make number literal
            ParcelNode n3 = new("Literal", "Parcel.CoreEngine.Primitives.Number", new Dictionary<string, string>
            {
                { "value", ":12" },
            });

            // Perform mathamtical computation
            ParcelNode n4 = new("Result", "Parcel.CoreEngine.Primitives.Number.Add");
            n4.AddInput("A", "$Test Variable");
            n4.AddInput("B", ":15");
            n4.AddInput("C", "@Literal");

            // Hook up preview node
            ParcelNode n5 = new("{Preview}");
            n5.AddInput("source", "@Result.value");

            // Create document
            ParcelDocument document = new();
            var graph = document.MainGraph;

            // Add nodes
            ParcelNode[] nodes = [n1, n2, n3, n4, n5];
            for (int i = 0; i < nodes.Length; i++)
            {
                ParcelNode node = nodes[i];
                document.AddNode(graph, node, new System.Numerics.Vector2(i * 50, i * 50));
            }

            return document;
        }
        #endregion
    }
}
