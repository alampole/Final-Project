using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Catmul : MonoBehaviour
{
    float x;
    float z;
    //Use the transforms of GameObjects in 3d space as your points or define array with desired points
    public Vector3[] points;
    public GameObject[] PointObjects;

    //Store points on the Catmull curve so we can visualize them
    public GameObject[] sp;

    //How many points you want on the curve
    public float amountOfPoints = 20.0f;
    public Vector3[,] lanes;
    //set from 0-1
    public float alpha = 0.5f;
    public GameObject RoadPrefab;
    int pointIndex;
    public int spc = 320;
    public float trackwidth = 200.0f;

    public Vector3 firstPoint;

    public void Start()
    {

    }

    public void GenerateSpline()
    {

        sp = new GameObject[320];
        lanes = new Vector3[3, 320];
        x = transform.position.x;
        z = transform.position.z;
        points = new Vector3[16];
        // first, make control points.  Going to be 16 of them - 4 on each side of a cube - so we'll wind up with a rounded rect.
        for (int i = 0; i < 4; i++)
        {
            points[i] = new Vector3(x - 15 + i * 10, 0, z - 25);
            points[i + 4] = new Vector3(x + 25, 0, z - 15 + i * 10);
            points[i + 8] = new Vector3(x + 15 - i * 10, 0, z + 25);
            points[i + 12] = new Vector3(x - 25, 0, z + 15 - i * 10);
        }

        PointObjects = new GameObject[16];

        for (int i = 0; i < points.Length; i++)
        {
            if (PointObjects[i] == null)
            {
                PointObjects[i] = new GameObject("PointObject" + i.ToString());
            }

            PointObjects[i].transform.position = points[i];
            PointObjects[i].transform.parent = transform;
            PointObjects[i].tag = gameObject.tag;
            PointObjects[i].AddComponent<SphereCollider>().isTrigger = true;
        }

        pointIndex = 0;
        for (int i = -1; i < points.Length - 1; i++)
        {
            CatmulRom(i);
        }
        // now generate the parallels
        for (int i = 0; i < spc; i++)
        {
            Vector3 a1 = lanes[1, i];
            Vector3 a2 = lanes[1, (i + 1) % spc];
            Vector3 perp = Vector3.Cross(a2 - a1, Vector3.up).normalized;
            Vector3 r = a1 + perp * trackwidth;
            Vector3 l = a1 - perp * trackwidth;
            lanes[2, i] = l;
            lanes[0, i] = r;
        }

        // noe generate the mesh tiles
        for (int i = 0; i < spc; i++)
        {
            MeshFilter mf = sp[i].GetComponent<MeshFilter>();
            MeshCollider mc = sp[i].GetComponent<MeshCollider>();
            sp[i].gameObject.tag = "Track";
            Mesh mesh = new Mesh();
            mf.mesh = mesh;
  

            Vector3[] vertices = new Vector3[4];
            vertices[0] = lanes[0, i] - lanes[1,i]; // must subtract location of center or we will offset things a lot.
            vertices[1] = lanes[2, i] - lanes[1,i]; // the verts are relative to the game object we are drawing...
            vertices[2] = lanes[0, (i + 1) % spc] - lanes[1,i];
            vertices[3] = lanes[2, (i + 1) % spc]- lanes[1,i];


            mesh.vertices = vertices;
            int[] tri = new int[6];
            tri[0] = 0;
            tri[1] = 2;
            tri[2] = 1;

            tri[3] = 2;
            tri[4] = 3;
            tri[5] = 1;

            mesh.triangles = tri;

            Vector3[] normals = new Vector3[4];
            normals[0] = Vector3.up;
            normals[1] = Vector3.up;
            normals[2] = Vector3.up;
            normals[3] = Vector3.up;

            mesh.normals = normals;
            Vector2[] uv = new Vector2[4];
            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(1, 0);
            uv[2] = new Vector2(0, 1);
            uv[3] = new Vector2(1, 1);

            mesh.uv = uv;
            mesh.RecalculateBounds();
            mc.sharedMesh = null;
            mc.sharedMesh = mesh;
        }

        Vector3[] positions = new Vector3[sp.Length];

        for (int i = 0; i < sp.Length; i++)
        {
            positions[i] = sp[i].transform.position;
            if (positions[i] == Vector3.zero)
                positions[i] = sp[i].transform.position;

            positions[i].y += 0.1f;
        }

        //gameObject.AddComponent<LineRenderer>();
        LineRenderer line = GetComponent<LineRenderer>();
        line.SetPositions(positions);

        firstPoint = sp[0].transform.position;
    }
    int getIndex(int index)
    {
        return (index+ points.Length) % points.Length;
    }

    void CatmulRom(int index)
    {


        Vector3 p0 = points[getIndex(index+0)]; // Vector3 has an implicit conversion to Vector2
        Vector3 p1 = points[getIndex(index + 1)];
        Vector3 p2 = points[getIndex(index + 2)];
        Vector3 p3 = points[getIndex(index + 3)];

        float t0 = 0.0f;
        float t1 = GetT(t0, p0, p1);
        float t2 = GetT(t1, p1, p2);
        float t3 = GetT(t2, p2, p3);

        for (float t = t1; t < t2; t += ((t2 - t1) / amountOfPoints))
        {
            Vector3 A1 = (t1 - t) / (t1 - t0) * p0 + (t - t0) / (t1 - t0) * p1;
            Vector3 A2 = (t2 - t) / (t2 - t1) * p1 + (t - t1) / (t2 - t1) * p2;
            Vector3 A3 = (t3 - t) / (t3 - t2) * p2 + (t - t2) / (t3 - t2) * p3;

            Vector3 B1 = (t2 - t) / (t2 - t0) * A1 + (t - t0) / (t2 - t0) * A2;
            Vector3 B2 = (t3 - t) / (t3 - t1) * A2 + (t - t1) / (t3 - t1) * A3;

            Vector3 C = (t2 - t) / (t2 - t1) * B1 + (t - t1) / (t2 - t1) * B2;

            if (pointIndex == 0 || sp[pointIndex-1].transform.position != C)
            {
                lanes[1,pointIndex] = C;
                sp[pointIndex++]=Instantiate(RoadPrefab,C,Quaternion.identity);
            }
        }
    }

    float GetT(float t, Vector3 p0, Vector3 p1)
    {
        float a = Mathf.Pow((p1.x - p0.x), 2.0f) + Mathf.Pow((p1.y - p0.y), 2.0f)+ Mathf.Pow((p1.z-p0.z),2.0f);
        float b = Mathf.Pow(a, 0.5f);
        float c = Mathf.Pow(b, alpha);

        return (c + t);
    }

    private void OnDrawGizmos()
    {
        if (points == null)
            return;
        for (int i = 0; i < points.Length;i++)
        {
            Gizmos.DrawSphere(points[i], 0.5f);
        }
        for (int i = 0; i < sp.Length; i++)
        {
            Gizmos.DrawLine(sp[i].transform.position, sp[(i + 1) % spc].transform.position);
        }
    }
}