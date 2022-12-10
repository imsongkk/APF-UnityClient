using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] GameObject characterCamera;
    [SerializeField] GameObject character;
    
    void Start()
    {
        DontDestroyOnLoad(character);
        DontDestroyOnLoad(characterCamera);
    }
}
