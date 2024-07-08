# Parcel NExT

<!-- Keep it brief. -->

Parcel is a general-purpose visual scripting platform and workflow execution engine. The Parcel NExT platform is an ecosystem surrounding what's known as Parcel Open Standards (POS). This is the current *active* repository for the official reference implementation.

Formerly known as ParcelV7. This repo contains the runtime and other core components for Parcel NExT implementation and some of the less-complicated front-ends. This repo is open-source for general accessibility; More dedicated front-ends are managed in specialized repos.

```mermaid
timeline
title The Parcel Development Timeline
    2022.05
        : First prototype in C#, WPF
    2023
        : Iterative designs
        : Attempted implementation in SFML
        : Emergence of Expresso and Pure
    2024
        : Attempting implementation (frontend and backend) in NodeJS
        : Deprecate JS-based approach and Electron GUI, use Godot for frontend and C# WebSocket for backend
        : Inception of Gospel and Tranquility
    2024.09
        : Methodox Parcel
```

<!-- Including developer (env setup) instructions for new people and new computer setup. -->

# Parcel (V7) - The Workflow Engine

<!-- This is for the website, it's now deprecated, and we should migrate/replace contents with Methodox.io, and manage it in dedicated repo; See MethodoxLandingPageReactWebApp for current official website -->
<!-- This is the entry point of public facing Github Pages -->

Welcome to Parcel!
Github: https://github.com/Charles-Zhang-Parcel

<!-- Consider removing the first timeline - move to personal note; The second timeline can be kept -->
```mermaid
timeline
title Parcel.NExT Development Timeline
    2024.01 Week 3
        : Inception
        : ADO
    2024.01 Week 4
        : Core implementation - text serialization
        : MiniParcel
    2024.01 Week 5
        : Gospel
```

```mermaid
timeline
title Events
    2024.02.25 First Parcel DevDay
    2024.08.30 Prototype-ready Announcement
    2024.09.01 Methodox Parcel
    2024.12.25 Release V1
```

# Parcel - the Visual Programming Platform

