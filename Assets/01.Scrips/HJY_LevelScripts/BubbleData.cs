using UnityEngine;

[CreateAssetMenu(fileName = "BubbleData", menuName = "Game/Bubble Data")]
public class BubbleData : ScriptableObject
{
    public string bubbleName;
    public int level;
    public GameObject prefab;
    [Tooltip("각 unlock 레벨별 등장 확률 (순서대로: unlock_1 ~ unlock_7)")]
    public float[] unlockChances = new float[7];
}