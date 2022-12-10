using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScene : MonoBehaviour
{
    [SerializeField] Button startButton;
	[SerializeField] Button startButton2;
	[SerializeField] Button endButton;

	private void Awake()
	{
		startButton.onClick.AddListener(GameStart);
		startButton2.onClick.AddListener(GameStart2);
		endButton.onClick.AddListener(Exit);
	}

    private void Exit()
    {
		Application.Quit();
    }

    public void GameStart()
	{
		SceneManager.LoadScene("InGameScene");
	}

	public void GameStart2()
	{
		SceneManager.LoadScene("InGameScene2");
	}
}
