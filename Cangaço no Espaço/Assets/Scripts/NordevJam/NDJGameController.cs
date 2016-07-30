﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class NDJGameController : MonoBehaviour
{
	public float timeToShowBoss = 5f;
	public float timeToPlayer = 5f;
	public HUDController hudController;
	public GameObject grayscaleCamera;

	private Transform player;
	private Transform boss;
	private Transform cameraTarget;
	private NPCController playerController;
	private NPCController bossController;
	private int deathCount;
	bool gameStarted = false;
	private bool gameReady = false;

	void Start()
	{
		grayscaleCamera.SetActive(false);
		StartCoroutine(LookAtBoss());
	}

	IEnumerator LookAtBoss()
	{
		yield return null;
		yield return null;
		while (boss == null)
		{
			boss = GameObject.FindGameObjectWithTag("Boss").transform;
			yield return null;
		}
		while (player == null)
		{
			player = GameObject.FindGameObjectWithTag("Player").transform;
			yield return null;
		}
		while (cameraTarget == null)
		{
			cameraTarget = GameObject.FindGameObjectWithTag("CameraTarget").transform;
			yield return null;
		}
		bossController = boss.GetComponent<NPCController>();
		bossController.gameController = this;
		playerController = player.GetComponent<NPCController>();
		playerController.gameController = this;
		playerController.canMove = false;
		ChangeHealth(playerController.myBodyInfo.maxLife, playerController.myBodyInfo.maxLife);
		ChangeStrength(playerController.myBodyInfo.strength);
		ChangeSpeed(playerController.myBodyInfo.speed);
		cameraTarget.position = boss.position;
		gameReady = true;
		yield return null;
		//yield return new WaitForSeconds(timeToShowBoss);
	}

	IEnumerator LookAtPlayer()
	{

		float elapsedTime = 0.0f;
		while (Vector3.Distance(cameraTarget.position, player.position) > 0.1f)
		{
			cameraTarget.position = Vector3.Lerp(boss.position, player.position, elapsedTime / timeToPlayer);
			yield return null;
			elapsedTime += Time.deltaTime;
		}
		cameraTarget.localPosition = Vector3.zero;
		playerController.canMove = true;
	}


	void Update()
	{
		if (!gameStarted && gameReady && Input.anyKey)
		{
			gameStarted = true;
			StartCoroutine(LookAtPlayer());
		}
	}

	public void ChangeHealth(int currentHealth, int maxHealth)
	{
		hudController.SetLife(currentHealth, maxHealth);
	}

	public void ChangeSpeed(int currentSpeed)
	{
		hudController.SetSpeed(currentSpeed);
	}

	public void ChangeStrength(int currentStrength)
	{
		hudController.SetStrength(currentStrength);
	}

	public void BossDefeated()
	{
		deathCount = 0;
		playerController.myBodyInfo.SaveBody();
		ActivateGrayscale();
		Invoke("RestartScene", 5f);
	}

	public void PlayerDefeated()
	{
		deathCount++;
		ActivateGrayscale();
		Invoke("RestartScene", 5f);
	}

	void ActivateGrayscale()
	{
		hudController.gameObject.SetActive(false);
		grayscaleCamera.SetActive(true);
	}

	void RestartScene()
	{
		SceneManager.LoadScene(1);
	}
}
