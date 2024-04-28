# Description
This application is a collection of games designed to help with the rehabilitation process for people who have suffered a stroke or similar brain injury by creating games that encourage repetitive movements but in an engaging way. This project is an attempt at gamifying the rehabilitation process. There are four proof-of-concept games included. 
1. A game where you pop balloons with darts.
2. A game where you throw paper airplanes.
3. A game where you climb a wall.
4. A game where you stack blocks.

To use this application, the files must be built in the Unity engine and deployed to a virtual reality headset such as the Meta Quest 2.

# Repository Notes
Use master now.

# Release Notes:

### 2/29/24 (Milestone 1)

Fixed TextMesh Pro Metadata
Connected client to new DB and local server
Enabled old games to run
Updated build settings to load game
Updated Runtime settings and Resources metadata to fix issues preventing old games loading

### 3/21/24 (Milestone 2)

Added randomized spawn locations for targets
Added gripless grabbing of planes
Added first iteration of 180 degree adjustable plane spawns from clinitian view
Updated master to now reflect all changes
Reworked some folders and script/manager locations to be more consistent with convention established by Baloons group

### 4/16/2024 (Milestone 3)

Added an expanded iterated of the 180 degree adjustable plane spawns
- Fully randomized spawn location for the planes
- Plane can spawn anywhere in a 180 degree arc
- Added game-side setup for clinitian controlled spawn distance from player
- Updated table to handle 180 degree spawning
-- Updated material types for table

Completely decoupled targets, spawn, destroy, and respawn scripts from the generic games manager
- Only score remains to be decoupled from the monolith game manager

Several bugs fixed
- Targets sometimes don't destroy on contact
- Plane sometimes doesn't respawn on target destroy
- Table materials inheretence was set incorrectly
- Fixed plane colliding with target somtimes spawning hundreds of planes
- Fixed a client crash on target spawn under specific conditions

Known issues
- Plane sometimes spawns twice


### 4/28/2024 (Milestone 4)

Made target manager aware of number of targets currently in the scene.
Made target manager spawn a new wave of targets if there are no targets in the scene
Added functionality to target manager to dynamically spawn targets while the game is running.
Added functionality to targets to inform target manager when they begin and cease existing.
Added functionality to target manager to allow for compatibility with clinician view, laying the foundation to allow clinician view to spawn targets on demand.

Added auto-aim
- Uses required aim time
- Target will have a circle appear around it and fill up
- Once indicator is full, plane is locked on to target and will track to hit target, regardless of release angle.
- Plane will rotate to point at target 
- Auto-aim tracks moving targets, which allows for future itteration on the game
- Is compatible with Distance from Head, auto-release, and button press for throw
- Requires gripless grabbing

Added Auto-Release timer
- Uses required aim time
- When aim time is met, plane will automatically release
- Default behavior is plane will fly straight forward
- If auto-aim is used, target will lock on to, auto-release, and track the target upon target lock aquired.
- Is compatible with auto-aim
- Requires gripless grabbing

Added Aiming Indicator
- Uses Required Aim Time
- A yellow ring will fill while aiming at a target if using auto-release or auto-aim
- Automatically applies when auto-release or auto-aim are toggled

Added Use Button Press for Throw
- Use a button press to release plane
- When toggled on, it ursurps OVR Grabbable control
- Button can be selected between any button on the controller
- Buttons are: A, B, Joystick press, Trigger, and Grip
- Is compatible with auto-aim
- Requires Gripless Grabbing

Added Distance From Head Throw
- Uses Throw Threshold (in meters)
- When on, patient will raise controller to head then move arm forward to release plane
- Primes throw by tilting controller so plane is facing striaght out
- Once primed, player moves arm forward to specificed throw threshold
- Throw is based on distance from controller to eyes
- Is compatible with auto-aim and auto-release
- Requires Gripless Grabbing

Known bugs:
- Targeting progress indicator disappears after pointing away from target when target lock is already aquired (behavior of target lock is still present)
- When angle is too steep, auto-aim will not track onto target
- Auto-aim tracks bottom of target until close, resulting in an upwards curve motion