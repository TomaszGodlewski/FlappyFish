using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class FlappyObstaclesSpawner : MonoBehaviour 
{	
	float x = 3;
	float y = 0;
	float gapHeight = 10;

	const int MIN_X = 5;
	const int MAX_X = 7;
	const int MIN_Y = 0;
	const int MAX_Y = 4;
	const int MIN_GAP_HEIGHT = 6;
	const int MAX_GAP_HEIGHT = 8;

	const int OBSTACLE_LIMIT = 10;

	static float obstacleX;
	static bool destroyFlag = false;
		
	public GameObject obstaclePrefab;

	List<GameObject> spawnedObstacles = new List<GameObject>();

	void Awake() 
	{
		Spawn(x, y, gapHeight);
		obstacleX = spawnedObstacles [0].transform.position.x;
	}

	void Update()
	{			
		if (spawnedObstacles.Count < OBSTACLE_LIMIT) CreateObstacle ();
		if (destroyFlag) 
		{
			DestroyObstacle();
			setDestroyFlag(false);
		}
	}

	void Spawn(float x, float y, float gapHeight)
	{
		GameObject spawned = Instantiate(obstaclePrefab);
        spawned.transform.parent = transform;
		spawned.transform.position = new Vector3(x, y, 0);
		spawnedObstacles.Add(spawned);

        Transform bottomTransform = spawned.transform.Find("Bottom");
        Transform topTransform = spawned.transform.Find("Top");
        float bottomY = -gapHeight/2;
        float topY = +gapHeight/2;
        bottomTransform.localPosition = Vector3.up * bottomY;
        topTransform.localPosition = Vector3.up * topY;
    }

	void CreateObstacle()
	{
		x += 5;//UnityEngine.Random.Range (MIN_X, MAX_X);
		y = UnityEngine.Random.Range (MIN_Y, MAX_Y);
		gapHeight = 5;//UnityEngine.Random.Range (MIN_GAP_HEIGHT, MAX_GAP_HEIGHT);
		Spawn(x, y, gapHeight);	
	}


	void DestroyObstacle()
	{
		Destroy(spawnedObstacles[0]);
		spawnedObstacles.RemoveAt (0);
		obstacleX = spawnedObstacles [1].transform.position.x;
	}

	public static void setDestroyFlag(bool flag)
	{
		destroyFlag = flag;
	}

	public static float getObstacleX()
	{
		return obstacleX;
	}
}