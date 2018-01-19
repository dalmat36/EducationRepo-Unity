using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	public int xDim;
	public int yDim;

	public enum PieceType
	{
		BUBBLE,
		EMPTY,
		NORMAL,
		COUNT
	};

	[System.Serializable]
	public struct PiecePrefab
	{
		public PieceType type;
		public GameObject prefab;
	};

	public PiecePrefab[] piecePrefabs;
	public GameObject backgroundPrefab;

	private Dictionary<PieceType,GameObject> piecePrefabDict;

	private GamePiece[,] pieces;
	public float FillTime;

	public bool inverse = false;

	private GamePiece pressedPiece;
	private GamePiece enteredPiece;

	// Use this for initialization
	void Start () {
		piecePrefabDict = new Dictionary<PieceType,GameObject> ();

		//add piece prefabs to prefac dictionary
		for (int i = 0; i < piecePrefabs.Length; i++) {
			if (!piecePrefabDict.ContainsKey (piecePrefabs [i].type)) {
				piecePrefabDict.Add (piecePrefabs [i].type, piecePrefabs [i].prefab);
			}
		}

		//create background gameobjects
		for (int x = 0; x < xDim; x++) {
			for (int y = 0; y < yDim; y++) {
				GameObject background = (GameObject)Instantiate (backgroundPrefab, GetWorldPosition( x, y), Quaternion.identity);
				background.transform.parent = transform;
			}
		}

		pieces = new GamePiece[xDim, yDim];
		//add game pieces to grid
		for (int x = 0; x < xDim; x++) {
			for (int y = 0; y < yDim; y++) {
				SpawnNewPiece (x, y, PieceType.EMPTY);
			}
		}


		Destroy (pieces [1, 4].gameObject);
		SpawnNewPiece (1, 4, PieceType.BUBBLE);

		Destroy (pieces [2, 4].gameObject);
		SpawnNewPiece (2, 4, PieceType.BUBBLE);

		Destroy (pieces [3, 4].gameObject);
		SpawnNewPiece (3, 4, PieceType.BUBBLE);

		Destroy (pieces [5, 4].gameObject);
		SpawnNewPiece (5, 4, PieceType.BUBBLE);

		Destroy (pieces [6, 4].gameObject);
		SpawnNewPiece (6, 4, PieceType.BUBBLE);

		Destroy (pieces [7, 4].gameObject);
		SpawnNewPiece (7, 4, PieceType.BUBBLE);

		Destroy (pieces [4, 0].gameObject);
		SpawnNewPiece (4, 0, PieceType.BUBBLE);

		StartCoroutine(Fill ());
			
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Vector2 GetWorldPosition(int x, int y)
	{
		return new Vector2 (transform.position.x - xDim / 2.0f + x, transform.position.y + xDim / 2.0f - y);
	}

	public GamePiece SpawnNewPiece(int x, int y, PieceType type)
	{
		GameObject newPiece = (GameObject)Instantiate (piecePrefabDict [type], GetWorldPosition (x, y), Quaternion.identity);
		newPiece.transform.parent = transform;

		pieces [x, y] = newPiece.GetComponent<GamePiece> ();
		pieces [x, y].Init (x, y, this, type);

		return pieces [x, y];
	}

	public IEnumerator Fill()
	{
		while (FillStep ()) {
			inverse = !inverse;
			yield return new WaitForSeconds (FillTime);
		}
	}

	public bool FillStep()
	{
		bool movedPiece = false;

		for (int y = yDim-2; y >= 0; y--)
		{
			for (int loopX = 0; loopX < xDim; loopX++)
			{
				int x = loopX;

				if (inverse) {
					x = xDim - 1 - loopX;
				}

				GamePiece piece = pieces [x, y];

				if (piece.IsMovable ())
				{
					GamePiece pieceBelow = pieces [x, y + 1];

					if (pieceBelow.Type == PieceType.EMPTY) {
						Destroy (pieceBelow.gameObject);
						piece.MovableComponent.Move (x, y + 1, FillTime);
						pieces [x, y + 1] = piece;
						SpawnNewPiece (x, y, PieceType.EMPTY);
						movedPiece = true;
					} else {
						for (int diag = -1; diag <= 1; diag++)
						{
							if (diag != 0)
							{
								int diagX = x + diag;

								if (inverse)
								{
									diagX = x - diag;
								}

								if (diagX >= 0 && diagX < xDim)
								{
									GamePiece diagonalPiece = pieces [diagX, y + 1];

									if (diagonalPiece.Type == PieceType.EMPTY)
									{
										bool hasPieceAbove = true;

										for (int aboveY = y; aboveY >= 0; aboveY--)
										{
											GamePiece pieceAbove = pieces [diagX, aboveY];

											if (pieceAbove.IsMovable ())
											{
												break;
											}
											else if(!pieceAbove.IsMovable() && pieceAbove.Type != PieceType.EMPTY)
											{
												hasPieceAbove = false;
												break;
											}
										}

										if (!hasPieceAbove)
										{
											Destroy (diagonalPiece.gameObject);
											piece.MovableComponent.Move (diagX, y + 1, FillTime);
											pieces [diagX, y + 1] = piece;
											SpawnNewPiece (x, y, PieceType.EMPTY);
											movedPiece = true;
											break;
										}
									} 
								}
							}
						}
					}
				}
			}
		}

		for (int x = 0; x < xDim; x++)
		{
			GamePiece pieceBelow = pieces [x, 0];

			if (pieceBelow.Type == PieceType.EMPTY)
			{
				Destroy (pieceBelow.gameObject);
				GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[PieceType.NORMAL], GetWorldPosition(x, -1), Quaternion.identity);
				newPiece.transform.parent = transform;

				pieces [x, 0] = newPiece.GetComponent<GamePiece> ();
				pieces [x, 0].Init (x, -1, this, PieceType.NORMAL);
				pieces [x, 0].MovableComponent.Move (x, 0, FillTime);
				pieces [x, 0].ColorComponent.SetColor ((ColorPiece.ColorType)Random.Range (0, pieces [x, 0].ColorComponent.NumColors));
				movedPiece = true;
			}
		}

		return movedPiece;
	}

	public bool IsAdjacent(GamePiece piece1, GamePiece piece2)
	{
		Debug.Log ("Piece 1: " + piece1.X + ", " + piece1.Y);
		Debug.Log ("Piece 2: " + piece2.X + ", " + piece2.Y);
		bool result = (piece1.X == piece2.X && (int)Mathf.Abs (piece1.Y - piece2.Y) == 1)
			|| (piece1.Y == piece2.Y && (int)Mathf.Abs (piece1.X - piece2.X) == 1);
				Debug.Log(result);
		return(result);
	}

	public void SwapPieces(GamePiece piece1, GamePiece piece2)
	{
		if (piece1.IsMovable () && piece2.IsMovable ()) {
			pieces [piece1.X, piece1.Y] = piece2;
			pieces [piece2.X, piece2.Y] = piece1;

			int piece1X = piece1.X;
			int piece1Y = piece1.Y;

			piece1.MovableComponent.Move (piece2.X, piece2.Y, FillTime);
			piece2.MovableComponent.Move (piece1X, piece1Y, FillTime);
		}
	}

	public void PressPiece(GamePiece piece)
	{
		pressedPiece = piece;
	}

	public void EnterPiece(GamePiece piece)
	{
		enteredPiece = piece;
	}

	public void ReleasePiece()
	{
		
		if (IsAdjacent (pressedPiece, enteredPiece)) {
			Debug.Log ("Swap pieces");
			SwapPieces (pressedPiece, enteredPiece);
		}
	}
}
