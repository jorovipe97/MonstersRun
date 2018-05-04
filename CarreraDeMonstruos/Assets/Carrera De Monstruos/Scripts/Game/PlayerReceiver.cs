using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReceiver : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void MovePlayer(JR2J.JR2JMessage msg)
    {
        Debug.Log("address: " + msg.address + ", value: " + msg.value);
    }
}
