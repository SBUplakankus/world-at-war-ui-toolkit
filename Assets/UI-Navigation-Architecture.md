# UI Toolkit Navigation Architecture

A convention-based, zero-boilerplate navigation system for Unity UI Toolkit main menus. No enums. No inspector mappings. No switch statements. Views are resolved automatically by matching their class name to a UXML file in `Resources/Views/`.

This architecture is inspired by Unreal Engine 5's Lyra sample project — specifically its `UGameUIManagerSubsystem` pattern, where a single service locator owns the UI lifecycle and screens are pushed onto named layers. Adapted for a single-player Unity context with the minimal number of classes needed.

---

## The Five Core Classes

### 1. UIRouter — Service Locator + Navigation Bus

This is the single entry point every view uses to navigate. It's a plain C# class — not a MonoBehaviour, not a ScriptableObject. A static `Instance` property lazily creates the singleton. It holds a reference to `UILayout` (set during `Awake`) and delegates all navigation through it.

```csharp
// Assets/Scripts/UI/Router/UIRouter.cs
using UI.Services;
using UI.Views;

namespace UI
{
    public class UIRouter
    {
        private static UIRouter _instance;
        public static UIRouter Instance => _instance ??= new();

        private UILayout _layout;

        public void Register(UILayout layout)
        {
            _layout = layout;
        }

        public void NavigateTo<TView>() where TView : BaseView
        {
            var view = ViewFactory.Create<TView>();
            if (view != null)
                _layout.ShowScreen(view);
        }

        public void OpenModal<TModal>() where TModal : BaseView
        {
            var view = ViewFactory.Create<TModal>();
            if (view != null)
                _layout.ShowModal(view);
        }

        public void Back()
        {
            _layout.BackScreen();
        }

        public void CloseModal()
        {
            _layout.CloseModal();
        }
    }
}
```

Key design points:
- `NavigateTo<T>()` and `OpenModal<T>()` use C# generics — the type IS the destination. No string lookups, no enum matching.
- It doesn't own state beyond a single `_layout` reference. It's a pure delegation layer.
- `Register()` is called once by `UILayout.Awake()`. Before that, navigation calls would fail — but in practice views are never created before `Awake`, so this is safe.
- Being a plain class (not `UnityEngine.Object`), it has no `OnEnable`/`OnDisable` lifecycle surprises, no asset file to misplace, and no domain-reload fragility.

### 2. ViewFactory — Convention-Based View Constructor

A static class with one method. Given a type like `OptionsView`, it loads `Resources/Views/OptionsView.uxml` and constructs the view using `Activator.CreateInstance`.

```csharp
// Assets/Scripts/UI/Services/ViewFactory.cs
using UI.Views;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Services
{
    public static class ViewFactory
    {
        public static TView Create<TView>() where TView : BaseView
        {
            var name = typeof(TView).Name;
            var template = Resources.Load<VisualTreeAsset>($"Views/{name}");

            if (template == null)
            {
                Debug.LogError($"ViewFactory: missing Resources/Views/{name}.uxml");
                return null;
            }

            return (TView)System.Activator.CreateInstance(typeof(TView), template);
        }
    }
}
```

The naming convention is the only contract: if your class is `SoloView`, the UXML must be at `Resources/Views/SoloView.uxml`. Break this rule and you get an immediate `Debug.LogError` when navigation is attempted — not a silent null reference crash later.

`Activator.CreateInstance(typeof(TView), template)` requires every concrete view to have a constructor accepting exactly one `VisualTreeAsset` parameter. This is enforced by the `BaseView` abstract class. If a view later needs extra dependencies, `ViewFactory` is a single-file change — the rest of the system doesn't know how views are constructed.

`Resources.Load` is used for simplicity. For a main menu with roughly a dozen screens, the build-size overhead of including all UXML files in the Resources folder is negligible. If the system later scales to 100+ screens or needs lazy loading, `Resources.Load` can be swapped for Unity's Addressables system inside this single method.

### 3. BaseView — View Lifecycle Contract

An abstract class with a three-phase lifecycle. Every screen and modal inherits from this.

```csharp
// Assets/Scripts/UI/Views/BaseView.cs
using System;
using UnityEngine.UIElements;

namespace UI.Views
{
    public abstract class BaseView : IDisposable
    {
        public VisualElement Root { get; private set; }
        private bool _disposed;

        protected BaseView(VisualTreeAsset template)
        {
            Root = template.CloneTree();
        }

        public void Activate()
        {
            GetElements();
            Bind();
        }

        public void Deactivate() => UnBind();

        protected abstract void GetElements();
        protected abstract void Bind();
        protected abstract void UnBind();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                UnBind();
                Root.RemoveFromHierarchy();
                Root = null;
            }

            _disposed = true;
        }
    }
}
```

