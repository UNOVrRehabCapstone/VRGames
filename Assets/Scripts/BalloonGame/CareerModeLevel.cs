using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The CareerModeLevel class stores the schedule of the spawn of balloons for a level as well as
 * score for a level.
 */
public class CareerModeLevel
{
    public int score = 0;      /**< The user score of a level. */
    public string[] schedule;  /**< The schedule of balloons.  */

    /**
     * The CareerModeLevel constructor create a level with the passed in schedule.
     *
     * @param schedule The schedule of balloons to spawn.
     */
    public CareerModeLevel(string[] schedule)
    {
        this.schedule = schedule;
    }
}