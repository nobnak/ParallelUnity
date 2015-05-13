using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using Threading;

public class Movement : MonoBehaviour {
	public enum ModeEnum { Sequential = 0, Parallel }

	public const string PROP_VERTEX_BUF = "VertexBuf";
	public const string PROP_POSITION_BUF = "PositionBuf";

	public ModeEnum mode;
	public GameObject fab;
	public Material instanceMat;
	public int nInstances;
	public float radius;
	public float speed;

	public Bounds bounds;
	public Vector3 boundsMode = new Vector3 (1, 0, 1);

	Character[] _characters;
	Vector3[] _positions;
	ComputeBuffer _vertexBuf;
	ComputeBuffer _positionBuf;

	void Start () {
		var mesh = fab.GetComponent<MeshFilter> ().sharedMesh;
		var expandedVertices = new Vector3[mesh.triangles.Length];
		for (var i = 0; i < expandedVertices.Length; i++)
			expandedVertices [i] = fab.transform.TransformPoint(mesh.vertices [mesh.triangles [i]]);
		_vertexBuf = new ComputeBuffer (expandedVertices.Length, Marshal.SizeOf (expandedVertices [0]));
		_vertexBuf.SetData (expandedVertices);

		_positions = new Vector3[nInstances];
		_positionBuf = new ComputeBuffer (_positions.Length, Marshal.SizeOf (_positions [0]));

		_characters = new Character[nInstances];
		for (var i = 0; i < _characters.Length; i++) {
			_characters[i] = new Character(){ 
				position = radius * Random.insideUnitSphere, 
				velocity = speed * Random.onUnitSphere };
		}
	}
	void OnDestroy() {
		_vertexBuf.Release ();
		_positionBuf.Release ();
	}
	void Update () {
		var dt = Time.deltaTime;
		var vectorBounds = new VectorBounds (bounds.min, bounds.max, boundsMode);

		System.Action<int> body = (i) => {
			var c = _characters [i];
			var speed = c.velocity.magnitude;
			c.time += speed * dt;
			c.position += c.velocity * dt;
			c.position = vectorBounds.Clamp(c.position);
			_positions [i] = _characters [i].position;
		};
		if (mode == ModeEnum.Sequential) {
			for (var i = 0; i < _characters.Length; i++)
				body(i);
		} else {
			Parallel.For(0, _characters.Length, body);
		}
		_positionBuf.SetData (_positions);
	}
	void OnRenderObject() {
		GL.PushMatrix ();
		GL.MultMatrix (transform.localToWorldMatrix);
		instanceMat.SetPass (0);
		instanceMat.SetBuffer (PROP_VERTEX_BUF, _vertexBuf);
		instanceMat.SetBuffer (PROP_POSITION_BUF, _positionBuf);
		Graphics.DrawProcedural (MeshTopology.Triangles, _vertexBuf.count, nInstances);
		GL.PopMatrix ();
	}

	public class Character {
		public float time;
		public Vector3 position;
		public Vector3 velocity;
	}
}
