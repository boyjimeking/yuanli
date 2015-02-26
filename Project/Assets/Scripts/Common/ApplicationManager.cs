using UnityEngine;
using System.Collections;

public class ApplicationManager : MonoBehaviour {

	void Awake() {
		Application.targetFrameRate = 30;
	}
}
