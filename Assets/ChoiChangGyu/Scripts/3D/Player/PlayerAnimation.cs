using System.Collections;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{

    private Animator anim;
    private PlayerControl playerControl;
    private PlayerMining mining;

    private void Start()
    {
        anim = GetComponent<Animator>();
        playerControl = GetComponent<PlayerControl>();
        mining = GetComponent<PlayerMining>();
    }

    private void Update()
    {
        if (playerControl.IsInteracting)
        {
            anim.SetBool("IsWalk", false);
            return;
        }


        if(Input.GetKeyDown(KeyCode.E))
        {
            
            StartCoroutine(InteractionCorou());
            
        }

        bool isWalking = playerControl.MoveDirection!=Vector3.zero;

        anim.SetBool("IsWalk",isWalking);
    }

    private IEnumerator InteractionCorou()
    {
        playerControl.IsInteracting = true;
        anim.SetTrigger("PressE");
        anim.SetBool("IsWalk", false);

        yield return new WaitForSeconds(0.7f);
        mining.TryMine();
        yield return new WaitForSeconds(0.4f);

        playerControl.IsInteracting=false;
    }

}
