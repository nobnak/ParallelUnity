using UnityEngine;
using System.Collections;

public class VectorBounds {
	Vector3 _boundsMin;
	Vector3 _boundsMax;
	Vector3 _boundsSize;
	Vector3 _boundsMode;

	public VectorBounds(Vector3 boundsMin, Vector3 boundsMax, Vector3 boundsMode) {
		this._boundsMin = boundsMin;
		this._boundsMax = boundsMax;
		this._boundsSize = boundsMax - boundsMin;
		this._boundsMode = boundsMode;
	}

	public Vector3 Clamp(Vector3 p) {
		for (var i = 0; i < 3; i++) {
			var x = p[i];
			var min = _boundsMin[i];
			var max = _boundsMax[i];
			var size = _boundsSize[i];
			var mode = _boundsMode[i];

			var of = (max <= x ? 1 : 0);
			var uf = (x <= min ? 1 : 0);
			var rp = (uf - of) * size + x;
			var cl = Mathf.Clamp(x, min, max);
			p[i] = Mathf.Lerp(cl, rp, mode);
		}
		return p;
	}
}
