# IFN584 – Assignment 3: Board Game Framework

## Overview

This project is a console-based board game framework developed in C# (.NET 10).
It supports two game families, Gomoku and Reversi, with six variants using a
shared object-oriented architecture.
The framework uses inheritance, polymorphism and design patterns to reuse common
game behaviour while allowing each variant to implement its own rules.

# Games

| Family | Variant | Board | Key Rule |
|---|---|---|---|
| Gomoku | Standard | 10x10 | Five in a row wins |
| Gomoku | Plus | 10x10 | Ordinary / Heavy / Eraser stones |
| Gomoku | Fog | 10x10 | Standard rules with limited board visibility |
| Reversi | Standard | 8x8 | More disks wins |
| Reversi | Anti | 8x8 | Fewer disks wins |
| Reversi | Corner | 8x8 | Controlling 3 of 4 corners wins immediately |

# Features
## Game Modes

* Human vs Human
* Human vs Computer

## Undo and Redo

The Command pattern is used to support full-turn Undo and Redo.

* Undo restores the previous full turn
* Redo restores an undone turn
* A new move after Undo clears the Redo history
* Undo/Redo remains available after loading a saved game

## Save and Load

Games can be saved and loaded using JSON files.
Saved games preserve the game configuration, turn state and move history.

## Fog Visibility

GomokuFog uses a visibility strategy to display only cells visible to the
current player without modifying the true board state.

## Reversi

Reversi supports legal move validation, disk flipping, PASS behaviour and
variant-specific victory conditions.

## Console Interface

The application includes:
* Game and variant selection
* Game mode and AI selection
* Help and game-specific move instructions
* Save/Load information
* Turn and board display
* Input validation and feedback

## Automated CLI Mode
Games can also be executed using a sequence of commands from the command line.
The final board and game status are displayed after execution.

# In-game Commands

| Command | Meaning |
|---|---|
| `O5:3` | Place an Ordinary Gomoku stone |
| `H5:3` | Place a Heavy stone (GomokuPlus) |
| `E5:3` | Place an Eraser stone (GomokuPlus) |
| `P3:4` | Place a Reversi disk |
| `PASS` | Pass when no legal Reversi move exists |
| `undo` | Undo the previous full turn |
| `redo` | Redo the previous full turn |
| `save` | Save the current game |
| `load` | Load a saved game |
| `help` | Show commands and current game rules |
| `quit` | End the game |

# Architecture

| Pattern | Implementation |
|---|---|
| Template Method | `Game`, `GomokuGame`, `ReversiGame` |
| Factory | `GameFactory`, `GomokuFactory`, `ReversiFactory` |
| Strategy | AI, Fog visibility and Reversi victory strategies |
| Command | `MoveCommand` and `CommandHistory` |
| Observer | `IGameObserver` and `ConsoleView` |

## Main Classes

### Game
Controls the shared game lifecycle, player turns and command processing.

### Board
Stores the true board state and provides board operations.

### GomokuGame / ReversiGame
Implement family-specific rules and provide the base behaviour for their variants.

### CommandHistory
Manages full-turn Undo and Redo using reversible move commands.

### GameStore / SaveData
Manage JSON persistence and restoration of saved games.

### ConsoleView
Handles console presentation including boards, turns, messages and help.

### GameSelection
Handles game, variant, mode and AI selection and loading saved games.

# Running the Program
## Requirements

* .NET 10 SDK
* Terminal environment

## Interactive Mode

```bash
dotnet run --project BoardGameFramework
