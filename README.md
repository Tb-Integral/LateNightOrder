# Late Night Order

**One-week Unity test assignment.** I built a short first-person horror prototype over one week. You play a barista on the night shift. Preparing and serving coffee introduces the controls before a routine customer order gives way to a horror sequence with two possible endings.

![The coffee station and counter in Late Night Order](https://raw.githubusercontent.com/Tb-Integral/Tb-Integral/main/assets/late-night-order.jpg)

## Run in Unity

1. Clone the repository and open its root in **Unity 6000.2.9f1**.
2. Open [`Assets/Scenes/CoffeeHorror.unity`](Assets/Scenes/CoffeeHorror.unity), the scene listed in the build settings.
3. Enter Play Mode. The on-screen prompts are in Russian.

| Input | Action |
| --- | --- |
| WASD or arrow keys | Move |
| Mouse | Look around |
| Left mouse button | Interact, or hold to move a cup or lid and release to place it |
| Right mouse button while holding an object | Throw it |
| Space | Advance dialogue |

## How the coffee order advances the scene

The first order doubles as a tutorial. [`TutorialManager`](Assets/Scripts/TutorialManager.cs) accepts completion only for the current step, so the prompt advances when the corresponding interaction succeeds. [`CupPoint`](Assets/Scripts/CoffeMachine/CupPoint.cs) handles placing the cup and lid at the machine, while [`MakingCoffe`](Assets/Scripts/CoffeMachine/MakingCoffe.cs) marks the cup as filled.

At the counter, [`NPCcontroller`](Assets/Scripts/NPC/NPCcontroller.cs) waits for a finished drink. Serving it advances the tutorial and starts the next dialogue. The end of that conversation returns control to the player and sends the customer away or starts the next horror sequence, depending on the encounter.

## Assets

The Unity project uses third-party assets, including the café environment. See [`Assets/ExternalAssets`](Assets/ExternalAssets) for the imported packages.
