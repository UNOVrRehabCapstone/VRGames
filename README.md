This application is a collection of games designed to rehabilitate people who have suffered a stroke or similar brain injury.
There are four proof-of-concept games included.
1. A game where you pop balloons with darts.
2. A game where you throw paper airplanes.
3. A game where you climb a wall.
4. A game where you stack blocks.

To use this application, the files must be built in the Unity engine and deployed to a virtual reality headset such as the Meta Quest 2.


# Release Notes:

### 9/28/2023

General:
* Added a guest login button to allow play if the server is offline or not set up.

Balloon Game:
  * Added multiple difficulty levels that automatically trigger upon a designated score threshold. Balloons will move faster and become harder to predict.
  * Added a win condition that triggers a "You won!" message. The game will (for now) automatically restart after this happens.
  * Added basic effect that will be applied to new "Powerup" balloons that have not yet been implemented. Currently they are applied to normal balloons as a proof of concept.


### 10/19/2023
General:
* Connected VR-Games to our student server

Balloon Game:
* Refactored BalloonGameplayManager class
* Created new BalloonManager class to handle the balloons in the scene and balloon spawning
* Created a game settings class that can be used to create and specify different game settings such as:
  * Game mode
  * Spawn pattern
  * Probability of different types of balloons spawning
  * Where a balloon spawns
  * Time between each balloon spawn
* Implemented alternating and cocurrent spawn patterns
