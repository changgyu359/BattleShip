using System.Collections;
using UnityEngine;


public class Ore : MonoBehaviour
{
    
    private OreSO currentData;

    [SerializeField]
    private GameObject[] models;


    private int hp;

    private float shakeIntensity = 0.1f;
    private float shakeDuration = 0.1f;
   

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
        OnHit();
        hp--; 
        if(hp<=0)
        {
            Vector3 spawnPos = transform.position;
            spawnPos.y+=1f;
            DropItemManager.Instance.SpawnItem(currentData, spawnPos);
            
            gameObject.SetActive(false);
        }
        
    }

    public void OnHit()
    {
        StartCoroutine(ShakeEffect());
    }

    private IEnumerator ShakeEffect()
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            // 랜덤한 방향으로 아주 살짝 이동
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float z = Random.Range(-1f, 1f) * shakeIntensity;

            transform.localPosition = originalPos + new Vector3(x, 0, z);

            elapsed += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 효과 끝나면 원래 위치로 복귀
        transform.localPosition = originalPos;
    }

}
