using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateMesh : MonoBehaviour
{
	public int numPoints = 6;
	public Camera targetCamera;
	public float deltaUvY = 0.1f;
//	public Vector3[] cVertices;
//	public int[] cIndeces;

	Vector3[] shape;
	Vector3 prePos;
	Mesh mesh;
	float uvY;
	bool lineStart;
	// Use this for initialization
	void Start ()
	{
		targetCamera = Camera.main;
		shape = new Vector3[numPoints];
		for (int i = 0; i < numPoints; i++) {
			Quaternion dRot = Quaternion.Euler (Vector3.forward * 360f * (float)i / (float)numPoints);
			shape [i] = dRot * Vector3.right * 0.5f;
		}
		mesh = new Mesh ();
		mesh.MarkDynamic ();
		GetComponent<MeshFilter> ().mesh = mesh;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (mesh.vertexCount > 60000)
			return;
		Vector3 pos = Input.mousePosition;
		pos.z = 10f;
		pos = targetCamera.ScreenToWorldPoint (pos);
		if (Input.GetMouseButton (0) && (pos - prePos).magnitude > 0.1f) {
			if (Input.GetMouseButtonDown (0))
				BeginLine (pos);
			else
				DrawLine (pos, 0.1f * Mathf.Log ((pos - prePos).magnitude));
		} else if (Input.GetMouseButtonUp (0))
			EndLine (pos);
	}

	public void BeginLine (Vector3 pos)
	{
		int vCount = mesh.vertexCount;

		Vector3[] vertices = new Vector3[vCount + 1];
		System.Array.Copy (mesh.vertices, vertices, vCount);
		vertices [vCount] = pos;
		
		uvY = 0;
		Vector2[] uv = new Vector2[vCount + 1];
		System.Array.Copy (mesh.uv, uv, vCount);
		uv [vCount] = new Vector2 (0.5f, uvY);

		mesh.vertices = vertices;
		mesh.uv = uv;
		prePos = pos;
		lineStart = true;
	}

	public void DrawLine (Vector3 pos, float size)
	{
		int vCount = mesh.vertexCount - (lineStart ? 0 : 1);

		Vector3[] vertices = new Vector3[vCount + numPoints];
		System.Array.Copy (mesh.vertices, vertices, vCount);
		System.Array.Copy (GetNextPoints (prePos, pos, size), 0, vertices, vCount, numPoints);
		
		uvY += (pos - prePos).magnitude * deltaUvY;
		Vector2[] uv = new Vector2[vCount + numPoints];
		System.Array.Copy (mesh.uv, uv, vCount);
		System.Array.Copy (GetNextUv (), 0, uv, vCount, numPoints);

		int[] meshIndeces = mesh.GetIndices (0);
		int iCount = meshIndeces.Length - (lineStart ? 0 : 3 * numPoints);
		
		int[] indeces = new int[iCount + numPoints * (lineStart ? 3 : 6)];
		System.Array.Copy (meshIndeces, indeces, iCount);
		if (lineStart) 
			System.Array.Copy (GetBeginIndeces (vCount - 1), 0, indeces, iCount, numPoints * 3);
		else 
			System.Array.Copy (GetNextIndeces (vCount - numPoints), 0, indeces, iCount, numPoints * 6);


		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.SetIndices (indeces, MeshTopology.Triangles, 0);
		
		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();

		prePos = pos;
		lineStart = false;

		EndLine (pos);
	}

	public void EndLine (Vector3 pos)
	{
		if (lineStart)
			DrawLine (pos, 0.1f);
		int vCount = mesh.vertexCount;

		Vector3[] vertices = new Vector3[vCount + 1];
		System.Array.Copy (mesh.vertices, vertices, vCount);
		System.Array.Copy (new Vector3[1]{pos}, 0, vertices, vCount, 1);

		uvY += (pos - prePos).magnitude * deltaUvY;
		Vector2[] uv = new Vector2[vCount + 1];
		System.Array.Copy (mesh.uv, uv, vCount);
		System.Array.Copy (new Vector2[1]{new Vector2 (0.5f, uvY)}, 0, uv, vCount, 1);

		int[] meshIndeces = mesh.GetIndices (0);
		int iCount = meshIndeces.Length;

		int[] indeces = new int[iCount + numPoints * 3];
		System.Array.Copy (meshIndeces, indeces, iCount);
		System.Array.Copy (GetEndIndeces (vCount - numPoints), 0, indeces, iCount, numPoints * 3);

		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.SetIndices (indeces, MeshTopology.Triangles, 0);

		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();
	}

	Vector3[] GetNextPoints (Vector3 from, Vector3 to, float size = 1f)
	{
		Vector3 dir = to - from;
		Quaternion rot = Quaternion.FromToRotation (Vector3.forward, dir);
		Vector3[] points = new Vector3[numPoints];
		for (int i = 0; i < numPoints; i++)
			points [i] = rot * shape [i] * size + to;
		return points;
	}
	Vector2[] GetNextUv ()
	{
		Vector2[] uv = new Vector2[numPoints];
		for (int i = 0; i < numPoints; i++)
			uv [i] = new Vector2 (Mathf.PingPong ((float)i / (float)numPoints * 2f, 1f), uvY);
		return uv;
	}
	int[] GetBeginIndeces (int from)
	{
		int[] indeces = new int[numPoints * 3];
		for (int i = 0; i < numPoints; i++) {
			indeces [i * 3 + 0] = from;
			indeces [i * 3 + 1] = from + (i + 1) % numPoints + 1;
			indeces [i * 3 + 2] = from + i + 1;
		}
		return indeces;
	}
	int[] GetNextIndeces (int from)
	{
		int[] indeces = new int[numPoints * 6];
		for (int i = 0; i < numPoints; i++) {
			indeces [i * 6 + 0] = from + i;
			indeces [i * 6 + 1] = from + (i + 1) % numPoints;
			indeces [i * 6 + 2] = from + numPoints + i;
			
			indeces [i * 6 + 3] = from + (i + 1) % numPoints;
			indeces [i * 6 + 4] = from + numPoints + (i + 1) % numPoints;
			indeces [i * 6 + 5] = from + numPoints + i;
		}
		return indeces;
	}
	int[] GetEndIndeces (int from)
	{
		int[] indeces = new int[numPoints * 3];
		for (int i = 0; i < numPoints; i++) {
			indeces [i * 3 + 0] = from + numPoints;
			indeces [i * 3 + 1] = from + i;
			indeces [i * 3 + 2] = from + (i + 1) % numPoints;
		}
		return indeces;
	}
}
