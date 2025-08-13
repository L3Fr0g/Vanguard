using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnemyNamespace;
using JetBrains.Annotations;
using Unity.Cinemachine;

namespace CharacterNamespace
{
    [RequireComponent(typeof(Collider2D))]
    public class AreaOfEffectController : MonoBehaviour
    {
        private float damagePerTick;
        private float tickRate;
        private Transform owner;
        private List<EnemyHealth> targetsInArea = new List<EnemyHealth>();

        public void Initialize(float totalDamage, float duration, float rate, Transform abilityOwner)
        {
            owner = abilityOwner;
            tickRate = rate;

            float tickCount = duration / tickRate;
            damagePerTick = totalDamage / tickCount;

            StartCoroutine(DamageTickCoroutine());
            Destroy(gameObject, duration);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<EnemyHealth>(out var enemyHealth))
            {
                if(!targetsInArea.Contains(enemyHealth))
                {
                    targetsInArea.Add(enemyHealth);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent<EnemyHealth>(out var enemyHealth))
            {
                if (targetsInArea.Contains(enemyHealth))
                {
                    targetsInArea.Remove(enemyHealth);
                }
            }
        }

        private IEnumerator DamageTickCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(tickRate);

                for (int i = targetsInArea.Count - 1; i >= 0; i--)
                {
                    if (targetsInArea[i] != null)
                    {
                        targetsInArea[i].TakeDamage(damagePerTick, owner);
                    }
                    else
                    {
                        targetsInArea.RemoveAt(i);
                    }
                }
            }
        }
    }
}
