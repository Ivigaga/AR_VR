using System.Collections;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLogic : MonoBehaviour
{
    public float damage = 10f; 

    public void OnCollisionEnter(Collision col)
    {
        // 1. Buscamos la clase base "EnemyHealth"
        HealthSystem enemyHealth = col.gameObject.GetComponent<HealthSystem>();

        // 2. Comprobamos si el objeto tenía CUALQUIER script que herede de EnemyHealth
        if (enemyHealth != null)
        {
            // 3. Le hacemos daño.
            // Unity es lo bastante inteligente para saber si debe llamar al
            // TakeDamage() de EnemyHealth o al TakeDamage() sobrescrito
            // de DragonHealth. ¡Esto es la magia de la herencia!
            enemyHealth.TakeDamage(damage);
        }
    }
}