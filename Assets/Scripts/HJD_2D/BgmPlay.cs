using UnityEngine;

public class BgmPlay : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayBgm();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlayBgm()
    {
        DaniTechSoundManager.Inst.PlayBGM("2D/Music_Sample_01");
    }
}
