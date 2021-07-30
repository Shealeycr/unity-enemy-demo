using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	public static float Health;

	void Start()
	{
		Health = 100;
	}

	void Update ()
	{
		if (Health <= 0) 
		{
			StartCoroutine(GameMaster.KillNinja(gameObject));
			BossAI.readyToAttack = false;
		}
	}

	public void DamagePlayer(int damage)
	{
		Health -= damage;
	}
}