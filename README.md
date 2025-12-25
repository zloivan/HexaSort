# HexaSort

A Unity puzzle game with automatic merging mechanics of colored tiles on a hexagonal grid.

![Preview](docs/gameplay.gif) 

## Development Time

**4 days**

## Design Patterns

Singleton.
Used for BoardGrid and MergeController to have a single access point to the two most central systems. Yes, globals are evil in theory, but in a small game it simplifies life, it's all good.

Strategy.
IMergeRule and concrete implementations (ConsolidationRule, InboundRule, OutboundRule) — this is the way to plug in different merge algorithms without rewriting the main code. Need a new rule? Write a class, plug it in — done.

Factory.
ColoredTileFactory is responsible for creating tiles and immediately ensures that colors are valid. Convenient to have all creation logic in one place instead of spread across the scene.

Observer.
Events OnNewStackPlaced and OnStackRemoved allow BoardGrid to simply tell the world "hey, something appeared/disappeared here", without knowing who's listening. HandProvider, UI, effects — doesn't matter. Clean and decoupled.

DirtyFlag.
Responsible for updating the visual component of stacks

## Code Style Quirks

**Event handler methods:**
- Format `ClassName_EventName`: `BoardGrid_OnNewStackPlaced`

## Classes and Their Responsibilities

### Grid System

- **`GridSystemHex<T>`** - Hexagonal grid. Coordinates → world positions, neighbor search, offset for even/odd rows
- **`GridPosition`** - Coordinate structure (X, Z)
- **`GridObject`** - Container for TileStack on the grid
- **`GridDebugObject`** - Visual grid debug

### Board System

- **`BoardGrid`** - Main board controller. Stack placement/removal, position validation, events
- **`BoardGridVisual`** - Grid visualization, cell highlighting on hover
- **`BoardGridVisualSingle`** - Single visualization cell

### Merge System

- **`MergeController`** - Merge orchestrator. Launching cascades, sequence control, destruction check (10+ tiles)
- **`MergeAnalyzer`** - Analysis and operation search. Applies all rules, sorts by score, finds affected positions
- **`MergeExecutor`** - Operation execution. Tile movement, empty stack removal, destruction
- **`MergeOperation`** - Operation structure (From, To, Count, Score)

#### Rules

- **`IMergeRule`** - Merge rule interface
- **`ConsolidationRule`** - Collects from 2+ neighbors of the same color (priority 500+)
- **`InboundRule`** - Pulls tiles TO the placed stack (priority 50-200)
- **`OutboundRule`** - Sends tiles FROM the placed stack (priority 10-100)

### Tiles System

- **`TileStack`** - Stack model. LIFO operations, color block counting, monochrome check
- **`ColoredTile`** - Tile model with color
- **`ColoredTileFactory`** - Tile creation (random or by color)
- **`DraggableStack`** - Drag & Drop controller. Input, validation, merge trigger
- **`HandProvider`** - Generation of 3 random stacks. Updates after all are placed
- **`TileVisual`** - Visual representation of a tile
- **`TileStackVisual`** - Base stack visualization
- **`TileStackAnimatedVisual`** - Animated visualization
- **`TilesVisualSpawner`** - Creation of visual elements for tiles

### Utilities

- **`PointerToWorld`** - Screen → world coordinate conversion
- **`GameSignals`** - Event/signal system - not implemented

## Possible Improvements

- Object Pooling for stacks and visuals
- Could use DI instead of Singleton (but I think it's overkill, though replacement would be simple since singletons aren't used directly anywhere, we get references to the needed class in Start, easy to replace with Initialize)
- ScriptableObjects for configuration (scores, thresholds) or JSON
- Combo system with bonuses
- Add field rotation using rotating camera (Cinemachine)
- Add proper bootstrap main menu and level selection (or a sequential level delivery system)


## Gameplay

1. Get 3 random stacks
2. Place them on the hexagonal grid
3. Automatic cascading merges
4. 10+ tiles of the same color → destruction
5. After 3 placements → new stacks

---

**Unity:** 2022.3.62f

