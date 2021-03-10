using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue {

    [TextArea(1,2)]//한줄 입력이 아니라 여러줄입력이 되기 위해
    //교체가 되야하고 많으니까
    public string[] sentences;
    public Sprite[] sprites;
    public Sprite[] dialogueWindows;
}
