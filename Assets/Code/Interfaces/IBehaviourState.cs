using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBehaviourState
{
    void EnterState(Entity _entity);
    void UpdateState();
    void ExitState();
}

public class IdleState : IBehaviourState
{
    private Entity _entity;

    public void EnterState(Entity entity)
    {
        _entity = entity;
    }

    public void UpdateState()
    {

    }

    public void ExitState()
    {

    }
}

public class ChasingState : IBehaviourState
{
    private Entity _entity;
    private Transform _player;

    public ChasingState (Transform player)
    {
        _player = player;
    }

    public void EnterState(Entity entity)
    {
        _entity = entity;
    }

    public void UpdateState()
    {
        _entity.MoveTowards(_player.position);
    }

    public void ExitState()
    {
        //reset animations and etc
    }
}