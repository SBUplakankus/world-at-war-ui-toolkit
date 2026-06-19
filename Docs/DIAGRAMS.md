# Architecture Diagrams

Visual reference for the CoD: WaW UI Toolkit navigation system. These diagrams cover class relationships, runtime flow, layer states, screen categories, modal chains, data flow, and the debug tool interaction.

---

## Class Hierarchy

### Core Navigation

```mermaid
classDiagram
    class UIRouter {
        -UIRouter _instance
        -MenuLayout _layout
        +Register(layout)
        +NavigateTo~TView~()
        +OpenModal~TModal~()
        +Back()
        +CloseModal()
        +ClearModals()
    }

    class MenuLayout {
        -UILayer _screenLayer
        -UILayer _modalLayer
        -ScreenLayoutElements _chrome
        -HeaderController _header
        -ModalController _modalCtrl
        +ShowScreen(view)
        +ShowModal(view)
        +BackScreen()
        +CloseModal()
    }

    class UILayer {
        -VisualElement _container
        -Stack~BaseView~ _history
        -BaseView _current
        +Push(view)
        +Pop()
        +Clear()
    }

    class HeaderController {
        -Label _title
        -Label _username
        -Button _backButton
        +SetTitle(text)
        +SetUsername(text)
        +ShowBackButton()
        +HideBackButton()
        +BindBackButton(action)
        +UnbindBackButton()
    }

    class ModalController {
        -VisualElement _container
        -Label _title
        +Show()
        +Hide()
        +SetTitle(view)
    }

    class UIAudioHandler {
        -AudioSource _source
        +OnPointerEnter(evt)
        +OnClick(evt)
    }

    UIRouter --> MenuLayout : delegates to
    MenuLayout --> UILayer : owns 2x
    MenuLayout --> HeaderController : owns
    MenuLayout --> ModalController : owns
    MenuLayout *--> UIAudioHandler : owns
```

### View Hierarchy

```mermaid
classDiagram
    class BaseView {
        <<abstract>>
        +VisualElement Root
        +Activate()
        +Deactivate()
        +Dispose()
        #GetElements()*
        #Bind()*
        #UnBind()*
    }

    class IScreen {
        <<interface>>
        +string HeaderName
    }

    BaseView ..|> IScreen : optional

    MainMenuView --|> BaseView
    SoloView --|> BaseView
    CoOpView --|> BaseView
    ZombiesView --|> BaseView
    MultiplayerMenuView --|> BaseView
    MissionSelectView --|> BaseView
    GameOptionsView --|> BaseView
    GameVolumeView --|> BaseView
    CreditsView --|> BaseView
    SaveNoticeView --|> BaseView
    NewGameModalView --|> BaseView
    ResumeGameModalView --|> BaseView
    ContentWarningModalView --|> BaseView
    DifficultyModalView --|> BaseView
    NoConnectionModalView --|> BaseView
```

### Factory & Records

```mermaid
classDiagram
    class ViewFactory {
        <<static>>
        +Create~T~() T
    }

    class ElementsFactory {
        <<static>>
        +ScreenLayout(root) ScreenLayoutElements
        +MainMenu(root) MainMenuElements
        +MissionSelect(root) MissionSelectElements
        +... 15 methods total
    }

    class BaseView {
        <<abstract>>
        #GetElements()*
        #Bind()*
        #UnBind()*
    }

    ViewFactory --> BaseView : creates
    BaseView --> ElementsFactory : calls in GetElements()
```

### Data Layer

```mermaid
classDiagram
    class SaveDataManager {
        <<static>>
        +PlayerSaveData CurrentSave
        +Save(data)
        +Load()
        +Delete()
        +Flush()
    }

    class PlayerSaveData {
        +string username
        +bool campaignStarted
        +int missionsCompleted
        +PlayerSettingsData settings
    }

    class UIResources {
        <<static>>
        +DifficultyIcons
        +MissionThumbnails
        +AudioClips
        +MissionTitles
        +MissionDescriptions
        +MessagesOfTheDay
        -EnsureLoaded()
    }

    class DebugTool {
        +OnGUI()
        +TogglePanel()
    }

    SaveDataManager --> PlayerSaveData : reads/writes
    DebugTool --> SaveDataManager : edits
    DebugTool --> UIRouter : triggers modals
```

---

