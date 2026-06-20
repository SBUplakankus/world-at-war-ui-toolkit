# Quick Start: Add a Screen

This walks through creating the Solo Campaign screen from scratch, referencing the actual files in the project. Follow these seven steps in order every time you add a new screen.

---

## Step 1: Create the UXML

File: `Assets/Resources/Views/SoloView.uxml`

```xml
<ui:UXML xmlns:ui="UnityEngine.UIElements">
    <ui:Template name="MenuButton" src="MenuButton.uxml" />
    <ui:Template name="Divider" src="Divider.uxml" />
    <Style src="core.uss" />
    <Style src="screens.uss" />

    <ui:VisualElement name="solo-view" class="view-root">
        <ui:VisualElement name="menu-list-ctr" class="menu-list-ctr single-list">
            <ui:Instance template="Divider" />
            <ui:Instance template="MenuButton" name="resume-btn">
                <AttributeOverrides element-name="btn" text="RESUME GAME" />
            </ui:Instance>
            <ui:Instance template="MenuButton" name="new-btn">
                <AttributeOverrides element-name="btn" text="NEW GAME" />
            </ui:Instance>
            <ui:Instance template="MenuButton" name="mission-btn">
                <AttributeOverrides element-name="btn" text="MISSION SELECT" />
            </ui:Instance>
        </ui:VisualElement>
    </ui:VisualElement>
</ui:UXML>
```

What matters here:

- The filename must match the C# class name exactly. `SoloView.cs` means `SoloView.uxml`.
- It lives in `Resources/Views/` so `ViewFactory.Create<T>()` can load it at runtime.
- The `name` attributes on each `Instance` are what the ElementsFactory queries against in Step 4. If you rename `new-btn` here, update the constant in Step 2.
- Templates (`MenuButton`, `Divider`) are reusable widgets from `UI Toolkit/Templates/`. They keep the UXML lean.

---

## Step 2: Add element name constants

File: `Assets/Scripts/UI/Constants/Elements.cs`

Every `name` attribute from the UXML gets a `public const string`:

```csharp
public const string NewGameBtn = "new-btn";
public const string ResumeBtn = "resume-btn";
public const string MissionSelectBtn = "mission-btn";
```

This is the only C# file you touch when a UXML element gets renamed. The factory method, the record, and the view all reference this constant, never the raw string.

---

## Step 3: Create the Elements Record

File: `Assets/Scripts/UI/Records/SoloScreenElements.cs`

```csharp
using UnityEngine.UIElements;

namespace UI.Records
{
    public sealed record SoloScreenElements(
        Button ResumeButton,
        Button NewGameButton,
        Button MissionSelectButton
    );
}
```

One parameter per named element. The type is the UI Toolkit type you queried (`Button`, `Slider`, `Label`, `Image`, etc.). The naming convention is `{Purpose}{Type}`; `ResumeButton`, `NewGameButton`, `MissionSelectButton`.

Records come from the `IsExternalInit.cs` polyfill in `Utilities/`. No constructor boilerplate, value equality for free, and immutable by default.

---

## Step 4: Add a factory method

File: `Assets/Scripts/UI/Factories/ElementsFactory.cs`

```csharp
public static SoloScreenElements SoloScreen(VisualElement root)
{
    return new SoloScreenElements(
        ResumeButton: QButton(root, Elements.ResumeBtn),
        NewGameButton: QButton(root, Elements.NewGameBtn),
        MissionSelectButton: QButton(root, Elements.MissionSelectBtn)
    );
}
```

`QButton` searches for a `TemplateContainer` with the given name, then finds the `Button` inside it. This handles the `Instance` wrapping that UI Builder adds. For direct elements (not inside templates), use `root.Q<Label>(name)` or similar.

Every factory method returns the record from Step 3. Every parameter matches the record's constructor in order.

---

## Step 5: Create the View class

File: `Assets/Scripts/UI/Views/SoloView.cs`

