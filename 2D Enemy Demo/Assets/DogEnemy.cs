using System;
using UnityEngine;
using System.Collections;

public class DogEnemy : MonoBehaviour {
	
	public static int Health = 30;
	private Rigidbody2D rb;
	
	void Start()
	{
		rb = GetComponent<Rigidbody2D> ();
	}
	
	Punch yes = new Punch();
	
	public void DamageEnemy (int damage)
	{
		int power = 1;
		
		if (yes.getFlipPunch() == true)
		{
			power = -1;
		}
		else
		{
			power = 1;
		}
		
		Health -= damage;
		if (Health > 0) {
			GameMaster.ShowHarm (gameObject);
		}
		rb.AddForce (new Vector2(3500f * power, 0f));
		if (Health <= 0)
		{
			GameMaster.KillEnemy(gameObject);
		}
	}
}
