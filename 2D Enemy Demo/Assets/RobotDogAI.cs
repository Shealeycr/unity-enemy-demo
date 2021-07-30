using UnityEngine;
using Pathfinding;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using System;

[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (Seeker))]
public class RobotDogAI: MonoBehaviour {
	
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

	private Animator Anim;

	private GameObject HealthBar;
	float HealthScale = 1f;

	public AudioSource Bark;
	private bool BarkPlayed;

	void Start ()
	{
		BarkPlayed = false;
		Anim = GetComponent<Animator>();

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

		HealthBar = GameObject.Find ("DogHealth");
		if (HealthBar == null)
		{
			Debug.Log ("No Health FOOLS!!!");
		}
		
		seeker.StartPath (transform.position, target.position, OnPathComplete);
		
		StartCoroutine(UpdatePath());
	}

	void Update()
	{
		HealthScale = (DogEnemy.Health / 30f);
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
		float m = rb.velocity.magnitude;

		Anim.SetFloat("Speed", Mathf.Abs(m));

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
		if (AttackDist <= distanceToPlayer && attacking == false)
		{
			if (BarkPlayed == false)
			{
				Bark.Play ();
				StartCoroutine(setBarkFalse ());
			}
			rb.velocity = dir;
		}
		
		float dist = Vector3.Distance (transform.position, path.vectorPath[currentWaypoint]);
		if(dist < nextWaypointDistance)
		{
			currentWaypoint++;
			return;
		}
	}
	
	void OnTriggerEnter2D(Collider2D thing)
	{
		Player newPlayer = new Player ();

		if (thing.tag == "Player")
		{
			newPlayer.DamagePlayer (6);
			Anim.SetBool ("Attack", true);
			attacking = true;
			StartCoroutine (Attack ());
		}
	}

	IEnumerator Attack()
	{
		yield return new WaitForSeconds (1f);

		Anim.SetBool ("Attack", false);
		attacking = false;
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

	IEnumerator setBarkFalse()
	{
		yield return new WaitForSeconds(.1f);
		BarkPlayed = true;
	}
}

