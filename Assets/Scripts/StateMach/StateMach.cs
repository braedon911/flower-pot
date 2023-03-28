using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;

//this state machine system is designed to be attached to monobehaviours. it keeps track of a list of methods and manages transitions between them.
//you can call Execute to run whichever method is represented by the current state
public class StateMachine
{
    public List<State> states;
    public int stateTimer = 0;
    public int subState = 0;
    public State state;

    bool isSuspended = false;
    readonly bool canChangeStateWhileSuspended;

    public StateMachine(params State.StateFunction[] _states)
    {
        canChangeStateWhileSuspended = false;
        states = new List<State>();
        Debug.Log("States created in : ");
        foreach (State.StateFunction function in _states)
        {
            State state = new State(function);

            Debug.Log("----------" + state.name);
            states.Add(state);
        }
        ChangeState(0);
        RemoveDuplicates();
    }

    public StateMachine(bool canChangeStateWhileSuspended, params State.StateFunction[] _states) : this(_states)
    {
        this.canChangeStateWhileSuspended = canChangeStateWhileSuspended;
    }
    public StateMachine(StateMachine old)
    {
        states = new List<State>(old.states);
        stateTimer = 0;
        subState = 0;
        isSuspended = old.isSuspended;
        canChangeStateWhileSuspended = old.canChangeStateWhileSuspended;

        ChangeState(0);
    }
    public void ChangeState(int index, int newSubstate = 0)
    {
        SubChangeState(index, newSubstate);
    }
    public void ChangeState(string name, int newSubstate = 0)
    {
        bool found = false;
        for (int i = 0; i < states.Count; i++)
        {
            if (states[i].name.Equals(name))
            {
                SubChangeState(i, newSubstate);
                found = true;
                break;
            }
        }
        Debug.Assert(found, "State change attempted but state " + name + " was not found.");
    }
    void SubChangeState(int newState, int newSubState)
    {
        if (!isSuspended || canChangeStateWhileSuspended)
        {
            state = states[newState];
            subState = newSubState;
            stateTimer = -1;
        }   
    }
    public void Suspend()
    {
        //Debug.Assert(isSuspended, $"Machine {this} already suspended but suspend called anyway.");
        isSuspended = true;
    }
    public void Resume()
    {
        //Debug.Assert(!isSuspended, $"Machine {this} not suspended but resume called anyway.");
        isSuspended = false;
    }
    public void Execute()
    {
        if (!isSuspended)
        {
            state.function();
            stateTimer += 1;
        }
    }

    public override string ToString()
    {
        return states.ToString();
    }
    void RemoveDuplicates()
    {
        List<State> cleanedList = new List<State>();
        HashSet<State> hash = new HashSet<State>();

        foreach (State item in this.states)
        {
            if (hash.Add(item))
            {
                cleanedList.Add(item);
            }
        }
        states = cleanedList;
        ChangeState(state.name);
    }
    public static StateMachine operator +(StateMachine mach, State.StateFunction newState)
    {
        StateMachine finalStateMachine = new StateMachine(mach);
        finalStateMachine.states.Add(new State(newState));
        return finalStateMachine;
    }
}
public class State
{
    public string name;
    public StateFunction function;
    public delegate void StateFunction();

    public State(StateFunction function)
    {
        name = function.Method.Name;
        this.function = function;
    }
    public override string ToString()
    {
        return name;
    }
    public StateFunction ToStateFunction()
    {
        return function;
    }
}