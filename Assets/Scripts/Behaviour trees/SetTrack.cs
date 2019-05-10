using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTrack : Node
{
    public string ArrayKey;
    public string IndexKey;
    public string DirectionKey;

    public override NodeResult Execute()
    {
        //detect if we're at an intersection

        Collider[] colliders = Physics.OverlapSphere(tree.gameObject.transform.position, 1.0f);

        foreach (Collider coll in colliders)
        {
            string tag = coll.gameObject.tag;

            if (tag != ((GameObject[])tree.GetValue(ArrayKey))[0].tag)
            {
                if (tag.Substring(0, 5) == "track")
                {
                    int direction = Random.Range(0, 2);

                    if (tree.gameObject.tag == "Player")
                    {
                        direction = tree.gameObject.GetComponent<PlayerInput>().Direction;
                    }

                    if (direction == 0)
                    {
                        //Keep going straight
                        return NodeResult.SUCCESS;
                    }

                    //Keep this so we can get array
                    Catmul catmul = coll.gameObject.transform.parent.GetComponent<Catmul>();
                    int index = 0;

                    for (int i = 0; i < catmul.PointObjects.Length; i++)
                    {
                        if (coll.gameObject.name == catmul.PointObjects[i].name)
                        {
                            index = i;
                            break;
                        }
                    }

                    //Choose direction
                    tree.SetValue(IndexKey, index);
                    tree.SetValue(ArrayKey, catmul.PointObjects);

                    if (direction == 1)
                    {
                        tree.SetValue(DirectionKey, 1);
                    }
                    else
                    {
                        tree.SetValue(DirectionKey, -1);
                    }

                    return NodeResult.SUCCESS;
                }
            }
        }

        return NodeResult.SUCCESS;
    }
}
