

# Flat Arrows & Bow Speed Tweaks

A configurable **BepInEx** mod for **Tainted Grail: The Fall of Avalon (Mono build)** that improves bow gameplay by reducing arrow drop and giving full control over bow pull and release speeds.

This mod is designed to be lightweight, Mono-safe, and fully configurable through a standard BepInEx config file.

---

## Features

### 🏹 Flat / Low-Drop Arrows
- Arrows fly straighter with minimal or no gravity fall-off
- Optional vertical compensation to fine-tune trajectory
- Configurable arrow speed multiplier

### ⚡ Bow Pull Speed Control
- Adjust how fast the bow is drawn
- Useful for faster-paced or more responsive archery

### 🎯 Bow Release Speed Control
- Modify how quickly the release animation plays
- Makes bow shots feel snappier or heavier depending on preference

### ⚙️ Fully Configurable
- All values adjustable via config
- Safe min/max limits to prevent extreme values
- Can be enabled or disabled without removing the mod

---

## Configuration

### Config file location:

> BepInEx/config/RedJohn260.FlatArrows.cfg

### Example configuration:

    [General]
    EnableFlatArrows = true
    ArrowSpeedMultiplier = 2.0
    VerticalCompensation = 0.02
    BowPullSpeedMultiplier = 1.0
    BowReleaseSpeedMultiplier = 1.0

### Config Options Explained
| Option                      | Description                                       |
| --------------------------- | ------------------------------------------------- |
| `EnableFlatArrows`          | Enables or disables the mod                       |
| `ArrowSpeedMultiplier`      | Multiplies arrow velocity (higher = longer range) |
| `VerticalCompensation`      | Small upward force to counter gravity and reduce the fall-off             |
| `BowPullSpeedMultiplier`    | Multiplier for bow draw speed                     |
| `BowReleaseSpeedMultiplier` | Multiplier for bow release speed                  |

## Requirements

> Tainted Grail: The Fall of Avalon (Mono build) 
> Ensure you have [BepInEx](https://www.nexusmods.com/taintedgrailthefallofavalon/mods/50) installed for Tainted Grail.


## Installation
 - Install BepInEx 5.4.23 
 - Copy the mod .dll into: `BepInEx/plugins/`
 - Launch the game once to generate the config file
 - Edit the config to your liking
 - Restart the game

---

*Created by RedJohn260*
