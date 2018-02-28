using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {
    Animator animator;
    Vector3 matchTarget;
    CharacterController characterController;
    public GameObject log;
    public Transform left;
    public Transform right;
    public Transform rightElbow;
	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

    }
	
	// Update is called once per frame
	void Update () {
        //NormalAnimator();
        BlendAnimator();
        ControlVault();
        ControlCharacterController();
        ControlSlide();
    }
    /// <summary>
    /// 不使用混合树的动画状态机，摁下shift奔跑
    /// </summary>
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
    /// <summary>
    /// 使用混合树的动画状态机
    /// </summary>
    void BlendAnimator()
    {
        animator.SetFloat("MoveZ", Input.GetAxis("Vertical")*4.1f);
        animator.SetFloat("MoveX", Input.GetAxis("Horizontal")*126f);
    }
    /// <summary>
    /// 跳跃翻过障碍物
    /// </summary>
    void ControlVault()
    {
        bool isVault = false;
        if (animator.GetFloat("MoveZ") > 3f && animator.GetCurrentAnimatorStateInfo(0).IsName("Move"))
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position+Vector3.up*0.3f, transform.forward, out hitInfo, 4f))
            {
                if (hitInfo.transform.CompareTag("Obstacle"))
                {
                        isVault = true;
                        Vector3 point = hitInfo.point;
                        point.y = hitInfo.collider.transform.position.y + hitInfo.collider.bounds.size.y;
                        matchTarget = point;
                }
            }
        }
        animator.SetBool("Vault", isVault);

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Vault") && animator.IsInTransition(0)==false)
        {
            animator.MatchTarget(matchTarget, Quaternion.identity, AvatarTarget.LeftHand, new MatchTargetWeightMask(Vector3.one, 0), 0.32f, 0.40f);
        }

    }
    /// <summary>
    /// 控制第一人称控制器的有效期
    /// </summary>
    void ControlCharacterController()
    {
        characterController.enabled = animator.GetFloat("Collider") < 0.5f;
    }
    /// <summary>
    /// 下滑穿过障碍物
    /// </summary>
    void ControlSlide()
    {
        bool isSlide = false;
        if (animator.GetFloat("MoveZ") > 3f && animator.GetCurrentAnimatorStateInfo(0).IsName("Move"))
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position + Vector3.up * 0.6f, transform.forward, out hitInfo, 3f))
            {
                if (hitInfo.transform.CompareTag("Obstacle"))
                {
                    if (hitInfo.distance > 2)
                    {
                        isSlide = true;
                        Vector3 point = hitInfo.point;
                        point.y = hitInfo.collider.transform.position.y + hitInfo.collider.bounds.size.y;
                        matchTarget = point;
                    }
                }
            }
        }
        animator.SetBool("Slide", isSlide); 
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Log"))
        {
            Destroy(other.gameObject);
            CarryLog();
        }
    }

    void CarryLog()
    {
        log.SetActive(true);
        animator.SetBool("HoldLog", true);
    }

    void OnAnimatorIK(int layerIndex)
    {
        if(layerIndex == 1)
        {
            int weight = animator.GetBool("HoldLog") ? 1 : 0;
            animator.SetIKPosition(AvatarIKGoal.LeftHand, left.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, left.rotation);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, weight);
            animator.SetIKPosition(AvatarIKGoal.RightHand, right.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, right.rotation);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, weight);
            animator.SetIKHintPosition(AvatarIKHint.RightElbow, rightElbow.position);
            animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, weight);
        }
    }
}
