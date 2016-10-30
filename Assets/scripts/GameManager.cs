﻿using UnityEngine;
using System.Collections;

namespace SS.GameLogic {
	public class GameManager : MonoBehaviour {
		public GameObject tile;
		public GameObject tileGrid;
		public int gridSizeX = 100;
		public int gridSizeY = 100;

		public GameObject playerCamera;
		public float panSpeed = 10.0f;

		public enum Direction {Up, Left, Down, Right};
		public enum TileType {Space, Floor, Wall};

		private Sprite[] spaceSheet;
		private GameObject[,] grid;

		// Use this for initialization
		void Start () {
			InitTiles();
		}
		
		// Update is called once per frame
		void Update () {
			float moveHorizontal = Input.GetAxis ("Horizontal");
			float moveVertical = Input.GetAxis ("Vertical");

			Vector3 movement = new Vector3 (moveHorizontal, moveVertical) * panSpeed;

			playerCamera.transform.position = playerCamera.transform.position + movement;
		}

		private void InitTiles() {
			grid = new GameObject[gridSizeX, gridSizeY];
			TileManager tileManager;
			spaceSheet = Resources.LoadAll<Sprite>("turf/space"); //TODO replace this with AssetBundles for proper release

			/*
			TODO find size of the sprite then use it to dynamically build the grid. Set to 32px right now
			int tileWidth = (int)spaceSheet[0].bounds.size.x;
			int tileHeight = (int)spaceSheet[0].bounds.size.y;

			Debug.Log(tileWidth + " " + tileHeight);
			*/
			for (int i = 0; i < gridSizeX; i++) {
				for (int j = 0; j < gridSizeY; j++) {
					grid[i, j] = Instantiate(tile);
					grid[i, j].transform.position = new Vector3(i * 32, j * 32);
					grid[i, j].transform.SetParent(tileGrid.transform);

					tileManager = grid[i, j].GetComponent<TileManager>();
					int randomSpaceTile = (int)Random.Range(0, 101);
					tileManager.tileSprite = spaceSheet[randomSpaceTile];
					tileManager.gridX = i;
					tileManager.gridY = j;
					tileManager.gameManager = gameObject.GetComponent<GameManager>();
				}
			}
		}

		public bool CheckPassable(int gridX, int gridY, Direction direction) {
			int newGridX = gridX;
			int newGridY = gridY;

			Direction newDirection = Direction.Up;

			switch (direction) {
			case Direction.Up:
				newGridY = newGridY + 1;
				newDirection = Direction.Down;
				break;
			case Direction.Right:
				newGridX = newGridX + 1;
				newDirection = Direction.Left;
				break;
			case Direction.Down:
				newGridY = newGridY - 1;
				newDirection = Direction.Up;
				break;
			case Direction.Left:
				newGridX = newGridX - 1;
				newDirection = Direction.Right;
				break;
			}

			if (newGridX > grid.GetLength(0) - 1 || newGridX < 0 || newGridY > grid.GetLength(1) - 1 || newGridY < 0) {
				Debug.Log("Attempting to move off map");
				return false;
			}

			return (
				grid[gridX, gridY].GetComponent<TileManager>().passable[(int)direction] && 
				grid[newGridX, newGridY].GetComponent<TileManager>().passable[(int)newDirection]
			);
		}
	}
}