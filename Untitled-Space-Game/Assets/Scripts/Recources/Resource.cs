using UnityEngine;

[CreateAssetMenu(menuName = "Recources/Recource")]
public class Resource : ScriptableObject
{
    public Item item;
    public float mineDuration = 3f;
    public int recourceAmount = 1;
}