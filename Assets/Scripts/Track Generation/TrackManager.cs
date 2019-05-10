using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackManager : MonoBehaviour {
    public Catmul[] splines;
    public GameObject splinePrefab;
    public GameObject[] swarmleaderPrefab;
    public string[][] maskNames;

	// Use this for initialization
	void Start () {
        maskNames = new string[5][];

        maskNames[0] = new string[] { "flock1", "flock2", "flock3", "flock4" };
        maskNames[1] = new string[] { "flock0", "flock2", "flock3", "flock4" };
        maskNames[2] = new string[] { "flock0", "flock1", "flock3", "flock4" };
        maskNames[3] = new string[] { "flock0", "flock1", "flock2", "flock4" };
        maskNames[4] = new string[] { "flock0", "flock1", "flock2", "flock3" };
        splines = new Catmul[5]; // TODO - change
        splines[0] = Instantiate(splinePrefab, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<Catmul>();
        splines[1] = Instantiate(splinePrefab, new Vector3(30, 0, 30), Quaternion.identity).GetComponent<Catmul>();
        splines[2] = Instantiate(splinePrefab, new Vector3(30, 0, -30), Quaternion.identity).GetComponent<Catmul>();
        splines[3] = Instantiate(splinePrefab, new Vector3(-30, 0, 30), Quaternion.identity).GetComponent<Catmul>();
        splines[4] = Instantiate(splinePrefab, new Vector3(-30, 0, -30), Quaternion.identity).GetComponent<Catmul>();
        // TODO add code here

        for (int i = 0; i < splines.Length; i++) // TO DO - change code
        {
            splines[i].gameObject.tag = "track" + i.ToString();
            splines[i].GenerateSpline();
        }
        // spawn the flocks on the tracks.  Track 0 is where the player begins.
        for (int i = 0; i < swarmleaderPrefab.Length; i++) // TO DO CHANGE CODE 
        {
            // TO DO - Spawn the swarm leader
            // TODO - Get the follow track script, and tell it about the track manager (so it can find more tracks), and the spline.
            // make sure to set the mask on the flock, and to say which is the player. 

            // = Instantiate(swarmleaderPrefab[i], Vector3.zero, Quaternion.identity);

            GameObject leader = Instantiate(swarmleaderPrefab[i], splines[i].firstPoint, Quaternion.identity);
            if (i == 0)
                leader.tag = "Player";
            //else
            {
                BehaviorTree bt = leader.GetComponent<BehaviorTree>();

                Sequence sq = new Sequence();
                bt.root = sq;

                SelectNextGameObject select = new SelectNextGameObject();
                select.ArrayKey = "array";
                select.IndexKey = "index";
                select.GameObjectKey = "currentObject";
                select.DirectionKey = "direction";
                select.tree = bt;

                MoveTo mt = new MoveTo();
                mt.TargetName = "currentObject";
                mt.tree = bt;

                SetTrack st = new SetTrack();
                st.ArrayKey = "array";
                st.IndexKey = "index";
                st.DirectionKey = "direction";

                sq.children.Add(select);
                sq.children.Add(mt);
                sq.children.Add(st);

                bt.AddKey("array", splines[i].PointObjects);
                bt.AddKey("index", 1);
                bt.AddKey("currentObject", splines[i].PointObjects[1]);
                bt.AddKey("direction", 1);
                bt.AddKey("Speed", 10.0f);
                bt.AddKey("TurnSpeed", 10.0f);
                bt.AddKey("Accuracy", 1.5f);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
