using UnityEngine;
using System.Collections;

public class cardScript : MonoBehaviour {
    bool isPress;
    float pressTime;

	// Use this for initialization
	void Start () {
        isPress = false;
        pressTime = 0f;
	}
	
	// Update is called once per frame
	void Update () {
	    if(isPress)
        {
            pressTime+=Time.deltaTime;
        }
        if (pressTime>3.0f)
        {
            print(this.name);
            GameObject.Find(this.name).transform.localScale=new Vector3(3.0f,3.0f,3.0f);
        }
	}

    public void PointDown()
    {
        isPress = true;
        print("point down");
    }

    public void PointUp()
    {
        isPress = false;
        pressTime = 0f;
        print("point up");
    }
}
