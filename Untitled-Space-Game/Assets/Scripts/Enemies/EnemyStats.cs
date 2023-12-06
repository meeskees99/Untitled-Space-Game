using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    [Header("Stats")]
    public int health;
    public int attackDamage;
    public float attackRadius;

    [Header("Movement Options")]
    public float movementSpeed;
    public float movementRadius;
    public float stopDistance;
    public bool canJump;
}
