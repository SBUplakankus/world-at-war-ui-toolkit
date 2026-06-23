# AGENTS.md

UI Toolkit reference project (Unity `6000.4.8f1`, C# 9, .NET Standard 2.1). Code-only architecture: no ScriptableObjects, no Inspector references, only **3 MonoBehaviours** in the entire system (`MenuRoot`, `UIAudioHandler`, `DebugTool`).

The full design rationale, lifecycle diagrams, and per-screen recipes live under [`Docs/`](./Docs/). Read these before changing architecture — `QUICK_START.md` is the canonical "add a screen" walkthrough, `TECH_DOC.md` explains the router/layer state machine, and `AI_USAGE.md` describes what the author expects cheap agents to handle.

## Build / test / lint

- **No CLI build, no CI, no linter, no formatter, no pre-commit hook.** The project is opened in the Unity Editor. Use `File ▸ Open Project` on this folder, or Unity Hub "Add project from disk."
- **Tests are run from `Window ▸ General ▸ Test Runner`** in the Editor. There is no headless test invocation wired up. The test framework package is `com.unity.ui.test-framework 6.4.0` (see `Packages/manifest.json`).
- Three Editor-only test suites guard the seams:
  - `Assets/Tests/ViewFactory/ViewFactoryTests.cs` (assembly `Tests`)
  - `Assets/Tests/ElementsFactory/ElementsFactoryTests.cs` (assembly `ElementsFactory.Tests` — separate `asmdef` lives next to the file)
  - `Assets/Tests/Data/UIResourcesTests.cs` (assembly `Tests`)
  Both `Tests.asmdef` and `ElementsFactory.Tests.asmdef` declare `includePlatforms: ["Editor"]` and require `UNITY_INCLUDE_TESTS` (defined in the `DefineConstants` of `UiToolkit.csproj`).
- The root `*.csproj` / `*.sln` files are **Unity auto-generated**. Do not hand-edit them — Unity rewrites them on each asset import. Hardcoded paths in `UiToolkit.csproj` reference `C:\Program Files\Unity\Hub\Editor\6000.4.8f1\…`, so this project assumes a Windows install at that path.
- `Library/`, `Logs/`, `Temp/`, `obj/`, `UserSettings/`, `ProjectSettings/`, `obj/` are gitignored and generated. Never edit them by hand.

## Architecture rules that will silently break things

These are the most-missed contracts. Violating any of them is usually caught by tests, but only after a full reimport.

- **UXML filename MUST match the C# class name exactly (case-sensitive).** `ViewFactory.Create<TView>()` loads `Resources/Views/{typeof(TView).Name}` (`Assets/Scripts/UI/Factories/ViewFactory.cs:16`). A mismatch produces a `Debug.LogError` at runtime and returns `null`. There is no fallback.
- **All runtime UXML lives in `Assets/Resources/Views/`**, not under `Assets/UI Toolkit/`. The `Assets/UI Toolkit/` tree holds design-time assets only:
  - `Views/LayoutView.uxml` — root shell document (assigned to a `UIDocument` in the scene)
  - `Templates/` — `MenuButton.uxml`, `MenuSlider.uxml`, `Divider.uxml` (reusable widgets)
  - `Styles/` — `core.uss` (design tokens), `screens.uss`, `modals.uss`
  UXML files in `Assets/Resources/Views/` use `project://database/...` template/style references that point back at `Assets/UI Toolkit/`. Do not move files around without updating those refs.
- **`Assets/Scripts/Utilities/IsExternalInit.cs` is required.** Unity 6 ships the Roslyn compiler with C# 9 enabled but the .NET Standard 2.1 BCL lacks `System.Runtime.CompilerServices.IsExternalInit`. Every `record` and `init` setter in the project (16 records under `Assets/Scripts/UI/Records/`) emits a reference to it. Deleting this file causes `CS0518` and breaks the whole assembly. Keep it; if you ever bump the API compatibility level away from `.NET Standard 2.1`, the polyfill may become redundant and can be removed.
- **All element name strings live in `Assets/Scripts/UI/Constants/Elements.cs`.** UXML `name` attributes, factory queries, and tests all reference these constants. There are no raw magic strings in view code. When adding or renaming a UXML `name`, change it in exactly one place (`Elements.cs`) and the constants flow to the factory, the record, and the test.
- **`MenuRoot.Awake()` calls `SaveDataManager.Delete()`** (`Assets/Scripts/UI/Core/MenuRoot.cs:25`). This wipes `Application.persistentDataPath/playersavedata.json` on every play-mode start. It is intentional — the project ships a mock save layer, not a real one. The `DebugTool` (F3) is the way to repopulate state. Do not "fix" this.
- **`UIRouter` is a plain C# singleton, not a MonoBehaviour** (`Assets/Scripts/UI/Core/UIRouter.cs`). It throws no exceptions on misuse; if `Register(MenuRoot)` has not run, every public method logs an error and no-ops. Any view that needs to navigate must call `UIRouter.Instance.NavigateTo<TView>()` for screens or `UIRouter.Instance.OpenModal<TModal>()` for modals — never instantiate views directly.

## Adding a new screen (5 files, 7 if you add tests)

The contract is strict; the tests rely on it.

1. `Assets/Resources/Views/{Name}View.uxml` — filename MUST match the C# class. Every named element must have a `name="…"` attribute.
2. `Assets/Scripts/UI/Constants/Elements.cs` — one `public const string` per `name`.
3. `Assets/Scripts/UI/Records/{Name}Elements.cs` — `public sealed record` with one typed parameter per element.
4. `Assets/Scripts/UI/Factories/ElementsFactory.cs` — one `public static` method that returns the record; use the `QButton`/`QSlider` helpers (which unwrap `TemplateContainer`) for `ui:Instance`-wrapped elements, or `root.Q<Label>(name)` for direct children.
5. `Assets/Scripts/UI/Views/{Name}View.cs` — `sealed class {Name}View : BaseView, IScreen`. Override `GetElements()` (one tree walk, calls the factory), `Bind()` (subscribe), `UnBind()` (unsubscribe — every subscription here must be removed). Constructor takes a `VisualTreeAsset` and forwards to `base(template)`.
6. `Assets/Tests/ViewFactory/ViewFactoryTests.cs` — add `yield return typeof({Name}View);` to the `ViewTypes()` enumerator. Skipping this lets a missing UXML ship silently.
7. `Assets/Tests/ElementsFactory/ElementsFactoryTests.cs` — add one `{Name}View_AllElementsResolve` test that calls `Resources.Load<VisualTreeAsset>("Views/{Name}View").CloneTree()` and asserts every record property is non-null.

`IScreen.HeaderName` controls the chrome title. Modals are normal `BaseView` subclasses that also implement `IScreen`; the modal stack is independent of the screen stack, so they compose freely.

## Misc

- **F3** in play mode opens the `DebugTool` `OnGUI` panel for editing the mock save (campaign flag, mission count, save/delete/new-player actions).
- **Audio is USS-class-driven** (`Assets/Scripts/UI/Core/UIAudioHandler.cs`): tag an element with `audio--hover` or `audio--click` in UXML/USS and the root-level handler plays the right `AudioClip` from `Resources/Audio/`. No C# event wiring per button.
- **The `Refs/` directory contains 34 reference screenshots** from the original CoD: WaW menus — visual targets only, not loaded at runtime.
- **`UIElementsSchema/*.xsd`** are XSD files for UI Builder / UXML IntelliSense; safe to ignore unless authoring UXML outside Unity.
- **`.idea/` is tracked and per-branch** (`.idea.ui-toolkit-waw/`, `.idea.ui-toolkit-series/`). Don't merge these across machines.
- **Branch:** `main`. PRs go to `origin/main`. No release branches or version tags in this repo.

See `Docs/QUICK_START.md` for a full worked example, `Docs/TECH_DOC.md` for the navigation/lifecycle diagrams, and `Docs/FUTURE_PLANS.md` for the roadmap (mission completion writeback, gamepad, localization, HUD demo, etc. are explicitly still TODO).
