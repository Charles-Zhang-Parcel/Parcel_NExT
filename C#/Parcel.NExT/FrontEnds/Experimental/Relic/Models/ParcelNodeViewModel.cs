using DynamicData;
using NodeNetwork.Toolkit.ValueNode;
using NodeNetwork.ViewModels;
using NodeNetwork.Views;
using ReactiveUI;
using System.Reactive.Linq;

namespace Relic.Models
{
    internal class ParcelNodeViewModel: NodeViewModel
    {
        #region Construction
        public ParcelNodeViewModel(string name)
        {
            Name = name;

            InputPin = new ValueNodeInputViewModel<string>()
            {
                Name = "Name"
            };
            Inputs.Add(InputPin);

            OutputPin = new ValueNodeOutputViewModel<string>()
            {
                Name = "Text",
                Value = this.WhenAnyObservable(vm => vm.InputPin.ValueChanged)
                    .Select(name => $"Hello {name}!")
            };
            Outputs.Add(OutputPin);
        }
        /// <summary>
        /// ReactiveUI requires a static constructor to register the view with the viewmodel
        /// </summary>
        static ParcelNodeViewModel()
        {
            // TODO: Study how ReactiveUI works
            Splat.Locator.CurrentMutable.Register(() => new NodeView(), typeof(IViewFor<ParcelNodeViewModel>));
        }
        #endregion

        #region Node Values
        public ValueNodeInputViewModel<string> InputPin { get; }
        public ValueNodeOutputViewModel<string> OutputPin { get; }
        #endregion
    }
}
