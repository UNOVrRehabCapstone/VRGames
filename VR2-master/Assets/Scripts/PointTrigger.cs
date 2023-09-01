using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Use this class to check if the player has a certain amount of points. If they do, call a function within the GameplayManager.
public class PointTrigger
{
    string equality;
    int points;
    string function;
    public PointTrigger(string equality, int points, string function){
        this.equality = equality;
        this.points = points;
        this.function = function;
    }
    public bool testEquality(){
        bool passes = false;
        switch( equality ){
            case "<":
                passes = PointsManager.getPoints() < points;
                break;
            case "<=":
                passes = PointsManager.getPoints() <= points;
                break;
            case "==":
                passes = PointsManager.getPoints() == points;
                break;
            case "!=":
                passes = PointsManager.getPoints() != points;
                break;
            case ">=":
                passes = PointsManager.getPoints() >= points;
                break;
            case ">":
                passes = PointsManager.getPoints() > points;
                break;
        }
        return passes;
    }

    public void execute(){
        GameplayManager.getManager().SendMessage( function );
    }
}
