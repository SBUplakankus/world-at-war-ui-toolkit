<div align="center">

# Call of Duty: World at War in UI Toolkit

![Unity](https://shieldcn.dev/badge/Unity-6000.4.8f1-ececec.png?variant=outline&logo=unity)
![C](https://shieldcn.dev/badge/C%23-9.0-239120.png?variant=outline&logo=csharp)
![MIT License](https://shieldcn.dev/badge/License-MIT-blue.png?variant=outline)

A structured reference for building multi-screen UIs in Unity 6 with UI Toolkit.  
It is a pure code system. No ScriptableObjects, No Inspector references, and No custom frameworks.  
The core architecture is designed to showcase UI Programming fundamentals that can translate to any other UI framework.

<br/>

<img src="./Media/hero_gif.gif" alt="Navigation Demo" width="700"/>

<br/>

</div>

---

## About

UI Toolkit resources and examples are scarce, and existing samples are either too small to demonstrate real navigation patterns or too complex with heavy MVVM abstractions. I based this system on my work in Avalonia and Epic's Lyra sample to fill that gap.

I spent over half a year doing R&D with UI Toolkit and wanted to put something together to showcase how I would handle a large scale UI system before moving on to other projects.

A driving goal was to push UI Toolkit's visual boundaries by recreating a gritty, stylised game menu instead of the clean website-like UIs it is often associated with. World at War's menu was the perfect target to stress-test what the toolkit can do visually without needing to rely on custom shaders and textures.

The architecture is straightforward routing backed by convention, not configuration. Every screen follows the same lifecycle: a UXML template, a typed elements record, and a C# code-behind class wired through `NavigateTo<T>()`. The result is a system simple enough that cheap AI agents can add new screens once the first few are in place.

I designed it to be pure code. No ScriptableObjects, no Inspector references, and no custom frameworks. There are only 3 MonoBehaviour classes in the entire system.

I am also creating a YouTube Tutorial series where I recreate the initial steps of the system in videos. You can find the repo for that [here](https://github.com/SBUplakankus/ui-toolkit-mini-series).

---

## Docs

| Doc | Description |
|---|---|
| [Quick Start](./Docs/QUICK_START.md) | Step-by-step guide to adding a screen |
| [Technical Breakdown](./Docs/TECH_DOC.md) | Architecture diagrams, features, design choices |
| [Architecture Diagrams](./Docs/DIAGRAMS.md) | Class diagrams, navigation sequences, layer states, data flow |
| [Future Plans](./Docs/FUTURE_PLANS.md) | Upcoming features, open to community ideas |
| [AI Usage](./Docs/AI_USAGE.md) | How AI was used (and not used) in this project |

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
      MenuRoot.cs                    bootstrap, owns UIDocument
      ChromeController.cs            chrome title / username
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
      StyleClasses.cs                USS class name constants

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
- **UI Toolkit**
- **C# 9.0**
- **NUnit + Unity Test Framework**
- **JSON**

---

## Related

- [UI Toolkit Mini Series](https://github.com/SBUplakankus/ui-toolkit-mini-series) : companion tutorial repo

---

> Data layer exists to populate the UI and validate the navigation pipeline. Not a production persistence solution.
>
> Audio is a basic USS-class-driven add-on for hover and click feedback. No spatial audio, no mixer groups, no dynamic mixing.
>
> This is a programming portfolio piece, so I kept the visual scope tight. The background gradients are custom SVGs applied via USS background-image, and the film grain and vignette are handled through Unity's post-processing stack on the UI camera.
>
> Screen creation was halted once it became repetitive and I had enough views to demonstrate the system. I may expand it in the future and extend the architecture to a full HUD demo.
>
> The `Refs/` directory contains 34 reference screenshots from the original Call of Duty: World at War used as visual targets during development, covering every menu screen, HUD state, pause variant, and modal across console, PC, and split-screen.
