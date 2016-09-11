using UnityEngine;
using System.Collections;

public class SimpleRotater : MonoBehaviour {

    public Vector3 rotatePerTime = new Vector3(0f,100f,0f);
    private bool isPaused = false;

	void Start () {
	
	}
	
	void Update () {
        if(isPaused == false)
        {
            transform.Rotate(rotatePerTime * Time.deltaTime);
        }

	}

    public void OnPause()
    {
        isPaused = true;
    }

    public void OnResume()
    {
        isPaused = false;
    }
}
