using UnityEngine;
using System.Collections;

public class rotateArrow : MonoBehaviour {

	bool collisionOccurred;
	public GameObject arrowHead;
	public GameObject risingText;
	public GameObject bow;
	public AudioClip targetHit;
	float alpha;
	float   life_loss;
	public Color color = Color.white;

	void Start () {
		float duration = 2f;
		life_loss = 1f / duration;
		alpha = 1f;
	}

	void Update () {
		if (transform.GetComponent<Rigidbody>() != null) {
			if (GetComponent<Rigidbody>().velocity != Vector3.zero) {
				Vector3 vel = GetComponent<Rigidbody>().velocity;
				float angleZ = Mathf.Atan2(vel.y,vel.x)*Mathf.Rad2Deg;
				float angleY = Mathf.Atan2(vel.z,vel.x)*Mathf.Rad2Deg;
				transform.eulerAngles = new Vector3(0,-angleY,angleZ);
			}
		}

		if (collisionOccurred) {
			alpha -= Time.deltaTime * life_loss;
			GetComponent<Renderer>().material.color = new Color(color.r,color.g,color.b,alpha);
			
			if (alpha <= 0f) {
				bow.GetComponent<GamePlay>().createArrow(true);
				Destroy(gameObject);
			}
		}
	}



	void OnCollisionEnter(Collision other) {
		float y;
		int actScore = 0;

		if (collisionOccurred) {
			transform.position = new Vector3(other.transform.position.x,transform.position.y,transform.position.z);
			return;
		}

		if (other.transform.name == "Cube") {
			bow.GetComponent<GamePlay>().createArrow(false);
			Destroy(gameObject);
		}

		
		if (other.transform.name == "target") {
			GetComponent<AudioSource>().PlayOneShot(targetHit);
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			GetComponent<Rigidbody>().isKinematic = true;
			transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
			collisionOccurred = true;
			arrowHead.SetActive(false);
			y = other.contacts[0].point.y;
			y = y - other.transform.position.y;

			// we hit at least white...
			if (y < 1.48557f && y > -1.48691f)
				actScore = 10;
			// ... it could be black, too ...
			if (y < 1.36906f && y > -1.45483f)
				actScore = 20;
			// ... even blue is possible ...
			if (y < 0.9470826f && y > -1.021649f)
				actScore = 30;
			// ... or red ...
			if (y < 0.6095f && y > -0.760f)
				actScore = 40;
			// ... or gold !!!
			if (y < 0.34f && y > -0.53f)
				actScore = 50;

			// create a rising text for score display
			GameObject rt = (GameObject)Instantiate(risingText, new Vector3(0,0,0),Quaternion.identity);
			rt.transform.position = other.transform.position + new Vector3(-1,1,0);
			rt.transform.name = "rt";
			rt.GetComponent<TextMesh>().text= "+"+actScore;
			bow.GetComponent<GamePlay>().setPoints(actScore);
		}
	}


	public void setBow(GameObject _bow) {
		bow = _bow;
	}
}
