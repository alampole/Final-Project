using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTree : MonoBehaviour
{
    Stack<Node> CallStack;
    public Hashtable Blackboard;
    public Node root;
    // Use this for initialization
    void Awake()
    {
        CallStack = new Stack<Node>();
        Blackboard = new Hashtable();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        RunStack();
    }

    #region BlackBoardFunction
    public void AddKey(string key)
    {
        if (Blackboard.ContainsKey(key) == false)
        {
            Blackboard.Add(key, null);
        }
    }

    public void AddKey(string aKey, object aValue)
    {
        AddKey(aKey);
        SetValue(aKey, aValue);
    }

    public object GetValue(string key)
    {
        if (Blackboard.ContainsKey(key))
        {
            return Blackboard[key];
        }
        else
        {
            return null;
        }
    }

    public void SetValue(string key, object value)
    {
        Blackboard[key] = value;
    }
    #endregion

    public void PushNode(Node node)
    {
        node.Reset();
        node.tree = this;
        CallStack.Push(node);
    }

    public void PushNodeWithOutReset(Node node)
    {
        node.tree = this;
        CallStack.Push(node);
    }
    public void PopTop()
    {
        CallStack.Pop();
    }

    public NodeResult RunStack()
    {
        if (CallStack.Count == 0)
        {
            // stack is empty - add to it
            PushNode(root);
        }

        Node top = CallStack.Peek();
        NodeResult result = top.Execute();
        switch (result)
        {
            case NodeResult.FAILURE:
            case NodeResult.SUCCESS:
                CallStack.Pop(); // remove this node
                if (CallStack.Count == 0)
                {
                    return result;
                }
                Node parent = CallStack.Peek();
                bool runstack = parent.SetChildResult(result);
                if (runstack == true)
                {
                    return RunStack(); // and let the parent node continue
                }
                else
                {
                    return result;
                }
            case NodeResult.RUNNING:
                return result; // we do not need to do anything in this case.
                        // we will continue with this node in the next frame.
            case NodeResult.STACKED:
                return RunStack(); // let the newly added child node have some CPU
                ;
            default:
                return result;
        }
    }
}
