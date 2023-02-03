using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

//this state machine system is designed to be attached to monobehaviours. it keeps track of a list of methods and manages transitions between them.
//you can call Execute to run whichever method is represented by the current state
public class StateMachine
{
    public List<State> states;
    public int stateTimer = 0;
    public int subState = 0;
    public State state;

    public StateMachine(params State.StateFunction[] _states)
    {
        states = new List<State>();
        foreach (State.StateFunction function in _states)
        {
            State state = new State(function);
            states.Add(state);
        }
        ChangeState(0);
    }
    public void ChangeState(int index, int new_substate = 0)
    {
        state = states[index];
        subState = new_substate;
        stateTimer = -1;
    }
    public void ChangeState(string name, int new_substate = 0)
    {
        bool found = false;
        for (int i = 0; i < states.Count; i++)
        {
            if (states[i].name.Equals(name))
            {
                state = states[i];
                subState = new_substate;
                stateTimer = -1;
                found = true;
                break;
            }
        }
        if (found == false)
        {
            Debug.LogWarning("State change attempted but state " + name + " was not found.");
        }

    }
    public void Execute()
    {
        state.function();
        stateTimer += 1;
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
}