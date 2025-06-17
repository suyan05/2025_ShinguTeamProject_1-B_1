using UnityEngine;

[CreateAssetMenu(fileName = "BubbleData", menuName = "Game/Bubble Data")]
public class BubbleData : ScriptableObject
{
    public string bubbleName;
    public int level;
    public GameObject prefab;
    [Tooltip("�� unlock ������ ���� Ȯ�� (�������: unlock_1 ~ unlock_7)")]
    public float[] unlockChances = new float[7];
}