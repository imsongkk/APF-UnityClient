using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameScene2 : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI scoreText;
	[SerializeField] List<GameObject> walls = new List<GameObject>();

	AudioSource source;
	GameObject uiCamera;
	int idx = 0;
	float score = 0;

	private void Start()
	{
		uiCamera = GameObject.Find("OverUICamera");
		uiCamera?.SetActive(false);

		source = GetComponent<AudioSource>();

		scoreText.text = $"Score : {string.Format("{0:0.00}", score)}";
	}

	bool needWall = true;
	bool soundPlayed = false;

	public void Update()
    {
		if(needWall)
        {
			idx = Random.Range(0, walls.Count - 1);
			walls[idx].transform.position = new Vector3(0, 0, 9);
			needWall = false;
		}

		score += Time.deltaTime;
		scoreText.text = $"Score : {string.Format("{0:0.00}", score)}";
		walls[idx].transform.position += new Vector3(0, 0, -3f) * Time.deltaTime;

		if (walls[idx].transform.position.z <= 0f && !soundPlayed)
		{
			source.PlayOneShot(source.clip);
			soundPlayed = true;
		}

		if (walls[idx].transform.position.z <= -2f)
        {
			walls[idx].transform.position = new Vector3(0, 0, -15);
			needWall = true;
			soundPlayed = false;
        }
	}

    public void MainMenu()
	{
		uiCamera.SetActive(true);
		SceneManager.LoadScene("MainScene");
	}
}
