using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFollower : MonoBehaviour {

    public AnimationCurve animationCurve;
    public Transform target;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update() {
        transform.position = target.position;
        //transform.rotation = Quaternion.Euler(0, target.rotation.eulerAngles.y,0);
    }
}
