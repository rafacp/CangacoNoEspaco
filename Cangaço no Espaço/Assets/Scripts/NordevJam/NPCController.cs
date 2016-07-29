﻿using System;
using UnityEngine;
using System.Collections;

public class NPCController : MonoBehaviour
{
	public bool NPC = true;
	public Animator myAnimator;

	public tk2dSprite bodySprite;
	public tk2dSprite headSprite;
	public tk2dSprite faceSprite;
	public tk2dSprite shirtSprite;
	public tk2dSprite pantSprite;
	public tk2dSprite feetSprite;
	public tk2dSprite knifeSprite;

	//public tk2dSpriteCollection bodyCollection;
	//public tk2dSpriteCollection headCollection;
	//public tk2dSpriteCollection faceCollection;
	//public tk2dSpriteCollection shirtCollection;
	//public tk2dSpriteCollection pantCollection;
	//public tk2dSpriteCollection knifeCollection;

	public int bodyCount = 4;
	public int headCount = 9;
	public int faceCount = 5;
	public int shirtCount = 8;
	public int pantCount = 5;
	public int feetCount = 1;
	public int knifeCount = 6;

	public float moveSpeed = 0.01f;

	public BoxCollider2D peixeiraCollider;

	public BodyInfo myBodyInfo;

	public Transform target;

	private Vector2 moveVector;
	private bool walking;
	private bool facingLeft = true;
	private int currentStrength;

	public float minDistance = 1f;
	public float maxDistance = 5f;

	public int startStrength = 1;
	public int maxLife = 5;
	private int currentLife;

	public Vector2 GetInput()
	{
		return moveVector;
	}

	private float distance;
	private bool dead;

	void Start()
	{
		if (NPC)
		{
			target = GameObject.FindGameObjectWithTag("Player").transform;
		}
		currentLife = maxLife;
		currentStrength = startStrength;
	}



	void Update()
	{
		if (dead)
		{
			moveVector = Vector2.zero;
		}
		else if (!NPC)
		{
			moveVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

			if (Input.GetButtonDown("Fire1"))
			{
				myAnimator.SetTrigger("Attack");
			}

			if (Input.GetButtonDown("Fire2"))
			{
				RandomizeMyClothing();
			}
		}
		else
		{
			distance = Vector3.Distance(transform.position, target.position);
			//Vector3 direction = target.position - transform.position;
			//Debug.DrawRay(transform.position, direction);
			//RaycastHit2D hit = Physics2D.Raycast(transform.position, direction);
			//Debug.Log("Hit: " + hit.collider.name);
			//if (hit.collider.tag != "Wall")
			//{
			if (distance < maxDistance)
			{
				if (distance > minDistance)
				{
					moveVector = Vector3.Normalize(target.position - transform.position);
				}
				else
				{
					if (!myAnimator.GetCurrentAnimatorStateInfo(0).IsName("NPC Attack"))
					{
						myAnimator.SetTrigger("Attack");
					}
				}
			}
			//}
			else
			{
				moveVector = Vector2.zero;
			}
		}

		if (facingLeft && moveVector.x > 0 || !facingLeft && moveVector.x < 0)
		{
			Flip();
		}

		walking = moveVector != Vector2.zero;

		myAnimator.SetBool("Walking", walking);


		if (walking)
		{
			Move();
		}
	}

	void Move()
	{
		transform.Translate(moveVector * moveSpeed);
	}

	void Flip()
	{
		facingLeft = !facingLeft;
		Vector3 originalScale = transform.localScale;
		transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
	}

	public void RandomizeMyClothing()
	{
		int body = UnityEngine.Random.Range(0, bodyCount);
		int head = UnityEngine.Random.Range(-1, headCount);
		int face = UnityEngine.Random.Range(-1, faceCount);
		int shirt = UnityEngine.Random.Range(-1, shirtCount);
		int pant = UnityEngine.Random.Range(0, pantCount);
		int feet = UnityEngine.Random.Range(-1, feetCount);
		int knife = UnityEngine.Random.Range(0, bodyCount);

		myBodyInfo = new BodyInfo(body, head, face, shirt, pant, feet, knife);

		DressPlayer();
	}

	public void DressPlayer(BodyInfo bodyInfo)
	{
		myBodyInfo = bodyInfo;
		DressPlayer();
	}

	private void DressPlayer()
	{
		bodySprite.spriteId = myBodyInfo.body;

		if (myBodyInfo.head >= 0)
		{
			headSprite.gameObject.SetActive(true);
			headSprite.spriteId = myBodyInfo.head;
		}
		else
		{
			headSprite.gameObject.SetActive(false);
		}

		if (myBodyInfo.face >= 0)
		{
			faceSprite.gameObject.SetActive(true);
			faceSprite.spriteId = myBodyInfo.head;
		}
		else
		{
			faceSprite.gameObject.SetActive(false);
		}

		if (myBodyInfo.shirt >= 0)
		{
			shirtSprite.gameObject.SetActive(true);
			shirtSprite.spriteId = myBodyInfo.head;
		}
		else
		{
			shirtSprite.gameObject.SetActive(false);
		}

		pantSprite.spriteId = myBodyInfo.pant;

		if (myBodyInfo.feet >= 0)
		{
			feetSprite.gameObject.SetActive(true);
			feetSprite.spriteId = myBodyInfo.feet;
		}
		else
		{
			feetSprite.gameObject.SetActive(false);
		}

		knifeSprite.spriteId = myBodyInfo.knife;

		RescaleTk2dSpriteCollider(knifeSprite);
	}

	void RescaleTk2dSpriteCollider(tk2dSprite target)
	{
		Vector2 S = target.GetBounds().size;
		BoxCollider2D targetCollider = target.GetComponent<BoxCollider2D>();
		targetCollider.size = S;
		targetCollider.offset = Vector2.zero;// = new Vector2((S.x/2f), (S.y/2f));
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("NPC Attack"))
		{
			if (peixeiraCollider.IsTouching(other))
			{
				if (other.tag == "Punchable")
				{
					other.transform.parent.parent.GetComponent<NPCController>().TakeDamage(currentStrength);
				}
				else if (other.tag == "Tall Grass")
				{
					Destroy(other.gameObject);
				}
			}
		}
	}

	public void TakeDamage(int strength)
	{
		Debug.Log(name + " took " + strength + " damage");
		currentLife -= strength;
		if(currentLife <= 0)
		{
			Die();
		}
	}

	private void Die()
	{
		dead = true;
		myAnimator.SetTrigger("Dead");
	}
}

public struct BodyInfo
{
	public int body;
	public int head;
	public int face;
	public int shirt;
	public int pant;
	public int feet;
	public int knife;

	public BodyInfo(int body, int head, int face, int shirt, int pant, int feet, int knife)
	{
		this.body = body;
		this.head = head;
		this.face = face;
		this.shirt = shirt;
		this.pant = pant;
		this.feet = feet;
		this.knife = knife;
	}

	public void ChangeKnife(int knife)
	{
		this.knife = knife;
	}
}