The lifecycle in detail:
- **Constructor**: clones the `VisualTreeAsset` into `Root`. The view now owns a detached VisualElement tree. It's not yet in the UI hierarchy.
- **Activate()**: called by `UILayer` when this view becomes the active screen. Runs `GetElements()` (query all button/label references from `Root`) then `Bind()` (subscribe to clicks, set initial text).
- **Deactivate()**: called when another view is pushed on top or when this view is popped. Runs `UnBind()` to unsubscribe all callbacks. The view remains in memory but is detached from the hierarchy.
- **Dispose()**: full cleanup. Unbinds, removes from hierarchy, nulls `Root`.

The split between `GetElements` and `Bind` is deliberate. `GetElements` queries the VisualElement tree once (expensive). `Bind` subscribes to events (cheap, but must be undone). `UnBind` unsubscribes. This prevents stale event handlers and allows GC to collect the view.

### 4. UILayout — Root Document Owner + Layer Host

A MonoBehaviour that must be attached to the same GameObject that has the `UIDocument` component. It creates two `UILayer` instances — one for screens, one for modals — and manages the persistent shell chrome (header bar, back button, footer).

```csharp
// Assets/Scripts/UI/Services/UILayout.cs
using UI.Constants;
using UI.Factories;
using UI.Records;
using UI.Views;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Services
{
    [RequireComponent(typeof(UIDocument))]
    public class UILayout : MonoBehaviour
    {
        private UILayer _screenLayer;
        private UILayer _modalLayer;
        private MenuShellElements _chrome;

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            _screenLayer = new UILayer(root.Q(UIElements.MainContainer));
            _modalLayer = new UILayer(root.Q(UIElements.ModalContainer));

            _chrome = ElementsFactory.MenuShell(root);
            BindChrome();

            UIRouter.Instance.Register(this);

            var mainMenu = ViewFactory.Create<MainMenuView>();
            if (mainMenu != null)
                _screenLayer.Push(mainMenu);
        }

        private void BindChrome()
        {
            _chrome.MenuName.text = "MAIN MENU";
            _chrome.Username.text = "Signed In: Uplakankus";
            _chrome.GameVersion.text = "V1.0.8";
            _chrome.BackButton.clicked += OnBackClicked;
        }

        private void OnDestroy()
        {
            if (_chrome.BackButton != null)
                _chrome.BackButton.clicked -= OnBackClicked;
        }

        private void OnBackClicked()
        {
            UIRouter.Instance.Back();
        }

        public void ShowScreen(BaseView view) => _screenLayer.Push(view);
        public void ShowModal(BaseView view) => _modalLayer.Push(view);
        public void BackScreen() => _screenLayer.Pop();
        public void CloseModal() => _modalLayer.Pop();
    }
}
```

What happens in `Awake` in order:
1. Queries the root `VisualElement` for `main-ctr` and `modal-ctr` — these are the named containers in the shell UXML
2. Creates a `UILayer` for each, passing the container as the target for view insertion
3. Uses `ElementsFactory.MenuShell` to query the chrome elements (header title, username, version, back button) into a strongly-typed record
4. Binds the chrome: sets initial label text, subscribes the back button
5. Registers with `UIRouter` — this is when the router gets its shell reference
6. Creates and pushes `MainMenuView` as the initial screen

The chrome (header and footer) is managed directly by UILayout — it is never on the screen stack. This means it is always visible regardless of which screen is active. When views are pushed and popped in `main-ctr`, the chrome remains undisturbed.

### 5. UILayer — Stack Container

Manages a stack of `BaseView` instances within a single `VisualElement` container. Only one view is visually active at a time; the rest are deactivated and stored in history.

