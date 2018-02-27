using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {
    Animator animator;
	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        //NormalAnimator();
        BlendAnimator();
    }

    void NormalAnimator()
    {
        animator.SetFloat("Walk", Input.GetAxis("Vertical"));
        animator.SetFloat("Turn", Input.GetAxis("Horizontal"));
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            animator.SetBool("Run", true);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            animator.SetBool("Run", false);
        }
    }
    void BlendAnimator()
    {
        animator.SetFloat("MoveZ", Input.GetAxis("Vertical")*4.1f);
        animator.SetFloat("MoveX", Input.GetAxis("Horizontal")*126f);
    }
}
