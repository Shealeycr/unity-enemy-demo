using UnityEngine;
using Pathfinding;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using System;

[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (Seeker))]
public class BossAI: MonoBehaviour {
	
	public Transform target;
	
	public float updateRate = 2f;
	
	private Seeker seeker;
	private Rigidbody2D rb;
	
	public Path path;
	
	public float speed = 300f;
	
	[HideInInspector]
	public bool pathIsEnded = false;
	
	public float nextWaypointDistance = 3;
	
	private bool searchingForPlayer = false;
	
	private int currentWaypoint = 0;
	
	public float distanceToPlayer = 2;
	private bool facingLeft = true;
	
	private bool attacking = false;
	public static bool readyToAttack = false;
	private bool particlesSpawned = false;

	public Transform AngryParticles;
	public Transform SuperNova;
	private GameObject HealthBar;

	public AudioSource Sound;

	float HealthScale = 1f;
	
	void Start ()
	{
		StartCoroutine(Attack());
		seeker = GetComponent<Seeker> ();
		rb = GetComponent<Rigidbody2D> ();
		
		if (target == null)
		{
			if (!searchingForPlayer)
			{
				searchingForPlayer = true;
				StartCoroutine(SearchForPlayer());
			}
			return;
		}

		HealthBar = GameObject.Find ("BossHealth");
		if (HealthBar == null)
		{
			Debug.Log ("No Health FOOLS!!!");
		}
		
		seeker.StartPath (transform.position, target.position, OnPathComplete);
		
		StartCoroutine(UpdatePath());
	}

	void Update()
	{
		GameObject dust = GameObject.Find ("BossGlow(Clone)");
		if (dust == null) {
			return;
		}
		dust.transform.position = transform.position;

		HealthScale = (BossEnemy.Health / 150f);
		if (HealthScale <= 0) {
			HealthScale = 0;
		}
		HealthBar.transform.localScale = new Vector3 (HealthScale, .2f, 1);
	}
	
	IEnumerator SearchForPlayer()
	{
		GameObject sResult = GameObject.FindGameObjectWithTag ("Player");
		if (sResult == null) {
			yield return new WaitForSeconds (0.5f);
			StartCoroutine (SearchForPlayer ());
		} 
		else
		{
			searchingForPlayer = false;
			target = sResult.transform;
			StartCoroutine(UpdatePath());
			yield return null;
		}
	}
	
	IEnumerator UpdatePath()
	{
		if (target == null)
		{
			if (!searchingForPlayer)
			{
				searchingForPlayer = true;
				StartCoroutine(SearchForPlayer());
			}
			yield return null;
		}
		
		seeker.StartPath (transform.position, target.position, OnPathComplete);
		
		yield return new WaitForSeconds (1F / updateRate);
		StartCoroutine (UpdatePath ());
	}
	
	public void OnPathComplete (Path p)
	{
		//Debug.Log ("We got a path, Did it have an error? " + p.error);
		if (!p.error)
		{
			path = p;
			currentWaypoint = 0;
		}
	}
	
	void FixedUpdate()
	{		
		float h = transform.InverseTransformDirection (rb.velocity).x;
		
		if (target == null)
		{
			if (!searchingForPlayer)
			{
				searchingForPlayer = true;
				StartCoroutine(SearchForPlayer());
			}
			return;
		}
		
		if (h > 0 && facingLeft)
		{
			Flip();
		} 
		else if (h < 0 && !facingLeft)
		{
			Flip();
		}
		
		if (path == null)
		{
			return;
		}
		
		if (currentWaypoint >= path.vectorPath.Count)
		{
			if (pathIsEnded)
			{
				return;
			}
			
			pathIsEnded = true;
			return;
		}
		pathIsEnded = false;
		
		Vector3 dir = (path.vectorPath [currentWaypoint] - transform.position).normalized;
		dir *= speed * Time.fixedDeltaTime;
		
		GameObject Ninja = GameObject.FindGameObjectWithTag ("Player");
		float AttackDist = Vector3.Distance ((Ninja.transform.position), transform.position);
		if (AttackDist <= distanceToPlayer && attacking == false) {
			rb.velocity = dir;
			readyToAttack = true;
		}

		if (AttackDist <= 2.5f && particlesSpawned == false) {
			Instantiate (AngryParticles, transform.position, transform.rotation);
			particlesSpawned = true;
		}
		
		float dist = Vector3.Distance (transform.position, path.vectorPath[currentWaypoint]);
		if(dist < nextWaypointDistance)
		{
			currentWaypoint++;
			return;
		}
	}
	
	IEnumerator Attack()
	{
		if (readyToAttack == true) {
			Sound.Play();
			yield return new WaitForSeconds (4.3f);

			GameObject Ninja = GameObject.FindGameObjectWithTag ("Player");
			Vector3 difference = Ninja.transform.position - transform.position;
			difference.Normalize();

			float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
			Quaternion LocalRotation = Quaternion.Euler(0f, 0f, rotZ + 270);

			Instantiate (SuperNova, transform.position, LocalRotation);
			attacking = true;
			yield return new WaitForSeconds (3f);
			Destroy (GameObject.Find("BossAttackParticles(Clone)"));
			attacking = false;
			yield return new WaitForSeconds (2f);
		} 
		else {
			yield return new WaitForSeconds (10f);
		}
		StartCoroutine(Attack());
	}
	
	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		facingLeft = !facingLeft;
		
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}