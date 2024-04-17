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

Completely decoupled targets, spawn, destroy, and respawn scripts from the generic games manager
- Only score remains to be decoupled from the monolith game manager

Several bugs fixed
- Targets sometimes don't destroy on contact
- Plane sometimes doesn't respawn on target destroy
- Table materials inheretence was set incorrectly
- Fixed plane colliding with target somtimes spawning hundreds of planes
- Fixed a client crash on target spawn under specific conditions