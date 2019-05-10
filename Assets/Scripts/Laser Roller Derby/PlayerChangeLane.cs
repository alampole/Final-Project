using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChangeLane : Task
{
    public override NodeResult Execute()
    {
        Turning dir = (Turning)tree.GetValue("TurnRequested");
        if (dir != Turning.STRAIGHT)
        {
            // are we at a crossover node on the track?
            // if so, then turn to the other track, in the correct direction requested.
            // who sets the Turning value? The player does via keydown in another module.
            GameObject waypoint = (GameObject)tree.GetValue("Waypoint");
            int wpi = (int)tree.GetValue("Index");
            // is this a crossover point? (i.e is it on another track?
            TrackManager tm = (TrackManager)tree.GetValue("TrackManager");
            int trackIndex = (int)tree.GetValue("TrackIndex");
            // are we at a potential crossover on our track?
            Vector3 pos = waypoint.transform.position;
            for (int i = 0; i < 16; i++)
            {
                if (pos == tm.splines[trackIndex].points[i])
                {
                    // yes we are.  Now is it on the other tracks?, and if so, which one, and where on the track?
                    for (int s = 0; s < tm.splines.Length; s++)
                    {
                        if (s == trackIndex)
                        {
                            continue;
                        }
                        for (int j = 0; j < 16; j++)
                        {
                            if (pos == tm.splines[s].points[j])
                            {
                                // we have a winner - player requested to turn, and is at a waypoint that crosses
                                // track s at control point J
                                // cross product and 8 if statements to manage the turn.
                                Vector3 a1 = pos;
                                Vector3 b1 = pos;
                                Vector3 a2 = tm.splines[trackIndex].lanes[1,(wpi + 1) % 320];
                                Vector3 b2 = tm.splines[s].lanes[1, (j * 20+1)%320]; //  middle lane, add one to crossover
                                Vector3 AV = a2 - a1;
                                Vector3 BV = b2 - b1;
                                float angleAB = Vector3.SignedAngle(AV, BV,Vector3.up);
                                int ADir = (int)tree.GetValue("Direction");
                                int NDir = 1;
                                // 8 ifs now...
                                if (dir == Turning.LEFT)
                                {
                                    //Debug.Log("LEFT. ADIR: " + ADir + " angle:" + angleAB);
                                    if (ADir == 1 && angleAB > 0)
                                    {
                                        NDir = -1;
                                    }
                                    else if (ADir == 1 && angleAB <=0)
                                    {
                                        NDir = +1;
                                    }
                                    else if (ADir == -1 && angleAB > 0)
                                    {
                                        NDir = +1;
                                    }
                                    else if (ADir == -1 && angleAB <= 0)
                                    {
                                        NDir = -1;
                                    }
                                    //Debug.Log("LEFT:NDIR: " + NDir);
                                }
                                else // must be going right
                                {
                                    //Debug.Log("Right. ADIR: " + ADir + " angle:" + angleAB);
                                    if (ADir == 1 && angleAB > 0)
                                    {
                                        NDir = 1;
                                    }
                                    else if (ADir == 1 && angleAB <= 0)
                                    {
                                        NDir = -1;
                                    }
                                    else if (ADir == -1 && angleAB > 0)
                                    {
                                        NDir = -1;
                                    }
                                    else if (ADir == -1 && angleAB <= 0)
                                    {
                                        NDir = 1;
                                    }
                                    //Debug.Log("Right:NDIR: " + NDir);

                                }
                                // now that we have a new direction, a new start point (j * 20+1)%320, and a spline to follow (index s, actual tm.splines[s];
                                // we can update the blackboard
                                // and return sucess
                                //Debug.Log("changing from " + trackIndex + " To " + s);
                                //Debug.Log("direction from: " + ADir + " to "+ NDir);
                                tree.SetValue("Direction", NDir);
                                tree.SetValue("TrackIndex", s);
                                tree.SetValue("Waypoints", tm.splines[s].sp);
                                tree.SetValue("Waypoint", tm.splines[s].sp[(j * 20 + NDir+320)%320]);
                                tree.SetValue("Index", (j * 20 + NDir+ 320) % 320);
                                //Debug.Log("Back to straight now that we have turned");
                                tree.SetValue("TurnRequested", Turning.STRAIGHT);
                                return NodeResult.SUCCESS;
                            }
                        }
                    }
                }
            }
        }
        return NodeResult.SUCCESS;
    }

    public override void Reset()
    {

        base.Reset();
    }
}
