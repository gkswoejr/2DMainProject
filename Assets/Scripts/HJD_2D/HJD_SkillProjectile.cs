using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using System.Collections;


public class HJD_SkillProjectile : HJD_SkillBase
{

    void Update()
    {
        transform.Translate(Vector3.right * _moveSpeed * Time.deltaTime);
    }
    protected override void SKillLife()
    {

        StartCoroutine(ExecuteAfterTime(skillLifeTime));

    }

    IEnumerator ExecuteAfterTime(float time)
    {
        Debug.Log("3초 대기를 시작합니다.");

        // 지정한 초(time)만큼 유니티가 대기합니다.
        yield return new WaitForSeconds(time);

        Destroy(this.gameObject);


    }
}
