using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CareerModeLevel
{
    public int score = 0;
    public string[] schedule;

    public CareerModeLevel(string[] schedule)
    {
        this.schedule = schedule;
    }
}