```csharp
using UI.Constants;
using UI.Core;
using UI.Factories;
using UI.Interfaces;
using UI.Records;
using UnityEngine.UIElements;

namespace UI.Views
{
    public sealed class SoloView : BaseView, IScreen
    {
        private SoloScreenElements _elements;

        public string HeaderName => ScreenNames.Solo;

        public SoloView(VisualTreeAsset template) : base(template) { }

        // Called once when the view becomes active.
        // Runs a single tree walk to find all named elements.
        protected override void GetElements() => _elements = ElementsFactory.SoloScreen(Root);

        // Subscribe events and set initial state.
        protected override void Bind()
        {
            _elements.NewGameButton.clicked += HandleNewGameClicked;
            _elements.ResumeButton.clicked += HandleResumeGameClicked;
            _elements.MissionSelectButton.clicked += HandleMissionSelectClicked;

            DisplayButtonOptions();
        }

        // Unsubscribe everything. Called when another view pushes on top.
        protected override void UnBind()
        {
            _elements.NewGameButton.clicked -= HandleNewGameClicked;
            _elements.ResumeButton.clicked -= HandleResumeGameClicked;
            _elements.MissionSelectButton.clicked -= HandleMissionSelectClicked;
        }

        private void DisplayButtonOptions()
        {
            bool started = Data.SaveDataManager.CurrentSave.campaignStarted;
            _elements.ResumeButton.visible = started;
            _elements.MissionSelectButton.visible = started;
        }

        private static void HandleNewGameClicked()
            => UIRouter.Instance.OpenModal<NewGameModalView>();

        private static void HandleResumeGameClicked()
            => UIRouter.Instance.OpenModal<ResumeGameModalView>();

        private static void HandleMissionSelectClicked()
            => UIRouter.Instance.NavigateTo<MissionSelectView>();
    }
}
```

The key contracts:

| Method | Purpose | When called |
|---|---|---|
| Constructor | Clone UXML into `Root` | Once, by `ViewFactory` |
| `GetElements()` | One tree walk, populates `_elements` | First time `Activate()` runs |
| `Bind()` | Subscribe events, set labels | Every `Activate()` |
| `UnBind()` | Unsubscribe events | Every `Deactivate()` or `Dispose()` |

`IScreen` provides the `HeaderName` that appears in the chrome header bar. If your view is a modal, implement `IScreen` and the header will display its title.

---

## Step 6: Wire up navigation

From any button handler, call the router:

```csharp
// Navigate to a screen (pushes onto the screen stack)
UIRouter.Instance.NavigateTo<SoloView>();

// Open a modal (pushes onto the modal stack, overlays the current screen)
UIRouter.Instance.OpenModal<NewGameModalView>();
```

That is it. The back button automatically pops the current screen back to the previous one. No manual stack management, no enum registration, no switch statements.

---

## Step 7: Add tests (Optional but important if Designers will be editing the UXML in UI Builder)

Two test files cover every screen. Add your new view to both.

**ViewFactory test**: File: `Assets/Tests/ViewFactory/ViewFactoryTests.cs`

Add one line to the `ViewTypes` enumerator:

```csharp
yield return typeof(SoloView);
```

This validates the view instantiates, activates, deactivates, and disposes without throwing. It catches a missing UXML or a constructor signature mismatch immediately.

**ElementsFactory test**: File: `Assets/Tests/ElementsFactory/ElementsFactoryTests.cs`

Add one test method per factory method:

```csharp
[Test]
public void SoloView_AllElementsResolve()
{
    var root = CloneUxml("Views/SoloView");
    var result = ElementsFactory.SoloScreen(root);

    Assert.That(result.ResumeButton, Is.Not.Null);
    Assert.That(result.NewGameButton, Is.Not.Null);
    Assert.That(result.MissionSelectButton, Is.Not.Null);
}
```

This validates every named element in the UXML resolves through the factory method. If a constant gets out of sync with a UXML `name` attribute, this test catches it.

---

## Quick reference: files touched per screen

| File | What you write |
|---|---|
| `Resources/Views/{Name}.uxml` | UXML layout, name attributes |
| `Constants/Elements.cs` | One const per named element |
| `Records/{Name}Elements.cs` | One sealed record with typed parameters |
| `Factories/ElementsFactory.cs` | One public static method returning the record |
| `Views/{Name}.cs` | BaseView subclass with GetElements/Bind/UnBind |
| `Tests/ViewFactory/ViewFactoryTests.cs` | One `yield return typeof({Name})` |
| `Tests/ElementsFactory/ElementsFactoryTests.cs` | One `{Name}_AllElementsResolve` test |

Five steps across Five files with Two Extra if you want to add Tests.
