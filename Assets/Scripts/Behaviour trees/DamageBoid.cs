using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBoid : Node
{
    public string TargetKey;

    public override NodeResult Execute()
    {
        Boid target = ((GameObject)tree.GetValue(TargetKey)).GetComponent<Boid>();

        if (target != null)
        {
            target.PewPew();
            return NodeResult.SUCCESS;
        }

        return NodeResult.FAILURE;
    }
}
