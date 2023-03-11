using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

namespace ActorSolidSystem
{
    //the actor-solid system is a way of handling 2D collisions that ive been iterating on and reusing for a long time.
    //it needs a list of every actor in the scene
    public static class Actortracker
    {
        public static List<Actor> actorList = new List<Actor>();
    }
    //an actors and solids both have collision boxes. these are different from unity's built-in collision options because they are strictly axis-aligned (cant rotate) and locked to integer positions
    //in exchange for those restrictions, we can more easily accomplish pixel-perfect collisions
    public class Actorsolid : MonoBehaviour
    {
        public CollisionBox box;
        //the system behaves better when each actor/solid's position is changed with this custom position vector rather than with the transform. this ensures that it can only be set to integers
        public int X { get { return box.lockedPosition.x; } set { box.lockedPosition.x = value; } }
        public int Y { get { return box.lockedPosition.y; } set { box.lockedPosition.y = value; } }
        //we need a quick and easy method to check if an actorsolid is a solid
        public virtual bool IsSolid() { return false; }
    }
    //we also track all the collision boxes
    static class BoxSystem
    {
        public static List<CollisionBox> boxList = new List<CollisionBox>();

        public static void CullNullBoxes()
        {
            boxList.RemoveAll((CollisionBox box) => {return box == null; });
        }
    }

}
