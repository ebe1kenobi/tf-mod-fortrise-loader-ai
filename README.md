# LoaderAI

An AI loader: it does not play by itself, it lets **AI mods** take control of an
archer. With no AI mod installed it does nothing at all.

WiderSetMod supported.

A mod for **FortRise 5** (>= 5.3.3). The FortRise 4 version (`tf-mod-fortrise-loader-ai`) is no longer maintained: fixes and new features only land in this repository.

## Installation

1. Install FortRise 5 and start the game through `FortRise.exe`.
2. Install the mod this one depends on first: **Archer**.
3. Copy `release/tf-mod-fortrise-loader-ai` into `<TowerFall>/FortRise/Mods/`.

It used to depend on **CustomName** for the player names shown on the archer select
screen. CustomName no longer exports the interop delegates it relied on, and per-player
naming has moved to the **Archer** mod - which is what `meta.json` requires now.

Settings are under **Options > Mods > LoaderAI**.
Data and log files live in `<TowerFall>/FortRise/Saves/LoaderAI/` and `<TowerFall>/FortRise/Logs/`.

## Usage

### AI keyboard layout

An AI is picked on the archer select screen: up and down arrows appear around the
player name as soon as at least one agent is available.

| Action | P1 | P2 | P3 | P4 | P5 | P6 | P7 | P8 |
|--------|----|----|----|----|----|----|----|----|
| Down | A | Z | D | F | G | H | J | K |
| Up | Q | S | E | R | T | Y | U | I |
| Left | O | P | W | C | B | F9 | F11 | Page Up |
| Right | L | M | X | V | N | F10 | F12 | Page Down |
| Jump / **pick the AI** | NumPad1 | NumPad2 | NumPad3 | NumPad4 | NumPad5 | NumPad6 | NumPad7 | NumPad8 |
| Shoot / **drop the AI** | F1 | F2 | F3 | F4 | F5 | F6 | F7 | F8 |
| Dodge | F13 | F14 | F15 | F16 | F17 | F18 | F19 | F20 |

In short: **NumPad 1-8** assigns an agent to the matching player, **F1-F8** removes
it.

## Build / deployment

| Script | Purpose |
|--------|---------|
| `script/release.bat` | build, then assemble into `release/` |
| `script/deploy.bat` | copy `release/` into the TowerFall `Mods` folder |
| `script/release_deploy.bat` | both, one after the other |

Paths (game folder, module name) are set in `script/config.bat`.
