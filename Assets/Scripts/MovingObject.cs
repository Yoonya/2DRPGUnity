using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public string characterName;
    public float speed;

    //쯔꾸르게임이라 그냥 움직이는게 아니라 픽셀단위로 움직이기 위해 선언
    public int walkCount;
    protected int currentWalkCount;
    //ex) speed = 2.4, walkCount = 20 -> 2.4 * 20 = 48

    private bool notCoroutine = false;

    protected Vector3 vector;

    public Queue<string> queue;//캐릭터 이동명령에서 발생되는 빠르게 움직임, standing 애니메이션 전환으로 안바뀌는 것을 고치기 위하여

    public BoxCollider2D boxColider;
    public LayerMask layerMask; //충돌한 상대가 어떤 레이어인지 파악하기 위해
    public Animator animator;

    public void Move(string _dir, int _frequency = 5) //frequency 생략가능
    {
        queue.Enqueue(_dir);
        if (!notCoroutine)
        {
            notCoroutine = true;
            StartCoroutine(MoveCoroutine(_dir, _frequency));
        }     
    }

    IEnumerator MoveCoroutine(string _dir, int _frequency)
    {
        while (queue.Count != 0) //전부 다 deque될 때까지
        {
            switch (_frequency)
            {
                case 1:
                    yield return new WaitForSeconds(4f);
                    break;
                case 2:
                    yield return new WaitForSeconds(3f);
                    break;
                case 3:
                    yield return new WaitForSeconds(2f);
                    break;
                case 4:
                    yield return new WaitForSeconds(1f);
                    break;
                case 5:
                    break;
            }
            string direction = queue.Dequeue();
            vector.Set(0, 0, vector.z);

            switch (direction)
            {
                case "UP":
                    vector.y = 1f;
                    break;
                case "DOWN":
                    vector.y = -1f;
                    break;
                case "RIGHT":
                    vector.x = 1f;
                    break;
                case "LEFT":
                    vector.x = -1f;
                    break;
            }
            animator.SetFloat("DirX", vector.x); //Dirx에 vector.x값 설정
            animator.SetFloat("DirY", vector.y);

            while (true) //충돌방지
            {
                bool checkCollsionFalg = CheckCollsion(); //base는 부모
                if (checkCollsionFalg)//앞에 무언가가 있다.
                {
                    animator.SetBool("Walking", false);
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    break;//움직임을 멈춘다.
                }
                    
            }
            
            animator.SetBool("Walking", true);

            //콜라이더 위치->이동하는 위치에 캐릭터보다 먼저 콜라이더를 보낸다. npc 충돌방지, 겹치기 방지
            boxColider.offset = new Vector2(vector.x * 0.7f * speed * walkCount, vector.y * 0.7f * speed * walkCount);

            //픽셀단위의 움직임을 반복문으로
            while (currentWalkCount < walkCount)
            {
                transform.Translate(vector.x * speed, vector.y * speed, 0);

                currentWalkCount++;
                if (currentWalkCount == walkCount * 0.5f + 2)
                    boxColider.offset = Vector2.zero;//다시 초기화
                yield return new WaitForSeconds(0.01f);
            }

            currentWalkCount = 0;
            if (_frequency != 5)//5일 때 한발로만 걷는 것 고침
                animator.SetBool("Walking", false);
          
        }
        animator.SetBool("Walking", false);
        notCoroutine = false; 
    }

    protected bool CheckCollsion()
    {
        RaycastHit2D hit;
        //RaycastHit2D = A지점과 B지점이 있을 때, 레이저가 A에서 출발하여 B에 도착한다면 Null반환, 방해물을 만나 도착하지 못한다면 방해물을 반환받는다.

        Vector2 start = new Vector2(transform.position.x + vector.x * speed * walkCount,
            transform.position.y + vector.y * speed * walkCount);  //A지점 = 캐릭터의 현재 위치 + 앞으로 나아갈 타일
        Vector2 end = start + new Vector2(vector.x * speed, vector.y * speed); //B지점 = 캐릭터가 이동하고자 하는 위치 값

        boxColider.enabled = false; //hit값이 캐릭터의 colider도 감지하기 때문에 잠깐 비활성화
        hit = Physics2D.Linecast(start, end, layerMask);
        boxColider.enabled = true;

        if (hit.transform != null) //벽 충돌
            return true;
        return false;
    }
}
