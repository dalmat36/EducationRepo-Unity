using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
	public GameObject prefab;
	private Vector3 spawnPoint; 
/*	void Start()
	{
		for (int i = 0; i < 5; i++)
			Instantiate(prefab, new Vector3(i * 1.0f, 0, 0), Quaternion.identity);
	}*/
	void Update()
	{
		InvokeRepeating ("SpawnBubble", 0.5f, 3.0f);
	}

	void SpawnBubble()
	{
		spawnPoint = new Vector3(Random.Range(-5,5),0); 
		Instantiate (prefab, spawnPoint, Quaternion.identity);
		CancelInvoke ();
	}


}

