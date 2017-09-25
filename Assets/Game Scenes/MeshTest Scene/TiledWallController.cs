using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiledWallController : MonoBehaviour {
	public GameObject basicTile;
	public GameObject cubeHole;
	public GameObject cylinderHole;
	
	public GameObject pyrimidHole;
	
	
	GameObject[,] tiles = new GameObject[3,5];
	public int[][] wallIds = new int[][] {
	new int[]{0, 0, 0, 0, 0},
	new int[]{0, 0, 0, 0, 0},
	new int[]{0, 0, 0, 0, 0}
	};
	void Start () {
		for(int i = 0; i < 3; i++) {
			for(int j = 0; j < 5; j++) {
				tiles[i,j] = transform.GetChild(0).GetChild(i*5 + j).gameObject;
			}
		}
		setWallArray(new int[][] {
					new int[]{0, 1, 2, 3, 0}, //btm
					new int[]{0, 0, 0, 1, 0}, //mid
					new int[]{0, 0, 0, 0, 0} } //top
		);
	}
	public void setWallArray(int[][] newWallIds) {
		for(int i = 0; i < 3; i++) {
			for(int j = 0; j < 5; j++) {
				if(wallIds[i][j] != newWallIds[i][j]) {
					replaceTile(i, j, intToTile(newWallIds[i][j]));
					wallIds[i][j] = newWallIds[i][j];
				}
			}
		}
	}

	GameObject intToTile(int id) {
		switch(id) {
			case 1:
			return cubeHole;
			case 2: 
			return cylinderHole;
			case 3:
			return pyrimidHole;
			default:
			return basicTile;
		}
	}
	void replaceTile(int row, int columnId, GameObject newTile) {
		switch(row) {
			case 0:
			replaceBottomTile(row, columnId, newTile);
			break;
			case 1:
			replaceMiddleTile(row, columnId, newTile);
			break;
			case 2:
			replaceTopTile(row, columnId, newTile);
			break;
		}
	}

	void replaceTopTile(int rowId, int columnId, GameObject newTile) {
		var x = tiles[rowId, columnId];
		var a = Instantiate (newTile, x.transform.position,
			x.transform.rotation, transform.GetChild(0));	
		tiles[rowId, columnId] = a;	
		Destroy(x.gameObject);
	}
	void replaceMiddleTile(int rowId, int columnId, GameObject newTile) {
		var x = tiles[rowId, columnId];
		var a = Instantiate (newTile, x.transform.position,
			x.transform.rotation, transform.GetChild(0));			
		Destroy(x.gameObject);
		tiles[rowId, columnId] = a;
		if(newTile == cubeHole) {
			var top = tiles[rowId+1, columnId];
			top.transform.position = a.transform.GetChild(0).position;
			a.transform.GetChild(0).position = a.transform.GetChild(0).position + (new Vector3(0f, 0.65f, 0f))*transform.localScale.y;
		}
	}
	void replaceBottomTile(int rowId, int columnId, GameObject newTile) {
		var x = tiles[rowId, columnId];
		var a = Instantiate (newTile, x.transform.position,
			x.transform.rotation, transform.GetChild(0));			
		Destroy(x.gameObject);
		tiles[rowId, columnId] = a;
		if(newTile == cubeHole) {
			var middle = tiles[rowId+1, columnId];
			middle.transform.position = a.transform.GetChild(0).position;
			var top = tiles[rowId+2, columnId];
			top.transform.position = a.transform.GetChild(0).position + (new Vector3(0f, 0.65f, 0f))*transform.localScale.y;

			a.transform.GetChild(0).position = a.transform.GetChild(0).position + (new Vector3(0f, 1.3f, 0f))*transform.localScale.y;
		}
	}	
	
	// Update is called once per frame
	void Update () {
		
	}
}
