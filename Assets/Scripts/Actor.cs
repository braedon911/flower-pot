using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(CollisionBox))]
public class Actor : Actorsolid
{
    public delegate void CollisionAction();
    //the default action refers to the action invoked on any collision
    public CollisionAction defaultAction = () => { };
    //the squish action is invoked when an actor is caught between two solids
    public CollisionAction squishAction;

    public override bool IsSolid() { return false; }

    void Start()
    {
        squishAction = Squish;
        box = gameObject.GetComponent<CollisionBox>();
        Actortracker.actorList.Add(this);
    }

    //this runs a PlaceMeeting check at the current position with a qualifier that looks only at collidable solids
    public bool CollideCheck(int x_check, int y_check)
    {
        Func<CollisionBox, bool> qualifier = (box) => 
        {
            Solid solid = box.GetComponent<Solid>();
            bool isSolid = (solid != null) && (solid.collidable==true);
            return isSolid;
        };
        return box.PlaceMeeting(x_check, y_check, qualifier);
    }

    //simply checks if there is a collidable solid below us
    public bool IsStanding()
    {
        return CollideCheck(X, Y - 1);
    }

    void Squish()
    {
        //
    }
    //this is distinct from IsStanding because it will return true for *non* collidable solids as well
    public bool IsRiding(Solid solid)
    {
        bool hitSolid;
        hitSolid = box.InstancePlace(X, Y - 1)?.GetComponent<Solid>() == solid;
        return hitSolid;
    }
    //this lets us preserve inertia correctly while maintaing our integer lock
    float xRemainder = 0f;
    float yRemainder = 0f;
    public void MoveX(float distance, CollisionAction action)
    {
        xRemainder += distance;
        int move = Mathf.RoundToInt(xRemainder);

        if (move != 0)
        {
            xRemainder -= move;
            int direction = Math.Sign(move);

            while (move != 0)
            {

                if (!CollideCheck(X + direction, Y))
                {
                    X += direction;
                    move -= direction;
                }
                else
                {
                    action();
                    break;
                }
            }
        }
    }
    public void MoveY(float distance, CollisionAction action)
    {
        yRemainder += distance;
        int move = Mathf.RoundToInt(yRemainder);

        if (move != 0)
        {
            yRemainder -= move;
            int direction = Math.Sign(move);

            while (move != 0)
            {
                if (!CollideCheck(X, Y + direction))
                {
                    Y += direction;
                    move -= direction;
                }
                else
                {
                    action();
                    break;
                }
            }
        }
    }
}

