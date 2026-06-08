<div align="center">

# Call of Duty: World at War in UI Toolkit

![Unity](https://shieldcn.dev/badge/Unity-6000.4.8f1-ececec.png?variant=outline&logo=unity)
![C#](https://shieldcn.dev/badge/C%23-9.0-239120.png?variant=outline&logo=csharp)
![MIT License](https://shieldcn.dev/badge/License-MIT-blue.png?variant=outline)

**Recreating Call of Duty: World at War's menu system using Unity UI Toolkit.**

</div>

A screen-stack navigation shell built on Unity UI Toolkit. Screens and
modals are UXML views loaded from Resources, wired to C# view classes
through typed element records. A `UIRouter` singleton pushes and pops
views on `UILayer` stacks (one for screens, one for modals), with a
persistent chrome layout (header, footer, modal overlay) defined in
`LayoutView.uxml`. Styled with USS variables and text-shadows to
match the CoD: WaW aesthetic.

This project is the culmination of my research over the last 7 months
of UI programming in Unity 6 through UI Toolkit and is still very
much a work in progress right now but it's my most recent code work.

Includes: MainMenu, Solo, CoOp, Zombies, Multiplayer, Options,
Credits screens + Notice, Difficulty, NewGame, Warning modals.
Save data serialized as JSON to persistent data path. HUD/pause
layout scaffolding in place for in-game work.