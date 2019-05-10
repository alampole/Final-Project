using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectTarget : Node
{
    private float m_RaycastLength = 50.0f;
    public string TargetKey;

    public override NodeResult Execute()
    {
        string tag = tree.gameObject.tag;

        RaycastHit hit;

        if (Physics.Raycast(tree.gameObject.transform.position, tree.gameObject.transform.forward, out hit, m_RaycastLength))
        {
            string hitTag = hit.transform.gameObject.tag;
            //If we have the same tag, we're the same flock
            if (hitTag == tag)
                return NodeResult.FAILURE;

            //If its a valid tag, it will start with flock therefore we can assume we hit an enemy
            if (hitTag.Substring(0, 5) == "flock")
            {
                tree.SetValue(TargetKey, hit.transform.gameObject);
                return NodeResult.SUCCESS;
            }
        }

        return NodeResult.FAILURE;
    }
}
