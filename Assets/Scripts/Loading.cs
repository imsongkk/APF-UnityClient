using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Loading : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI lodingText;

    float endAlpha = 0.0f;
    float startAlpha = 1.0f;

    float animationTime = 1.0f;
    float deltaTime = 0.0f;

	private void Start()
	{
        GameManager.Loading = this;
	}

	void Update()
    {
        if (animationTime < deltaTime)
        {
            deltaTime = 0.0f;
            float tempAlpha = startAlpha;
            startAlpha = endAlpha;
            endAlpha = tempAlpha;
        }

        deltaTime += Time.deltaTime;

        float alpha = Mathf.Lerp(startAlpha, endAlpha, deltaTime / animationTime);
        lodingText.color = new Color(lodingText.color.r, lodingText.color.g, lodingText.color.b, alpha);
    }
}
