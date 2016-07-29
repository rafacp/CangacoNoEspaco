﻿using UnityEngine;
using System.Collections;

public class BossEnemyInstantiator : MonoBehaviour
{
	public GameObject enemyPrefab;

	void Start()
	{
		GameObject enemy = (GameObject)Instantiate(enemyPrefab, transform.position, Quaternion.identity);
		NPCController npcController = enemy.GetComponent<NPCController>();
		
		npcController.LoadBossInfo();
		Destroy(this.gameObject);
	}
}
