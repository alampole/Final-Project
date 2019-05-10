using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour {
    public Flock flock;
    public float speed = 0;
    public float turnspeed = 0;
    public Vector3 direction = Vector3.zero;
    public Vector3 forward = Vector3.zero;
    public int NeighborCount = 0;
    Vector3 noise = Vector3.zero;
    LayerMask ground;

    //[SerializeField]
    private string m_TrackTag = "Track";
	// Use this for initialization
	void Start () {
        speed = flock.speed;
        turnspeed = flock.turnspeed;
        ground = LayerMask.GetMask("Tracks");
	}
	
	// Update is called once per frame
	void Update () {
        forward = gameObject.transform.forward;
        Motivate();
	}
    private void LateUpdate()
    {
        Move();

        if (!CheckOnTrack())
        {
            GameObject boom = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            boom.transform.position = transform.position;
            boom.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            boom.AddComponent<Boom>();
            flock.removeBoid(gameObject);
            Destroy(gameObject);
        }
    }

    bool isNeighBor(Boid b)
    {
        if (b == this)
        {
            return false;
        }
        // distance check
        Vector3 offset = b.gameObject.transform.position- transform.position;
        float ds = offset.sqrMagnitude;
        if (ds > flock.NeighborDistanceSquared)
        {
            return false;
        }
        // line of sight check
        float angle = Vector3.Angle(offset, transform.forward);
        if (angle > flock.FOV)
        {
            return false;
        }

        return true;
    }

    private bool CheckOnTrack()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 10.0f, ground))
        {
            //Debug.Log("Tag = " + hit.transform.tag);
            //Debug.Log("Tag Search = " + m_TrackTag);
            if (hit.transform.tag == m_TrackTag)
            {
                return true;
            }
        }

        return false;
    }

    void Move()
    {
        Quaternion turnDirection = Quaternion.FromToRotation(Vector3.forward,direction);
        Quaternion rotation = Quaternion.RotateTowards(gameObject.transform.rotation, turnDirection, Time.deltaTime* turnspeed);
        rotation.x = 0;
        transform.rotation = rotation;
        // then move

        Vector3 pos = transform.position + transform.forward * speed * Time.deltaTime;
        pos.y = 0.05f;
        transform.position = pos;
        // did I fall off the track?
        if (!Physics.Raycast(pos,-Vector3.up,5,ground))
        {
            flock.removeBoid(gameObject);

        }
    }

    void Motivate()
    {
        //// not run into stuff is top priority
        //RaycastHit hit;
        //if (Physics.Raycast(transform.position, transform.forward * 2, out hit))
        //{
        //    Debug.DrawRay(transform.position, transform.forward * 2, Color.red);
        //    direction = Vector3.Reflect(transform.forward, hit.normal);
        //    return;
        //    // we need to turn away from it
        //}
        Vector3 Alignment = Vector3.zero;
        Vector3 Cohesion = Vector3.zero;
        Vector3 Avoidance = Vector3.zero;
        NeighborCount = 0;
        foreach (GameObject g in flock.boids)
        {
            Boid b = g.GetComponent<Boid>();
            if (isNeighBor(b))
            {
                NeighborCount++;
                Cohesion += g.transform.position;
                Alignment += g.transform.forward;
                if (Vector3.Distance(g.transform.position, transform.position) < flock.AvoidMininum)
                {
                    Avoidance += (transform.position - g.transform.position);
                }
            }
        }
        if (NeighborCount != 0)
        {
            Cohesion = Cohesion / NeighborCount;
            Alignment = Alignment / NeighborCount;
            Cohesion = Cohesion - transform.position;
        }
        // normalize the three motivations


        Vector3 SwarmDir = flock.target.transform.position - transform.position;
        //Alignment = Vector3.Normalize(Alignment);
        //Avoidance = Vector3.Normalize(Avoidance);
        //if (Random.Range(0, 100) < 5)
        //{
        //    noise = UnityEngine.Random.onUnitSphere * flock.noise * (flock.boids.Length - NeighborCount);
        //}
        // and add and scale them
        direction = SwarmDir + Cohesion * flock.cohesionWeight + Alignment * flock.alignmentWeight + Avoidance * flock.avoidanceWeight;
        // then renormalize again
        direction = Vector3.Normalize(direction);
    }

    public void PewPew()
    {
        speed *= 2;
        turnspeed *= 0.1f;
    }
}
