using UnityEngine;


public class Ore : MonoBehaviour
{

    //[SerializeField]
    //private GameObject orePF;

    private int hp;

    private void OnEnable()
    {
        hp = 3;
    }

    public void TakeDamage()
    {
        hp--; Debug.Log("아야");
        if(hp<=0)
        {
            //템 드랍 로직
            Debug.Log("파개댐...");
            gameObject.SetActive(false);
        }
        
    }

}
