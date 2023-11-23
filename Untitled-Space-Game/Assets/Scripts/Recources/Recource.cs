using UnityEngine;

[CreateAssetMenu(menuName = "Recources/Recource")]
public class Recource : ScriptableObject
{
    public Item item;
    public float mineDuration = 3f;
    public int recourceAmount = 1;
}