# Lyra UI Architecture: Complete Analysis for Unity + UI Toolkit

> A detailed breakdown of Epic's Lyra Sample UI system and how every pattern maps to Unity C# with UI Toolkit.

---

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Layer 1: UI Manager Subsystem](#layer-1-ui-manager-subsystem)
3. [Layer 2: UI Policy](#layer-2-ui-policy)
4. [Layer 3: Primary Game Layout](#layer-3-primary-game-layout)
5. [Layer 4: Activatable Widgets](#layer-4-activatable-widgets)
6. [Layer 5: UI Extension System](#layer-5-ui-extension-system)
7. [Screen Navigation: The Frontend Control Flow](#screen-navigation-the-frontend-control-flow)
8. [HUD Construction via Game Features](#hud-construction-via-game-features)
9. [Complete Unity Implementation](#complete-unity-implementation)
10. [Architectural Decisions and Tradeoffs](#architectural-decisions-and-tradeoffs)

---

## Architecture Overview

Lyra does **not** use a traditional screen manager (no `OpenScreen("MainMenu")`, no global route table). Instead it uses a **layered push/pop stack** driven by **gameplay tags**. Every "screen" is an activatable widget that gets pushed onto a named layer. When it deactivates, the widget beneath resumes automatically.

The full stack (top to bottom):

```
┌─────────────────────────────────────────────────────────┐
│                  GameUIManagerSubsystem                   │
│        (GameInstanceSubsystem — owns the policy)          │
├─────────────────────────────────────────────────────────┤
│                      GameUIPolicy                         │
│      (Creates root layouts per player, manages            │
│       viewport attachment, split-screen modes)            │
├─────────────────────────────────────────────────────────┤
│                   PrimaryGameLayout                       │
│  ┌────────────────────────────────────────────────────┐  │
│  │  UI.Layer.Modal    (ActivatableWidgetContainer)    │  │
│  │  ┌──────────────┐ ┌──────────────┐                 │  │
│  │  │  Dialog A    │ │  Dialog B    │  ← Stack       │  │
│  │  └──────────────┘ └──────────────┘                 │  │
│  ├────────────────────────────────────────────────────┤  │
│  │  UI.Layer.Menu     (ActivatableWidgetContainer)    │  │
│  │  ┌──────────────┐                                 │  │
│  │  │  Main Menu   │              ← Stack             │  │
│  │  └──────────────┘                                 │  │
│  ├────────────────────────────────────────────────────┤  │
│  │  UI.Layer.Game      (ActivatableWidgetContainer)    │  │
│  │  ┌──────────────┐                                 │  │
│  │  │  HUD Layout  │              ← Stack             │  │
│  │  └──────────────┘                                 │  │
│  └────────────────────────────────────────────────────┘  │
│                    UIExtension Points                      │
│     (Pub/sub slots for decoupled widget injection)        │
└─────────────────────────────────────────────────────────┘
```

Each player in split-screen gets their own `PrimaryGameLayout` — all UI is per-player, never global.

---

## Layer 1: UI Manager Subsystem

### How It Works in Lyra

`UGameUIManagerSubsystem` is a `UGameInstanceSubsystem` — meaning it lives for the entire lifetime of the game instance, surviving world/level transitions. It is **abstract**, so Lyra subclasses it as `ULyraUIManagerSubsystem`.

**Source**: `Plugins/CommonGame/Source/Public/GameUIManagerSubsystem.h`

```cpp
UCLASS(MinimalAPI, Abstract, config = Game)
class UGameUIManagerSubsystem : public UGameInstanceSubsystem
{
    GENERATED_BODY()

public:
    virtual void Initialize(FSubsystemCollectionBase& Collection) override;
    virtual void Deinitialize() override;
    virtual bool ShouldCreateSubsystem(UObject* Outer) const override;

    const UGameUIPolicy* GetCurrentUIPolicy() const { return CurrentPolicy; }
    UGameUIPolicy* GetCurrentUIPolicy() { return CurrentPolicy; }

    virtual void NotifyPlayerAdded(UCommonLocalPlayer* LocalPlayer);
    virtual void NotifyPlayerRemoved(UCommonLocalPlayer* LocalPlayer);
    virtual void NotifyPlayerDestroyed(UCommonLocalPlayer* LocalPlayer);

protected:
    void SwitchToPolicy(UGameUIPolicy* InPolicy);

private:
    UPROPERTY(Transient)
    TObjectPtr<UGameUIPolicy> CurrentPolicy = nullptr;

    UPROPERTY(config, EditAnywhere)
    TSoftClassPtr<UGameUIPolicy> DefaultUIPolicyClass;  // Configurable in .ini
};
```

**Key responsibilities**:

1. **Owns the UI Policy** — the policy is created when the subsystem initializes, based on the class configured in `DefaultUIPolicyClass`
2. **Listens to player lifecycle** — when a local player is added, removed, or destroyed, the subsystem tells the policy
3. **Singleton access point** — any code can reach the policy via `GameInstance->GetSubsystem<UGameUIManagerSubsystem>()->GetCurrentUIPolicy()`
4. **Policy switching** — supports `SwitchToPolicy()` at runtime (though Lyra never actually switches)

### Unity Equivalent

```csharp
// Assets/Scripts/UI/Core/UIManager.cs
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Single entry point for all UI lifecycle. Lives in DontDestroyOnLoad.
/// Owns the UIPolicy and exposes it as a static accessor.
/// </summary>
[DefaultExecutionOrder(-1000)]
public class UIManager : MonoBehaviour
{
    [SerializeField]
    private UIPolicy defaultPolicy;

    public static UIManager Instance { get; private set; }
    public UIPolicy CurrentPolicy { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CurrentPolicy = Instantiate(defaultPolicy);
        CurrentPolicy.Initialize();
    }

    public void SwitchToPolicy(UIPolicy newPolicy)
    {
        if (CurrentPolicy != null)
        {
            CurrentPolicy.Shutdown();
            Destroy(CurrentPolicy.gameObject);
        }

        CurrentPolicy = Instantiate(newPolicy);
        CurrentPolicy.Initialize();
    }

    // Called by your player management system when players join/leave.
    // In single-player, this fires once on startup.
    public void NotifyPlayerAdded(GameObject playerObject)
    {
        CurrentPolicy?.NotifyPlayerAdded(playerObject);
    }

    public void NotifyPlayerRemoved(GameObject playerObject)
    {
        CurrentPolicy?.NotifyPlayerRemoved(playerObject);
    }
}
```

**Why this pattern**: The manager acts as a seam — the rest of the game doesn't need to know which policy class is active. You can swap policies for different game modes (e.g., a "training mode" policy that shows different UI). It also makes testing trivial: inject a mock policy.

**Set up the boot scene**: Create a scene with just a `UIInitializer` GameObject that has the `UIManager` component. Load it once at `[RuntimeInitializeOnLoadMethod]` or in your project's "initialization" scene (before the splash screen).

---

## Layer 2: UI Policy

### How It Works in Lyra

`UGameUIPolicy` defines **the rules** for creating and managing UI per player. It is `Within = GameUIManagerSubsystem`, meaning it's always created as a subobject of the manager, giving it a guaranteed lifetime.

**Source**: `Plugins/CommonGame/Source/Public/GameUIPolicy.h`

```cpp
UENUM()
enum class ELocalMultiplayerInteractionMode : uint8
{
    PrimaryOnly,   // Fullscreen for primary player only
    SingleToggle,  // Fullscreen for one player, can swap who's active
    Simultaneous   // Split-screen: both players visible simultaneously
};

USTRUCT()
struct FRootViewportLayoutInfo
{
    UPROPERTY(Transient)
    TObjectPtr<ULocalPlayer> LocalPlayer = nullptr;

    UPROPERTY(Transient)
    TObjectPtr<UPrimaryGameLayout> RootLayout = nullptr;

    UPROPERTY(Transient)
    bool bAddedToViewport = false;
};
```

The policy maintains a `TArray<FRootViewportLayoutInfo> RootViewportLayouts` — one entry per local player. Each entry holds:
- Which `ULocalPlayer` this layout belongs to
- The `UPrimaryGameLayout` widget instance
- Whether it's currently added to the viewport

**Critical implementation detail** — the policy handles three states for player layouts:

```
Player joins (NotifyPlayerAdded)
    │
    ├─ Has layout, not in viewport → AddLayoutToViewport()
    │                                 (re-add on player controller change)
    ├─ Has layout, in viewport     → No-op (already good)
    └─ No layout                   → CreateLayoutWidget()

Player leaves (NotifyPlayerRemoved)
    │
    └─ RemoveLayoutFromViewport() + mark bAddedToViewport = false
       (Layout stays alive in memory for potential rejoins)

Player destroyed (NotifyPlayerDestroyed)
    │
    └─ Free layout entirely + OnRootLayoutReleased()
```

The policy delegates to `GetLayoutWidgetClass()` to resolve the layout class — in Lyra's case, this loads `B_LyraUIPolicy` which is configured to use `W_OverallUILayout` as the root. The critical flexibility: you can override `GetLayoutWidgetClass()` to return different layouts for different players or contexts (e.g., a "spectator layout" vs "player layout").

The implementation in `GameUIPolicy.cpp`:

```cpp
void UGameUIPolicy::CreateLayoutWidget(UCommonLocalPlayer* LocalPlayer)
{
    if (APlayerController* PlayerController = LocalPlayer->GetPlayerController(GetWorld()))
    {
        TSubclassOf<UPrimaryGameLayout> LayoutWidgetClass = GetLayoutWidgetClass(LocalPlayer);
        if (ensure(LayoutWidgetClass && !LayoutWidgetClass->HasAnyClassFlags(CLASS_Abstract)))
        {
            UPrimaryGameLayout* NewLayoutObject = CreateWidget<UPrimaryGameLayout>(
                PlayerController, LayoutWidgetClass);
            RootViewportLayouts.Emplace(LocalPlayer, NewLayoutObject, true);
            AddLayoutToViewport(LocalPlayer, NewLayoutObject);
        }
    }
}

void UGameUIPolicy::AddLayoutToViewport(UCommonLocalPlayer* LocalPlayer, UPrimaryGameLayout* Layout)
{
    Layout->SetPlayerContext(FLocalPlayerContext(LocalPlayer));
    Layout->AddToPlayerScreen(1000);  // Z-order 1000
    OnRootLayoutAddedToViewport(LocalPlayer, Layout);
}
```

The `1000` Z-order is intentional: it puts the root above everything but below the debug layer. In the Unity world, this maps to `UIDocument.sortingOrder`.

### Split-Screen Interaction

Lyra supports three split-screen modes. `SingleToggle` is the most interesting: when player 2 takes control, player 1's layout goes "dormant" (collapses visibility) and player 2's layout becomes active. The dormancy system uses `SetIsDormant(true/false)` on the `PrimaryGameLayout`.

### Unity Equivalent

```csharp
// Assets/Scripts/UI/Core/UIPolicy.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public struct RootViewportInfo
{
    public GameObject PlayerObject;
    public UIDocument RootDocument;
    public UIPrimaryLayout RootLayout;
    public bool IsAddedToViewport;
}

public enum SplitScreenMode
{
    PrimaryOnly,
    SingleToggle,
    Simultaneous
}

/// <summary>
/// Defines how UIs are created and managed per player. Owned by UIManager.
/// Creates the root UIDocument and Primary Layout for each local player.
/// </summary>
public class UIPolicy : MonoBehaviour
{
    [SerializeField]
    private GameObject uiDocumentPrefab;  // Prefab with UIDocument + UIPrimaryLayout

    [SerializeField]
    private SplitScreenMode splitScreenMode = SplitScreenMode.PrimaryOnly;

    private readonly List<RootViewportInfo> _rootViewports = new();

    public void Initialize()
    {
        // Find or create the UI camera if needed
        // Setup UIDocument panel settings from a common config
    }

    public void Shutdown() { }

    public UIPrimaryLayout GetRootLayout(GameObject playerObject)
    {
        foreach (var info in _rootViewports)
        {
            if (info.PlayerObject == playerObject)
                return info.RootLayout;
        }
        return null;
    }

    public void NotifyPlayerAdded(GameObject playerObject)
    {
        var existingIndex = _rootViewports.FindIndex(i => i.PlayerObject == playerObject);

        if (existingIndex >= 0)
        {
            if (!_rootViewports[existingIndex].IsAddedToViewport)
            {
                var info = _rootViewports[existingIndex];
                info.IsAddedToViewport = true;
                _rootViewports[existingIndex] = info;
                OnRootLayoutAddedToViewport(info);
            }
        }
        else
        {
            CreateLayoutForPlayer(playerObject);
        }
    }

    public void NotifyPlayerRemoved(GameObject playerObject)
    {
        var index = _rootViewports.FindIndex(i => i.PlayerObject == playerObject);
        if (index >= 0)
        {
            var info = _rootViewports[index];
            RemoveLayoutFromViewport(info);

            if (splitScreenMode == SplitScreenMode.SingleToggle)
            {
                HandleSingleToggleTransfer(info);
            }

            info.IsAddedToViewport = false;
            _rootViewports[index] = info;
        }
    }

    public void NotifyPlayerDestroyed(GameObject playerObject)
    {
        NotifyPlayerRemoved(playerObject);

        var index = _rootViewports.FindIndex(i => i.PlayerObject == playerObject);
        if (index >= 0)
        {
            var info = _rootViewports[index];
            _rootViewports.RemoveAt(index);

            if (info.RootDocument != null)
            {
                info.RootLayout.Shutdown();
                Destroy(info.RootDocument.gameObject);
            }
        }
    }

    private void CreateLayoutForPlayer(GameObject playerObject)
    {
        var go = Instantiate(uiDocumentPrefab, transform);
        var doc = go.GetComponent<UIDocument>();
        var layout = go.GetComponent<UIPrimaryLayout>();

        layout.Initialize(playerObject);

        var info = new RootViewportInfo
        {
            PlayerObject = playerObject,
            RootDocument = doc,
            RootLayout = layout,
            IsAddedToViewport = true
        };

        _rootViewports.Add(info);
        OnRootLayoutAddedToViewport(info);
    }

    private void AddLayoutToViewport(RootViewportInfo info)
    {
        info.RootDocument.sortingOrder = 1000;
        // UIDocument.gameObject is already in scene, no separate "add to viewport" needed
    }

    private void RemoveLayoutFromViewport(RootViewportInfo info)
    {
        // Optionally hide or disable the document
        info.RootDocument.enabled = false;
    }

    private void OnRootLayoutAddedToViewport(RootViewportInfo info)
    {
        // Hook: override in subclasses for platform-specific setup
        // (e.g., set focus for gamepad-only platforms)
    }

    private void HandleSingleToggleTransfer(RootViewportInfo removedInfo)
    {
        if (removedInfo.RootLayout != null && !removedInfo.RootLayout.IsDormant)
        {
            removedInfo.RootLayout.SetIsDormant(true);

            foreach (var info in _rootViewports)
            {
                if (info.RootLayout != null && info.RootLayout.IsDormant)
                {
                    info.RootLayout.SetIsDormant(false);
                    break;
                }
            }
        }
    }

    // Accessor used by game systems
    public static UIPolicy GetUIPolicy(Component context)
    {
        return UIManager.Instance?.CurrentPolicy;
    }
}
```

---

## Layer 3: Primary Game Layout

### How It Works in Lyra

`UPrimaryGameLayout` is the root widget for a single player. It extends `UCommonUserWidget` (not `UCommonActivatableWidget` — it's always visible when the player is active, it doesn't get pushed/popped). It owns a **dictionary of named layers**.

**Source**: `Plugins/CommonGame/Source/Public/PrimaryGameLayout.h`

The core data structure:

```cpp
// Layers are registered by tag and backed by a stack container
UPROPERTY(Transient, meta = (Categories = "UI.Layer"))
TMap<FGameplayTag, TObjectPtr<UCommonActivatableWidgetContainerBase>> Layers;
```

Layers are registered in blueprint (typically in `W_OverallUILayout`):

```
W_OverallUILayout (PrimaryGameLayout)
├── Overlay
│   ├── UI.Layer.Game       → ActivatableWidgetStack (horizontal)
│   ├── UI.Layer.GameMenu   → ActivatableWidgetStack
│   ├── UI.Layer.Menu       → ActivatableWidgetStack
│   ├── UI.Layer.Modal      → ActivatableWidgetStack
│   └── UI.Layer.Cinematic  → ActivatableWidgetStack
```

**The Push/Pop API**:

```cpp
// Synchronous push: creates widget immediately, adds to layer stack, activates it
template <typename ActivatableWidgetT>
ActivatableWidgetT* PushWidgetToLayerStack(
    FGameplayTag LayerName,
    UClass* ActivatableWidgetClass,
    TFunctionRef<void(ActivatableWidgetT&)> InitInstanceFunc
);

// Async push: suspends input, async-loads the class, then pushes
template <typename ActivatableWidgetT>
TSharedPtr<FStreamableHandle> PushWidgetToLayerStackAsync(
    FGameplayTag LayerName,
    bool bSuspendInputUntilComplete,
    TSoftClassPtr<UCommonActivatableWidget> ActivatableWidgetClass,
    TFunction<void(EAsyncWidgetLayerState, ActivatableWidgetT*)> StateFunc
);

// Remove a widget from whichever layer it's on
void FindAndRemoveWidgetFromLayer(UCommonActivatableWidget* ActivatableWidget);
```

**How the async push works** (this is critical):

1. Optionally suspend input for the owning player via `UCommonUIExtensions::SuspendInputForPlayer()`
2. Call `UAssetManager::Get().GetStreamableManager().RequestAsyncLoad()` to async-load the widget class
3. On load complete: resume input, call `PushWidgetToLayerStack<ActivatableWidgetT>(LayerName, Class, InitFunc)` synchronously (the class is now loaded, so creating it is fast)
4. If the async load is canceled (e.g., level transition occurs mid-load), resume input and call the state func with `EAsyncWidgetLayerState::Canceled`

**The synchronous push**:

```cpp
template <typename ActivatableWidgetT>
ActivatableWidgetT* PushWidgetToLayerStack(
    FGameplayTag LayerName,
    UClass* ActivatableWidgetClass,
    TFunctionRef<void(ActivatableWidgetT&)> InitInstanceFunc)
{
    if (UCommonActivatableWidgetContainerBase* Layer = GetLayerWidget(LayerName))
    {
        return Layer->AddWidget<ActivatableWidgetT>(ActivatableWidgetClass, InitInstanceFunc);
    }
    return nullptr;
}
```

The `AddWidget<T>()` call on the container creates the widget, calls `InitInstanceFunc` to configure it, adds it to the stack, and activates it. The container handles:
- Deactivating the previously-active widget in the stack
- Transitioning focus to the new widget
- Managing the activation/deactivation lifecycle callbacks

**Dormancy**: When a layout is "dormant," it collapses to hidden and stops ticking. This is used for split-screen `SingleToggle` mode — only one player's layout is "alive" at a time.

**Input suspension during layer transitions**:

```cpp
void UPrimaryGameLayout::OnWidgetStackTransitioning(
    UCommonActivatableWidgetContainerBase* Widget, bool bIsTransitioning)
{
    if (bIsTransitioning)
    {
        const FName SuspendToken = UCommonUIExtensions::SuspendInputForPlayer(
            GetOwningLocalPlayer(), "GlobalStackTransition");
        SuspendInputTokens.Add(SuspendToken);
    }
    else
    {
        if (ensure(SuspendInputTokens.Num() > 0))
        {
            const FName SuspendToken = SuspendInputTokens.Pop();
            UCommonUIExtensions::ResumeInputForPlayer(GetOwningLocalPlayer(), SuspendToken);
        }
    }
}
```

Input suspension is **reference-counted** — when multiple widgets are pushing simultaneously, only the final Pop() resumes input.

### Unity Equivalent

```csharp
// Assets/Scripts/UI/Core/UIPrimaryLayout.cs
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Root UI layout for a single player. Manages named widget layers.
/// Each layer is a VisualElement container that acts as a stack for activatable widgets (screens).
/// </summary>
public class UIPrimaryLayout : MonoBehaviour
{
    [SerializeField]
    private UIDocument document;

    [SerializeField]
    private PanelSettings panelSettings;

    private readonly Dictionary<string, UILayer> _layers = new();
    private readonly List<string> _suspendedInputTokens = new();
    private bool _isDormant;

    public UIDocument Document => document;
    public bool IsDormant => _isDormant;

    /// <summary>
    /// Call from Awake. Registers named layers from UXML by element name.
    /// Expects child elements named with the pattern "Layer_<Name>" or similar.
    /// </summary>
    public void Initialize(GameObject playerObject)
    {
        document.panelSettings = panelSettings;

        // Auto-discover layers: find all VisualElements whose name starts with "Layer_"
        RegisterLayersFromUXML();
    }

    private void RegisterLayersFromUXML()
    {
        var root = document.rootVisualElement;
        if (root == null) return;

        foreach (var child in root.Children())
        {
            if (child.name.StartsWith("Layer_"))
            {
                var layerTag = child.name.Substring(6); // "Layer_Menu" → "Menu"
                RegisterLayer(layerTag, child);
            }
        }
    }

    public void RegisterLayer(string layerTag, VisualElement container)
    {
        if (!_layers.ContainsKey(layerTag))
        {
            var layer = new UILayer(layerTag, container);
            layer.OnTransitionStarted += SuspendInputForTransition;
            layer.OnTransitionComplete += ResumeInputForTransition;
            _layers[layerTag] = layer;
        }
    }

    public UILayer GetLayer(string layerTag)
    {
        _layers.TryGetValue(layerTag, out var layer);
        return layer;
    }

    public void SetIsDormant(bool dormant)
    {
        if (_isDormant == dormant) return;

        _isDormant = dormant;
        document.rootVisualElement.style.display = dormant
            ? DisplayStyle.None
            : DisplayStyle.Flex;
    }

    /// <summary>
    /// Push a screen onto a layer. Creates the widget from an addressable or prefab,
    /// adds it to the layer's container, and manages activation lifecycle.
    /// </summary>
    public T PushWidgetToLayerStack<T>(
        string layerTag,
        VisualTreeAsset widgetTemplate,
        Action<T> initFunc = null
    ) where T : UIScreen
    {
        var layer = GetLayer(layerTag);
        if (layer == null)
        {
            Debug.LogError($"Layer '{layerTag}' not registered on {name}");
            return null;
        }

        var widgetElement = widgetTemplate.CloneTree();
        var screen = widgetElement.Q<T>();
        // If the screen component is on a child, find it
        if (screen == null)
        {
            Debug.LogError($"No {typeof(T).Name} component found on widget element");
            return null;
        }

        initFunc?.Invoke(screen);
        layer.PushScreen(screen);
        return screen;
    }

    /// <summary>
    /// Async variant: loads the VisualTreeAsset from Addressables, then pushes.
    /// Returns an IAsyncOperation that callers can await or cancel.
    /// </summary>
    public async Awaitable<T> PushWidgetToLayerStackAsync<T>(
        string layerTag,
        string addressableKey,
        bool suspendInputUntilComplete,
        Action<EAsyncWidgetState, T> stateCallback = null
    ) where T : UIScreen
    {
        string suspendToken = null;
        if (suspendInputUntilComplete)
        {
            suspendToken = SuspendInput(this);
        }

        try
        {
            var handle = UnityEngine.AddressableAssets.Addressables
                .LoadAssetAsync<VisualTreeAsset>(addressableKey);
            await handle.Task;

            if (suspendInputUntilComplete)
            {
                ResumeInput(suspendToken);
            }

            var widget = PushWidgetToLayerStack<T>(layerTag, handle.Result,
                screen => stateCallback?.Invoke(EAsyncWidgetState.Initialize, screen));

            stateCallback?.Invoke(EAsyncWidgetState.AfterPush, widget);
            return widget;
        }
        catch (OperationCanceledException)
        {
            if (suspendInputUntilComplete)
            {
                ResumeInput(suspendToken);
            }
            stateCallback?.Invoke(EAsyncWidgetState.Canceled, null);
            return null;
        }
    }

    /// <summary>
    /// Remove a screen from whatever layer it's on.
    /// </summary>
    public void FindAndRemoveWidgetFromLayer(UIScreen screen)
    {
        foreach (var layer in _layers.Values)
        {
            if (layer.RemoveScreen(screen))
                return;
        }
    }

    // --- Input Suspension (reference-counted) ---

    private void SuspendInputForTransition(UILayer layer)
    {
        var token = SuspendInput(this);
        _suspendedInputTokens.Add(token);
    }

    private void ResumeInputForTransition(UILayer layer)
    {
        if (_suspendedInputTokens.Count > 0)
        {
            var token = _suspendedInputTokens[^1];
            _suspendedInputTokens.RemoveAt(_suspendedInputTokens.Count - 1);
            ResumeInput(token);
        }
    }

    public static string SuspendInput(object requester)
    {
        var token = $"UI_Suspend_{requester.GetHashCode()}_{Time.frameCount}";
        // Implementation: disable input system action maps, set picking mode, etc.
        return token;
    }

    public static void ResumeInput(string token)
    {
        // Implementation: re-enable input if no more suspension tokens exist
    }

    // --- Static accessors ---

    public static UIPrimaryLayout GetPrimaryGameLayout(Component context)
    {
        // Walk up from context to find the owning UIPolicy
        var policy = UIPolicy.GetUIPolicy(context);
        // In single-player, return the first/primary root layout
        return policy?.GetPrimaryRootLayout();
    }

    public void Shutdown()
    {
        foreach (var layer in _layers.Values)
        {
            layer.Clear();
        }
        _layers.Clear();
    }
}


// --- UILayer (Stack Container) ---

/// <summary>
/// Manages a stack of UIScreens within a VisualElement container.
/// Only the topmost screen is "active" — the rest are deactivated.
/// </summary>
public class UILayer
{
    private readonly string _layerTag;
    private readonly VisualElement _container;
    private readonly Stack<UIScreen> _stack = new();

    public event Action<UILayer> OnTransitionStarted;
    public event Action<UILayer> OnTransitionComplete;

    public string Tag => _layerTag;
    public UIScreen TopScreen => _stack.Count > 0 ? _stack.Peek() : null;

    public UILayer(string layerTag, VisualElement container)
    {
        _layerTag = layerTag;
        _container = container;
    }

    public void PushScreen(UIScreen screen)
    {
        // Deactivate the current top screen if there is one
        if (_stack.Count > 0)
        {
            _stack.Peek().OnScreenDeactivated();
        }

        _stack.Push(screen);
        _container.Add(screen.RootVisualElement);
        screen.OwningLayer = this;

        OnTransitionStarted?.Invoke(this);

        screen.OnScreenActivated();

        OnTransitionComplete?.Invoke(this);
    }

    public bool RemoveScreen(UIScreen screen)
    {
        if (_stack.Peek() == screen)
        {
            return PopScreen(screen);
        }

        // Screen is not at the top — uncommon, but handle it
        var tempStack = new Stack<UIScreen>();
        bool found = false;

        while (_stack.Count > 0)
        {
            var top = _stack.Pop();
            if (top == screen)
            {
                found = true;
                break;
            }
            tempStack.Push(top);
        }

        // Restore remaining screens
        while (tempStack.Count > 0)
        {
            _stack.Push(tempStack.Pop());
        }

        if (found)
        {
            OnTransitionStarted?.Invoke(this);
            screen.OnScreenDeactivated();
            screen.RootVisualElement.RemoveFromHierarchy();
            OnTransitionComplete?.Invoke(this);
        }

        return found;
    }

    public bool PopScreen(UIScreen screen)
    {
        if (_stack.Count == 0 || _stack.Peek() != screen)
            return false;

        OnTransitionStarted?.Invoke(this);

        _stack.Pop();
        screen.OnScreenDeactivated();
        screen.RootVisualElement.RemoveFromHierarchy();

        // Reactivate the new top screen
        if (_stack.Count > 0)
        {
            _stack.Peek().OnScreenActivated();
        }

        OnTransitionComplete?.Invoke(this);
        return true;
    }

    public void Clear()
    {
        while (_stack.Count > 0)
        {
            var screen = _stack.Pop();
            screen.OnScreenDeactivated();
            screen.RootVisualElement.RemoveFromHierarchy();
        }
    }
}
```

---

## Layer 4: Activatable Widgets (Screens)

### How It Works in Lyra

`ULyraActivatableWidget` extends Unreal's `UCommonActivatableWidget`. This is the base class for **every UI screen** — menus, HUD layouts, dialogs, confirmations. The key addition is `ELyraWidgetInputMode`:

```cpp
UENUM(BlueprintType)
enum class ELyraWidgetInputMode : uint8
{
    Default,
    GameAndMenu,  // Both game AND UI input active (e.g., HUD with cursor)
    Game,         // Only game input (typical HUD)
    Menu          // Only UI input (menus, dialogs)
};

UCLASS(Abstract, Blueprintable)
class ULyraActivatableWidget : public UCommonActivatableWidget
{
    UPROPERTY(EditDefaultsOnly, Category = Input)
    ELyraWidgetInputMode InputConfig = ELyraWidgetInputMode::Default;

    UPROPERTY(EditDefaultsOnly, Category = Input)
    EMouseCaptureMode GameMouseCaptureMode = EMouseCaptureMode::CapturePermanently;
};
```

The `GetDesiredInputConfig()` override tells the CommonUI framework what input rules to apply when this widget activates:

```cpp
TOptional<FUIInputConfig> ULyraActivatableWidget::GetDesiredInputConfig() const
{
    switch (InputConfig)
    {
        case ELyraWidgetInputMode::GameAndMenu:
            // Both game and UI input active
            return FUIInputConfig(ECommonInputMode::All, GameMouseCaptureMode);
        case ELyraWidgetInputMode::Game:
            // Only game input (HUD)
            return FUIInputConfig(ECommonInputMode::Game, EMouseCaptureMode::CapturePermanently);
        case ELyraWidgetInputMode::Menu:
            // Only UI input (menus)
            return FUIInputConfig(ECommonInputMode::Menu, EMouseCaptureMode::NoCapture);
        default:
            return TOptional<FUIInputConfig>();
    }
}
```

The `UCommonActivatableWidget` base provides the activation lifecycle:
- `NativeOnActivated()` — called when this widget becomes the active widget on its layer
- `NativeOnDeactivated()` — called when another widget pushes on top or this one is popped
- `OnActivated()` / `OnDeactivated()` — Blueprint-accessible delegates

The key lifecycle: `SetIsDormant → OnActivated → OnDeactivated → SetIsDormant`

**Input routing**: When a "Menu" widget is activated on `UI.Layer.Menu`, the game input is automatically suspended for the player — no explicit "disable input" calls needed. When it deactivates, game input resumes. This per-widget input declaration is what makes the push/pop model work cleanly.

### LyraHUDLayout

`ULyraHUDLayout` extends `ULyraActivatableWidget` — it's the in-game HUD layout that hosts crosshairs, health bars, etc. It also handles:
- **Escape menu**: Listening for the "Escape" button and pushing `EscapeMenuClass` onto `UI.Layer.Menu`
- **Controller disconnection**: Detecting when the player's controller disconnects and pushing a `ControllerDisconnectedScreen` — platform-specific via `FGameplayTagContainer PlatformRequiresControllerDisconnectScreen`

```cpp
class ULyraHUDLayout : public ULyraActivatableWidget
{
    UPROPERTY(EditDefaultsOnly)
    TSoftClassPtr<UCommonActivatableWidget> EscapeMenuClass;

    UPROPERTY(EditDefaultsOnly, Category="Controller Disconnect Menu")
    TSubclassOf<ULyraControllerDisconnectedScreen> ControllerDisconnectedScreen;

    UPROPERTY(EditDefaultsOnly, Category="Controller Disconnect Menu")
    FGameplayTagContainer PlatformRequiresControllerDisconnectScreen;
};
```

### Unity Equivalent

```csharp
// Assets/Scripts/UI/Core/UIScreen.cs
using System;
using UnityEngine;
using UnityEngine.UIElements;

public enum EScreenInputMode
{
    Default,
    GameAndMenu,  // Both game and UI input active
    Game,         // Only game input
    Menu          // Only UI input
}

public enum EAsyncWidgetState
{
    Canceled,
    Initialize,
    AfterPush
}

/// <summary>
/// Base class for every screen in the game. Attached to UXML root element.
/// Each screen declares its input mode — the layer system automatically
/// configures input based on which screen is active.
/// </summary>
public abstract class UIScreen : MonoBehaviour
{
    [Header("Input Configuration")]
    [SerializeField]
    private EScreenInputMode _inputMode = EScreenInputMode.Default;

    [SerializeField]
    private bool _captureMouseInGameMode = true;

    public EScreenInputMode InputMode => _inputMode;
    public bool CaptureMouseInGameMode => _captureMouseInGameMode;
    public UILayer OwningLayer { get; set; }
    public VisualElement RootVisualElement { get; protected set; }

    /// <summary>
    /// Set the root element — called after the UXML tree is cloned.
    /// </summary>
    public void SetRootElement(VisualElement root)
    {
        RootVisualElement = root;
        OnRootElementSet();
    }

    protected virtual void OnRootElementSet() { }

    /// <summary>
    /// Called when this screen becomes the active screen on its layer.
    /// Override to register callbacks, bind data, play entrance animation.
    /// </summary>
    public virtual void OnScreenActivated()
    {
        ApplyInputConfig();
        PlayEntranceAnimation();
    }

    /// <summary>
    /// Called when another screen is pushed on top, or this screen is popped.
    /// Override to unregister callbacks, save state, play exit animation.
    /// </summary>
    public virtual void OnScreenDeactivated()
    {
        RevertInputConfig();
    }

    private void ApplyInputConfig()
    {
        switch (_inputMode)
        {
            case EScreenInputMode.Menu:
                // Disable gameplay action map, enable UI action map
                // Set picking mode on this element's hierarchy
                InputSystem.SetUIInputMode();
                break;

            case EScreenInputMode.Game:
                // Enable gameplay action map, disable UI action map
                InputSystem.SetGameInputMode(_captureMouseInGameMode);
                break;

            case EScreenInputMode.GameAndMenu:
                // Both action maps active
                InputSystem.SetAllInputMode(_captureMouseInGameMode);
                break;

            case EScreenInputMode.Default:
                // Don't change input — let parent's settings remain
                break;
        }
    }

    private void RevertInputConfig()
    {
        // If this was a Menu screen, and there's still another menu screen
        // below it, don't revert game input yet. The layer system handles this.
        InputSystem.RevertToPreviousInputMode();
    }

    protected virtual void PlayEntranceAnimation() { }
    protected virtual void PlayExitAnimation() { }

    /// <summary>
    /// Helper: pop this screen from its layer (e.g., on "Back" button press).
    /// </summary>
    protected void DismissSelf()
    {
        OwningLayer?.PopScreen(this);
    }
}
```

---

## Layer 5: UI Extension System

### How It Works in Lyra

This is the **most architecturally interesting** part. The `UIExtension` plugin implements a publish/subscribe pattern that decouples "where UI goes" from "what UI should be shown." It uses `FGameplayTag` for addressing.

**Source**: `Plugins/UIExtension/Source/Public/UIExtensionSystem.h`

**Core data structures**:

```cpp
// An extension: something registered to be shown at a specific tag
struct FUIExtension : TSharedFromThis<FUIExtension>
{
    FGameplayTag ExtensionPointTag;  // e.g., "UI.Extension.HUD.HealthBar"
    int32 Priority = INDEX_NONE;
    TWeakObjectPtr<UObject> ContextObject;
    TObjectPtr<UObject> Data = nullptr;  // Could be a WidgetClass, a UObject, etc.
};

// An extension point: a slot in the UI that listens for matching extensions
struct FUIExtensionPoint : TSharedFromThis<FUIExtensionPoint>
{
    FGameplayTag ExtensionPointTag;       // e.g., "UI.Extension.HUD"
    EUIExtensionPointMatch ExtensionPointTagMatchType; // ExactMatch or PartialMatch
    TArray<UClass*> AllowedDataClasses;    // Filter for what type of data to accept
    FExtendExtensionPointDelegate Callback;  // Called when extensions are added/removed
};
```

**Tag matching rules**:

- `ExactMatch`: `UI.Extension.HUD` ↔ `UI.Extension.HUD` only
- `PartialMatch`: `UI.Extension.HUD` ↔ `UI.Extension.HUD` OR `UI.Extension.HUD.HealthBar` OR `UI.Extension.HUD.Ammo`

This means a single `UIExtensionPoint` with `PartialMatch` on `UI.Extension.HUD` will pick up ALL HUD extensions regardless of how specific their tag is. This is how "game features" inject HUD elements.

**The subsystem**:

```cpp
UCLASS(MinimalAPI)
class UUIExtensionSubsystem : public UWorldSubsystem
{
public:
    // Register a listener at a tag
    FUIExtensionPointHandle RegisterExtensionPoint(
        const FGameplayTag& ExtensionPointTag,
        EUIExtensionPointMatch ExtensionPointTagMatchType,
        const TArray<UClass*>& AllowedDataClasses,
        FExtendExtensionPointDelegate ExtensionCallback);

    // Register a widget to be shown at matching extension points
    FUIExtensionHandle RegisterExtensionAsWidget(
        const FGameplayTag& ExtensionPointTag,
        TSubclassOf<UUserWidget> WidgetClass,
        int32 Priority);

    // Register data (not a widget) at a tag
    FUIExtensionHandle RegisterExtensionAsData(
        const FGameplayTag& ExtensionPointTag,
        UObject* ContextObject,
        UObject* Data,
        int32 Priority);

private:
    // Both maps are keyed by tag
    TMap<FGameplayTag, FExtensionPointList> ExtensionPointMap;  // Listeners
    TMap<FGameplayTag, FExtensionList> ExtensionMap;            // Publishers
};
```

**The critical algorithm — `NotifyExtensionPointOfExtensions`**:

When a new extension point is registered, or when a new extension is registered, the subsystem walks its maps:

```
For each extension point registered:
    For each extension registered under a matching tag:
        If tag matches (according to the point's match type)
        AND the extension's Data class is in AllowedDataClasses:
            → Fire the extension point's Callback with Action=Added and the data

When an extension unregisters:
    → Fire the Callback with Action=Removed
```

**The UIExtensionPointWidget** (the Unity equivalent of the most interesting part):

```cpp
UCLASS(MinimalAPI)
class UUIExtensionPointWidget : public UDynamicEntryBoxBase
{
    UPROPERTY(EditAnywhere, Category = "UI Extension")
    FGameplayTag ExtensionPointTag;  // Tag this slot listens to

    UPROPERTY(EditAnywhere, Category = "UI Extension")
    EUIExtensionPointMatch ExtensionPointTagMatch = EUIExtensionPointMatch::ExactMatch;

    UPROPERTY(EditAnywhere, Category = "UI Extension")
    TArray<TSubclassOf<UObject>> DataClasses;  // Filter

    // Delegate: given a data object, return the widget class to spawn
    FOnGetWidgetClassForData GetWidgetClassForData;

    // Delegate: configure the spawned widget with the data
    FOnConfigureWidgetForData ConfigureWidgetForData;
};
```

When an extension arrives:

1. `GetWidgetClassForData(Data)` → returns the widget class to spawn
2. Creates the widget and adds it to the entry box
3. `ConfigureWidgetForData(Widget, Data)` → sets up the widget with the data
4. Stores the widget in `ExtensionMapping[Handle] = Widget`

When the extension unregisters, the corresponding widget is removed.

**Real-world example in Lyra**: The `ShooterCore` game feature plugin registers a health bar at tag `UI.Extension.HUD.PlayerStatus`. The HUD layout has a `UIExtensionPointWidget` listening on `UI.Extension.HUD` with `PartialMatch`. When the shooter experience activates, the health bar appears. If the game switches to a racing experience, the health bar would unregister and a speedometer would register.

### Unity Equivalent

```csharp
// Assets/Scripts/UI/Extensions/UIExtensionSubsystem.cs
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// World-scoped publish/subscribe system for decoupled UI widget injection.
/// Extension points listen for extensions; extensions are published by game features.
/// Uses string tags (could use scriptable object enums for type safety).
/// </summary>
public enum EUIExtensionPointMatch
{
    ExactMatch,
    PartialMatch
}

public enum EUIExtensionAction
{
    Added,
    Removed
}

public class UIExtension
{
    public string ExtensionPointTag;
    public int Priority = -1;
    public object ContextObject;
    public object Data;
}

public class UIExtensionPoint
{
    public string ExtensionPointTag;
    public EUIExtensionPointMatch MatchType = EUIExtensionPointMatch.ExactMatch;
    public List<Type> AllowedDataClasses = new();
    public Action<EUIExtensionAction, UIExtension> Callback;
}

public class UIExtensionSubsystem
{
    private static UIExtensionSubsystem _instance;
    public static UIExtensionSubsystem Instance =>
        _instance ??= new UIExtensionSubsystem();

    private readonly Dictionary<string, List<UIExtensionPoint>> _extensionPoints = new();
    private readonly Dictionary<string, List<UIExtension>> _extensions = new();

    /// <summary>
    /// Register a listener at a tag. Returns a handle that can be used to unregister.
    /// </summary>
    public UIExtensionPointHandle RegisterExtensionPoint(
        string tag,
        EUIExtensionPointMatch matchType,
        List<Type> allowedDataClasses,
        Action<EUIExtensionAction, UIExtension> callback)
    {
        var point = new UIExtensionPoint
        {
            ExtensionPointTag = tag,
            MatchType = matchType,
            AllowedDataClasses = allowedDataClasses,
            Callback = callback
        };

        if (!_extensionPoints.ContainsKey(tag))
            _extensionPoints[tag] = new List<UIExtensionPoint>();

        _extensionPoints[tag].Add(point);

        // Notify this point of all existing matching extensions
        NotifyExtensionPointOfExtensions(point);

        return new UIExtensionPointHandle(point, () => UnregisterExtensionPoint(point));
    }

    /// <summary>
    /// Register a widget (as a VisualTreeAsset) at a tag.
    /// </summary>
    public UIExtensionHandle RegisterExtensionAsWidget(
        string tag,
        VisualTreeAsset widgetAsset,
        int priority = -1)
    {
        var extension = new UIExtension
        {
            ExtensionPointTag = tag,
            Priority = priority,
            Data = widgetAsset
        };

        RegisterExtension(tag, extension);
        return new UIExtensionHandle(extension, () => UnregisterExtension(extension));
    }

    /// <summary>
    /// Register arbitrary data at a tag (e.g., ViewModel, struct).
    /// </summary>
    public UIExtensionHandle RegisterExtensionAsData(
        string tag,
        object contextObject,
        object data,
        int priority = -1)
    {
        var extension = new UIExtension
        {
            ExtensionPointTag = tag,
            Priority = priority,
            ContextObject = contextObject,
            Data = data
        };

        RegisterExtension(tag, extension);
        return new UIExtensionHandle(extension, () => UnregisterExtension(extension));
    }

    private void RegisterExtension(string tag, UIExtension extension)
    {
        if (!_extensions.ContainsKey(tag))
            _extensions[tag] = new List<UIExtension>();

        _extensions[tag].Add(extension);

        // Notify all matching extension points
        NotifyExtensionPointsOfExtension(EUIExtensionAction.Added, extension);
    }

    public void UnregisterExtension(UIExtension extension)
    {
        NotifyExtensionPointsOfExtension(EUIExtensionAction.Removed, extension);

        if (_extensions.TryGetValue(extension.ExtensionPointTag, out var list))
        {
            list.Remove(extension);
        }
    }

    public void UnregisterExtensionPoint(UIExtensionPoint point)
    {
        if (_extensionPoints.TryGetValue(point.ExtensionPointTag, out var list))
        {
            list.Remove(point);
        }
    }

    private void NotifyExtensionPointOfExtensions(UIExtensionPoint point)
    {
        foreach (var kvp in _extensions)
        {
            if (DoesTagMatch(point, kvp.Key))
            {
                foreach (var extension in kvp.Value)
                {
                    if (DoesExtensionPassContract(point, extension))
                    {
                        point.Callback?.Invoke(EUIExtensionAction.Added, extension);
                    }
                }
            }
        }
    }

    private void NotifyExtensionPointsOfExtension(
        EUIExtensionAction action, UIExtension extension)
    {
        // Check extension points that match the extension's tag
        // AND also check extension points with PartialMatch whose tag is a prefix
        foreach (var kvp in _extensionPoints)
        {
            if (DoesExtensionPointMatchExtensionTag(kvp.Key, extension.ExtensionPointTag))
            {
                foreach (var point in kvp.Value)
                {
                    if (DoesTagMatch(point, extension.ExtensionPointTag) &&
                        DoesExtensionPassContract(point, extension))
                    {
                        point.Callback?.Invoke(action, extension);
                    }
                }
            }
        }
    }

    private bool DoesTagMatch(UIExtensionPoint point, string extensionTag)
    {
        if (point.MatchType == EUIExtensionPointMatch.ExactMatch)
            return point.ExtensionPointTag == extensionTag;

        // PartialMatch: point tag must be a prefix of extension tag
        // "UI.HUD" matches "UI.HUD.HealthBar"
        return extensionTag.StartsWith(point.ExtensionPointTag);
    }

    private bool DoesExtensionPointMatchExtensionTag(
        string pointTag, string extensionTag)
    {
        // True if they're exactly equal OR the point has PartialMatch and the
        // extension tag starts with the point tag
        return pointTag == extensionTag ||
               extensionTag.StartsWith(pointTag);
    }

    private bool DoesExtensionPassContract(UIExtensionPoint point, UIExtension extension)
    {
        if (point.AllowedDataClasses.Count == 0) return true;

        if (extension.Data == null) return false;

        foreach (var allowedType in point.AllowedDataClasses)
        {
            if (allowedType.IsAssignableFrom(extension.Data.GetType()))
                return true;
        }

        return false;
    }
}


// --- Handle types for safe unregistration ---

public class UIExtensionPointHandle
{
    private readonly UIExtensionPoint _point;
    private readonly Action _unregisterAction;

    public UIExtensionPointHandle(UIExtensionPoint point, Action unregisterAction)
    {
        _point = point;
        _unregisterAction = unregisterAction;
    }

    public void Unregister() => _unregisterAction?.Invoke();
}

public class UIExtensionHandle
{
    private readonly UIExtension _extension;
    private readonly Action _unregisterAction;

    public UIExtensionHandle(UIExtension extension, Action unregisterAction)
    {
        _extension = extension;
        _unregisterAction = unregisterAction;
    }

    public void Unregister() => _unregisterAction?.Invoke();
}
```

Now the **UIExtensionPointWidget** in Unity UI Toolkit:

```csharp
// Assets/Scripts/UI/Extensions/UIExtensionPointWidget.cs
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// A VisualElement that acts as an extension point in a UXML layout.
/// When placed in a UXML file with a tag, it auto-listenes for
/// matching extensions and spawns widgets accordingly.
/// </summary>
[UxmlElement("UIExtensionPoint")]
public class UIExtensionPointWidget : VisualElement
{
    // UXML attributes
    [UxmlAttribute]
    public string ExtensionPointTag { get; set; }

    [UxmlAttribute]
    public string MatchType { get; set; } = "Exact"; // "Exact" or "Partial"

    private readonly Dictionary<UIExtensionHandle, VisualElement> _widgetMap = new();
    private UIExtensionPointHandle _pointHandle;

    public UIExtensionPointWidget()
    {
        RegisterCallback<AttachToPanelEvent>(OnAttach);
        RegisterCallback<DetachFromPanelEvent>(OnDetach);
    }

    private void OnAttach(AttachToPanelEvent evt)
    {
        var matchType = MatchType == "Partial"
            ? EUIExtensionPointMatch.PartialMatch
            : EUIExtensionPointMatch.ExactMatch;

        _pointHandle = UIExtensionSubsystem.Instance.RegisterExtensionPoint(
            ExtensionPointTag,
            matchType,
            new List<Type> { typeof(VisualTreeAsset) },
            OnExtensionChanged
        );
    }

    private void OnDetach(DetachFromPanelEvent evt)
    {
        _pointHandle?.Unregister();
        _pointHandle = null;

        foreach (var widget in _widgetMap.Values)
        {
            widget.RemoveFromHierarchy();
        }
        _widgetMap.Clear();
    }

    private void OnExtensionChanged(EUIExtensionAction action, UIExtension extension)
    {
        if (extension.Data is not VisualTreeAsset treeAsset)
            return;

        if (action == EUIExtensionAction.Added)
        {
            var widget = treeAsset.CloneTree();
            Add(widget);

            var handle = new UIExtensionHandle(extension, null); // Simplified
            _widgetMap[handle] = widget;
        }
        else // Removed
        {
            // Find and remove the matching widget
            foreach (var kvp in _widgetMap)
            {
                // This is simplified — in production you'd track handles properly
                break;
            }
        }
    }

    // UXML factory
    [Obsolete]
    public new class UxmlFactory : UxmlFactory<UIExtensionPointWidget, UxmlTraits> { }

    [Obsolete]
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        private readonly UxmlStringAttributeDescription _tag = new()
            { name = "extension-point-tag", defaultValue = "" };
        private readonly UxmlStringAttributeDescription _match = new()
            { name = "match-type", defaultValue = "Exact" };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var widget = (UIExtensionPointWidget)ve;
            widget.ExtensionPointTag = _tag.GetValueFromBag(bag, cc);
            widget.MatchType = _match.GetValueFromBag(bag, cc);
        }
    }

    // Required for UI Toolkit to find this class
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void RegisterUxmlFactory()
    {
        // Ensure the type is registered with UI Toolkit
    }
}
```

### Real Usage Example

**1. Core game registers extension points in UXML**:

```xml
<!-- HUDLayout.uxml -->
<ui:UXML xmlns:ui="UnityEngine.UIElements">
    <ui:VisualElement name="Layer_Game">
        <!-- Extension points for decoupled HUD elements -->
        <ui:UIExtensionPoint name="Extension_Points_Container"
            extension-point-tag="UI.Extension.HUD"
            match-type="Partial" />

        <ui:VisualElement name="ReticleContainer">
            <ui:UIExtensionPoint extension-point-tag="UI.Extension.HUD.Reticle"
                match-type="Exact" />
        </ui:VisualElement>
    </ui:VisualElement>
</ui:UXML>
```

**2. A game feature (e.g., ShooterCore) publishes HUD elements**:

```csharp
// ShooterCoreUIRegistration.cs
public class ShooterCoreUIRegistration : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset healthBarTemplate;
    [SerializeField] private VisualTreeAsset ammoTemplate;
    [SerializeField] private VisualTreeAsset reticleTemplate;

    private List<UIExtensionHandle> _handles = new();

    private void OnEnable()
    {
        _handles.Add(UIExtensionSubsystem.Instance.RegisterExtensionAsWidget(
            "UI.Extension.HUD.PlayerHealth",
            healthBarTemplate,
            priority: 0));

        _handles.Add(UIExtensionSubsystem.Instance.RegisterExtensionAsWidget(
            "UI.Extension.HUD.AmmoCount",
            ammoTemplate,
            priority: 1));

        _handles.Add(UIExtensionSubsystem.Instance.RegisterExtensionAsWidget(
            "UI.Extension.HUD.Reticle",
            reticleTemplate,
            priority: 0));
    }

    private void OnDisable()
    {
        foreach (var handle in _handles)
        {
            handle.Unregister();
        }
        _handles.Clear();
    }
}
```

**3. A racing experience registers different HUD elements instead**:

```csharp
// RacingCoreUIRegistration.cs
public class RacingCoreUIRegistration : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset speedometerTemplate;
    [SerializeField] private VisualTreeAsset lapCounterTemplate;

    private List<UIExtensionHandle> _handles = new();

    private void OnEnable()
    {
        _handles.Add(UIExtensionSubsystem.Instance.RegisterExtensionAsWidget(
            "UI.Extension.HUD.Speedometer",
            speedometerTemplate));

        _handles.Add(UIExtensionSubsystem.Instance.RegisterExtensionAsWidget(
            "UI.Extension.HUD.LapCounter",
            lapCounterTemplate));
    }

    private void OnDisable()
    {
        foreach (var handle in _handles)
        {
            handle.Unregister();
        }
        _handles.Clear();
    }
}
```

Neither the HUD layout nor the core game code knows about health bars, ammo counters, or speedometers — the extension system handles the binding.

---

## Screen Navigation: The Frontend Control Flow

### How It Works in Lyra

`ULyraFrontendStateComponent` (a `UGameStateComponent`) orchestrates the **frontend screen flow**. It uses `FControlFlow` — Unreal Engine's sequential task chaining system.

**Source**: `Source/LyraGame/UI/Frontend/LyraFrontendStateComponent.cpp`

The control flow is a declarative pipeline of steps:

```cpp
void ULyraFrontendStateComponent::OnExperienceLoaded(const ULyraExperienceDefinition* Experience)
{
    FControlFlow& Flow = FControlFlowStatics::Create(this, "FrontendFlow")
        .QueueStep("Wait For User Initialization", ..., &FlowStep_WaitForUserInitialization)
        .QueueStep("Try Show Press Start Screen",  ..., &FlowStep_TryShowPressStartScreen)
        .QueueStep("Try Join Requested Session",   ..., &FlowStep_TryJoinRequestedSession)
        .QueueStep("Try Show Main Screen",         ..., &FlowStep_TryShowMainScreen);

    Flow.ExecuteFlow();
    FrontEndFlow = Flow.AsShared();
}
```

Each step receives a `FControlFlowNodeRef SubFlow` — a handle that the step must call `ContinueFlow()` or `CancelFlow()` on to advance the pipeline. This is **push-based sequencing**: each step decides when (or if) the next step runs.

**Step 1: Wait For User Initialization** — runs synchronously, immediately calls `SubFlow->ContinueFlow()`

**Step 2: Try Show Press Start Screen** — the most interesting step:

```cpp
void ULyraFrontendStateComponent::FlowStep_TryShowPressStartScreen(FControlFlowNodeRef SubFlow)
{
    // Check: does a user already exist?
    if (FirstUser is already logged in)
    {
        SubFlow->ContinueFlow();  // Skip press start screen
        return;
    }

    // Check: does this platform need a "press start" screen?
    if (!UserSubsystem->ShouldWaitForStartInput())
    {
        // Auto-login: start auto-login, then continue on completion
        InProgressPressStartScreen = SubFlow;
        UserSubsystem->OnUserInitializeComplete.AddDynamic(this, &OnUserInitialized);
        UserSubsystem->TryToInitializeForLocalPlay(0, FInputDeviceId(), false);
        return;
    }

    // Actually show the press start screen
    RootLayout->PushWidgetToLayerStackAsync<UCommonActivatableWidget>(
        "UI.Layer.Menu",
        bSuspendInputUntilComplete: true,
        PressStartScreenClass,
        [this, SubFlow](EAsyncWidgetLayerState State, UCommonActivatableWidget* Screen)
        {
            case EAsyncWidgetLayerState::AfterPush:
                bShouldShowLoadingScreen = false;
                // Chain: when this screen deactivates, continue the flow
                Screen->OnDeactivated().AddWeakLambda(this, [SubFlow]() {
                    SubFlow->ContinueFlow();
                });
                break;
            case EAsyncWidgetLayerState::Canceled:
                bShouldShowLoadingScreen = false;
                SubFlow->ContinueFlow();
                return;
        });
}
```

This is the pattern: **push a screen, wait for it to deactivate, then continue**. The flow pauses at this step until the user presses start (which triggers deactivation of the press start widget → `SubFlow->ContinueFlow()`).

**Step 3: Try Join Requested Session** — checks if there's a pending session join (e.g., from a platform invite). If yes, calls `GameInstance->JoinRequestedSession()` and cancels the rest of the flow (because joining a session will load a level). If no pending join, immediately continues.

**Step 4: Try Show Main Screen** — pushes the main menu to `UI.Layer.Menu`:

```cpp
void ULyraFrontendStateComponent::FlowStep_TryShowMainScreen(FControlFlowNodeRef SubFlow)
{
    if (UPrimaryGameLayout* RootLayout = GetPrimaryGameLayoutForPrimaryPlayer(this))
    {
        constexpr bool bSuspendInputUntilComplete = true;
        RootLayout->PushWidgetToLayerStackAsync<UCommonActivatableWidget>(
            "UI.Layer.Menu",
            bSuspendInputUntilComplete,
            MainScreenClass,
            [this, SubFlow](EAsyncWidgetLayerState State, UCommonActivatableWidget* Screen)
            {
                case EAsyncWidgetLayerState::AfterPush:
                    bShouldShowLoadingScreen = false;
                    SubFlow->ContinueFlow();  // Flow completes
                    return;
                case EAsyncWidgetLayerState::Canceled:
                    bShouldShowLoadingScreen = false;
                    SubFlow->ContinueFlow();
                    return;
            });
    }
}
```

**Loading screen integration** — the `ILoadingProcessInterface` implementation:

```cpp
bool ULyraFrontendStateComponent::ShouldShowLoadingScreen(FString& OutReason) const
{
    if (bShouldShowLoadingScreen)
    {
        OutReason = "Frontend Flow Pending...";
        if (FrontEndFlow.IsValid())
        {
            const TOptional<FString> StepDebugName = FrontEndFlow->GetCurrentStepDebugName();
            if (StepDebugName.IsSet())
            {
                OutReason = StepDebugName.GetValue();  // e.g., "Try Show Press Start Screen"
            }
        }
        return true;
    }
    return false;
}
```

While the frontend flow is executing, the loading screen stays visible. As soon as `bShouldShowLoadingScreen` is set to `false` (when the main screen is pushed), the loading screen disappears. This gives a seamless transition from "loading → press start → main menu."

**Important detail: flow cancellation**. If `SubFlow->CancelFlow()` is called (e.g., because the player joined a session), the remaining steps never execute. This is how Lyra handles DLC/session joins that bypass the normal flow.

### Unity Equivalent

```csharp
// Assets/Scripts/UI/Core/UIFlowController.cs
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ControlFlow: a sequential pipeline of async steps.
/// Each step receives a FlowNode that it must call Continue() or Cancel() on.
/// </summary>
public class UIFlowNode
{
    public string DebugName { get; init; }
    internal UIFlowController Owner { get; set; }

    public void Continue() => Owner?.OnStepComplete(this, cancelled: false);
    public void Cancel()  => Owner?.OnStepComplete(this, cancelled: true);
}

public class UIFlowController
{
    private readonly Queue<(string name, Action<UIFlowNode> step)> _steps = new();
    private UIFlowNode _currentNode;
    private bool _cancelled;

    public string CurrentStepName => _currentNode?.DebugName;

    public UIFlowController QueueStep(string name, Action<UIFlowNode> step)
    {
        _steps.Enqueue((name, step));
        return this;
    }

    public void Execute()
    {
        ProcessNextStep();
    }

    private void ProcessNextStep()
    {
        if (_cancelled || _steps.Count == 0) return;

        var (name, step) = _steps.Dequeue();
        _currentNode = new UIFlowNode { DebugName = name, Owner = this };

        Debug.Log($"[UIFlow] Executing step: {name}");
        step?.Invoke(_currentNode);
    }

    internal void OnStepComplete(UIFlowNode node, bool cancelled)
    {
        if (cancelled)
        {
            _cancelled = true;
            _steps.Clear();
            Debug.Log($"[UIFlow] Flow cancelled at step: {node.DebugName}");
            return;
        }

        ProcessNextStep();
    }
}


/// <summary>
/// Unity equivalent of LyraFrontendStateComponent.
/// Orchestrates the frontend screen flow: user init → press start → main menu.
/// </summary>
public class FrontendScreenFlow : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset pressStartScreenTemplate;
    [SerializeField] private VisualTreeAsset mainScreenTemplate;

    private UIPrimaryLayout _rootLayout;
    private bool _shouldShowLoadingScreen = true;

    private void Awake()
    {
        _rootLayout = GetComponent<UIPrimaryLayout>();
    }

    private void Start()
    {
        var flow = new UIFlowController()
            .QueueStep("Wait For User Initialization", Step_WaitForUserInitialization)
            .QueueStep("Try Show Press Start Screen",  Step_TryShowPressStartScreen)
            .QueueStep("Try Join Requested Session",   Step_TryJoinRequestedSession)
            .QueueStep("Try Show Main Screen",         Step_TryShowMainScreen);

        flow.Execute();
    }

    private void Step_WaitForUserInitialization(UIFlowNode node)
    {
        // Reset session/user state
        // In Unity: check if the player has a saved profile, load it
        node.Continue();
    }

    private void Step_TryShowPressStartScreen(UIFlowNode node)
    {
        // If player is already authenticated, skip
        if (UserManager.IsLoggedIn)
        {
            node.Continue();
            return;
        }

        if (!PlatformRequiresPressStartScreen())
        {
            // Auto-login for single-user platforms (PC, mobile)
            UserManager.AutoLogin(() => node.Continue());
            return;
        }

        // Show press start screen, continue when user presses start
        _rootLayout.PushWidgetToLayerStack<UIScreen>("Menu", pressStartScreenTemplate,
            screen =>
            {
                _shouldShowLoadingScreen = false;

                // When the press start screen deactivates (user pressed start),
                // continue the flow to show the main menu
                var pressStartScreen = screen as PressStartScreen;
                pressStartScreen.OnUserStarted += () =>
                {
                    UserManager.LoginPrimaryUser(() => node.Continue());
                };
            });
    }

    private void Step_TryJoinRequestedSession(UIFlowNode node)
    {
        // Check for pending session joins (deep links, platform invites)
        // If joining, cancel the flow (it'll restart after level load)
        if (SessionManager.HasPendingJoin)
        {
            SessionManager.JoinPending(() => { /* Handled by level load */ });
            node.Cancel();
            return;
        }

        node.Continue();
    }

    private void Step_TryShowMainScreen(UIFlowNode node)
    {
        _rootLayout.PushWidgetToLayerStack<UIScreen>("Menu", mainScreenTemplate,
            screen =>
            {
                _shouldShowLoadingScreen = false;
                // Main screen is now active. Flow is complete.
                // The user is now in control — navigation is driven by
                // button presses (Play → server browser → join → load level)
                node.Continue();
            });
    }

    private bool PlatformRequiresPressStartScreen()
    {
        // Consoles: yes (multiple profiles per device)
        // PC/Mobile: no
        return Application.platform == RuntimePlatform.PS4 ||
               Application.platform == RuntimePlatform.XboxOne ||
               Application.platform == RuntimePlatform.Switch;
    }

    // Loading screen check
    public bool ShouldShowLoadingScreen(out string reason)
    {
        if (_shouldShowLoadingScreen)
        {
            reason = "Frontend Flow Pending...";
            return true;
        }

        reason = null;
        return false;
    }
}
```

---

## HUD Construction via Game Features

### How It Works in Lyra

The `ShooterCore` experience (and other game-feature plugins) declare their UI dependencies using `UGameFeatureAction_AddWidgets`. This is how experiences customize the HUD without the core game code knowing about health bars, ammo counters, etc.

**Source**: `Source/LyraGame/GameFeatures/GameFeatureAction_AddWidget.h`

```cpp
USTRUCT()
struct FLyraHUDLayoutRequest
{
    UPROPERTY(EditAnywhere, Category=UI)
    TSoftClassPtr<UCommonActivatableWidget> LayoutClass;  // e.g., ShooterHUDLayout

    UPROPERTY(EditAnywhere, Category=UI, meta=(Categories="UI.Layer"))
    FGameplayTag LayerID;  // e.g., "UI.Layer.Game"
};

USTRUCT()
struct FLyraHUDElementEntry
{
    UPROPERTY(EditAnywhere, Category=UI)
    TSoftClassPtr<UUserWidget> WidgetClass;  // e.g., CrosshairWidget

    UPROPERTY(EditAnywhere, Category=UI)
    FGameplayTag SlotID;  // e.g., "UI.Extension.HUD.Reticle"
};

UCLASS(MinimalAPI, meta = (DisplayName = "Add Widgets"))
class UGameFeatureAction_AddWidgets final : public UGameFeatureAction_WorldActionBase
{
    UPROPERTY(EditAnywhere, Category=UI)
    TArray<FLyraHUDLayoutRequest> Layout;   // Full HUD layouts to push onto layers

    UPROPERTY(EditAnywhere, Category=UI)
    TArray<FLyraHUDElementEntry> Widgets;   // Individual widgets to inject via extension system
};
```

When the game feature activates:

1. **Layout push**: For each `FLyraHUDLayoutRequest`, it finds the `PrimaryGameLayout` for the relevant player and calls `PushWidgetToLayerStack(LayerID, LayoutClass)` — this adds the entire HUD to `UI.Layer.Game`
2. **Element injection**: For each `FLyraHUDElementEntry`, it calls `RegisterExtensionAsWidget(SlotID, WidgetClass)` on the `UIExtensionSubsystem` — this triggers any matching `UIExtensionPointWidget` to spawn the widget

When the feature deactivates (e.g., a different experience loads), the action removes all layouts and unregisters all extension handles.

### Unity Equivalent

```csharp
// Assets/Scripts/UI/Features/GameFeatureAction_AddWidgets.cs
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct HUDLayoutRequest
{
    [field: SerializeField] public VisualTreeAsset LayoutAsset { get; private set; }
    [field: SerializeField] public string LayerTag { get; private set; }
}

[Serializable]
public struct HUDElementEntry
{
    [field: SerializeField] public VisualTreeAsset WidgetAsset { get; private set; }
    [field: SerializeField] public string ExtensionTag { get; private set; }
}

/// <summary>
/// Declares what UI a game feature contributes.
/// Attach to a GameObject in a feature's prefab or scene sub-asset.
/// </summary>
public class GameFeatureAction_AddWidgets : MonoBehaviour
{
    [Header("Full HUD Layouts")]
    [SerializeField] private List<HUDLayoutRequest> _layouts = new();

    [Header("Extension Point Widgets")]
    [SerializeField] private List<HUDElementEntry> _widgets = new();

    private readonly List<UIScreen> _pushedScreens = new();
    private readonly List<UIExtensionHandle> _extensionHandles = new();

    private void OnEnable()
    {
        var layout = UIManager.Instance.CurrentPolicy.GetPrimaryRootLayout();

        // Push full layouts
        foreach (var request in _layouts)
        {
            var screen = layout.PushWidgetToLayerStack<UIScreen>(
                request.LayerTag, request.LayoutAsset);
            _pushedScreens.Add(screen);
        }

        // Register individual widgets via extension system
        foreach (var entry in _widgets)
        {
            var handle = UIExtensionSubsystem.Instance.RegisterExtensionAsWidget(
                entry.ExtensionTag, entry.WidgetAsset);
            _extensionHandles.Add(handle);
        }
    }

    private void OnDisable()
    {
        var layout = UIManager.Instance.CurrentPolicy.GetPrimaryRootLayout();

        foreach (var screen in _pushedScreens)
        {
            layout.FindAndRemoveWidgetFromLayer(screen);
        }
        _pushedScreens.Clear();

        foreach (var handle in _extensionHandles)
        {
            handle.Unregister();
        }
        _extensionHandles.Clear();
    }
}
```

**How this maps to Unity's addressables workflow**: the `VisualTreeAsset` references in `_layouts` and `_widgets` would typically reference Addressables. The full async push would handle the loading:

```csharp
// Feature activates → async-loads ShooterHUDLayout.uxml → pushes to "Game" layer
await layout.PushWidgetToLayerStackAsync<UIScreen>(
    "Game",
    "addressables://ShooterHUDLayout.uxml",
    suspendInput: false,
    (state, screen) => {
        if (state == EAsyncWidgetState.AfterPush)
        {
            // HUD is now visible — crosshair, health bar, ammo all injected
            // via extension points that ShooterCore also registered
        }
    }
);
```

---

## Complete Unity Implementation

Below is the full directory structure and a minimal implementation that ties everything together.

### Recommended Directory Structure

```
Assets/
└── Scripts/
    └── UI/
        ├── Core/
        │   ├── UIManager.cs              // Singleton, owns policy
        │   ├── UIPolicy.cs               // Creates root layouts per player
        │   ├── UIPrimaryLayout.cs        // Named layers + push/pop API
        │   ├── UILayer.cs                // Stack container for screens
        │   ├── UIScreen.cs               // Base activatable widget
        │   └── UIFlowController.cs       // Sequential screen flow (frontend)
        │
        ├── Extensions/
        │   ├── UIExtensionSubsystem.cs   // Pub/sub for decoupled injection
        │   └── UIExtensionPointWidget.cs // UXML element for extension slots
        │
        ├── Foundation/
        │   ├── MenuScreen.cs             // Base class for menu screens
        │   ├── DialogScreen.cs           // Base class for modal dialogs
        │   ├── HUDLayout.cs              // Base class for in-game HUD
        │   └── ConfirmationDialog.cs     // Confirm/Cancel dialog
        │
        ├── Frontend/
        │   ├── FrontendScreenFlow.cs     // User init → press start → main menu
        │   ├── PressStartScreen.cs       // "Press Start to Play"
        │   └── MainMenuScreen.cs         // Play, Settings, Quit
        │
        └── Features/
            └── GameFeatureAction_AddWidgets.cs  // Feature UI declarations
```

### UXML Structure for the Primary Layout

```xml
<!-- W_OverallUILayout.uxml -->
<ui:UXML xmlns:ui="UnityEngine.UIElements"
    xmlns:my="MyGame.UI.Extensions">

    <!-- Root overlay: all layers stacked on top of each other -->
    <ui:VisualElement name="LayerContainer" style="flex-grow: 1;">

        <!-- Bottom layer: Game/HUD UI -->
        <ui:VisualElement name="Layer_Game" style="flex-grow: 1; position: absolute; top: 0; left: 0; right: 0; bottom: 0;">
            <ui:VisualElement name="HUD_Container" style="flex-grow: 1;">
                <!-- Extension points for HUD elements -->
                <my:UIExtensionPoint extension-point-tag="UI.Extension.HUD"
                    match-type="Partial" style="flex-grow: 1;" />
            </ui:VisualElement>
            <ui:VisualElement name="ReticleContainer" style="position: absolute; top: 50%; left: 50%;">
                <my:UIExtensionPoint extension-point-tag="UI.Extension.HUD.Reticle"
                    match-type="Exact" />
            </ui:VisualElement>
        </ui:VisualElement>

        <!-- Middle layer: Game menu (escape menu, scoreboard, etc.) -->
        <ui:VisualElement name="Layer_GameMenu" style="flex-grow: 1; position: absolute; top: 0; left: 0; right: 0; bottom: 0;" />

        <!-- Top layer: Menus (main menu, settings, etc.) -->
        <ui:VisualElement name="Layer_Menu" style="flex-grow: 1; position: absolute; top: 0; left: 0; right: 0; bottom: 0;" />

        <!-- Topmost layer: Modals (confirmations, system messages) -->
        <ui:VisualElement name="Layer_Modal" style="flex-grow: 1; position: absolute; top: 0; left: 0; right: 0; bottom: 0;" />

    </ui:VisualElement>
</ui:UXML>
```

### Putting It Together: The Boot Sequence

```csharp
// Assets/Scripts/UI/Core/UIBootstrapper.cs
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Placed in the boot/initialization scene. Sets up the full UI system
/// before any gameplay begins.
/// </summary>
public class UIBootstrapper : MonoBehaviour
{
    [SerializeField] private GameObject uiDocumentPrefab;
    [SerializeField] private PanelSettings panelSettings;

    // Called via RuntimeInitializeOnLoadMethod or from the init scene
    private void Awake()
    {
        // 1. Create the UIManager (DontDestroyOnLoad singleton)
        var managerGo = new GameObject("UIManager");
        DontDestroyOnLoad(managerGo);
        var manager = managerGo.AddComponent<UIManager>();

        // 2. Create and assign the UIPolicy
        var policy = new GameObject("DefaultUIPolicy").AddComponent<UIPolicy>();
        var policyType = policy.GetType();
        // Use reflection or prefab assignment to inject the uiDocumentPrefab
        var field = policyType.GetField("uiDocumentPrefab",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);
        field?.SetValue(policy, uiDocumentPrefab);

        manager.defaultPolicy = policy;

        // 3. Simulate "player joined" — in a real game this would come from
        //    your player management / lobby system
        //    In single-player, call this once.
        manager.NotifyPlayerAdded(gameObject);

        // At this point: UIManager → UIPolicy → PrimaryLayout → Layers registered
        // Ready for the frontend flow to start pushing screens.
    }
}
```

### Typical Screen Navigation Patterns

**Pattern 1: Flow-driven frontend** (covered above in FrontendScreenFlow) — good for linear sequences with branching (press start → main menu → settings → back to main menu).

**Pattern 2: Event-driven in-game**:

```csharp
// Inside HUDLayout.OnScreenActivated():
InputSystem.actions["Escape"].performed += OnEscapePressed;

void OnEscapePressed(InputAction.CallbackContext ctx)
{
    // Push escape menu onto GameMenu layer
    _rootLayout.PushWidgetToLayerStack<PauseMenu>("GameMenu",
        pauseMenuTemplate,
        screen => {
            // When pause menu deactivates (resume pressed), game resumes automatically
            screen.OnDeactivated += () => Debug.Log("Game resumed");
        });
}
```

**Pattern 3: Cross-layer pushes**:

```csharp
// In a main menu screen, the "Settings" button:
public void OnSettingsClicked()
{
    var layout = UIPrimaryLayout.GetPrimaryGameLayout(this);

    // Push settings on top of the current menu layer
    layout.PushWidgetToLayerStack<SettingsScreen>("Menu",
        settingsTemplate,
        screen => {
            // Configure the settings screen's back button to dismiss itself
            screen.OnBackRequested += () => {
                layout.FindAndRemoveWidgetFromLayer(screen);
            };
        });
}
```

**Pattern 4: Modal confirmation dialogs**:

```csharp
// Show a confirmation dialog on the Modal layer (on top of everything)
public void ShowQuitConfirmation()
{
    var layout = UIPrimaryLayout.GetPrimaryGameLayout(this);

    layout.PushWidgetToLayerStack<ConfirmationDialog>("Modal",
        confirmationTemplate,
        dialog => {
            dialog.Title = "Quit Game?";
            dialog.Message = "Any unsaved progress will be lost.";

            dialog.OnConfirmed += () => {
                layout.FindAndRemoveWidgetFromLayer(dialog);
                Application.Quit();
            };

            dialog.OnCancelled += () => {
                layout.FindAndRemoveWidgetFromLayer(dialog);
            };
        });
}
```

---

## Architectural Decisions and Tradeoffs

### Why Lyra Chose This Architecture

| Decision | Reason |
|---|---|
| **Tag-driven instead of enum-driven** | Gameplay tags are data-driven, can be added by any plugin, and support hierarchies (`UI.Layer.Menu` → parent `UI.Layer`). No central enum to maintain. |
| **Per-player layouts instead of global** | Split-screen support. Even in single-player, it means the UI is always associated with a specific `LocalPlayer` — no ambiguity about "whose screen is this?" |
| **Push/pop instead of screen routing** | Simpler mental model. Each layer is a stack; pushing shows a screen, popping hides it. No global state machine to reason about. |
| **Input mode per widget instead of per state** | The screen declares its input needs. The framework enforces them on activation. No bugs where "the game thinks it's in a menu but the HUD still receives input." |
| **Async loading with input suspension** | Prevents players from pressing buttons while UI is loading (which could trigger actions before the UI is ready to handle them). |
| **Extension system instead of direct references** | Decoupled feature injection. The core game ships with only the layout structure — individual widgets come from game features. |
| **Control flow for frontend** | The frontend is a linear pipeline with branching. A flow is simpler to reason about than a state machine for this use case. |

### Things Lyra's System Does NOT Handle (Intentionally Left Out)

1. **No global screen transition animations** — animations are per-screen (entrance/exit animations in `OnActivated`/`OnDeactivated`), not centrally managed
2. **No centralized navigation history/back stack** — the layer stacks ARE the history. Popping a screen reveals the previous one naturally
3. **No "deep link" URL routing** — if you need `UI.Menu.Settings.Audio.Volume` style deep links, you'd build it on top; Lyra doesn't have this
4. **No automatic cleanup on level transition** — Lyra relies on the `WorldSubsystem` and `GameFeature` deactivation to clean up; you must ensure your Unity equivalent handles `OnDestroy` properly

### Adapting for Unity: What to Simplify

1. **Skip the policy abstraction if you only have 1 player** — a single `PrimaryLayout` on a DontDestroyOnLoad GameObject is sufficient
2. **Use string tags instead of gameplay tags** — Unity doesn't have `FGameplayTag`. Use `const string` fields or a `ScriptableObject`-based tag system if you want design-time safety
3. **Skip async loading if not using Addressables** — if all your UXML is in Resources or referenced directly, the sync push is enough
4. **Skip the extension system if you don't have dynamic HUD injection** — if you know all your HUD elements at compile time, just add them directly to the UXML
5. **Use Coroutines for flow control** instead of the custom `UIFlowController` — they're simpler if you don't need the explicit cancellation/deferred-continue pattern

### Porting Checklist

When building this system for Unity, implement in this order:

1. **`UIScreen` base class** — the activatable widget with `OnScreenActivated()`/`OnScreenDeactivated()` and `EScreenInputMode`
2. **`UILayer`** — stack container that manages push/pop with activation lifecycle
3. **`UIPrimaryLayout`** — owns layers, provides `PushWidgetToLayerStack<T>()` and `FindAndRemoveWidgetFromLayer()`
4. **`UIManager` singleton** — bootstrapper that creates the layout on startup
5. **First screen** — a simple menu pushed to the "Menu" layer to validate the pipeline
6. **`UIFlowController`** — only if you have a multi-step frontend flow (you probably do)
7. **`UIExtensionSubsystem` + `UIExtensionPointWidget`** — only when you have multiple game features contributing UI independently

### Key Differences from Unity UI Toolkit Defaults

| Concept | UMG/CommonUI | Unity UI Toolkit | Notes |
|---|---|---|---|
| Widget creation | `CreateWidget<T>(PC, Class)` | `VisualTreeAsset.CloneTree()` | In Unity, you typically clone from a template rather than instantiating a class |
| Widget lifetime | Owned by player controller | Owned by VisualElement hierarchy | Use `RegisterCallback<AttachToPanelEvent>` and `RegisterCallback<DetachFromPanelEvent>` for lifecycle |
| Focus management | Automatic via CommonUI | Manual via `element.Focus()` or `panel.focusController` | You'll need to manage focus yourself when screens push/pop |
| Input routing | `FUIInputConfig` per widget | `InputSystemUIInputModule` + action map enable/disable | Map `EScreenInputMode` to enabling/disabling action maps |
| Styling | Blueprint, C++, UMG Designer | USS (Unity Style Sheets) | USS is simpler — no need for the Style/ControlTheme distinction |
| Data binding | Properties + delegates | `Binding` API (`element.dataSource = viewModel`) | Unity's binding is closer to WPF-style, simpler than UMG's property system |

---

## Summary

The Lyra UI system is fundamentally a **layered push/pop stack with tag-driven extensibility**. The core loop is:

1. `UIManagerSubsystem` creates a `UIPolicy` on game start
2. When a player joins, the policy creates a `PrimaryGameLayout` with named layers (`Game`, `Menu`, `Modal`)
3. Screens are `UCommonActivatableWidget` subclasses pushed onto layers
4. Each screen declares its input needs via `ELyraWidgetInputMode`
5. Pushing a screen automatically deactivates the previous one and adjusts input
6. The `UIExtensionSystem` allows decoupled injection of HUD elements from game features
7. Frontend flow is a sequential `FControlFlow` pipeline driven by screen deactivation

The entire system achieves its goals without a single `OpenScreen("screenName")` call, a central route table, or a global screen manager state machine. Everything is emergent from the push/pop behavior of named layers.
