# AI Usage

## Was This Vibe Coded?

No. Since there are essentially zero resources and concrete examples of UI Toolkit at scale out there, if you try to vibe code it AI will hallucinate almost everything and have you running around in over-engineered circles. I came up with this myself based on experience and over half a year of trial and error.

## Agentic AI Viability

AI Agents can work in the system but with a caveat. After the initial structure is laid out and you have 4-5 fully fleshed out working modals and screens, a low-tier agent can comfortably take care of all the boilerplate associated with adding a new screen. I use Deepseek V4 Flash due to its fantastic cost-quality ratio and for only 2-3 cents it was able to create new views with almost zero issues.

## AI Assistance

For the architecture and initial work, I couldn't use AI for anything more than the usual basic questions or isolated bugs in some online chat. However, once I had completed the system, templates, and initial views and it came down to just grunt work, creating more screens was able to be done using a cheap AI agent like I described above.

Its UXML and USS needs heavy tweaking but it can populate a document with the named elements once you declare the record. It can add them to the constants class, create the factory method, and populate the View C# file with all the necessary bind and unbind functions. It handles all the boilerplate work very well.

Outside of that I used agents pretty heavily to create the mock data layer. It created the JSON files and basic save system. I didn't want to design a full backend for a FPS game, but I wanted to demonstrate how data can be wired up in the system.

I also use AI to help me out with these markdown docs to flesh things out like the project structure and diagrams.
