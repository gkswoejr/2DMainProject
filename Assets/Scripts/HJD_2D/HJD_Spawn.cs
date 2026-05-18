using UnityEngine;

enum SpawnType
{
    None = 0,
    Coin,
    EndingUI

}

public class HJD_Spawn : MonoBehaviour
{

    [SerializeField] private string _spawnObjectDataPath;
    [SerializeField] private SpawnType _spawnSpotType;
    [SerializeField] private GameObject EndingUI;
    [SerializeField] private Collider2D Collider2D;



    private void Start()
    {

        testStartSpawn(_spawnObjectDataPath);

    }

    private void testStartSpawn(string Path)
    {
        Object SpawnObject = Resources.Load(Path);

        switch (_spawnSpotType)
        {
            case SpawnType.Coin:

                if (SpawnObject == null)
                {
                    Debug.Log("박스 스폰 실패");
                    return;
                }
                Instantiate(SpawnObject, this.transform.position, Quaternion.identity);
                return;
            case SpawnType.EndingUI:


                return;

        }
        








    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (EndingUI != null)
            {
                EndingUI.SetActive(true);
                Debug.Log("플레이어가 엔딩 포인트에 도달하여 엔딩 UI를 켭니다.");
            }
        }
       
    }
}


