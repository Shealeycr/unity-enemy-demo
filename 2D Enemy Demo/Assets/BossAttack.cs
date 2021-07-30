using UnityEngine;
using System.Collections;

public class BossAttack : MonoBehaviour {

	private int i = 0;

	Player player = new Player ();

	void OnTriggerEnter2D(Collider2D push)
	{
		if (push.tag == "Player")
		{
			StartCoroutine(Attack());
		}
	}

	IEnumerator Attack()
	{
		if (i <= 20) 
		{
			yield return new WaitForSeconds (.05f);
			player.DamagePlayer (2);
			i += 1;
			StartCoroutine (Attack ());
		}
	}
}
