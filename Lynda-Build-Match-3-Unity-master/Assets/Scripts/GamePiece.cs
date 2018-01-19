using UnityEngine;
using System.Collections;

public class GamePiece : MonoBehaviour {

	private int x;
	private int y;

	public int X {
		get {
			return this.x;
		}
		set{
			if (IsMovable ()) {
				x = value;
			}
		}
	}

	public int Y {
		get {
			return this.y;
		}
		set{
			if (IsMovable ()) {
				y = value;
			}
		}
	}



	private Grid.PieceType type;

	public Grid.PieceType Type {
		get {
			return this.type;
		}
	}

	private Grid grid;

	public Grid GridRef {
		get {
			return this.grid;
		}
	}

	private MovablePiece movableComponent;
	public MovablePiece MovableComponent {
		get {
			return this.movableComponent;
		}
	}

	private ColorPiece colorComponent;
	public ColorPiece ColorComponent {
		get {
			return this.colorComponent;
		}
	}

	void Awake()
	{
		movableComponent = GetComponent<MovablePiece> ();
		colorComponent = GetComponent<ColorPiece> ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Init(int _x, int _y, Grid _grid, Grid.PieceType _type)
	{
		x = _x;
		y = _y;
		grid = _grid;
		type = _type;
	}

	void OnMouseEnter()
	{
        Debug.Log("In mouse enter");
		grid.EnterPiece (this);
	}

	void OnMouseDown()
	{
        Debug.Log("In mouse down");
        grid.PressPiece (this);
	}

	void OnMouseUp()
	{
        Debug.Log("In mouse up");
        grid.ReleasePiece ();
	}

	public bool IsMovable()
	{
		return movableComponent != null;
	}

	public bool IsColored()
	{
		return colorComponent != null;
	}
}
