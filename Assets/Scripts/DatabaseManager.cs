using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour {

    public static DatabaseManager instance;
    private PlayerStat thePlayerStat;
    public GameObject prefabs_Floating_text;
    public GameObject parent;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);//다른 신으로 로드할 때마다 이 객체를 파괴하지 말아라
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public string[] var_name;
    public float[] var;
    
    public string[] switch_name;
    public bool[] switches;

    public List<Item> itemList = new List<Item>();

    private void FloatText(int number, string color)
    {
        Vector3 vector = thePlayerStat.transform.position;
        vector.y += 60; //캐릭터 위

        GameObject clone = Instantiate(prefabs_Floating_text, vector, Quaternion.Euler(Vector3.zero));
        clone.GetComponent<FloatingText>().text.text = number.ToString();
        if(color == "GREEN")
            clone.GetComponent<FloatingText>().text.color = Color.green;
        else if (color == "BLUE")
            clone.GetComponent<FloatingText>().text.color = Color.blue;
        clone.GetComponent<FloatingText>().text.fontSize = 25;
        clone.transform.SetParent(parent.transform);
    }

    public void UseItem(int _itemID)
    {
        switch (_itemID)
        {
            case 10001:
                if (thePlayerStat.hp >= thePlayerStat.currentHp + 50)
                    thePlayerStat.currentHp += 50;
                else
                    thePlayerStat.currentHp = thePlayerStat.hp;
                FloatText(50, "GREEN");
                break;
            case 10002:
                if (thePlayerStat.mp >= thePlayerStat.currentMp + 50)
                    thePlayerStat.currentMp += 50;
                else
                    thePlayerStat.currentMp = thePlayerStat.mp;
                FloatText(15, "BLUE");
                break;
        }
    }
  
    // Use this for initialization
    void Start () {
        thePlayerStat = FindObjectOfType<PlayerStat>();
        itemList.Add(new Item(10001, "빨간 포션", "체력을 50 회복시켜주는 기적의 물약", Item.ItemType.Use));
        itemList.Add(new Item(10002, "파란 포션", "마나를 15 회복시켜주는 기적의 물약", Item.ItemType.Use));
        itemList.Add(new Item(10003, "농축 빨간 포션", "체력을 350 회복시켜주는 기적의 농축 물약", Item.ItemType.Use));
        itemList.Add(new Item(10004, "농축 파란 포션", "마나를 80 회복시켜주는 기적의 농축 물약", Item.ItemType.Use));
        itemList.Add(new Item(11001, "랜덤 상자", "랜덤으로 포션이 나온다. 낮은 확률로 꽝", Item.ItemType.Use));
        itemList.Add(new Item(20001, "짧은 검", "기본적인 용사의 검", Item.ItemType.Equip, 3));
        itemList.Add(new Item(20002, "무딘 도끼", "기본적인 용사의 도끼", Item.ItemType.Equip, 4));
        itemList.Add(new Item(20003, "녹슨 창", "기본적인 용사의 창", Item.ItemType.Equip, 5));
        itemList.Add(new Item(20301, "사파이어 반지", "1초에 hp 1을 회복시켜주는 마법 반지", Item.ItemType.Equip, 0, 0, 1));
        itemList.Add(new Item(30001, "고대 유물의 조각 1", "반으로 쪼개진 고대 유물의 파편", Item.ItemType.Quest));
        itemList.Add(new Item(30002, "고대 유물의 조각 2", "반으로 쪼개진 고대 유물의 파편", Item.ItemType.Quest));
        itemList.Add(new Item(30003, "고대 유물", "고대 유적에 잠들어있던 고대의 유물", Item.ItemType.Quest));
    }
	
}
