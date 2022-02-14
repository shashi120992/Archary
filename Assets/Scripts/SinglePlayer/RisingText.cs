using UnityEngine;
using System.Collections;


[RequireComponent(typeof(TextMesh))]
public class RisingText : MonoBehaviour
{
	// private variables:
	Vector3 crds_delta;
	float   alpha;
	float   life_loss;
	Camera  cam;
	
	public Color color = Color.white;
	
	public void setup(int points, float duration, float rise_speed)
	{
		GetComponent<TextMesh>().text = points.ToString();        
		life_loss = 1f / duration;
		crds_delta = new Vector3(0f, rise_speed, 0f);        
	}
	
	void Start() 
	{
		alpha = 1f;
		cam = GameObject.Find("Main Camera").GetComponent<Camera>();
		crds_delta = new Vector3(0f, 1f, 0f);
		life_loss = 0.5f;
	}
	
	void Update () 
	{
		transform.Translate(crds_delta * Time.deltaTime, Space.World);
		
		alpha -= Time.deltaTime * life_loss;
		GetComponent<Renderer>().material.color = new Color(color.r,color.g,color.b,alpha);
		
		if (alpha <= 0f) Destroy(gameObject);
		
		transform.LookAt(cam.transform.position);
		transform.rotation = cam.transform.rotation;        
	}
}