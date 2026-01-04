# CombinedComponents

# CombinedComponents — Unity 2D Side-Scroller Starter (Player + Camera + Parallax + EXP + Stats UI)

## 1) Overview 
This Unity project is a 2D side-scroller foundation that includes:
- **Player movement + jumping + melee attack hitbox**
- **Camera follow** that tracks the player
- **Looping parallax background** using 3 repeating tiles
- **EXP orbs (collectables)** that home toward the player when close
- **Player stats system** with **leveling + EXP curve** and **UI bars** (Health/Magic/EXP) plus a **Level Up icon fade**

This is designed to be a functional prototype base for a platformer/action platformer with progression.

---

## 2) Script descriptions (what each script does)

### MainCharacterController.cs
Controls the player:
- Horizontal movement using acceleration (`Rigidbody2D.linearVelocity` + `Mathf.MoveTowards`)
- Ground check via `OverlapCircle` at a `groundCheck` transform
- Jump when grounded (`Input.GetButtonDown("Jump")`)
- Sprite flip when changing direction
- Attack on **E** or **Fire1**, enabling `attackHitbox` for `0.3s`
- On trigger contact with objects tagged `"Enemy"`, applies knockback velocity away from the enemy and starts an invincibility coroutine (Not implimented)

---

### FollowCam.cs
Tracks a target (to be placed infront and above of player):
- Each frame, sets camera x/y to match `target.position.x/y`

---

### Parallax.cs
Looping parallax background system:
- Uses `cameraTransform` to measure camera x movement and shifts the parallax parent by `camDelta * parallaxSpeed`
- Requires **3 child background tiles** under the object this script is attached to
- Repositions background tiles when the camera moves far enough so the background loops seamlessly
- `imageWidth` must match the world-space width of a single tile

---

### ExpCollectable.cs
EXP orb pickup behavior:
- Finds the player using `FindGameObjectWithTag("Player")`
- Waits ~1 second before it can home (`timer` → `moveToPlayer`)
- When the player is within `attractionRange`, the orb moves toward them using `Rigidbody2D.linearVelocity`
- On trigger with the player, destroys itself
- Includes `expValue` for per-orb EXP amount (intended to be added to `PlayerStats`)

---

### PlayerStats.cs
Player progression + UI:
- Tracks:
  - `level`, `currentExp`, `maxExp`, and `currentStatPoints`
  - health/magic values + core attributes (strength, speed, stamina, dexterity, agility)
- Levels up when `currentExp >= maxExp` with EXP carry-over
- Adds **+5 stat points** per level (Not fully implimented)
- Calculates EXP requirement with a curved formula:
  - base growth + additional 5-level tier bonuses
- Updates UI sliders and TMP text every frame
- Shows a Level Up icon and fades it out over ~3 seconds after leveling

---

## 3) Unity setup (Inspector + scene wiring)

### A) Tags and layers (do this first)
1. **Player tag**
   - Ensure your player GameObject is tagged: **Player**
   - (Required for `ExpCollectable` to find the player)
2. **Enemy tag**
   - Ensure enemy objects are tagged: **Enemy**
   - (Required for knockback trigger, not fully implimented)
3. **Ground layer**
   - Create a layer named `Ground`
   - Assign all platforms/ground colliders to this layer
   - In `MainCharacterController`, assign `groundLayer` to include `Ground`

---

### B) Player object setup (MainCharacterController + PlayerStats)
Create/Select `Player` and add:

**Required components**
- `Rigidbody2D` (Dynamic)
  - Recommended: Freeze Rotation Z
- `Collider2D` (BoxCollider2D or CapsuleCollider2D)
- `SpriteRenderer`

**Attach scripts**
- `MainCharacterController.cs`
- `PlayerStats.cs`

#### Configure `MainCharacterController` fields
- `groundCheck`:
  - Create child empty object `GroundCheck` at the player’s feet
  - Drag into `groundCheck`
- `groundCheckRadius`:
  - Start around `0.2` and adjust if needed
- `groundLayer`:
  - Select the `Ground` layer mask
- `attackHitbox`:
  - Create child object `AttackHitbox`
  - Add `Collider2D` (Is Trigger = true)
  - Disable the object in Inspector by default (unchecked)
  - Drag into `attackHitbox`
  - CameraTracker (Transform) (required)
  - Create an empty child object on Player called CameraTracker
  - Position it at ahead of the player and slightly above(adjust to your desired tracking          location).
- `spawnPoint`:
  - Optional (currently not used in the code)

#### Configure `PlayerStats` fields (UI + initial values)
Set initial values (example):
- `maxHealth = 100`, `currentHealth = 100`
- `maxMagic = 25`, `currentMagic = 25`
- `currentExp = 0`
- `level = 1` (serialized private)
- `currentStatPoints = 0`

UI references (drag from Canvas):
- `healthBar`, `magicBar`, `expBar` (Sliders)
- `healthSliderDisplay`, `magicSliderDisplay`, `expSliderDisplay` (TextMeshProUGUI)
- `levelText` (TextMeshProUGUI)
- `levelUpIcon` (UI Image)

---

### C) UI setup (Canvas)
In a Canvas:
1. Create 3 sliders:
   - Health Slider
   - Magic Slider
   - EXP Slider
2. Create TextMeshPro labels for each slider display:
   - health text: `"current / max"`
   - magic text: `"current / max"`
   - exp text: `"current / max"`
3. Create a TMP text for level:
   - e.g., `"Lvl : 1"`
4. Create an Image for “Level Up” icon and assign to `levelUpIcon`
   - The script will hide it at Start and fade it on level-up

---

### D) Camera setup (FollowCam)
1. Select `Main Camera`
2. Attach `FollowCam.cs`
3. Drag Player Transform into `target`
4. (Typical 2D) Ensure camera Z stays at something like `-10`
   - This script preserves Z by only changing x/y

---

### E) Parallax setup (Parallax)
1. Create empty parent object (e.g., `Parallax_Background`)
2. Attach `Parallax.cs`
3. Create **3 child objects** under it:
   - `BG_0`, `BG_1`, `BG_2`
4. Give each child a SpriteRenderer with your background sprite
5. Position them side-by-side on X so they tile seamlessly

Assign in Inspector:
- `cameraTransform` → Main Camera transform
- `parallaxSpeed` → e.g., `0.05`
- `imageWidth` → the world-space width of one tile (e.g., `20`)

---

### F) EXP orb setup (ExpCollectable)
Create an orb prefab:
1. Create a GameObject `ExpOrb`
2. Add:
   - `Rigidbody2D`
   - `Collider2D` set to **Is Trigger = true**
   - `ExpCollectable.cs`
3. Ensure orb is on a layer that collides with the player (default OK)

Recommended values:
- `speed = 5`
- `attractionRange = 5`
- `expValue = 1` (adjust per prefab)

#### Connect EXP gain to PlayerStats (recommended)
In `ExpCollectable.OnTriggerEnter2D`, add EXP to the player before destroying.
Replace the commented line with:

```csharp
other.GetComponent<PlayerStats>().currentExp += expValue;
Destroy(gameObject);
