using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScene : MonoBehaviour
{
    [SerializeField] Button startButton;

	private void Awake()
	{
		startButton.onClick.AddListener(GameStart);
	}

	public void GameStart()
	{
		SceneManager.LoadScene("InGameScene");
	}
}
