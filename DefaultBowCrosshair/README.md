# DefaultBowCrosshair

An enhancement mod for **Tainted Grail: Fall Of Avalon** that replaces the specialized bow crosshair with a minimalist dot and adds a dynamic, color coded rangefinder.

---

## 🚀 Features

* **Minimalist Bow HUD**: Permanently disables intrusive bow-specific crosshair lines while forcing the Default Dot to stay active even when a bow is equipped.
* **Tactical Rangefinder**: Adds a numerical distance readout (in meters) directly above your crosshair when looking at NPCs or interactable objects.
* **Dynamic RGB Color Lerp**: The distance text smoothly transitions through a color gradient based on proximity:
    * **White**: Safe/Maximum distance (70m).
    * **Green/Yellow**: Approaching optimal range.
    * **Red**: Close proximity (7m to 26m).
* **Enhanced Detection Range**: Increases the game's default NPC and water detection distance from 50m to a configurable 70m, allowing you to scout targets from further away.
* **Target Identification**: Customizes the crosshair dot color based on the target type (e.g., Red for Hostiles, Green for Non-Hostiles).

---

## 🛠 Installation

1. **Requirement**: Ensure you have [BepInEx](https://www.nexusmods.com/taintedgrailthefallofavalon/mods/50) installed for Tainted Grail.
2. **Download**: Get the latest `DefaultBowCrosshair.zip`.
3. **Place**: Extract the `.dll` file into your game folder at:
   `Tainted Grail/BepInEx/plugins/`
4. **Launch**: Start the game. The mod will load automatically and generate a configuration file.

---

## ⚙️ Configuration

The mod generates a comprehensive configuration file (`RedJohn260.DefaultBowCrosshair.cfg`) in the `BepInEx/config` folder after the first launch. 

### Editable Settings:

| Category | Setting | Default | Description |
| :--- | :--- | :--- | :--- |
| **1. General** | Hide Bow Crosshair | `true` | Toggles the visibility of bow-specific lines. |
| **1. General** | Enable Distance Text | `true` | Shows/hides the numerical distance readout. |
| **1. General** | Enable Custom Colors | `true` | Toggles color-changing based on target type. |
| **2. Range** | Max Detection Distance | `70.0` | Maximum distance for NPC/Water detection. |
| **2. Range** | Minimum Text Distance | `7.0` | Distance at which the text HUD hides. |
| **3. UI** | Font Size | `12.0` | Size of the distance text. |
| **3. UI** | Vertical Offset | `65.0` | Height of the text above the crosshair center. |
| **4. Colors** | Hostile Target Color | `Red` | Crosshair color for enemies. |
| **4. Colors** | Non-Hostile Target Color | `Green` | Crosshair color for friendlies. |
| **4. Colors** | Default Color | `White` | Standard crosshair color. |

---

*Created by RedJohn260*
