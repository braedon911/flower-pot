using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(CollisionBox)), Serializable]
public class Solid : Actorsolid
{

    public bool collidable = true;
    public override bool IsSolid() { return true; }

    void Start()
    {
        box = GetComponent<CollisionBox>();
    }

    float remainder_x = 0f;
    float remainder_y = 0f;
    //solids are allowed to move, but they do it very differently from actors
    //solids NEVER COLLIDE WITH EACH OTHER. they drag actors along with them.
    public void Move(float x_distance, float y_distance)
    {
        remainder_x += x_distance;
        remainder_y += y_distance;

        int move_x = Mathf.RoundToInt(remainder_x);
        int move_y = Mathf.RoundToInt(remainder_y);

        if (move_x != 0 || move_y != 0)
        {
            List<Actor> riderList = new List<Actor>();
            foreach(Actor actor in Actortracker.actorList)
            {
                //IsRiding is up to the actor. for instance, the player might be able to grip the side of a moving solid, but enemies probably wouldnt do this.
                if (actor.IsRiding(this)) riderList.Add(actor);
            }

            collidable = false;

            if (move_x != 0)
            {
                remainder_x -= move_x;
                X += move_x;

                if (move_x > 0)
                {
                    foreach(Actor actor in Actortracker.actorList)
                    {
                        //pushing an actor takes priority
                        if(actor.box.InstancePlace(actor.X, actor.Y) == this)
                        {
                            actor.MoveX(box.TopRight.x - box.BottomLeft.x, actor.squishAction);
                        }
                        //otherwise the actor rides
                        else if (riderList.Contains(actor))
                        {
                            actor.MoveX(move_x, actor.defaultAction);
                        }
                    }
                }
                else
                {
                    foreach (Actor actor in Actortracker.actorList)
                    {
                        if (actor.box.InstancePlace(actor.X, actor.Y) == this)
                        {
                            actor.MoveX(box.BottomLeft.x - box.TopRight.x, actor.squishAction);
                        }
                        else if (riderList.Contains(actor))
                        {
                            actor.MoveX(move_x, actor.defaultAction);
                        }
                    }
                }
            }
            if (move_y != 0)
            {
                remainder_y -= move_y;
                Y += move_y;

                if(move_y > 0)
                {
                    foreach (Actor actor in Actortracker.actorList)
                    {
                        if (actor.box.InstancePlace(actor.X, actor.Y) == this )
                        {
                            actor.MoveY(box.TopRight.y - box.BottomLeft.y, actor.squishAction);
                        }
                        else if (riderList.Contains(actor))
                        {
                            actor.MoveY(move_y, actor.defaultAction);
                        }
                    }
                }
                else
                {
                    foreach (Actor actor in Actortracker.actorList)
                    {
                        if (actor.box.InstancePlace(actor.X, actor.Y) == this)
                        {
                            actor.MoveY(box.BottomLeft.y - box.TopRight.y, actor.squishAction);
                        }
                        else if (riderList.Contains(actor))
                        {
                            actor.MoveY(move_y, actor.defaultAction);
                        }
                    }
                }
            }
            collidable = true;
        }
    }
}
