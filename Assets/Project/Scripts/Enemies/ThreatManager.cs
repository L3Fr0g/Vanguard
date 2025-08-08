using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace EnemyNamespace
{
    public class ThreatManager : MonoBehaviour
    {
        private Dictionary<Transform, float> threatTable = new Dictionary<Transform, float>();

        public Transform CurrentHighestThreatTarget { get; private set; }

        public void AddThreat(Transform target, float amount)
        {
            if (target == null) return;

            if (!threatTable.ContainsKey(target))
            {
                threatTable[target] = 0;
            }

            threatTable[target] += amount;
            Debug.Log($"Added {amount} threat to {target.name}. Total threat {threatTable[target]}");

            RecalculateHighestThreatTarget();
        }
        
        private void RecalculateHighestThreatTarget()
        {
            if (threatTable.Count == 0)
            {
                CurrentHighestThreatTarget = null;
                return;
            }
            CurrentHighestThreatTarget = threatTable.OrderByDescending(kvp => kvp.Value).FirstOrDefault().Key;
        }

        public void ClearThreat()
        {
            threatTable.Clear();
            CurrentHighestThreatTarget = null;
            Debug.Log("Threat clearerd, returning to start.");
        }
    }
}