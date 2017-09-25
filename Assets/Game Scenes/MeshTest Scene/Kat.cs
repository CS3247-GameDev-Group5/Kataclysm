using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kat : MonoBehaviour {
	public GameObject katBlock;

	void Start () {
		/* 
		SetShape(new int[][][] {
			// in x slices
			new int[][] {
				new int[] { 1, 1, 1 },
				new int[] { 1, 1, 0 },
			},
			new int [][] {
				new int[] { 1, 1, 0 },
				new int[] { 1, 0, 0 },
			}
		});
		*/
	}

	void SetShape(int[][][] katMatrix) {
		int xLength = katMatrix.Length;
		int yLength = katMatrix[0].Length;
		int zLength = katMatrix[0][0].Length;

		foreach(Transform child in this.transform) {
			Destroy(child.gameObject);
		}
		var oldPosition = transform.position;
		var oldRotation = transform.rotation;
		// instatiate child blocks at identity.
		transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);
		for(int x = 0; x < xLength; x++) {
			for(int y = 0; y < yLength; y++) {
				for(int z = 0; z < zLength; z++) {
					if(katMatrix[x][y][z] == 1) {
						Instantiate(katBlock, new Vector3(-1 + xLength - (x*2), 1 + (y*2), -1 + zLength - (z*2)), Quaternion.identity, this.transform);
					}
				}
			}
		}
		//restore original position and rotation.
		transform.SetPositionAndRotation(oldPosition, oldRotation);
	}
}