## Navigation Flow

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
    ActiveView->>UIRouter: NavigateTo[T]()
    UIRouter->>ViewFactory: Create[T]()
    ViewFactory->>Resources: Resources.Load uxml
    Resources-->>ViewFactory: VisualTreeAsset
    Note over ViewFactory: Activator.CreateInstance(template)<br/>CloneTree() into Root
    ViewFactory-->>UIRouter: returns view
    UIRouter->>UILayer: ShowScreen / Push(view)
    UILayer->>ActiveView: Deactivate() / UnBind()
    Note over UILayer: Push ActiveView to history<br/>Set NewView as current
    UILayer->>NewView: Activate()
    Note over NewView: GetElements() then Bind()
    Note over ActiveView,NewView: Chrome stays visible throughout
```

---

## Layer Stack States

### Screen Layer

```mermaid
stateDiagram-v2
    [*] --> Empty
    Empty --> HasCurrent: Push first view
    HasCurrent --> Stacked: Push another view
    Stacked --> HasCurrent: Pop
    Stacked --> Stacked: Push
    HasCurrent --> Empty: Clear
    Stacked --> Empty: Clear
    Empty --> [*]
```

### Modal Layer

```mermaid
stateDiagram-v2
    [*] --> Hidden
    Hidden --> Visible: Push modal
    Visible --> Visible: Push another modal
    Visible --> Hidden: Pop last modal
    Hidden --> [*]
```

---

## Screen Navigation Map

```mermaid
flowchart LR
    MainMenu --> Solo
    MainMenu --> CoOp
    MainMenu --> Zombies
    MainMenu --> Multi
    MainMenu --> Options
    MainMenu --> Credits

    Solo --> MissionSelect
    Options --> GameVolume

    CoOp --> NoConnection
    Zombies --> NoConnection
    Multi --> NoConnection
    Multi --> Options
```

### Modal Triggers

```mermaid
flowchart LR
    Solo --> NewGame
    Solo --> ResumeGame

    NewGame -->|Yes| ContentWarning
    NewGame -->|No| Solo
    ContentWarning --> Difficulty
    Difficulty --> Solo
    ResumeGame -->|No| Solo
```

---

## Modal Chain (New Game Flow)

```mermaid
sequenceDiagram
    participant SoloView
    participant UIRouter
    participant NewGameModal
    participant ContentWarningModal
    participant DifficultyModal

    SoloView->>UIRouter: OpenModal[NewGameModal]()
    UIRouter->>NewGameModal: Push

    NewGameModal->>UIRouter: OpenModal[ContentWarning]()
    UIRouter->>ContentWarningModal: Push
    Note over NewGameModal: Deactivated

    ContentWarningModal->>UIRouter: OpenModal[Difficulty]()
    UIRouter->>DifficultyModal: Push
    Note over ContentWarningModal: Deactivated

    DifficultyModal->>UIRouter: CloseModal()
    UIRouter->>SoloView: BackScreen()
```

---

## Elements Records Pattern

```mermaid
flowchart LR
    subgraph UXML["UXML Document"]
        A1["Button name='solo-btn'"]
        A2["Button name='coop-btn'"]
        A3["Label name='motd-content'"]
    end

    subgraph Constants["Constants/Elements.cs"]
        B1["const SoloBtn = 'solo-btn'"]
        B2["const CoopBtn = 'coop-btn'"]
        B3["const MotdContent = 'motd-content'"]
    end

    subgraph Factory["ElementsFactory"]
        C1["root.Q<Button>(SoloBtn)"]
        C2["root.Q<Button>(CoopBtn)"]
        C3["root.Q<Label>(MotdContent)"]
    end

    subgraph Record["sealed record MainMenuElements"]
        D1["Button SoloButton"]
        D2["Button CoOpButton"]
        D3["Label MessageLabel"]
    end

    subgraph View["View.Bind()"]
        E1["SoloButton.clicked += HandleSolo"]
        E2["MessageLabel.text = FetchMOTD()"]
    end

    UXML -->|name attrs match| Constants
    Constants -->|queried by| Factory
    Factory -->|returns| Record
    Record -->|consumed by| View
```

---

## Data Flow

```mermaid
flowchart LR
    subgraph Persistence["Save Data"]
        S1[playersavedata.json]
        S2[defaults.json]
    end

    subgraph Content["Static Content"]
        C1[content.json]
        R1[AudioClips]
        R2[DifficultyIcons]
        R3[MissionThumbnails]
    end

    subgraph Manager["Data Layer"]
        M1[SaveDataManager]
        M2[PlayerSaveData]
        M3[UIResources]
    end

    subgraph Views["UI Views"]
        V1[MissionSelectView]
        V2[GameOptionsView]
        V3[GameVolumeView]
        V4[DifficultyModalView]
    end

    S1 <-->|read/write| M1
    S2 -->|fallback| M1
    M1 -->|deserializes| M2
    C1 -->|lazy-loaded| M3
    R1 -->|lazy-loaded| M3
    R2 -->|lazy-loaded| M3
    R3 -->|lazy-loaded| M3

    M2 -->|missionsCompleted| V1
    M2 -->|settings| V2
    M2 -->|settings.volumes| V3
    M3 -->|descriptions, icons| V4
    M3 -->|titles, thumbnails| V1
    M3 -->|audio clips| UIAudioHandler
