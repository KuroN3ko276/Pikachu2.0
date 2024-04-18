using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
	public TextMeshPro timerText;
	private float startTime;
	private bool isRunning;

	void Start()
	{
		StartTimer();
	}

	void Update()
	{
		if (isRunning)
		{
			float t = Time.time - startTime;
			string minutes = ((int)t / 60).ToString("00");
			string seconds = (t % 60).ToString("00");
			timerText.text = minutes + ":" + seconds;
		}
	}

	public void StartTimer()
	{
		startTime = Time.time;
		isRunning = true;
	}

	public void StopTimer()
	{
		isRunning = false;
	}

	public void ResetTimer()
	{
		timerText.text = "00:00";
		isRunning = false;
	}
}
