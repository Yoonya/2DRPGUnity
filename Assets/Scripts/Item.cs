using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item {

    public int itemID; //아이템의 고유 ID값, 중복 불가능, 이미지 파일 이름과 같게 설정했음
    public string itemName; //아이템의 이름, 중복 가능
    public string itemDescription;//아이템 설명
    public int itemCount;//소지 개수
    public Sprite itemIcon;//아이템의 아이콘
    public ItemType itemType;

    public enum ItemType //아이템 종류 열거
    {
       Use, Equip, Quest, ETC
    }

    public int atk;
    public int def;
    public int recover_hp;
    public int recover_mp;
  
    public Item(int _itemID, string _itemName, string _itemDes, ItemType _itemType, 
        int _atk = 0, int _def = 0, int _recover_hp = 0, int _recover_mp = 0, int _itemCount = 1)
    {
        itemID = _itemID;
        itemName = _itemName;
        itemDescription = _itemDes;
        itemType = _itemType;
        itemCount = _itemCount;
        itemIcon = Resources.Load("ItemIcon/" + _itemID.ToString(), typeof(Sprite)) as Sprite;//스프라이트로 가져오겠다 + 캐스트

        atk = _atk;
        def = _def;
        recover_hp = _recover_hp;
        recover_mp = _recover_mp;
    }
}
