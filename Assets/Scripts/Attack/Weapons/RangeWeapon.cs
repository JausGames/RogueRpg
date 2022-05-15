using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bow", menuName = "Weapons/Range/Bow", order = 1)]
public class RangeWeapon : WeaponData
{
    [Header("Range Data")]
    [SerializeField] CombatData parent;
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileSpeed;
    [SerializeField] LayerMask floorLayer;

    public float ProjectileSpeed { get => projectileSpeed; set => projectileSpeed = value; }
    public GameObject Projectile { get => projectile; set => projectile = value; }

    public override void TriggerWeapon(Transform owner, Transform hitPoint, LayerMask enemyLayer, LayerMask friendLayer, AnimatorController animator)
    {
        //base.Attack(owner, hitPoint, enemyLayer, friendLayer);
        if (!animator && nextHit > Time.time) return;
        else if (animator && nextHit > Time.time) animator.TryCombo();
        else
        {
            if (animator) animator.AttackAnimation();
            nextHit = Time.time + coolDown;

            var projectileGo = Instantiate(projectile, hitPoint.transform.position, owner.transform.rotation * projectile.transform.rotation, null);
            var projectileCmp = projectileGo.GetComponent<Projectile>();
            projectileCmp.EnemyLayer = enemyLayer;

            projectileCmp.WillDestroyLayer = 1 | (1 << 6);
            //projectileCmp.WillDestroyLayer = ~friendLayer & (1 << projectileGo.layer) & ~(1 << floorLayer));
            projectileCmp.Data = parent;
            projectileCmp.Range = hitRange;
            projectileCmp.Speed = projectileSpeed;
        }
    }
}
