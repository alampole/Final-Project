using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZapTarget : Node
{
    public string TargetKey;

    public override NodeResult Execute()
    {
        Boid target = ((GameObject)tree.GetValue(TargetKey)).GetComponent<Boid>();

        if (target != null)
        {
            LineRenderer lr = tree.gameObject.GetComponent<LineRenderer>();
            Vector3[] position = new Vector3[2];
            position[0] = tree.transform.position;
            position[1] = target.transform.position;

            lr.SetPositions(position);

            return NodeResult.SUCCESS;
        }

        return NodeResult.FAILURE;
    }
}