```csharp
// Assets/Scripts/UI/Services/UILayer.cs
using System.Collections.Generic;
using UI.Views;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Services
{
    public class UILayer
    {
        private readonly VisualElement _container;
        private readonly Stack<BaseView> _history = new();
        private BaseView _current;

        public UILayer(VisualElement container)
        {
            _container = container;
        }

        public void Push(BaseView view)
        {
            if (_current != null)
            {
                _current.Deactivate();
                _history.Push(_current);
            }

            _current = view;
            _container.Clear();
            _container.Add(view.Root);
            view.Activate();
        }

        public void Pop()
        {
            if (_history.Count == 0)
            {
                Debug.LogWarning("UILayer: nothing to pop.");
                return;
            }

            _current.Deactivate();
            _current = _history.Pop();

            _container.Clear();
            _container.Add(_current.Root);
            _current.Activate();
        }

        public void Clear()
        {
            _current?.Dispose();
            _current = null;
            _history.Clear();
            _container.Clear();
        }
    }
}
```

Push flow:
1. If there's a current view, deactivate it and push it onto the history stack
2. Set the new view as current
3. Clear the container (removes the previous view's `Root` from the hierarchy)
4. Add the new view's `Root` to the container
5. Activate the new view (queries elements, binds callbacks)

Pop flow:
1. If history is empty, log a warning and return (prevents popping the root screen)
2. Deactivate the current view
3. Pop the previous view from history
4. Replace the container content with the previous view's `Root`
5. Reactivate the previous view

This simple push/pop model eliminates the need for a separate navigation history tracker, a state machine, or a centralized route table. The layer's stack IS the history.

---

## Supporting Infrastructure

### ElementsFactory & Records Pattern

Each screen has a `sealed record` type that holds typed `VisualElement` references. The `ElementsFactory` static class queries elements by name and returns the record. This keeps `GetElements()` in every view to exactly one line while providing strong typing for all subsequent code.

```csharp
// Assets/Scripts/UI/Records/MainMenuElements.cs
public sealed record MainMenuElements(
    Button SoloButton,
    Button CoOpButton,
    Button ZombiesButton,
    Button MultiplayerButton,
    Button OptionsButton,
    Button CreditsButton,
    Label MessageLabel
);

// Assets/Scripts/UI/Records/MenuShellElements.cs
public sealed record MenuShellElements(
    Label MenuName,
    Label Username,
    Label GameVersion,
    Button BackButton
);

// Assets/Scripts/UI/Factories/ElementsFactory.cs
public static class ElementsFactory
{
    public static MenuShellElements MenuShell(VisualElement root)
    {
        return new MenuShellElements(
            MenuName: root.Q<Label>(UIElements.MenuName),
            Username: root.Q<Label>(UIElements.UsernameLabel),
            GameVersion: root.Q<Label>(UIElements.VersionLabel),
            BackButton: root.Q<Button>(UIElements.BackBtn)
        );
    }

    public static MainMenuElements MainMenu(VisualElement root)
    {
        return new MainMenuElements(
            SoloButton: root.Q<Button>(UIElements.SoloBtn),
            CoOpButton: root.Q<Button>(UIElements.CoopBtn),
            ZombiesButton: root.Q<Button>(UIElements.ZombiesBtn),
            MultiplayerButton: root.Q<Button>(UIElements.MultiBtn),
            OptionsButton: root.Q<Button>(UIElements.OptionsBtn),
            CreditsButton: root.Q<Button>(UIElements.CreditsBtn),
            MessageLabel: root.Q<Label>(UIElements.MotdContent)
        );
    }
}
```

Records use C# 9's positional record syntax. They're immutable, value-equatable, and require zero boilerplate. The `IsExternalInit` polyfill in `Assets/Scripts/Utilities/IsExternalInit.cs` enables this syntax in Unity's .NET Standard 2.1 runtime.

### UIElements Constants

Element names are centralized in a single constants file so they can be referenced from both the factory and the UXML without magic strings scattered throughout the codebase.

```csharp
// Assets/Scripts/UI/Constants/UIElements.cs
public static class UIElements
{
    // View containers
    public const string MainContainer = "main-ctr";
    public const string ModalContainer = "modal-ctr";

    // Labels
    public const string MenuName = "hdr-title";
    public const string UsernameLabel = "username";
    public const string VersionLabel = "version";

    // Menu buttons
    public const string SoloBtn = "solo-btn";
    public const string CoopBtn = "coop-btn";
    public const string ZombiesBtn = "zombies-btn";
    public const string MultiBtn = "multi-btn";
    public const string OptionsBtn = "options-btn";
    public const string CreditsBtn = "credits-btn";
    public const string BackBtn = "back-btn";

    // MOTD section
    public const string MotdContent = "motd-content";
}
```

---

## Visual Hierarchy (UXML)

The shell UXML (`MenuShellView.uxml`) is the root document assigned to the `UIDocument` component. It's the only UXML that isn't loaded dynamically — everything inside `main-ctr` and `modal-ctr` is swapped at runtime.

```xml
<!-- Assets/UI Toolkit/Views/MenuShellView.uxml -->
<ui:UXML xmlns:ui="UnityEngine.UIElements">
    <!-- Fullscreen video background with vignette -->
    <ui:VisualElement name="video-bg" class="fill-absolute video-bg">
        <ui:VisualElement name="left-vig" class="vignette-side vignette-side--left" />
        <ui:VisualElement name="right-vig" class="vignette-side vignette-side--right" />
    </ui:VisualElement>

    <ui:VisualElement name="view-ctr" class="view-ctr">
        <!-- Header bar: title and signed-in user -->
        <ui:VisualElement name="hdr-ctr" class="font-arsenal-bold header-bar">
            <ui:Label text="MAIN MENU" name="hdr-title"
                class="font-arsenal-bold text-shadow-dark header-title" />
            <ui:Label text="Signed In: Uplakankus" name="username"
                class="font-arsenal-bold text-shadow-dark username-label" />
        </ui:VisualElement>

        <!-- Screen layer — views are pushed here -->
        <ui:VisualElement name="main-ctr" class="font-dmsans-thin main-ctr" />

        <!-- Modal overlay layer — positioned on top of everything -->
        <ui:VisualElement name="modal-ctr" class="modal-ctr"
            style="position: absolute; top: 0; left: 0; right: 0; bottom: 0;" />

        <!-- Footer bar: back button and version -->
        <ui:VisualElement name="ftr-ctr" class="footer-bar">
            <ui:Button text="Back" name="back-btn" class="ftr-btn" />
            <ui:Label text="V1.0.8" name="version" class="version-label" />
        </ui:VisualElement>
    </ui:VisualElement>
</ui:UXML>
```

Key design points in the UXML:
- `main-ctr` and `modal-ctr` are siblings inside `view-ctr`. Modals overlay screen content via `position: absolute`.
- Both containers start empty. Content is pushed dynamically by `UILayer` at runtime.
- The header (`hdr-ctr`) and footer (`ftr-ctr`) are permanent chrome. They're bound by `UILayout` and never change.
- The back button in the footer is always visible and always calls `UIRouter.Instance.Back()`.
- Static template references were removed. In the old system, `MainMenuView` was instantiated as a `<ui:Instance template="MainMenuView"/>` inside `main-ctr`. Now it's pushed dynamically by `UILayout.Awake()`.

### Concrete View UXML Example (MainMenuView)

```xml
<!-- Assets/Resources/Views/MainMenuView.uxml -->
<ui:UXML xmlns:ui="UnityEngine.UIElements">
    <!-- Reusable template references (Divider, MenuButton are in UI Toolkit/Templates/) -->
    <ui:Template name="Divider" src="project://database/...Divider.uxml" />
    <ui:Template name="MenuButton" src="project://database/...MenuButton.uxml" />

    <ui:VisualElement name="main-menu-view" style="flex-grow: 1;
        flex-direction: row; justify-content: space-between;">

        <ui:VisualElement name="left-ctr" class="left-ctr">
            <!-- Menu buttons using template instances with attribute overrides -->
            <ui:Instance template="MenuButton" name="solo-btn">
                <AttributeOverrides element-name="btn" text="SOLO"/>
            </ui:Instance>
            <ui:Instance template="MenuButton" name="coop-btn">
                <AttributeOverrides element-name="btn" text="COOPERATIVE CAMPAIGN"/>
            </ui:Instance>
            <ui:Instance template="MenuButton" name="zombies-btn">
                <AttributeOverrides element-name="btn" text="NAZI ZOMBIES"/>
            </ui:Instance>
            <ui:Instance template="MenuButton" name="multi-btn">
                <AttributeOverrides element-name="btn" text="MULTIPLAYER"/>
            </ui:Instance>
            <ui:Instance template="MenuButton" name="options-btn">
                <AttributeOverrides element-name="btn" text="OPTIONS"/>
            </ui:Instance>
            <ui:Instance template="MenuButton" name="credits-btn">
                <AttributeOverrides element-name="btn" text="CREDITS"/>
            </ui:Instance>

            <!-- Message of the Day section -->
            <ui:Label text="Message of the Day" name="motd-hdr"
                class="font-archivo-semibold text-shadow-dark motd-header" />
            <ui:Label text="..." name="motd-content"
                class="font-archivo-medium text-shadow-dark motd-content" />
        </ui:VisualElement>

        <!-- Right side: game logo -->
        <ui:VisualElement name="right-ctr" class="right-ctr">
            <ui:Image class="logo-image" />
        </ui:VisualElement>
    </ui:VisualElement>
</ui:UXML>
```

The menu uses `<ui:Instance>` template references for reusable button widgets. Each instance gets a unique `name` attribute (like `solo-btn`) which is what `ElementsFactory` queries. The template defines the visual structure, and `AttributeOverrides` customizes the text per instance.

---

## Concrete View Example

The full MainMenuView — a 6-button menu with a message of the day:

```csharp
// Assets/Scripts/UI/Views/MainMenuView.cs
using UI.Factories;
using UI.Records;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class MainMenuView : BaseView
    {
        private MainMenuElements _elements;

        public MainMenuView(VisualTreeAsset template) : base(template) { }

        private void HandleSoloClicked()
        {
            Debug.Log("SOLO clicked");
            UIRouter.Instance.NavigateTo<SoloView>();
        }

        private void HandleCoOpClicked()
        {
            Debug.Log("COOP clicked");
            UIRouter.Instance.NavigateTo<CoOpView>();
        }

        private void HandleMultiplayerClicked()
        {
            Debug.Log("Multiplayer clicked");
            UIRouter.Instance.NavigateTo<MultiplayerView>();
        }

        private void HandleZombiesClicked()
        {
            Debug.Log("Zombies clicked");
            UIRouter.Instance.NavigateTo<ZombiesView>();
        }

        private void HandleOptionsClicked()
        {
            Debug.Log("Options clicked");
            UIRouter.Instance.NavigateTo<OptionsView>();
        }

        private void HandleCreditsClicked()
        {
            Debug.Log("Credits clicked");
            UIRouter.Instance.NavigateTo<CreditsView>();
        }

        private string FetchMessageOfTheDay()
        {
            return "Message of the Day it is the Message of the Day. "
                + "Follow @massivemeltmedia on BlueSky and Subscribe on YouTube.";
        }

        protected override void GetElements()
        {
            _elements = ElementsFactory.MainMenu(Root);
        }

        protected override void Bind()
        {
            _elements.SoloButton.clicked += HandleSoloClicked;
            _elements.CoOpButton.clicked += HandleCoOpClicked;
            _elements.MultiplayerButton.clicked += HandleMultiplayerClicked;
            _elements.ZombiesButton.clicked += HandleZombiesClicked;
            _elements.OptionsButton.clicked += HandleOptionsClicked;
            _elements.CreditsButton.clicked += HandleCreditsClicked;
            _elements.MessageLabel.text = FetchMessageOfTheDay();
        }

        protected override void UnBind()
        {
            _elements.SoloButton.clicked -= HandleSoloClicked;
            _elements.CoOpButton.clicked -= HandleCoOpClicked;
            _elements.MultiplayerButton.clicked -= HandleMultiplayerClicked;
            _elements.ZombiesButton.clicked -= HandleZombiesClicked;
            _elements.OptionsButton.clicked -= HandleOptionsClicked;
            _elements.CreditsButton.clicked -= HandleCreditsClicked;
        }
    }
}
```

Every navigation call in the view is a single line: `UIRouter.Instance.NavigateTo<TypeName>()`. No constructor injection. No passed shell reference. No enum argument. The C# type IS the destination.

Note that views do NOT have their own back buttons. The back button lives in the shell chrome (footer), managed by `UILayout`. This is deliberate: a view should not know or care whether it's on the screen stack or modal stack, or whether there's something underneath it. It just declares what navigation it can trigger.

### Stub View (Minimal)

New screens start as minimal stubs. Here's `SoloView`:

```csharp
// Assets/Scripts/UI/Views/SoloView.cs
namespace UI.Views
{
    public class SoloView : BaseView
    {
        public SoloView(VisualTreeAsset template) : base(template) { }
        protected override void GetElements() { }
        protected override void Bind() { }
        protected override void UnBind() { }
    }
}

// Assets/Resources/Views/SoloView.uxml
<ui:UXML xmlns:ui="UnityEngine.UIElements">
    <ui:VisualElement style="flex-grow: 1; padding: 40px;">
        <ui:Label text="SOLO" name="screen-label"
            style="font-size: 32px; color: white;" />
    </ui:VisualElement>
</ui:UXML>
```

That's the minimum a view needs: a class extending `BaseView` with the constructor, and a UXML in `Resources/Views/` with the matching filename. `GetElements`, `Bind`, and `UnBind` can be empty for stubs.

### Modal Example (Has Its Own Close Button)

Modals follow the same pattern but include their own close button and call `UIRouter.Instance.CloseModal()` instead of `Back()`:

```csharp
// Assets/Scripts/UI/Views/WarningModal.cs
namespace UI.Views
{
    public class WarningModal : BaseView
    {
        private Button _closeButton;

        public WarningModal(VisualTreeAsset template) : base(template) { }

        protected override void GetElements()
        {
            _closeButton = Root.Q<Button>("close-btn");
        }

        protected override void Bind()
        {
            _closeButton.clicked += OnClose;
        }

        protected override void UnBind()
        {
            _closeButton.clicked -= OnClose;
        }

        private void OnClose() => UIRouter.Instance.CloseModal();
    }
}

// Assets/Resources/Views/WarningModal.uxml
<ui:UXML xmlns:ui="UnityEngine.UIElements">
    <ui:VisualElement style="flex-grow: 1; padding: 40px;
        background-color: rgba(0,0,0,0.8);
        align-items: center; justify-content: center;">
        <ui:Label text="WARNING" name="modal-label"
            style="font-size: 28px; color: white; margin-bottom: 20px;" />
        <ui:Button text="Close" name="close-btn" />
    </ui:VisualElement>
</ui:UXML>
```

---

## Complete Navigation Flow (Button Click to Screen Swap)

Tracing what happens when a user clicks "Options" from the main menu:

### Step 1: Button Click in MainMenuView

```csharp
// In MainMenuView.Bind():
_elements.OptionsButton.clicked += HandleOptionsClicked;

// The handler:
private void HandleOptionsClicked()
{
    UIRouter.Instance.NavigateTo<OptionsView>();
}
```

The view only knows two things: which button was clicked, and which view type to navigate to. It doesn't know about the shell, the layers, the stack, or how views are constructed.

### Step 2: UIRouter Receives the Navigation Request

```csharp
public void NavigateTo<TView>() where TView : BaseView
{
    var view = ViewFactory.Create<TView>();
    if (view != null)
        _layout.ShowScreen(view);
}
```

Two things happen: the view is constructed via `ViewFactory`, then passed to `UILayout.ShowScreen()`. The router doesn't know HOW the view is constructed or WHERE it goes — it delegates both concerns.

### Step 3: ViewFactory Constructs the View

```csharp
public static TView Create<TView>() where TView : BaseView
{
    var name = typeof(TView).Name;                    // "OptionsView"
    var template = Resources.Load<VisualTreeAsset>(
        $"Views/{name}");                             // Resources/Views/OptionsView.uxml

    if (template == null)
    {
        Debug.LogError(
            $"ViewFactory: missing Resources/Views/{name}.uxml");
        return null;
    }

    return (TView)Activator.CreateInstance(
        typeof(TView), template);                     // new OptionsView(template)
}
```

The factory loads the UXML from `Resources/Views/`, constructs the view via reflection, and returns it. The constructor clones the template into `Root`. The view is now a complete, detached VisualElement tree — it hasn't been added to the UI yet.

### Step 4: UILayout Routes to the Screen Layer

```csharp
public void ShowScreen(BaseView view) => _screenLayer.Push(view);
```

The layout knows that `ShowScreen` means "push onto the screen layer stack." If this were a modal, `ShowModal` would push onto the modal layer instead. The view doesn't know which layer it's going to.

### Step 5: UILayer Pushes the View

```csharp
public void Push(BaseView view)
{
    if (_current != null)
    {
        _current.Deactivate();    // MainMenuView.UnBind() called
        _history.Push(_current);  // MainMenuView pushed to history
    }

    _current = view;              // OptionsView becomes current
    _container.Clear();           // Remove MainMenuView.Root from hierarchy
    _container.Add(view.Root);    // Add OptionsView.Root to hierarchy
    view.Activate();              // OptionsView.GetElements() + Bind() called
}
```

The layer:
1. Deactivates MainMenuView (unsubscribes its button handlers, keeps it in memory)
2. Pushes MainMenuView onto the history stack
3. Sets OptionsView as the current view
4. Clears the `main-ctr` container (removes MainMenuView's VisualElement tree)
5. Adds OptionsView's VisualElement tree to the container
6. Activates OptionsView (queries its elements, binds its callbacks)

At this point, OptionsView is visible and interactive. MainMenuView is deactivated but preserved in the history stack.

### Step 6: User Presses Back (Chrome Button)

The back button in the footer is bound by UILayout:

```csharp
// In UILayout.BindChrome():
_chrome.BackButton.clicked += OnBackClicked;

private void OnBackClicked()
{
    UIRouter.Instance.Back();
}
```

### Step 7: UILayer Pops

```csharp
public void Pop()
{
    if (_history.Count == 0)
    {
        Debug.LogWarning("UILayer: nothing to pop.");
        return;
    }

    _current.Deactivate();        // OptionsView.UnBind() called
    _current = _history.Pop();    // MainMenuView restored from history

    _container.Clear();           // Remove OptionsView.Root
    _container.Add(_current.Root);// Add MainMenuView.Root back
    _current.Activate();          // MainMenuView.GetElements() + Bind() again
}
```

The user is now back at the main menu. The entire round-trip involves no state machine, no route table, no string lookups. Just stack operations on the `UILayer`.

---

## File Structure

```
Assets/
├── Resources/Views/                  UXML loaded at runtime by ViewFactory
│   ├── MainMenuView.uxml
│   ├── SoloView.uxml
│   ├── CoOpView.uxml
│   ├── ZombiesView.uxml
│   ├── MultiplayerView.uxml
│   ├── OptionsView.uxml
│   ├── CreditsView.uxml
│   ├── WarningModal.uxml
│   ├── DifficultyModal.uxml
│   └── NewGameModal.uxml
│
├── UI Toolkit/
│   ├── Views/MenuShellView.uxml      Shell layout (assigned to UILayout's UIDocument)
│   ├── Templates/
│   │   ├── MenuButton.uxml           Reusable button widget
│   │   └── Divider.uxml              Reusable divider widget
│   └── Styles/
│       ├── core.uss
│       └── main-menu.uss
│
├── Scripts/UI/
│   ├── Router/
│   │   └── UIRouter.cs
│   ├── Services/
│   │   ├── UILayout.cs
│   │   ├── UILayer.cs
│   │   └── ViewFactory.cs
│   ├── Views/
│   │   ├── BaseView.cs
│   │   ├── MainMenuView.cs
│   │   ├── SoloView.cs               (stub)
│   │   ├── CoOpView.cs               (stub)
│   │   ├── ZombiesView.cs            (stub)
│   │   ├── MultiplayerView.cs        (stub)
│   │   ├── OptionsView.cs            (stub)
│   │   ├── CreditsView.cs            (stub)
│   │   ├── WarningModal.cs           (stub)
│   │   ├── DifficultyModal.cs        (stub)
│   │   └── NewGameModal.cs           (stub)
│   ├── Factories/
│   │   └── ElementsFactory.cs
│   ├── Records/
│   │   ├── MainMenuElements.cs
│   │   └── MenuShellElements.cs
│   └── Constants/
│       └── UIElements.cs
│
└── Scripts/Utilities/
    └── IsExternalInit.cs             C# 9 records polyfill for Unity
```

---

## Naming Decisions (and Their Reasons)

| Class | Role | Chosen Name Over |
|-------|------|-----------------|
| `UIRouter` | Service locator + navigation bus | `UIService` (old name). "Router" because it receives a destination type and dispatches it — exactly what a router does. "Service" is too vague. |
| `UILayout` | Root document owner, layer host, chrome manager | `UIShell` (old name). "Layout" matches UE5 Lyra's `UPrimaryGameLayout` — it actively manages named layers. "Shell" implies a passive container. |
| `UILayer` | Stack container for views | Kept from original. Accurate — it's a layered stack. |
| `ViewFactory` | Static factory that creates views from UXML | `ViewLocator` (considered). "Locator" implies finding things that already exist. This class CREATES new instances. It's a factory. |
| `BaseView` | View lifecycle contract | Kept from original. Minimal, says what it is. |
| `ElementsFactory` | Queries VisualElements into typed records | Kept from original. Clear separation from ViewFactory (which creates VIEWS, not element references). |
| `MenuShellElements` | Record of chrome element references | Kept from original. Used by UILayout to access chrome without inline Q<>() calls. |
| `MainMenuElements` | Record of main menu element references | Kept from original. |
| `UIElements` | Constants for element names | Kept from original. |

---

## What Was Deleted (and Why)

| Deleted File | What It Did | Why Removed |
|---|---|---|
| `Enums/Screen.cs` | Enum of screen names (Solo, CoOp, etc.) | Replaced by C# generics. The type IS the identifier. |
| `Enums/Modal.cs` | Enum of modal names (Warning, etc.) | Same reason. `UIRouter.OpenModal<WarningModal>()` replaces `UIService.OpenModal(Modal.Warning)`. |
| `Services/UILibrary.cs` | ScriptableObject with inspector-mapped dictionary of enums → VisualTreeAssets | Replaced by convention: class name matches UXML filename in Resources/Views/. No more drag-and-drop mapping. |
| `Views/MainShellView.cs` | A BaseView subclass that handled chrome (header, back button) | Chrome is now managed directly by UILayout. It was never correct to have the chrome on the screen stack — it would disappear when another view was pushed on top. |
| `Services/UIShell.cs` | The old MonoBehaviour that owned the UIDocument and used UILibrary for template lookups | Replaced by UILayout. The old version relied on enums and the UILibrary mapping. |
| `Services/UIService.cs` | ScriptableObject service locator with enum-based navigation methods | Replaced by UIRouter. The ScriptableObject approach was fragile — domain reloads could null the Instance, and it required an asset file in the project. UIRouter is a plain C# class with a lazy static instance. |

---

## How to Add a New Screen

1. Create the UXML in `Assets/Resources/Views/NewScreenView.uxml`
2. Create the class in `Assets/Scripts/UI/Views/NewScreenView.cs`:
   ```csharp
   namespace UI.Views
   {
       public class NewScreenView : BaseView
       {
           public NewScreenView(VisualTreeAsset template) : base(template) { }
           protected override void GetElements() { }
           protected override void Bind() { }
           protected override void UnBind() { }
       }
   }
   ```
3. In whichever view triggers the navigation, add:
   ```csharp
   UIRouter.Instance.NavigateTo<NewScreenView>();
   ```

No enum entries. No ScriptableObject mappings. No switch statement cases. Two files and one line of navigation code.

---

## Design Principles

**Convention over configuration.** The filename-matches-classname rule eliminates all registration boilerplate. The only thing a developer needs to remember is that `FooView.cs` needs `FooView.uxml` in `Resources/Views/`.

**Generics over enums.** `NavigateTo<OptionsView>()` is compile-time safe, self-documenting, and discoverable via IDE autocomplete. An enum `Screen.Options` is none of those things.

**Layers over routing tables.** The screen stack is a `Stack<BaseView>`. Pushing shows a screen; popping reveals the previous one. No centralized route map, no string-to-type dictionary, no navigation history that needs to be manually tracked. The layer's stack IS the history.

**Plain C# over ScriptableObjects for infrastructure.** `UIRouter` is a simple class because it has no serialized state, no inspector fields, and no reason to exist as a project asset. ScriptableObjects are for data — configuration, settings, asset references. They're not for pure logic classes that just hold a reference and delegate methods.

**Views don't own navigation context.** A view calls `UIRouter.Instance.NavigateTo<T>()` but doesn't know whether it's pushing onto the screen layer or modal layer, doesn't know the stack depth, and doesn't know what's underneath it. It only declares what navigation it can trigger. The shell handles placement.

**Chrome is never on the stack.** The header bar, footer bar, and back button are bound by UILayout directly. They persist across all screen transitions. If chrome were a view on the stack, it would deactivate whenever another view was pushed.

---

## Scaling Notes

If this system grows beyond a main menu into a full game UI system:

- `Resources.Load` in `ViewFactory` can be swapped for Addressables with a single-file change. The rest of the system doesn't know how templates are loaded.
- Additional named layers (cinematics, notifications, HUD) can be registered in `UILayout` by adding more `UILayer` instances targeting new containers in the shell UXML.
- Input mode management per screen (Lyra's `ELyraWidgetInputMode` — distinguishing Menu input from Game input from Both) can be added as a property on `BaseView`, applied by `UILayer` on push/pop.
- The extension system from Lyra (decoupled HUD element injection via gameplay tags, where game features register widgets at named extension points) can be layered on top without touching the navigation core.
- Async loading with input suspension during transitions (Lyra's `PushWidgetToLayerStackAsync`) can be added to `UILayer` without changing `UIRouter` or any view code.

The current scope — a single-player main menu with a dozen screens — doesn't need any of these. They're documented as growth paths, not requirements.
