using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScene : MonoBehaviour
{
    Button backButton;
    Text scoreText;
    HumanController character;
    // Start is called before the first frame update
    void Start()
    {
        character = GameObject.Find("Character").GetComponent<HumanController>();
        backButton = GameObject.Find("BackButton").GetComponent<Button>();
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        backButton.onClick.AddListener(GoToMainScene);
        scoreText.text = character.finalScore.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GoToMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }
}
