# Parcel (General) Development Comprehensive TODO List

<!-- Migrate this to ADO! -->

This is the single-place reference for all TODO items for Parcel development.

Also consult: 

* (Parcel V7): [Issues](https://github.com/Charles-Zhang-Parcel/ParcelV7/issues), 
* (Parcel V7) [Discussion on Features](https://github.com/Charles-Zhang-Parcel/ParcelV7/discussions/categories/ideas)
* Organization [README](https://github.com/Charles-Zhang-Parcel/.github/blob/main/profile/README.md)

```mermaid
mindmap
  root((Parcel))
    (Development Aspects)
        [Core]
            Standard/Specification
            V7 Frontend
            Evaluation Engine
        [Peripheral]
            Documentation
            Usage Tutorials
        [Functionality]
            Domain-Specific Libraries
    (Observational and Dated Notes)
        [PENDING LOCATION - Use ADO Wiki for now]
```

## Current(<3)

Current in-progress items:

- [ ] (Management) Org and repos re-organization, design notes migrations
- [ ] (Parcel V7) New open standard and design specification draft
- [ ] (Experiment) Preliminary visual design, functional Electron implementation, and modular/plugin architecture

## Personal Items

### Current as of Feb 2024

(Consider organize them to ADO)

Core Engine: 
* Common conversion - from types to types. string/obj, string - obj, json, data grid.
* Dependency solver

(Investigate running godot as dedicated headless server on Linux with replication - may be supported out of the box)

Implement Gospel native Process run node.
Node Browser -> Library Browser -> Focus on Custom Node
Process Management - Run Process node: Standard Input, Command, Arguments, Standard Output, Standard Error

(Demo) Editor Benchmark
(Macro nodes) Populate 100 Nodes, Populate 1000 nodes (Interconnected)

Allow nodes to be switched as Headless or Header format - aka. this display is node agnostic.
Make a mock node for SQL Builder (single-node), provide: Select item list, From table list, where clause, on clause

(Consider putting to POS) Run/Debug: Like Execute Node context menu, runs one by one using reflection.
Execute: Run optimized version and auto-planned implementation, might not be fully debuggable/observable. Though code/execution plan. will be shown.

Minimal implementation for presentation: ReadCSV (with many options), ComputeStats (with varying outputs, including All pin), and Preview node (with toggle between table, JSON, Tree, Field Map formats.
ReadCSV
ChooseFile
ComputeTableStats

(Gospel) RMB Menu:
Primitives: Number, String and Bool are all implemented as primitives; 
Operators: +-/* are all implemented as "operators" and handled directly in Gospel front-end WHEN only primitives are connected as inputs.

(Parcel) Implement Demo.Exec and see how we might make it work.

F11: Display_Hide_All_UI
F12: Display_Presentation_Mode (Lock node movement and resizing and disables all other shortcuts)

Test table TreeItem set editable
Node Properties Panel: Add action - Copy Unique Name (can place it directly near header bar) (This is useful for technical use, e.g. assigning attributes as references without exposing as Input pin)
Gpspel measure and log node run time.

Guess game node (single input, text print output, no payload)

Work on AmaEngine
Stated endpoint: CreateNode, Update Attributes.

(Gospel) Graph Editor (below action tray)
Grapp runtime: Hybrid, Reflection,  C# (Code), Python, Logic, Functional.
Tool tip: Picking a graph run time restricts available nodes on that runtime.

(Parcel)
Save payload to node
Implement Preview node "Source" input
Hook on_connected() events for all nodes
Implement specialized Preview node for displaying table content upon incoming connection with valid payload
When payload is updated, call (only) immediately next connected nodes on_incoming_data_updated event

(POS, Appendix) Node Attribute
(Tentative) Anything after ~ in node attribute names should be intercepted as comments - front-end might display those as tooltips. At least, it should be implemented as a reserved feature.
(Section) ## Naming Conventions
All node attribute follow JSON and GDscript naming convention aka. snake case. All C# modules should use proper C# convention, and Python modules vice versa. A strictly followed naming conversion will be implemented and handled by the backend for missing attributes when mapping to function/type method calls, and thus following such convention when writing libraries are advised. Node names are PascalCase.
All nodes and pins for front-end with have proper "Human Natural Language" format with proper spacing.
(Design Decision, #20240213) Attribute level comments might make node graphs too verbose. An alternative is to provide a parsed "Style-sheet" that "drops-in" providing archetype-like functionality, adding comments through layering in a tartget-type-attribute addressing schemes. Either way, having ~as reserved special format is a good thing.

### Current as of 20240331

Casual progress:
* PSL and functional overall processing pipeline (Ama engine)
* DataTable: Provide Change Size, Change Headers with dedicated pop-ups.
* String-based Array Node

Gospel frontend:
* Exception display (backend-frontend communication)
* Notification push

Gosple Nodes:
* Might want to provide Data Table and Spreadsheet an input of DataGrid which ONLY when changed populates values. (And might want to automatically disconnect?)

Core engine and usability:
* Replace Notebook with Parcel implementation for Singleton context Pure (by simply providing some nodes where we can type the snippets)
* Backend to front-end communication (Create node, create payload, update attributes). When this is done we will be able to load documents.

### Unofficial Tasks as of 20240510

Those are legit tasks but we haven't allocated much thought or scheduled them for execution. Pending moving to ADO when tasks are official.

(Feature) Preview Node
(User Story) Add Audio Mode (Per DSP DSL)
(User Story) Add Video Mode (Per Video Editing DSL)
(User Story) Add Viewport (2D) Mode (Per 2D Graphics Novel Game DSL)
(User Stort) Add Viewport (3D) Mode (Per Procedural Modeling DSL)

(ADO) (Story) Investigate Advanced Custom GraphEdit Drawing
https://www.reddit.com/r/godot/comments/17apon5/possible_to_animate_edges_in_a_graphedit_node/ Might need to touch C++ source, which is not too much, but we must ensure Windows/Mac/Linux/Web builds still works and we are able to easily maintain forward compatibility with new engine versions, in general, avoid excessive changes.
Put results in Investigation folder.
- Study behaviors
- Summarize in design document
- Create demo setup

(ADO) Marketing
-> Personel Marketing
-> Name cards: Charles Zhang (CEO)
Charles Zhang
CEO
Methodox, Inc.
Mobile: +1 (647) 382 6850
Website: www.methodox.io
Email: Charles@methodox.io

(ADO) Automatic "MakeXXXX" for any type (class, structure, record) that have a public constructor. THIS IS HANDLED BY A SINGLE MakeTypeInstance node per standard or otherwise can be specially handled by runtime implementations.

(ADO) Investigate ElsaWorkflows (C#) and see whether we can: 1) build entire Methodox just by providnig DSLs for it; 2) Use it as a readily-usable cloud-ready front-end; 3) Make use of some of the backend runtime engines offered by this platform. Notice Elsa is MIT license so we should be fine.

(ADO) Research: Survey, Test, Practice potentially useful JS runtimes for C# - must make sure it's cross-platform and very easy to use and ideally robust and can consume existing NodeJS librarires. See: https://github.com/sebastienros/jint https://andrewlock.net/running-javascript-in-a-dotnet-app-with-javascriptengineswitcher/ Notice we are still pending an architecture design for using JS for front-end purpose.

(ADO) Feature: Allow limiting available nodes/libraries/function sets for any given graph. Useful for education or delivering to end-user purpose. Notice graphs users can still invoke advanced nodes but mostly no additional (selected types) nodes can be added. 

(ADO) Gospel "Publish" Document
Instead of using the exact same source file (extension), we use Publish to explicitly publish a read-only/presentation-mode/application from current graph. This gives a more streamlined experience. The published file has a different file extension but is otherwise exactly the same as source Parcel file. It's just the meta-data there disallows editing when opened in Gospel. People with text editor can of course directly edit the source file to switch such toggles. On the other hand, the Publishing process may allow stripping off all irrelevant design time stuff including nodes and graphs (and keeps generated codes only) for execution purpose.

(ADO) Support binary file compression with standard Oz4 format.
(ADO) Support binary file compression with password protection (content encryption) especially during Export time (exported binary presentation file). Notice to implement this properly we need to guarantee during program runtime everything is in memory - there is no intermediate dump files that might leak crucial data/information.

(ADO, Package) Support Remapping: Larger packages like DataAnalytics might provide extension methods which are best addressed in some more well-recognized namespaces. E.g. Parcel.DataGrid is a zero-dependency package, while Parcel.Excel provides excel reading and has nothing to do with DataGrid, but `Parcel.DataAnalytics` implemented `Parcel.DataAnalytics.DataGrid.DataGridExtensions.ReadExcel` - we wish to address it all under `DataGrid.Sourcing.XXX` "endpoint". Remapping can only be provided by sourcing package. (Remark) Might not be necessary since node typenames are usually displayed just as `type.method`.
(We had some notes on this, pending find where we mentioned it in POS.)

(ADO, Feature) Must provide cross-file searching capabilities
This is especially true since most of the graphs are going to be stored as binary. Very useful/essential when looking for stuff. Ideally Parcel can automatically detect ALL graph files on local system without needing user to have Everything. 
We can achieve that through two mechanisms:
1. Automatically cache and save locations of all opened graph files in app data
2. Allow specifying "search" folder and "including/exclusion" rules
3. Expose searching directly as nodes (part of Parcel NExT Services package)
Can alternatively also expose as a standalone tool: passing in an array of such files and search through all of them.

### Unofficial Tasks as of 20240616

For "feature requests" (our own ideas), we might need a dedicated place for such feature proposals - those are internal implementation ideas and should not be shared with public. (In general, this is just part of stack/TODO, but more towards "proposal/idea" but not formal; Could be managed with ADO - just mark as NOT DO if we don't accept it)

(ADO) Gospel (Beta Test) Add Node-Level User Engagement Buttons (RMB menu)
* Get Help: Jump to node doc with examples (online or in-app)
* Make Suggestion: Jump to contact form/issue/forum (online or in-app)

(ADO, Feature Proposal) Presentation Mode
Ordered (specific) control/component interactive highlighted focused presentation.
e.g. We have a expense hierarchy, and we want to present it. If we RMB on it, it enters a PPT-slide like interface with a dedicated header and maybe a single paragraph and some background and other decorative elements PLUS the element itself rendered in a suitable format for large screen presentation, and we can click on it just to see it expanding and contents chsnge.
Applicable only to such "content" type controls (as in a PowerBI dashboard), and is configured using dedicated nodes.

(ADO, Feature proposal) Like PowerBI, but more like PPT - imagine being able to interactively filter and show things update in real time while having a PPT presentation. This is slightly different from mere presentation mode (focused area) but can essentially be the same if we:
1. In presentation mode enable "Hide All Other Nodes" when doing focused presentation areas (such areas can be drawn just like group box)
2. In presentation mode enable all kinds of auxiliary adornments like background, page atyling, page number etc. when switching between focuses areas - just like when jumping between slides.

(ADO, Experiment, #New, #Important, @New Skill) Experiment with C# to Graph Conversion (using syntax analyzer), see how far we can go.

(ADO, Idea) Graph Native: Map Edit
Tags: #PENDING
(Pending more investigation)
One thing that is made possible in a graphical environment with code generation is we can do the following:
```c#
TType[] objects; // Some strongly typed objects
Dictionary<string, string> mapping; // Some values we wish to replace
// Pesudocode: Map(objects, "ObjectProperty", mapping)
foreach(var o in objects)
    o.ObjectProperty = mapping.TryGetValue(o.ObjectProperty, out string value) ? value : o.ObjectProperty;
```
So instead of writing that long for-loop, we could offer such a construct that implements such a functionality through automatic mechanism.
(Idea from) A nice thing about pandas Excel, DataFrame, SQL, or Lisp is that they are "raw" or "minimal" - despite being tedious, there is ever one single data type to deal with: cell, table, and list. On the contrary, we are often facing the question whether certain operations should be implemented as Csv, Excel, Parcel DataGrid, Vector, or Matrix.

(ADO, Front-end, Gospel) (Gospel RMB/tab menu feature)
"How-to" For easy onboarding programming.

(ADO) Investigate Debugging for Code Generation
Notice we will likely need one more layer on top of syntax analysis because we need to map execution points to the node levle.
Notes: Study Visual Studio Code's C# extension and see how it works.
Reference:
https://stackoverflow.com/questions/4236303/i-want-to-make-my-own-c-sharp-debugger-how-would-one-do-that-what-tools-shoul