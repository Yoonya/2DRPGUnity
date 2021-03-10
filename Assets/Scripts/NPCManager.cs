using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCMove //npc움직임 담당
{
    [Tooltip("NPCMove를 체크하면 NPC가 움직임")]
    public bool NPCmove;
    public string[] direction;//npc 움직일 방향 설정
    [Range(1,5)] [Tooltip("1 = 천천히, 2 = 조금 천천히, 3 = 보통, 4 = 빠르게, 5 = 연속적으로")]
    public int frequency;//움직일 방향으로 얼마나 빠른 속도로 하나

}

public class NPCManager : MovingObject {

    [SerializeField]
    public NPCMove npc;

	// Use this for initialization
	void Start () {
        queue = new Queue<string>();
        StartCoroutine(MoveCoroutine());
    }

    public void SetMove()
    {
        StartCoroutine(MoveCoroutine());
    }
    public void SetNotMove()
    {
        StopAllCoroutines();
    }

    IEnumerator MoveCoroutine()
    {
        if (npc.direction.Length != 0)
        {
            for (int i = 0; i < npc.direction.Length; i++)
            {

                yield return new WaitUntil(() => queue.Count < 2); //true가 될 때까지 대기, 큐의 값을 0과 1로 유지
                base.Move(npc.direction[i], npc.frequency);

                if (i == npc.direction.Length - 1) //무한반복
                {
                    i = -1;
                }



            }
        }
    }
}
