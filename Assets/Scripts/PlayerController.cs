using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Actor actor;
    CollisionBox collisionBox;
    Animator animator;

    private StateMachine stateMachine;

    #region physics values


    float velocity_x = 0f;
    float velocity_y = 0f;

    float max_velocity_x = 8f;
    float max_velocity_y = 8f;
    
    float drag = 1f;
    float gravity = 1f;
    float walkSpeed = 1f;

    int maxGrowth = 16;
    float growthSpeed = .2f;

    float growthPopBoost = 10f;
    #endregion

    private Actor.CollisionAction resetX;
    private Actor.CollisionAction resetY;

    #region controls to check on update
    float horizontalInput = 0f;
    bool actionButton = false;
    bool actionButtonDown = false;
    #endregion

    private void Start()
    {
        resetX = () => { velocity_x = 0; };
        resetY = () => { velocity_y = 0; };

        actor = GetComponent<Actor>();
        collisionBox = GetComponent<CollisionBox>();
        animator = GetComponent<Animator>();

        stateMachine = new StateMachine(StateGround, StateFall, StateGrow);
    }

    void ApplyVelocityAndDrag()
    {
        actor.MoveX(velocity_x, resetX);
        actor.MoveY(velocity_y, resetY);
        velocity_x = Mathf.Lerp(velocity_x, 0f, drag*Time.deltaTime);

        velocity_x = Mathf.Clamp(velocity_x, -1 * max_velocity_x, max_velocity_x);
        velocity_y = Mathf.Clamp(velocity_y, -1 * max_velocity_y, max_velocity_y);
    }
    int GetDistanceToGround()
    {
        int distance = 0;
        int maxDistance = 128;

        while(distance < maxDistance)
        {
            if (collisionBox.PlaceMeeting(actor.X, actor.Y - distance - 1)) break;
            distance++;
        }
        return distance;
    }

    void UpdateControlChecks()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        actionButton = Input.GetButton("Fire1");
        actionButtonDown = Input.GetButtonDown("Fire1");
    }

    string currentAnimation = "Idle";
    void ChangeAnimation(string name)
    {
        if (name != currentAnimation) { animator.Play(name); currentAnimation = name; }
    }

    private void Update()
    {
        UpdateControlChecks();
        stateMachine.Execute();
    }
    #region states
    void StateGround()
    {
        //get axes of input

        //apply movement
        velocity_x += horizontalInput * walkSpeed * Time.deltaTime;

        //stop quickly for QoL
        if (horizontalInput == 0f)
        {
            ChangeAnimation("Run Stop");
            velocity_x *= .5f * Time.deltaTime;
            
            if(actionButtonDown)
            {
                stateMachine.ChangeState("StateGrow", 0);
            }
        }
        else ChangeAnimation("Run Start");

        //fall if we are above air
        if (!actor.IsStanding())
        {
            ChangeAnimation("Idle");
            stateMachine.ChangeState("StateFall");
        }
        ApplyVelocityAndDrag();
    }
    void StateFall()
    {
        velocity_y -= gravity * Time.deltaTime;

        //go back to ground movement when we hit the ground
        if (actor.IsStanding())
        {
            stateMachine.ChangeState("StateGround");
            ChangeAnimation("Idle");
        }
        ApplyVelocityAndDrag();
    }
    void StateGrow()
    {
        velocity_x = 0;

        //float verticalInput = Input.GetAxis("Vertical");
        int distanceToGround = GetDistanceToGround();

        switch (stateMachine.subState) {
            //start growing
            case 0:
                ChangeAnimation("Grow Start");
                if (stateMachine.stateTimer > 28) stateMachine.ChangeState("StateGrow", 1);

                
                break;
            //grow
            case 1:
                if(distanceToGround < maxGrowth && actionButton)
                {
                    velocity_y = growthSpeed;
                }
                else stateMachine.ChangeState("StateGrow", 2);

                ChangeAnimation("Grow");
                break;
            //stop
            case 2:
                velocity_y = 0;

                if (actionButtonDown) {
                    velocity_y += growthPopBoost * Time.deltaTime;
                    stateMachine.ChangeState("StateGrow", 3);
                }

                if (stateMachine.stateTimer > 29) {
                    ChangeAnimation("Grow");
                }
                else ChangeAnimation("Grow Stop");
                break;
            //exit
            case 3:
                if (stateMachine.stateTimer > 29) stateMachine.ChangeState("StateFall");
                velocity_y -= gravity * Time.deltaTime;
                ChangeAnimation("Idle");
                break;
        }
        ApplyVelocityAndDrag();
    }
    #endregion
}
