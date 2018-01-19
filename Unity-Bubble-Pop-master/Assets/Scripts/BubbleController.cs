using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class BubbleController : MonoBehaviour {
	public GameObject track;
	public AudioClip PopSound;
	 AudioSource audio;
	private float moveSpeed = 3;
	// Use this for initialization
	void Start () {
		audio =  GetComponent<AudioSource> ();
	}

	// Update is called once per frame
	void Update () {
		float move = moveSpeed * Time.deltaTime;
		//transform.position = Vector3.MoveTowards (transform.position, track.transform.position, move);
		transform.Translate(0,move,0);
	}

	public void OnMouseDown ()
	{
		PopBubble ();
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.name == "Destination") {
			PopBubble ();
		}
	}

 void PopBubble(){
		audio.PlayOneShot(PopSound, 0.75F);
		Destroy (gameObject, PopSound.length);
	}
}
