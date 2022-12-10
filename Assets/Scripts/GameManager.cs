using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static GameManager Instance { get { Init(); return instance; } }

    public static Loading Loading { get; set; }

    public static void Init()
    {
        if (instance == null)
        {
            GameObject go = GameObject.Find("GameManager");
            if (go == null)
            {
                go = new GameObject { name = "GameManager" };
                go.AddComponent<GameManager>();
            }
            instance = go.GetComponent<GameManager>();

            DontDestroyOnLoad(instance.gameObject);
        }
    }

    public static void LoadComplete()
	{
        Loading.gameObject.SetActive(false);
	}
}
