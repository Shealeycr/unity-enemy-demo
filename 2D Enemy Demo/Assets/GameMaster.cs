using UnityEngine;
using System.Collections;

public class GameMaster : MonoBehaviour {

	public static GameMaster gm;

	void Start()
	{
		if (gm == null) {
			gm = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameMaster>();
		}
	}

	static GameObject Location;
	public Transform NinjaPrefab;
	public Transform spawnPoint;
	public Transform SpawnEffect;
	public Transform DeathParticles;
	public Transform HarmBurst;
	public AudioSource Clap;
	private GameObject HealthBar;
	float playerHealthScale = 1;

	static Vector3 LocPos;
	static Quaternion LocRot;

	Punch yes = new Punch();

	void Awake()
	{
		Location = GameObject.Find("Ninja");
		if (Location == null)
		{
			Debug.Log ("No Player Found");
		}
		HealthBar = GameObject.Find ("GreenBar");
		if (HealthBar == null)
		{
			Debug.Log ("No Health FOOLS!!!");
		}
	}

	void Update()
	{
		playerHealthScale = (Player.Health / 100f);
		if (playerHealthScale <= 0) {
			playerHealthScale = 0;
		}
		HealthBar.transform.localScale = new Vector3 (playerHealthScale, 1, 1);
	}


	IEnumerator RespawnPlayer()
	{
		yield return new WaitForSeconds(2);
		Instantiate (NinjaPrefab, spawnPoint.position, spawnPoint.rotation);
		Instantiate (SpawnEffect, spawnPoint.position, spawnPoint.rotation);

		Location = GameObject.Find("Ninja(Clone)");
	}

	public static IEnumerator KillNinja(GameObject Person)
	{
		LocPos = Location.transform.position;
		LocRot = Location.transform.rotation;

		yield return new WaitForSeconds(.1f);
		Destroy(Person.gameObject);
		gm.Clap.Play ();
		gm.StartCoroutine(gm.RespawnPlayer());

		if (gm.yes.getFlipPunch () == false) {
			gm.yes.setFlipPunch (true);
		}

		gm.StartCoroutine(gm.DestroyParticles());
	}

	public static void KillEnemy(GameObject Enemy)
	{
		LocPos = Enemy.transform.position;
		LocRot = Enemy.transform.rotation;

		Destroy (Enemy.gameObject);
		gm.Clap.Play ();
		gm.StartCoroutine(gm.DestroyParticles());
	}

	IEnumerator DestroyParticles()
	{
		Instantiate (DeathParticles, LocPos, LocRot);

		yield return new WaitForSeconds(5);
		Destroy(GameObject.Find("SpawnParticles(Clone)"));
		Destroy(GameObject.Find("DeathParticles(Clone)"));
	}

	public static void ShowHarm(GameObject Enemy)
	{
		LocPos = Enemy.transform.position;
		LocRot = Enemy.transform.rotation;

		gm.StartCoroutine(gm.HideHarm());
	}

	IEnumerator HideHarm()
	{
		Instantiate (HarmBurst, LocPos, LocRot);

		yield return new WaitForSeconds (.11f);

		Destroy (GameObject.Find ("HarmBurst(Clone)"));
	}
}