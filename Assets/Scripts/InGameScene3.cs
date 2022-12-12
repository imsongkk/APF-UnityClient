using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameScene3 : MonoBehaviour
{
	// Parameters
	const int MAX_THING_COUNT = 8;
	const float MAX_SPEED = 10f;
	float speed = 3f;
	float speedIncr = 0.25f;
	float genProb = 0.02f;

	// Do not modify
	GameObject uiCamera;
	GameObject[] things;
	Rigidbody[] things_rb;
	AudioSource[] sounds;
	Text scoreText;
	int score = 0;
	public bool isSceneReady = false;

	private void Start()
	{
		uiCamera = GameObject.Find("OverUICamera");
		uiCamera?.SetActive(false);
		scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
		sounds = GetComponents<AudioSource>();

		things = GameObject.FindGameObjectsWithTag("Thing");
		things_rb = new Rigidbody[13];
		for (int i = 0; i < 13; i++)
        {
			things[i].SetActive(false);
			things[i].transform.position = new Vector3(0, -10, 0);
			things_rb[i] = things[i].GetComponent<Rigidbody>();
		}

		isSceneReady = true;
	}

	public void FixedUpdate()
	{
		// Update UI
		scoreText.text = score.ToString();

		// Generate things
		int activeCount = 0;
		foreach (GameObject thing in things) if (thing.activeSelf) activeCount++;
		if (activeCount == 0) GenerateThing(); 
		else if (activeCount < MAX_THING_COUNT)
		{
			if (Random.Range(0f,1f) <= genProb) GenerateThing();
		}

		// Move or delete things on screen
		for (int i = 0; i < 13; i++)
        {
			if (!things[i].activeSelf) continue;
			if (things[i].transform.position.z < -2f) DestroyThing(things[i]);
			float dz = -speed * Time.deltaTime;
			things_rb[i].MovePosition(things[i].transform.position + new Vector3(0, 0, dz));
		}
	}

	public void MainMenu()
	{
		uiCamera.SetActive(true);
		SceneManager.LoadScene("MainScene");
	}

	private void GenerateThing()
    {
		// -3 <= x <= 3, 0.5 <= y <= 3
		// z: from 14 to -2 
		// Total 13 things (thing0 ~ thing12)
		// Choose one of inactive things
		GameObject thing;
		do {
			thing = things[Random.Range(0, 13)];
		} while (thing.activeSelf);
		thing.SetActive(true);

		float x = Random.Range(-3f, 3f);
		float y = Random.Range(0.5f, 3f);
		float z = 14f;
		thing.transform.position = new Vector3(x, y, z);

		x = Random.Range(0, 360);
		y = Random.Range(0, 360);
		z = Random.Range(0, 360);
		thing.transform.rotation = Quaternion.Euler(x, y, z);
		sounds[1].Play();
	}

	private void DestroyThing(GameObject thing)
    {
		thing.transform.position = new Vector3(0, -10, 0);
		thing.SetActive(false);
		score++;
		if (speed < MAX_SPEED) speed += speedIncr;
		sounds[0].Play();
    }
}
