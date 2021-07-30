using UnityEngine;
using System.Collections;

public class LavaKill : MonoBehaviour {

	private GameObject Dog;

	void Awake ()
	{
		Dog = GameObject.Find("RobotDog");
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player") {
			Player.Health = 0;
		}
		if (other.tag == "Enemy") {
			GameMaster.KillEnemy(Dog);
		}
	}
}