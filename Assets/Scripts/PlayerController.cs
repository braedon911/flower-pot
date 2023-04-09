using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActorSolidSystem;
using StateMachines;

public class PlayerController : MonoBehaviour
{
    #region behaviors, children, tools
    Actor actor;
    CollisionBox collisionBox;
    Animator animator;
    GameObject rootsObject;
    StateMachine stateMachine;
    #endregion

    #region physics values
    float velocity_x = 0f;
    float velocity_y = 0f;

    float max_velocity_x = 3f;
    float max_velocity_y = 6f;
    [SerializeField]
    float drag = .5f;
    [SerializeField]
    float gravity = .8f;
    [SerializeField]
    float strafeSpeed = .1f;
    [SerializeField]
    float walkSpeed = .2f;

    [SerializeField]
    int maxGrowth = 16;
    [SerializeField]
    float growthSpeed = 2f;
    [SerializeField]
    float growthPopBoost = .2f;
    [SerializeField]
    float gravityMod = .5f;
    #endregion

    private Actor.CollisionAction resetX;
    private Actor.CollisionAction resetY;

    #region controls to check on update
    float horizontalInput = 0f;
    bool actionButton = false;
    bool actionButtonDown = false;
    #endregion

    private void Awake()
    {
        resetX = () => { velocity_x = 0; };
        resetY = () => { velocity_y = 0; };

        actor = GetComponent<Actor>();
        collisionBox = GetComponent<CollisionBox>();
        animator = GetComponent<Animator>();
        rootsObject = transform.GetChild(0).gameObject;

        stateMachine = new StateMachine(StateGround, StateFall, StateGrow);
    }

    void ApplyVelocityAndDrag()
    {
        velocity_x = Mathf.Lerp(velocity_x, 0f, drag);

        velocity_x = Mathf.Clamp(velocity_x, -1 * max_velocity_x, max_velocity_x);
        velocity_y = Mathf.Clamp(velocity_y, -1 * max_velocity_y, max_velocity_y);

        actor.MoveX(velocity_x, resetX);
        actor.MoveY(velocity_y, resetY);
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

    public void Suspend()
    {
        animator.speed = 0;
        stateMachine.Suspend();
    }
    IEnumerator Unsuspend_()
    {
        yield return new WaitForSeconds(.1f);
        stateMachine.Resume();
    }
    public void Unsuspend()
    {
        animator.speed = 1;
        StartCoroutine(Unsuspend_());
    }

    private void Update()
    {
        UpdateControlChecks();
        stateMachine.Execute();
    }
    #region states
    void StateGround()
    {
        if(actionButtonDown)
        {
            velocity_x = drag*drag;
            stateMachine.ChangeState("StateGrow", 0);
        }
        //stop quickly for QoL
        if (horizontalInput == 0f)
        {
            if (currentAnimation != "Idle") ChangeAnimation("Run Stop");
            velocity_x *= drag;

        }
        else if (horizontalInput < 0f)
        {
            ChangeAnimation("Run Start Left");
            velocity_x += horizontalInput * walkSpeed;
        }
        else
        {
            ChangeAnimation("Run Start");
            velocity_x += horizontalInput * walkSpeed;
        }

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
        velocity_y -= gravity;
        velocity_x += horizontalInput * strafeSpeed;

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
        //float verticalInput = Input.GetAxis("Vertical");
        int distanceToGround = GetDistanceToGround();

        switch (stateMachine.subState) {
            //start growing
            case 0:
                ChangeAnimation("Grow Start");
                velocity_x = 0;
                if (stateMachine.stateTimer > 11) stateMachine.ChangeState("StateGrow", 1);

                break;
            //grow
            case 1:
                velocity_x = 0;
                rootsObject.SetActive(true);
                //do that lovely scaling for our roots
                rootsObject.transform.localScale = new Vector3(1, -0.125f * distanceToGround, 1);

                if(distanceToGround < maxGrowth && actionButton)
                {
                    velocity_y = growthSpeed;
                }
                else stateMachine.ChangeState("StateGrow", 2);

                ChangeAnimation("Grow");
                break;
            //stop
            case 2:
                velocity_x = 0;
                velocity_y = 0;
                actor.OverrideVelocity();
                rootsObject.transform.localScale = new Vector3(1, -0.125f * distanceToGround, 1);
                rootsObject.transform.localScale = new Vector3(1, -0.125f * distanceToGround, 1);
                if (actionButtonDown) {
                    //give our player a little pop when they get off the roots
                    velocity_y += growthPopBoost;
                    velocity_x = horizontalInput * growthPopBoost;
                    stateMachine.ChangeState("StateGrow", 3);
                }

                else ChangeAnimation("Grow Stop");
                break;
            //exit
            case 3:
                rootsObject.SetActive(false);
                if (stateMachine.stateTimer > 5) stateMachine.ChangeState("StateFall");

                velocity_y -= gravity * gravityMod;
                velocity_x += horizontalInput * strafeSpeed * growthPopBoost;
                ChangeAnimation("Idle");
                break;
        }
        ApplyVelocityAndDrag();
    }
    #endregion
}