```

---

## View Lifecycle

```mermaid
stateDiagram-v2
    [*] --> Created: new View(template)
    Created --> GetElements: Activate()
    GetElements --> Bind
    Bind --> Active: ready for interaction
    Active --> UnBind: Deactivate()
    UnBind --> Dormant: in history stack
    Dormant --> GetElements: Activate() again on Pop
    Dormant --> Disposed: Clear() or Dispose()
    Active --> Disposed: Clear()
    Disposed --> [*]

    note right of Created
        CloneTree() into Root
        detached from hierarchy
    end note

    note right of GetElements
        one tree walk
        ElementsFactory called here
    end note

    note right of Bind
        subscribe clicked events
        set initial label text
        set button enabled states
    end note

    note right of UnBind
        unsubscribe clicked events
        unregister PointerEnterEvent
        prevent stale handlers
    end note
```

---

## Debug Tool

```mermaid
flowchart TD
    F3[F3 pressed] --> Panel[OnGUI panel visible]
    Panel --> Edit[Edit live save data]
    Panel --> Save[Save]
    Panel --> Delete[Delete]
    Panel --> NewPlayer[New Player]

    Edit --> Campaign[Toggle campaignStarted]
    Edit --> Missions[Slider missionsCompleted 0-15]

    Save --> Write[SaveDataManager.Save]
    Delete --> Remove[SaveDataManager.Delete]
    Remove --> Reload[Load defaults.json]

    NewPlayer --> Remove
    NewPlayer --> Modal[UIRouter.OpenModal SaveNoticeView]

    Missions --> MissionSelect[MissionSelectView unlock chain]
    Campaign --> SoloView[SoloView resume/new game state]
```

---

## Mission Select

```mermaid
flowchart LR
    subgraph Bind["On Bind()"]
        E1[SetMissionAvailability]
        E2[DisplayMission SemperFi]
    end

    subgraph Hover["On PointerEnterEvent"]
        H1[Set title]
        H2[Set description]
        H3[Set thumbnail]
    end

    subgraph Click["On clicked"]
        C1[LoadMission]
    end

    subgraph Unlock["SetMissionAvailability"]
        U1[Read missionsCompleted]
        U2[SemperFi always enabled]
        U3[Each subsequent mission enabled when n < completed]
    end

    Bind --> E1
    Bind --> E2
    E1 --> Unlock
```

---

## Assembly Map

```mermaid
flowchart LR
    subgraph Runtime["Runtime"]
        A1[UiToolkit.asmdef]
        A1 --- A2[UI/Core]
        A1 --- A3[UI/Views]
        A1 --- A4[UI/Records]
        A1 --- A5[UI/Factories]
        A1 --- A6[UI/Constants]
        A1 --- A7[UI/Enums]
        A1 --- A8[UI/Interfaces]
        A1 --- A9[Data]
        A1 --- A10[Utilities]
    end

    subgraph EditorOnly["Editor Only"]
        B1[Tests.asmdef]
        B1 --- B2[Tests/ViewFactory]
        B1 --- B3[Tests/Data]
        B1 --- B4[Tests/ElementsFactory]
        B1 -.->|references| A1
    end

    subgraph Assets["Assets"]
        C1[Styles/*.uss]
        C2[Resources/Views/*.uxml]
        C3[Resources/Data/*.json]
    end

    A2 -->|loads at runtime| C2
    A9 -->|reads| C3
```

---

## USS Theming Chain

```mermaid
flowchart LR
    subgraph Core["core.uss"]
        Vars["--color-primary\n--color-bg\n--font-main\n--radius-btn"]
        Fonts["@font-face declarations"]
    end

    subgraph Screens["screens.uss"]
        S1[".menu-btn { color: var(--color-primary) }"]
        S2[".header-bar { background: var(--color-bg) }"]
    end

    subgraph Modals["modals.uss"]
        M1[".modal-overlay { background: var(--color-bg) }"]
    end

    Vars --> Screens
    Vars --> Modals
    Fonts --> Screens
    Fonts --> Modals
```

---

> All diagrams use Mermaid syntax. Render in any Mermaid-compatible viewer — GitHub, GitLab, Notion, Obsidian, or the [Mermaid Live Editor](https://mermaid.live).
