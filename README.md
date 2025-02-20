This is a Loader for fortrise Ai Mod.

This Mod does nothing if no Mod AI is in the Mods Directory.

A skeleton project for an AI mod exists here : https://github.com/ebe1kenobi/tf-mod-fortrise-ai-example , can be used but the player will only jump, you need to implement the AI.

A Simple AI using tf-mod-fortrise-ai-example : https://github.com/ebe1kenobi/tf-mod-fortrise-ai-simple

A Python AI using tf-mod-fortrise-ai-example : https://github.com/ebe1kenobi/tf-mod-fortrise-ai-python

You need to use the LoaderAIImport.cs File to give the Loader your AI agent using 
```
LoaderAIImport.addAgent(AINAME, agents);
```

If there is some Agent, the Archer select screen will display up and down arrow around the player name, and you will use the key for each player to choose the agent to use, below 
P1 is human, P2 to P3 are 3 different AI agent

![image](https://github.com/user-attachments/assets/1f0dd3af-0cf9-43a3-89c5-e4bc4371f74b)


**Keyboard config for each AI :**

Select an AI with the nu pad 1 to 4 ans deselect with F1 to F4

<a name="aikeyboardconfiguration">
  
|Action | P1 | P2 | P3 | P4 | P5 | P6 | P7 | P8 |
| ----- | -- | -- | -- | -- | -- | -- | -- | -- |
|Down   | A  | Z  | D  | F  | G  | H  | J  | K  |
|Up     | Q  | S  | E  | R  | T  | Y  | U  | I  |
|Left   | O  | P  | W  | C  | B  | F9  | F11  | PageUp  |
|Right  | L  | M  | X  | V  | N  | F10  | F12  | PageDOwn  |
|Jump / select AI  | NumPad1 | NumPad2 | NumPad3 | NumPad4 | NumPad5 | NumPad6 | NumPad7 | NumPad8 |
|Shoot / deselect AI | F1 | F2 | F3 | F4 | F4 | F5 | F6 | F7 | F8 |
|Dodge  | F13  | F14  | F15  | F16  | F17  | F18  | F19  | F20  |

![image](https://github.com/user-attachments/assets/a1c194c1-9603-4547-98f4-e5057d2b8a7b)

![image](https://github.com/user-attachments/assets/65b62c39-e25d-4dca-9cc6-230b5013af54)

