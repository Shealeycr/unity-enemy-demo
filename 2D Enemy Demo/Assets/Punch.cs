using UnityEngine;
using System.Collections;

public class Punch : MonoBehaviour {

	public int Damage = 10;
	public LayerMask ToHit;

	public static bool flipPunch;
	int flip = 1;

	Transform firePoint;

	// Use this for initialization
	void Awake () {

		firePoint = transform.Find ("PunchBlock");
		if (firePoint == null)
		{
			Debug.Log ("No Punching Found");
		}
		flipPunch = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetButtonDown ("Fire1")) 
		{
			Shoot();
		}
	}

	public void setFlipPunch(bool aPunch)
	{
		flipPunch = aPunch;
	}

	public bool getFlipPunch()
	{
		return flipPunch;
	}

	void Shoot ()
	{
		if (flipPunch == true)
		{
			flip = -1;
		}

		Vector2 punchTarget = new Vector2 ((firePoint.position.x + 20) * flip, firePoint.position.y);
		Vector2 firePointPosition = new Vector2 (firePoint.position.x, firePoint.position.y);

		RaycastHit2D hit = Physics2D.Raycast (firePointPosition, (punchTarget - firePointPosition), 1f, ToHit);
		Debug.DrawLine (firePointPosition, (punchTarget - firePointPosition));

		if (hit.collider != null)
		{
			Debug.DrawLine (firePointPosition, hit.point, Color.red);

			Enemy enemy = hit.collider.GetComponent<Enemy>();
			if (enemy != null)
			{
				enemy.DamageEnemy(Damage);
				Debug.Log ("You hit " + hit.collider.name + " and did " + Damage + " damage.");
			}

			DogEnemy enemyToo = hit.collider.GetComponent<DogEnemy>();
			if (enemyToo != null)
			{
				enemyToo.DamageEnemy(Damage);
				Debug.Log ("You hit " + hit.collider.name + " and did " + Damage + " damage.");
			}

			BossEnemy enemyAlso = hit.collider.GetComponent<BossEnemy>();
			if (enemyAlso != null)
			{
				enemyAlso.DamageEnemy(Damage);
				Debug.Log ("You hit " + hit.collider.name + " and did " + Damage + " damage.");
			}
		}

		flip = 1;
	}
}
