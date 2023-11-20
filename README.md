This application is a collection of games designed to rehabilitate people who have suffered a stroke or similar brain injury.
There are four proof-of-concept games included.
1. A game where you pop balloons with darts.
2. A game where you throw paper airplanes.
3. A game where you climb a wall.
4. A game where you stack blocks.

To use this application, the files must be built in the Unity engine and deployed to a virtual reality headset such as the Meta Quest 2.

# Balloon Game TODO
- [ ] Add in other balloon environment alongside original environment.
- [ ] Testing
    - [ ] X
- [ ] Stop spawing life balloon when max lives have been reached.
- [ ] Remove spawn text for darts
- [ ] Refactor balloon spawn probability method
- [ ] Create any remaining necessary methods needed for clinician control over the game.
# Release Notes:

### 9/28/2023

General:
* Added a guest login button to allow play if the server is offline or not set up.

Balloon Game:
  * Added multiple difficulty levels that automatically trigger upon a designated score threshold. Balloons will move faster and become harder to predict.
  * Added a win condition that triggers a "You won!" message. The game will (for now) automatically restart after this happens.
  * Added basic effect that will be applied to new "Powerup" balloons that have not yet been implemented. Currently they are applied to normal balloons as a proof of concept.




### 11/02/2023

Balloon Game:
  * New Additions:
      * New Balloon - The "Onion" balloon has multiple layers that need to be popped
      * New conffetti effect on victory

 
  * Improvements:
      * Recreated balloon stream powerup to align with refactored code
      * Created a stream of balloons prefab to be spawned by the powerup
      * Simplified balloon collider
      * Refactoring
      * Added a method to delay the destruction of balloons by a given number of seconds 
      * Created a dart manager class to handle dart spawning and got rid of the manual dart spawners
      * Tracking for separate left and right points
      * Fixed scoreboard not correctly displaying the current left and right scores
      * Disabled balloon spawner once goal has been reached
      * Fixed skipping over point goal (e.g. going from 3 to 5 points with a goal of 4) not triggering win state
   



### 11/16/23

Balloon Game:
* Implemented at the restore life balloon and the target balloon
* Simplified the spawning by changing the duration between balloon spawns to a static timer (no more random time spawns).
* Implemented another spawn pattern called "Random." This spawn pattern will be similar to picking a random balloon, that is, the balloon spawn is chosen based on a probability.
* Implemented the ability for the goal to be left hand points, right hand points, or total points.
* Added a speed modifier, and changed the update methods of the balloons to make use of said modifier (the modifier will be used by the clinician to control the speed of the balloons).
* Implemented the feature to adjust the kill zone with respect to the height of the player.
* Fixed dart getting destroyed when colliding with the kill zone.
*  Fixed the ungrabbable dart bug.

