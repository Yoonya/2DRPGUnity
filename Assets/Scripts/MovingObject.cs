using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public string characterName;
    public float speed;

    //��ٸ������̶� �׳� �����̴°� �ƴ϶� �ȼ������� �����̱� ���� ����
    public int walkCount;
    protected int currentWalkCount;
    //ex) speed = 2.4, walkCount = 20 -> 2.4 * 20 = 48

    private bool notCoroutine = false;

    protected Vector3 vector;

    public Queue<string> queue;//ĳ���� �̵���ɿ��� �߻��Ǵ� ������ ������, standing �ִϸ��̼� ��ȯ���� �ȹٲ�� ���� ��ġ�� ���Ͽ�

    public BoxCollider2D boxColider;
    public LayerMask layerMask; //�浹�� ��밡 � ���̾����� �ľ��ϱ� ����
    public Animator animator;

    public void Move(string _dir, int _frequency = 5) //frequency ��������
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
        while (queue.Count != 0) //���� �� deque�� ������
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
            animator.SetFloat("DirX", vector.x); //Dirx�� vector.x�� ����
            animator.SetFloat("DirY", vector.y);

            while (true) //�浹����
            {
                bool checkCollsionFalg = CheckCollsion(); //base�� �θ�
                if (checkCollsionFalg)//�տ� ���𰡰� �ִ�.
                {
                    animator.SetBool("Walking", false);
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    break;//�������� �����.
                }
                    
            }
            
            animator.SetBool("Walking", true);

            //�ݶ��̴� ��ġ->�̵��ϴ� ��ġ�� ĳ���ͺ��� ���� �ݶ��̴��� ������. npc �浹����, ��ġ�� ����
            boxColider.offset = new Vector2(vector.x * 0.7f * speed * walkCount, vector.y * 0.7f * speed * walkCount);

            //�ȼ������� �������� �ݺ�������
            while (currentWalkCount < walkCount)
            {
                transform.Translate(vector.x * speed, vector.y * speed, 0);

                currentWalkCount++;
                if (currentWalkCount == walkCount * 0.5f + 2)
                    boxColider.offset = Vector2.zero;//�ٽ� �ʱ�ȭ
                yield return new WaitForSeconds(0.01f);
            }

            currentWalkCount = 0;
            if (_frequency != 5)//5�� �� �ѹ߷θ� �ȴ� �� ��ħ
                animator.SetBool("Walking", false);
          
        }
        animator.SetBool("Walking", false);
        notCoroutine = false; 
    }

    protected bool CheckCollsion()
    {
        RaycastHit2D hit;
        //RaycastHit2D = A������ B������ ���� ��, �������� A���� ����Ͽ� B�� �����Ѵٸ� Null��ȯ, ���ع��� ���� �������� ���Ѵٸ� ���ع��� ��ȯ�޴´�.

        Vector2 start = new Vector2(transform.position.x + vector.x * speed * walkCount,
            transform.position.y + vector.y * speed * walkCount);  //A���� = ĳ������ ���� ��ġ + ������ ���ư� Ÿ��
        Vector2 end = start + new Vector2(vector.x * speed, vector.y * speed); //B���� = ĳ���Ͱ� �̵��ϰ��� �ϴ� ��ġ ��

        boxColider.enabled = false; //hit���� ĳ������ colider�� �����ϱ� ������ ��� ��Ȱ��ȭ
        hit = Physics2D.Linecast(start, end, layerMask);
        boxColider.enabled = true;

        if (hit.transform != null) //�� �浹
            return true;
        return false;
    }
}
