using UnityEngine;


public class Ore : MonoBehaviour
{
    
    private OreSO currentData;

    [SerializeField]
    private GameObject[] models;


    [SerializeField]
    private ItemSO dropData;

    private int hp;

   

    private void OnEnable()
    {
        hp = 3;
    }

    public void Setup(OreSO _newData)
    {
        currentData= _newData;

        foreach (GameObject model in models) 
            model.SetActive(false);

        if(_newData.oreIndex<models.Length)
            models[_newData.oreIndex].SetActive(true);

        gameObject.SetActive(true);
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
