using DynamicData;
using NodeNetwork.ViewModels;
using Relic.Models;
using System.Windows;

namespace Relic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Construction
        public MainWindow()
        {
            InitializeComponent();
            CreateView();
        }
        #endregion

        #region Events

        #endregion

        #region Routines
        private void CreateView()
        {
            InitializeComponent();

            // Create nodes
            NodeViewModel node1 = new()
            {
                Name = "Node 1"
            };
            NodeInputViewModel node1Input = new()
            {
                Name = "Node 1 input"
            };
            node1.Inputs.Add(node1Input);

            ParcelNodeViewModel node2 = new("Node 2");
            NodeOutputViewModel node2Output = new()
            {
                Name = "Node 2 output"
            };
            node2.Outputs.Add(node2Output);

            // Add nodes to network
            var network = new NetworkViewModel();
            network.Nodes.Add(node1);
            network.Nodes.Add(node2);

            // Assign the viewmodel to the view.
            networkView.ViewModel = network;
        }
        #endregion
    }
}