using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    static public CameraManager instance;

    public GameObject target;//ī�޶� ���� ���
    public float moveSpeed; //ī�޶� �ӵ�
    private Vector3 targetPosition; //��� ���� ��ġ ��

    public BoxCollider2D bound;

    //�ڽ� �ö��̴� ������ �ּ� �ִ� xyz���� ����
    private Vector3 minBound;
    private Vector3 maxBound;

    //ī�޶� ������ �ڽ� �ٱ����� ������ �ʾƾ��ϴµ� ���� �߽ɿ� �ִٺ��� �ݳʺ�ŭ ������ ������Ѵ�.
    private float halfWidth;
    private float halfHeight;

    //ī�޶��� �ݳ��̰��� ���� �Ӽ��� �̿��ϱ� ���� ����
    private Camera theCamera;

    //Start���� ���� �����
    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);//�ٸ� ������ �ε��� ������ �� ��ü�� �ı����� ���ƶ�
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        theCamera = GetComponent<Camera>();
        minBound = bound.bounds.min;
        maxBound = bound.bounds.max;
        halfHeight = theCamera.orthographicSize;//size��
        halfWidth = halfHeight * Screen.width / Screen.height; // �ڿ� ��ũ���� �ػ� ���� �ǹ�

       
    }

    // Update is called once per frame
    void Update()
    {
        if (target.gameObject != null)
        {
            targetPosition.Set(target.transform.position.x, target.transform.position.y, this.transform.position.z);//z���� �ڽ� ������ ���� ���´�.

            //�׳� ������ ���� �ϴ� ��찡 �ƴ϶� Lerp�� ���� ������ ������ ī�޶� �����̴°� �ƴ϶� ĳ���Ͱ� �����̸� õõ�� ������� ȿ���� �ֱ� ���� ������ �� ����.
            this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, moveSpeed * Time.deltaTime); //1���� 2���� 3�� �ӵ��� �����̰� �Ѵ�

            //(��, �ּ�, �ִ�) -> �ּ� �ִ밪 ���̿� ���� �ִٸ� �� ��ȯ, �ּҺ��� �۴ٸ� �ּҹ�ȯ, �ִ뺸�� ũ�ٸ� �ִ��ȯ
            float clampedX = Mathf.Clamp(this.transform.position.x, minBound.x + halfWidth, maxBound.x - halfWidth);       
            float clampedY = Mathf.Clamp(this.transform.position.y, minBound.y+ halfHeight, maxBound.y - halfHeight);

            this.transform.position = new Vector3(clampedX, clampedY, this.transform.position.z);
        }
    }

    public void SetBound(BoxCollider2D newBound)
    {
        bound = newBound;
        minBound = bound.bounds.min;
        maxBound = bound.bounds.max;

    }
}
