using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameScene : MonoBehaviour
{
	[SerializeField] Button backButton;

	private void Awake()
	{
		backButton.onClick.AddListener(MainMenu);
	}

	public void MainMenu()
	{
		SceneManager.LoadScene("MainScene");
	}
}
