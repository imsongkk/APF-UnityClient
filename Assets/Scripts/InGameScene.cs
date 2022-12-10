using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameScene : MonoBehaviour
{
	[SerializeField] Button backButton;
	[SerializeField] TextMeshProUGUI scoreText;
	[SerializeField] List<GameObject> walls = new List<GameObject>();

	GameObject uiCamera;
	int idx = 0;
	int score = 0;

	private void Start()
	{
		uiCamera = GameObject.Find("OverUICamera");
		uiCamera?.SetActive(false);

		scoreText.text = $"Score : {score}";

		backButton.onClick.AddListener(MainMenu);
	}

	bool needWall = true;

    public void Update()
    {
		if(needWall)
        {
			idx = Random.Range(0, walls.Count - 1);
			print(idx);
			walls[idx].transform.position = new Vector3(0, 0, 18);
			needWall = false;
		}

		walls[idx].transform.position += new Vector3(0, 0, -2f) * Time.deltaTime;

		if(walls[idx].transform.position.z <= -2f)
        {
			walls[idx].transform.position = new Vector3(0, 0, -15);
			score++;
			scoreText.text = $"Score : {score}";
			needWall = true;
        }
	}

    public void MainMenu()
	{
		uiCamera.SetActive(true);
		SceneManager.LoadScene("MainScene");
	}
}