Its direct "competitors" are Microsoft Excel, MathWorks MATLAB, and Python - though different tools are designed for different things. Parcel is designed for non-programmers, but it's also intended to be as extensible as possible 😆 My intention is to create a new kind of data analytics role which I shall call "Technical Analyst", which is like in between a usual data analyst and someone who is also concerned with higher-level insights (like manager). It's a bridge between lower level implementations (e.g. in Python or C#) and higher level analytics goals.

* Inspired by Houdini and Grasshooper, as a replacement to Excel, Ipython, and Matlab - it's a graphical platform designed for interactively designing, debugging and executing workflow-based processes, enabling efficient iterations of data operations;
* I'd like to think of Parcel as Unreal Engine Blueprint - but not just limited to making games.
* Currently working on second iteration, with full Python interoperability (through Cpython)

Parcel is an advanced lightweight general-purpose functional/procedural node based visual scripting platform/workflow execution engine, along with PSL (Parcel Standard Libraries), POF (Parcel Original Frameworks), PVM (Parcel Virtual Machine) and POS (Parcel Open Stadards) specs, with built-in runtime handling and context awareness for C# and Python, and a text-based scripting DSL (Domain-Specific Language) known as MiniParcel. Parcel is a hybrid between a Fourth generation languages (4GL) and Third generation languages (3GL), meaning it operates on high level domain objects but is still largely algorithm based despite declarative features. Here is a video demo of an [early prototype in C# implemented using WPF (2022)](https://youtu.be/yEHaf_4y5AE). It's simple enough to be used by non-programmers but also extensive enough to be used by experienced developers. The primary focus of Parcel at know is shell automation and data analytics.

Parcel is specified by a [suite of standards](https://github.com/Charles-Zhang-Parcel/Parcel_NExT/tree/main/Parcel%20Open%20Standard) and is provided as a [reference implementation](https://github.com/Charles-Zhang-Parcel/Parcel_NExT/tree/main/C%23).

Parcel is a workflow-engine designed to make office life and data analytics easier with better visualization, and with a stretch - maybe even fun! It's direct "competitors" are Microsoft Excel, MathWorks MATLAB, and Python - though different tools are designed for different things. Parcel is designed for non-programmers, but it's also intended to be as extensible as possible 😆 My intention is to create a new kind of data analytics role which I shall call "Technical Analyst", which is like in between a usual data analyst and someone who is also concerned with higher-level insights (like manager). It's a bridge between lower level implementations (e.g. in Python or C#) and higher level analytics goals.

* Inspired by Houdini and Grasshooper, as a replacement to Excel, Ipython, and Matlab - it's a graphical platform designed for interactively designing, debugging and executing workflow-based processes, enabling efficient iterations of data operations; 
* Currently working on second iteration, with full Python interoperability (through Cpython)

The key problem Parcel trying to solve is the "growth" problem of prototype analytical scripts - you will know what I mean if you've started something simple in Python, and a few months later is becomes a monster that you don't want to touch. Beginners and new programmers in Python are especially prone to this problem, because they thought Python is easy to learn, only to learn that it's not that easy to write good Python good after all.

|Repo|Description|Status|
|-|-|-|
|[Parcel V1 Prototype](https://github.com/Charles-Zhang-Parcel/Parcel_V1_Prototype)|Original functional prototype with dashboard function targeting Windows and .Net 6.|Demo Only; Active (Maintenance Only)|
|[Parcel V1](https://github.com/Charles-Zhang-Parcel/Parcel_V1)|Original prototype of Parcel without the dashboard (web server) part. This serves as functional demo of the concept. Implemented using C# in WPF.|Active (Maintenance Only); All design documents will be migrated|
|[Parcel NExT](https://github.com/Charles-Zhang-Parcel/Parcel_NExT)|Current active development of the "official" version of Parcel. Previously known as "Parcel V7".|Active (Development)|
|Prototypes|||
|[ParcelTasks](https://github.com/Charles-Zhang-Parcel/ParcelTasks)||In-migration, will eventually delete|

Optn source strategy: core runtimes and engine and specifications are all open source; major front-ends are closed source but free to use. Standard libraries all have open specifications but implementation may be closed source. DSLs are usually closed source. All software are generally free to use for individuals (check license for each) and charges licensing fees for commercial use with signficant revenues.

## Overview

Active repo: https://github.com/Charles-Zhang-Parcel/Parcel_NExT (Formerlly known as "Parcel V7")

(This software is still in-dev; When it's ready for production use I will open the private repos)

* Official Website: https://parcel.totalimagine.com
* Forum: [Github Discussion](https://github.com/Charles-Zhang-Parcel/Parcel_NExT/discussions)
* Official (End-User) Public Wiki: https://github.com/Charles-Zhang-Parcel/Parcel_NExT/tree/main/Wiki
* Itch.io Release: https://charles-zhang.itch.io/parcel
* YouTube Channel: [Charles Zhang](https://www.youtube.com/playlist?list=PLuGKdF2KHaWF6V9-eUWfelc5ZAoHCUbej) <!--In the forseeable future I will be the only one developing this, and to save management effort, I will just take all the glory and manage it under my own accounts.-->
* Telegram: [Telegram Group](https://t.me/+zFs-woUjnLVlNjUx)
* Training Series: PENDING (YouTube)
* Azure DevOps: https://dev.azure.com/ParcelEngine/Parcel

Parcel is completely free for non-commercial use and small businesses!🎉

## Parcel Developer Hub

Provides general purpose usage manual, [basic wiki](https://github.com/Charles-Zhang-Parcel/Parcel_NExT/tree/main/Wiki), general [Q&amp;A discussion](https://github.com/Charles-Zhang-Parcel/Parcel_NExT/discussions/categories/technical), and [bug](https://github.com/Charles-Zhang-Parcel/Parcel_NExT/issues)/[request firing](https://github.com/Charles-Zhang-Parcel/Parcel_NExT/discussions/categories/ideas).

This is mostly targeting developers. For end-users, use [Forum](https://github.com/Charles-Zhang-Parcel/Parcel_NExT/discussions) and [Official Wiki](https://github.com/Charles-Zhang-Parcel/Parcel_NExT/tree/main/Wiki).

Look into ~~ParcelTasks and ParcelDesignDocuments for more information~~ (migrating to Azure DevOps).

## A Note on NExT - History of Parcel🗓️

```mermaid
timeline
title Parcel Iterations before Main Version
  2022.05
    : Original Prototype
  2022
    : Parcel V1
  2023
    : Parcel V5
    : Expresso
    : Pure
  2024
    : Parcel V7, latter renamed to NExT
    : First Parcel DevDay
    : (Expected) First full release EoY
  2024.09
    : Methodox Parcel
```
