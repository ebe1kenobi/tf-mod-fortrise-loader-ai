# LoaderAI

<img width="943" height="595" alt="image" src="https://github.com/user-attachments/assets/0e553f12-b58f-4116-88d0-62d2ecd61f9c" />

<img width="779" height="195" alt="image" src="https://github.com/user-attachments/assets/0a24f4ca-0d29-4986-ba79-e1654e604e09" />

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
| Down | Q | S | D | F | G | H | J | K |
| Up | A | Z | E | R | T | Y | U | I |
| Left | O | P | W | C | B | F9 | F11 | Page Up |
| Right | L | M | X | V | N | F10 | F12 | Page Down |
| Jump / **pick the AI** | NumPad1 or 1 | NumPad2 or 2 | NumPad3 or 3 | NumPad4 or 4 | NumPad5 or 5 | NumPad6 or 6 | NumPad7 or 7 | NumPad8 or 8 |
| Shoot / **drop the AI** | F1 | F2 | F3 | F4 | F5 | F6 | F7 | F8 |
| Dodge / **alt costume** | F13 | F14 | F15 | F16 | F17 | F18 | F19 | F20 |

In short: **1-8** assigns an agent to the matching player - the top row or the number pad,
either works, which matters on a laptop with no pad - and **F1-F8** removes it.

> The first two columns used to be printed the wrong way round here: P1 is `Q` down and
> `A` up, P2 is `S` down and `Z` up. From P3 onwards the pairs read as they always did.

### The keys are written on the portrait

A slot held by an AI **shows the name of the key** for each gesture, around the portrait:
the two arrows above and below, the archer arrows left and right, and `ALT` underneath for
the alternative costume.

Without it the two triangles announced that you could move up and down to change AI, but
not *with what*. A slot held by an AI usually has no controller: its input is the fallback
keyboard in the table above, where P1's up and down are `A` and `Q` — not the arrow keys
the triangles let you assume. The alternative costume is worse still: the game defines
`MenuAlt` as `Config.Dodge`, so on an AI slot it is `F13`, a key nobody has and nobody
would go looking for.

The label is read off **the element's own input**, not deduced from the player number.
That instance is what the select screen actually questions, and depending on whether a
human controller was set aside it is either that controller's mapping or the fallback
keyboard. Building the name from the index would have shown the right key half the time,
with nothing to signal which half.

Names are drawn at **full size**. The game's font is pixels drawn one by one: shrinking it
does not make smaller letters, it deletes strokes — at 0.6 there were only white dots
left. Every label also goes through a character filter, whole and not just the key name:
`TFGame.Font` throws while measuring an unsupported character, which would bring the game
down mid-render.

## Build / deployment

| Script | Purpose |
|--------|---------|
| `script/release.bat` | build, then assemble into `release/` |
| `script/deploy.bat` | copy `release/` into the TowerFall `Mods` folder |
| `script/release_deploy.bat` | both, one after the other |

Paths (game folder, module name) are set in `script/config.bat`.
