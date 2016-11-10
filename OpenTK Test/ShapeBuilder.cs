using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/* Tool to create a shape defined as a series of vertices
 joined by straight lines. Shapes can be built as meshes. */
public class ShapeBuilder {

	private List<Vector2> vertices = new List<Vector2>();
	bool closed;

	/* Prepares shape by creating first point.
	 Does nothing if vertices have already been defined. */
	public void Open(Vector2 point)
	{
		if (vertices.Count == 0)
		{
			vertices.Add(point);
		}
	}

	/* Defines a straight line from the last added point to the passed point.
	 Adds the passed point to the list of vertices. */
	public void AddLine(Vector2 toPoint)
	{
		if (vertices.Count != 0)
		{
			vertices.Add(toPoint);
		}
	}

	/* Defines an arc starting from the last added point sweeping around
	 the z-axis at centre with the defined angular distance.
	 Adds a number of points at regular intevals along the arc defined by steps.
	 Steps must be at least 2. */
	public void AddArc(Vector2 centre, float angularDistance, uint steps)
	{
		if (vertices.Count != 0 && steps >= 2)
		{
			Vector2 start = vertices.Last() - centre;
			float delta = angularDistance / steps;
			float angle = 0.0f;

			for (int i = 1; Mathf.Abs(angle) < Mathf.Abs(angularDistance) - Vector2.kEpsilon; i++)
			{
				angle = delta * i;
				Vector2 current = Rotate(start, angle);
				vertices.Add(current + centre);
			}
		}
	}

	/* Sets the shape to be closed if there are at least three points.
	 The shape will no longer be mutable and can be built as a mesh. */
	public void Close()
	{
		if (vertices.Count >= 3)
		{
			if (vertices.Last() == vertices.First())
			{
				vertices.RemoveAt(vertices.Count - 1);
			}
			closed = true;
		}
	}

	/* Returns a 2D vector rotated around the z-axis at the origin by angle. */
	private static Vector2 Rotate(Vector3 point, float angle)
	{
		float cos = Mathf.Cos(angle);
		float sin = Mathf.Sin(angle);

		float x = point.x * cos - point.y * sin;
		float y = point.x * sin + point.y * cos;

		return new Vector2(x, y);
	}

	/* Transforms closed shape into a mesh and returns the result.
	 Triangles of mesh are calculated using the Triangulation methods. */
	public Mesh BuildMesh()
	{
		Mesh mesh = new Mesh();

		if (closed)
		{
			List<int> triangles;
			Triangulation.Process(vertices, out triangles);

			int n = vertices.Count;
			Vector3[] newVertices = new Vector3[n];
			for (int i = 0; i < vertices.Count; i++)
			{
				Vector2 vertex = vertices[i];
				newVertices[i] = new Vector3(vertex.x, vertex.y, 0);
			}
			int[] newTriangles = triangles.ToArray();

			mesh.vertices = newVertices;
			mesh.triangles = newTriangles;
		}

		return mesh;
	}

	public Vector3[] GetVertices()
	{
		Vector3[] newVertices = new Vector3[vertices.Count];
		for (int i = 0; i < vertices.Count; i++)
		{
			Vector2 vertex = vertices[i];
			newVertices[i] = new Vector3(vertex.x, vertex.y, 0);
		}
		return newVertices;
	}

	public void Print()
	{
		foreach(Vector2 vertex in vertices)
		{
			Debug.Log(vertex);
		}
	}
}
