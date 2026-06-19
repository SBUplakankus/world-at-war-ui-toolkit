<div align="center">

# Call of Duty: World at War in UI Toolkit

![Unity](https://shieldcn.dev/badge/Unity-6000.4.8f1-ececec.png?variant=outline&logo=unity)
![C](https://shieldcn.dev/badge/C%23-9.0-239120.png?variant=outline&logo=csharp)
![MIT License](https://shieldcn.dev/badge/License-MIT-blue.png?variant=outline)

Developed to showcase how I would handle a large scale UI System in Unity 6.

<br/>

<img src="./Media/hero_gif.gif" alt="Navigation Demo" width="700"/>

<br/>

</div>

---

## About

I have spent the past 7 months doing R&D with UI Toolkit and UI Programming both at work and at home and this system is the culmination of that.

UI Toolkit resources and examples are scarce, so I based the system on both my work on .NET tools in Avalonia, and how Epic Games handle UI through their Lyra Sample Project.

UWhile it is receiving some great updates and improvements in every iteration of Unity 6, there's no code generation for compile-time safe bindings. Without source generators like CommunityToolkit.Mvvm, implementing full MVVM requires manual wiring that cancels out the pattern's benefits.

The closest design pattern comparison is the HTML, CSS and Vanilla JS pattern from the 2000s. The toolkit is very similar to Web Development from that era with some unique game engine integrations and fantastic performance.

I designed the system to be Engine Agnostic so it is almost entirely written in pure C#, UXML and USS with 0 Unity Inspector References or Scriptable Objects. There are only 3 MonoBehaviour classes in the entire system, these being the Root UI Controller MenuLayout, the UI Audio Handler, and the Debug Tool.

I am also creating a YouTube Tutorial series teaching developers how to handle UI in Unity like this where I recreate the initial steps of the system in videos. You can find the repo for that [here](https://github.com/SBUplakankus/ui-toolkit-mini-series).

---

## Architecture

<details>
<summary>Core navigation architecture</summary>

```mermaid
flowchart TB
    UIRouter["UIRouter\nSingleton · plain C#"]

    subgraph Factory["Factory"]
        ViewFactory["ViewFactory\nStatic"]
        Resources["Resources\nViews/{Name}.uxml"]
        ElementsFactory["ElementsFactory\nTyped records"]
        ViewFactory -->|"Resources.Load"| Resources
    end

    subgraph Chrome["Chrome"]
        HeaderController["HeaderController\nTitle · back button"]
        ModalController["ModalController\nOverlay · title bind"]
    end

    subgraph Layout["MenuLayout · MonoBehaviour · UIDocument"]
        ScreenStack["UILayer\nScreen stack"]
        ModalStack["UILayer\nModal stack"]
        UIAudioHandler["UIAudioHandler\nHover · click audio"]
    end

    subgraph Views["Views"]
        BaseView["BaseView\n15 subclasses"]
    end

    UIRouter -->|"delegates"| Layout
    UIRouter -->|"Create&lt;T&gt;()"| ViewFactory
    Layout --> Chrome
    ScreenStack -->|"Stack&lt;BaseView&gt;"| BaseView
    ModalStack -->|"Stack&lt;BaseView&gt;"| BaseView
    BaseView -->|"GetElements()"| ElementsFactory
```

</details>

<details>
<summary>Navigation flow sequence</summary>

```mermaid
sequenceDiagram
    participant User
    participant ActiveView
    participant UIRouter
    participant ViewFactory
    participant Resources
    participant UILayer
    participant NewView

    User->>ActiveView: button click
    ActiveView->>UIRouter: "NavigateTo<T>()"
    UIRouter->>ViewFactory: "Create<T>()"
    ViewFactory->>Resources: Resources.Load (uxml)
    Resources-->>ViewFactory: VisualTreeAsset
    Note over ViewFactory: "Activator.CreateInstance(template)<br/>→ CloneTree() into Root"
    ViewFactory-->>UIRouter: returns view
    UIRouter->>UILayer: ShowScreen → Push(view)
    UILayer->>ActiveView: Deactivate() → UnBind()
    Note over UILayer: Push ActiveView to history<br/>Set NewView as current
    UILayer->>NewView: Activate()
    Note over NewView: GetElements()<br/>Bind()
    Note over ActiveView,NewView: Chrome (header/footer) stays visible throughout
```

</details>

<details>
<summary>View lifecycle state machine</summary>

```mermaid
stateDiagram-v2
    [*] --> Created : new View(template)
    Created --> GetElements : Activate()
    GetElements --> Bind : one tree walk done
    Bind --> Active : events subscribed

    Active --> UnBind : Deactivate() - Another view pushed
    UnBind --> Dormant : ready for reuse
    Dormant --> GetElements : Activate() again - Pop from history
    Dormant --> Disposed : Dispose()
    Active --> Disposed : Clear()

    note right of Created
        CloneTree() into Root.
        View detached from hierarchy.
    end note

    note right of GetElements
        One tree walk.
        ElementsFactory called here.
    end note

    note right of Bind
        Subscribe clicked events.
        Set initial label text.
        Set button enabled states.
    end note

    note right of UnBind
        Unsubscribe clicked events.
        Unregister PointerEnterEvent.
        Prevent stale handlers.
    end note
```

</details>

---

## Features

### Navigation & Routing

**Convention-Based Navigation System:** `UIRouter` is a plain C# singleton where `NavigateTo<T>()` uses the C# type as the destination identifier. No enums, no switch statements, no inspector mappings, no string lookups.

**Zero-Registration Architecture:** A new screen equals two files, `.cs` and `.uxml`. No enum entries to add, no switch cases to extend, no registration calls to write, no ScriptableObjects to assign.

**Convention-Based ViewFactory:** `ViewFactory.Create<T>()` loads `Resources/Views/{TypeName}.uxml` by convention. The class name is the file path. If the UXML doesn't exist, an immediate `Debug.LogError` fires before a null-ref can occur.

**Dual UILayer Stacks:** Two independent `Stack<BaseView>` instances: one for screens, one for modals. Each manages its own lifecycle. Push deactivates the current view and stores it in history. Pop restores the previous view. The two stacks are independent of each other, so modals can stack over any screen without affecting screen state.

**Modal Overlay System:** `ModalController` manages a dedicated `VisualElement` container positioned absolute over the screen area. When no modal is active, the overlay is invisible and input passes through to the screen below. When a modal pushes, opacity goes to 1 and `pickingMode` activates, blocking interaction with the view underneath.

#

### UI Layer Architecture

**Typed Elements Records Pattern:** Every view's `GetElements()` is a single call to `ElementsFactory`, which returns a `sealed record` of strongly-typed `VisualElement` references. 16 records decouple UXML structure from C# logic. Element names live in `Constants/Elements.cs`, not scattered as magic strings across views. C# 9 positional records give immutability, value equality, and zero boilerplate per record.

**Element-Name-to-Constant Integrity:** All UXML element `name` attributes are centralized in `Constants/Elements.cs`. Every `ElementsFactory` method and every USS selector references these constants.

**Persistent Chrome Shell:** The header bar (title, username), footer bar (back button, version), and modal overlay container live outside both stacks. They survive every screen push/pop without ever being deactivated. The back button is bound once by `MenuLayout` and always calls `UIRouter.Instance.Back()`, regardless of which view is active. Views don't own or know about the chrome.

**IScreen Interface for Chrome Binding:** The `IScreen` interface is implemented by any view that needs a chrome title. `HeaderController` and `ModalController` use pattern matching against `IScreen` to set header text without concrete type checks or per-view switch statements.

**Complete View Lifecycle Contract:** `BaseView` defines four phases:

- Constructor -- clones the UXML once
- `Activate()` -> `GetElements()` (one tree walk) + `Bind()` (subscribe events)
- `Deactivate()` -> `UnBind()` (unsubscribe)
- `Dispose()` -- full cleanup, removes from hierarchy

The split between element querying (expensive but one-shot) and event binding (cheap but must be undone) prevents stale handlers and ensures clean teardown.

#

### Styling & Data

**USS Custom Property Theming:** `core.uss` defines a `:root` block of CSS custom properties for all colours, fonts, border radii, and motion values. `screens.uss` and `modals.uss` inherit these tokens. Changing a single variable in `core.uss` propagates to every panel. Theme-wide changes require edits to one file, not per-panel hunting.

**C# 9 Records on .NET Standard 2.1:** `IsExternalInit.cs` polyfill enables sealed positional records in Unity's .NET Standard 2.1 runtime. The entire Elements Records pattern depends on this. Without it, every record would require explicit constructor and property boilerplate.

**USS-Class-Driven Audio:** `UIAudioHandler` listens for `PointerEnterEvent` and `ClickEvent` at the root level. It checks the target element for `audio--hover` or `audio--click` USS classes. Audio assignment happens in UXML/USS. No C# event wiring per button.

#

### Content & Interactions

**Mission Select with Progressive Unlock:** 15-mission select screen driven by `SaveDataManager.CurrentSave.missionsCompleted`. Each button enables when `n < completed`. Hover over any enabled mission button populates a preview panel from `UIResources` dictionaries. The `PointerEnterEvent` registration and unregistration pattern shows proper event lifecycle hygiene across 30 handlers (15 hover + 15 click).

<br/>

<img src="./Media/missions_gif.gif" alt="Mission Select Demo" width="600"/>

<br/>

**Runtime Debug Tool:** F3-toggled `OnGUI` panel attached by `MenuLayout` at startup. Edits `campaignStarted` and `missionsCompleted` on the live save data. Buttons: Save (write JSON), Delete (remove save + reload defaults), New Player (delete + open `SaveNoticeView` modal). Proves the modal system, save-data round-trip, and mission unlock logic work without needing a game loop.

---

## Design Choices

### The Core Idea

It is a very opinionated system due to it being designed for scale and not like anything I have come across for Unity before. In many ways I am fighting against what Unity want to do. In my experience, once you scale up past a small game, having to manage endless inspector references and scriptable object references becomes a nightmare and kills your workflow.

The goal is to keep it entirely in code so programmers can handle the backend, while designers can freely modify the UXML Documents in UI Builder. No need to worry about references, instead there are just strictly defined naming conventions. The tests exist to help verify these so nothing silently fails if mislabelled.

### Boilerplate Code

UI Toolkit is a very fragile system due to everything running off of string queries. I think the Constants, Records and Factory classes are non negotiable. Due to the elements themselves requiring manual binding and unbinding, I believe it is better for each element to have its own unique function instead of putting everything in an array and just iterating through it blindly. If an error occurs, you can immediately go to the exact function and cause.

I value Safety, Scanability, Readability and Separation of Concerns above all else. This means that you will end up with some large views that might look verbose. MissionSelectView for example where each of the 15 buttons are uniquely named and have their own binding and unbinding functions, but once the initial boilerplate is done, working with it is much better than using Button[i] and trying to debug which one isn't working.

### No MVVM or Full DI

It is incredibly easy to get carried away trying to replicate common UI Architecture and Dependency Injection in a large UI Toolkit based system. I think the reality is just that the library isn't developed enough to handle this well, there's no built-in navigation, DI, or view locator. So attempting full MVVM/DI patterns from Web, WPF or Avalonia leads to more overhead than it is worth. You'll end up with a verbose code base where trying to add new views becomes a nightmare of touching 8-9 classes.

The system navigation running off of a UI Router singleton is a necessary evil. It is not modelled off of the cliche nightmare fat controller of old Unity Systems that has been phased out for more Scriptable Object based architecture. Instead, it is based off of the Unreal Engine 5 Subsystems which exist for these exact cases. On a fully developed game I would just fold this into a Service Locator that contains other static classes and singletons.

---

## Testing

**Three Test Suites at Architectural Seams.**

- **ViewFactoryTests:** Validates every view instantiates, activates, deactivates, and disposes. Guards the convention seam.
- **ElementsFactoryTests:** Validates every factory method resolves every expected UXML element. Guards the query seam.
- **UIResourcesTests:** Validates all resource dictionaries populated from `content.json`. Guards the data seam.

---

## Project Structure

<details>
<summary>Click to expand full project tree</summary>

```
Assets/
  UI Toolkit/
    Views/
      LayoutView.uxml               root shell document
    Templates/
      MenuButton.uxml                reusable button widget
      MenuSlider.uxml                reusable slider widget
      Divider.uxml                   reusable divider widget
    Styles/
      core.uss                       design tokens, fonts (52 lines)
      screens.uss                    all screen styles (488 lines)
      modals.uss                     modal overlay styles (129 lines)

  Scripts/
    UI/Core/
      UIRouter.cs                    singleton navigation bus
      UILayer.cs                     stack container for views
      MenuLayout.cs                  bootstrap, owns UIDocument
      HeaderController.cs            chrome title / username
      ModalController.cs             modal overlay visibility
      UIAudioHandler.cs              USS-class-driven audio

    UI/Views/
      BaseView.cs                    abstract lifecycle (4 phases)
      MainMenuView.cs                +14 more screens and modals

    UI/Records/                      16 sealed positional records
    UI/Factories/
      ViewFactory.cs                 convention-based constructor
      ElementsFactory.cs             15 query methods

    UI/Constants/
      Elements.cs                    element name strings (113 lines)
      ScreenNames.cs                 header display strings

    UI/Enums/                        Missions, Difficulty, Audio
    UI/Interfaces/
      IScreen.cs                     HeaderName contract

    Data/
      SaveDataManager.cs             JSON save/load with throttle
      PlayerSaveData.cs              serializable save POCO
      PlayerSettingsData.cs          serializable settings POCO
      ContentData.cs                 serializable content POCOs
      UIResources.cs                 lazy-loaded resource dictionaries
      ArrayExtensions.cs             Random() extension

    Utilities/
      IsExternalInit.cs              C# 9 records polyfill
      DebugTool.cs                   F3 runtime save editor

  Resources/
    Views/                           15 UXML files (loaded at runtime)
    Data/
      content.json                   missions, difficulties, MOTDs
      defaults.json                  default player settings

  Tests/
    ViewFactory/
      ViewFactoryTests.cs            all 15 types create
    ElementsFactory/
      ElementsFactoryTests.cs        all 15 factory methods resolve
      ElementsFactory.Tests.asmdef
    Data/
      UIResourcesTests.cs            resource integrity checks
    Tests.asmdef                     Editor-only test assembly

  UiToolkit.asmdef                   runtime assembly
```

</details>

---

## Demo

https://github.com/user-attachments/assets/460aafd6-12ea-424b-9edf-4004942891b1

---

## Tech Stack

- **Unity** 6000.4.8f1
- **UI Toolkit** (runtime UI Documents in world space)
- **C# 9.0** (sealed records, `IsExternalInit` polyfill for .NET Standard 2.1)
- **USS** (custom properties, `:root` design tokens, font faces)
- **NUnit + Unity Test Framework** (3 test suites, 39 tests)
- **JSON** (save data serialization, content data)

---

## AI Usage

### Was This Vibe Coded?

No. Since there are essentially zero resources and concrete examples of UI Toolkit at scale out there, if you try to vibe code it AI will hallucinate almost everything and have you running around in over-engineered circles. I came up with this myself based on experience and over half a year of trial and error.

### Agentic AI Viability

AI Agents can work in the system but with a caveat. After the initial structure is laid out and you have 4-5 fully fleshed out working modals and screens, a low-tier agent can comfortably take care of all the boilerplate associated with adding a new screen. I use Deepseek V4 Flash due to its fantastic cost-quality ratio and for only 2-3 cents it was able to create new views with almost zero issues.

### AI Assistance

For the architecture and initial work, I couldn't use AI for anything more than the usual basic questions or isolated bugs in some online chat. However, once I had completed the system, templates, and initial views and it came down to just grunt work, creating more screens was able to be done using a cheap AI agent like I described above.

Its UXML and USS needs heavy tweaking but it can populate a document with the named elements once you declare the record. It can add them to the constants class, create the factory method, and populate the View C# file with all the necessary bind and unbind functions. It handles all the boilerplate work very well.

Outside of that I used agents pretty heavily to create the mock data layer. It created the JSON files and basic save system. I didn't want to design a full backend for a FPS game, but I wanted to demonstrate how data can be wired up in the system.

I also use AI to help me out with these markdown docs to flesh things out like the project structure and diagrams.

---

## Related

- [Architecture Diagrams](./Docs/DIAGRAMS.md) -- class diagrams, navigation sequences, layer states, data flow
- [UI Toolkit Mini Series](https://github.com/SBUplakankus/ui-toolkit-mini-series) -- companion tutorial repo

---

> Data layer (`SaveDataManager`, `UIResources`) exists to populate the UI and validate the navigation pipeline. Not a production persistence solution -- no encryption, no schema versioning, no error recovery beyond `defaults.json` fallback.
>
> Audio (`UIAudioHandler`) is a basic USS-class-driven add-on for hover and click feedback. No spatial audio, no mixer groups, no dynamic mixing.
>
> Design: This is a programming portfolio piece, so I kept the visual scope tight. The background gradients are custom SVGs applied via USS background-image, and the film grain and vignette are handled through Unity's post-processing stack on the UI camera. No external design tools or assets were used.
>
> Future: Screen creation was halted once it became repetitive and I had enough views to demonstrate the system. I may expand it in the future and extend the architecture to a full HUD demo.
>
> Refs: The `Refs/` directory contains 34 reference screenshots from the original Call of Duty: World at War used as visual targets during development, covering every menu screen, HUD state, pause variant, and modal across console, PC, and split-screen.
