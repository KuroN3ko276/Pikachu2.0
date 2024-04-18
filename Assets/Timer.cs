using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
	public TextMeshPro timerText;
	public float totalTime = 300f; // Total countdown time in seconds
	private float timeLeft;
	private bool isRunning;
	public GameObject gameFinishObject;

	void Start()
	{
		StartTimer();
		//blockHandler = transform.parent.GetComponent<BlockHandler>();
	}

	void Update()
	{
		if (isRunning)
		{
			timeLeft -= Time.deltaTime;
			if (timeLeft <= 0)
			{
				timeLeft = 0;
				gameFinishObject.SetActive(true);
				StopTimer();
			}
			DisplayTimeLeft();
		}
	}

	public void StartTimer()
	{
		timeLeft = totalTime;
		isRunning = true;
	}

	public void StopTimer()
	{
		isRunning = false;
	}

	void DisplayTimeLeft()
	{
		int minutes = Mathf.FloorToInt(timeLeft / 60);
		int seconds = Mathf.FloorToInt(timeLeft % 60);
		timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
	}
}
