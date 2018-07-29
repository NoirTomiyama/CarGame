using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private GUIStyle labelStyle;
    Quaternion start_gyro;
    Quaternion gyro;

	// Use this for initialization
	void Start () {

        this.labelStyle = new GUIStyle();
        this.labelStyle.fontSize = Screen.height / 22;
        this.labelStyle.normal.textColor = Color.white;
        //後述するがここで「Start」シーンのジャイロの値を取っている

        //start_gyro = StartCameraController.ini_gyro;

		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
