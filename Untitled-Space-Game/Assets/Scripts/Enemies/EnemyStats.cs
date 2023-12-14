using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    public string enemyName;
    [Header("Stats")]
    public int health = 100;
    public int attackDamage = 20;
    [Tooltip("Make sure to keep in mind the radius of the player and the radius of this enemy")]
    public float attackRange = 3f;
    [Tooltip("Delay In Seconds Between Attacks")]
    public float attackRate = 2f;
    public float chaseRadius = 7f;
    [Tooltip("Amount of time the enemy will chase the player after leaving their range")]
    public float chaseTime = 4f;

    [Header("Movement Options")]
    public float movementSpeed = 6f;
    public float movementRadius = 10f;
    [Tooltip("Keep this value above 120")]
    public float angularSpeed = 160f;
    public float stopDistance = 0.5f;
    public bool canJump = true;
}
