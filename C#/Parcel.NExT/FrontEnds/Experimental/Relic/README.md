# Relic (Express Parcel Windows Desktop Frontend)

A quick and limited C# WPF based graph editor for Windows Desktop while we are still working on the full version in NodeJS. This is based on [NodeNetwork](https://www.nuget.org/packages/NodeNetwork/) rather than Nodify like the V1 Prototype for its API simplicity and program driven pattern.

Main reference:

* https://wouterdek.me/NodeNetwork/doc

## Technical Notes

* The use of a node GUI library is very light and we didn't need any sort of NodeNetworkToolkit and built-in interactivity because we have our own evaluation framework and engine setup.
* The MVVM model is very unpleasant and causing issue again here: NodeNetwork has no event mechanism and it's not possible to raise events based on node selection - and update properties panel in an easy (lean-code) way.