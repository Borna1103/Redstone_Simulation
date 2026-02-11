# 🟥 Redstone Simulation Engine

A tick-based Minecraft-inspired redstone logic simulator built in C#.

This project recreates the internal mechanics of redstone circuits using a deterministic grid simulation, directional signal propagation, and scheduled state updates.

It models how signals travel, decay, invert, and delay across components such as repeaters, torches, comparators, and power sources.

---

## 📌 Overview

Minecraft’s redstone system behaves like a digital logic engine built on discrete time steps (ticks).

This simulator replicates that behavior using:

- A 2D grid-based world
- Strength-based signal propagation (0–15)
- Directional components
- Delayed scheduling for repeaters
- Breadth-first signal spreading
- Deterministic tick advancement

The goal was to accurately simulate real redstone logic while maintaining clean object-oriented design.

---

## ⚙️ Core Features

- 🧱 Grid-based object placement and updates
- ⚡ Signal strength propagation with decay
- 🔁 Repeaters with configurable delay (1–4 ticks) [WIP]
- 🔥 Redstone torches (inversion logic)
- 🟩 Levers and redstone blocks (power sources)
- ⚖️ Comparators with rear/side input logic
- 💡 Lamps and blocks reacting to power
- 🔄 Directional orientation (North, East, South, West)
- 🧠 Tick-based scheduling system

---

## 🧠 Simulation Model

### 1️⃣ Discrete Tick Engine

Each call to `AdvanceTick()` performs:

1. Reset non-power-source strengths
2. Feed repeater inputs
3. Execute scheduled repeater updates
4. Propagate repeater outputs
5. Propagate power sources
6. Compute comparator outputs
7. Update torch inversion logic

This ensures predictable, conflict-free signal updates.

---

### 2️⃣ Signal Strength System

- Signal strength ranges from **0–15**
- Strength decreases by 1 per redstone step
- Repeaters output full strength (15)
- Signals only update a target if incoming strength is greater than its current strength

Propagation is implemented using a queue-based BFS to simulate wave-like signal spreading.

---

### 3️⃣ Repeater Scheduling

Repeaters:

- Accept power only from their **input side**
- Output only from their **output side**
- Delay output by 1–4 ticks
- Emit strength 15 when powered

Example logic:

```csharp
public void ReceiveInput(bool powered, int currentTick)
{
    if (powered == poweredInput) return;

    poweredInput = powered;
    scheduledTick = currentTick + DelayTicks;
}
