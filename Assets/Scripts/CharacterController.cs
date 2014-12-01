using UnityEngine;
using System.Collections;

namespace IsoEngine1
{
	public class CharacterController : MonoBehaviour
	{

		public float speed = 0.1f;
		public Vector3 direction;

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
			transform.Translate (direction * speed * Time.deltaTime);
		}
	}
}
