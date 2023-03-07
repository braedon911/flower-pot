using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//the collision box is at the heart of the actor-solid system. this is designed to approximate the GameMaker collision system, which has a lot of useful tools for 2D action games.
namespace ActorSolidSystem
{
    public class CollisionBox : MonoBehaviour
    {
        [SerializeField]
        private Vector2Int dimensions;
        public Vector2Int lockedPosition;
        public Vector3Int lockedScale;
        private Vector2Int topLeft, bottomRight;
        //these properties are useful shorthand
        public Vector2Int Dimensions { get { return dimensions; } set { } }
        public Vector2Int TopLeft { get { return topLeft + lockedPosition; } set { } }
        public Vector2Int TopRight { get { return dimensions + lockedPosition; } set { } }
        public Vector2Int BottomLeft { get { return lockedPosition; } set { } }
        public Vector2Int BottomRight { get { return bottomRight + lockedPosition; } set { } }


        public SpriteRenderer spriteRenderer;
        //this checks if a pixel overlaps with the collision box
        public bool PointInBox(int x_check, int y_check)
        {
            return ((x_check <= TopRight.x
                    && x_check >= BottomLeft.x)
                    && (y_check <= TopRight.y
                    && y_check >= BottomLeft.y));
        }
        //this is identical to the above method except that it takes a single vector2int as an argument
        public bool PointInBox(Vector2Int checkPosition)
        {
            int x_check = checkPosition.x;
            int y_check = checkPosition.y;
            return PointInBox(x_check, y_check);
        }
        //the addition of the qualifier argument makes for a very versatile conditional collision system
        public bool PlaceMeeting(int x_check, int y_check, Func<CollisionBox, bool> qualifier)
        {
            Vector2Int checkPosition = new Vector2Int(x_check, y_check);
            bool check = false;
            foreach (CollisionBox box in BoxSystem.boxList)
            {
                if (box == null)
                {
                    BoxSystem.boxList.Remove(box);
                    break;
                }
                if (box != this)
                {
                    check = PlaceMeetingSubcheck(box, checkPosition);
                    if (check && qualifier(box)) break;
                }
            }
            return check;
        }
        public bool PlaceMeeting(int x_check, int y_check)
        {
            Vector2Int checkPosition = new Vector2Int(x_check, y_check);
            bool check = false;
            foreach (CollisionBox box in BoxSystem.boxList)
            {
                if (box == null)
                {
                    BoxSystem.boxList.Remove(box);
                    break;
                }
                if (box != this)
                {
                    check = PlaceMeetingSubcheck(box, checkPosition);
                    if (check) break;
                }
            }
            return check;
        }
        //this is like PlaceMeeting except that it returns the first collision box found
        public CollisionBox InstancePlace(int x_check, int y_check)
        {
            Vector2Int checkPosition = new Vector2Int(x_check, y_check);
            bool check = false;
            foreach (CollisionBox box in BoxSystem.boxList)
            {
                if (box == null)
                {
                    BoxSystem.boxList.Remove(box);
                    break;
                }
                if (box != this)
                {
                    check = PlaceMeetingSubcheck(box, checkPosition);
                    if (check) return box;
                }
            }
            return null;
        }
        private bool PlaceMeetingSubcheck(CollisionBox box, Vector2Int checkPosition)
        {
            Vector2Int difference = checkPosition - lockedPosition;
            return box.PointInBox(checkPosition) ||
                        box.PointInBox(dimensions + checkPosition) ||
                        box.PointInBox(new Vector2Int(dimensions.x, 0) + checkPosition) ||
                        box.PointInBox(new Vector2Int(0, dimensions.y) + checkPosition) ||

                        //makes it impossible to slip through!
                        PointInBox(box.BottomLeft - (difference)) ||
                        PointInBox(box.TopRight - (difference)) ||
                        PointInBox(box.BottomRight - (difference)) ||
                        PointInBox(box.TopLeft - (difference))
                        ;
        }
        private void FixedUpdate()
        {
            transform.position = new Vector3Int(lockedPosition.x, lockedPosition.y);
            transform.localScale = lockedScale;
            //Debug.DrawLine(new Vector3(lockedPosition.x, lockedPosition.y), new Vector3(TopRight.x, TopRight.y), Color.red);
        }
        void Start()
        {
            BoxSystem.boxList.Add(this);
            //we have to ensure that the collision box is locked to our integers-only system
            lockedPosition.x = Mathf.RoundToInt(transform.position.x);
            lockedPosition.y = Mathf.RoundToInt(transform.position.y);
            lockedScale = new Vector3Int(Mathf.RoundToInt(transform.localScale.x), Mathf.RoundToInt(transform.localScale.y));
            transform.position = new Vector3(lockedPosition.x, lockedPosition.y);
            //we also really need to collect the spriterenderer. things can get weird if we dont.
            if (TryGetComponent(out SpriteRenderer renderer))
            {
                spriteRenderer = renderer;
                Bounds bounds = renderer.bounds;
                dimensions = new Vector2Int(Mathf.RoundToInt(bounds.size.x - 1), Mathf.RoundToInt(bounds.size.y - 1));
                topLeft = new Vector2Int(0, dimensions.y);
                bottomRight = new Vector2Int(dimensions.x, 0);
            }
            else
            {
                dimensions = new Vector2Int(lockedScale.x, lockedScale.y);
                topLeft = new Vector2Int(0, dimensions.y);
                bottomRight = new Vector2Int(dimensions.x, 0);
            }
        }
    }

}
