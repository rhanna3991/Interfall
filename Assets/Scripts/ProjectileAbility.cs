using UnityEngine;

[CreateAssetMenu(menuName = "Interfall/Abilities/Projectile Ability")]
public class ProjectileAbility : Ability
{
    public float projectileLifetime = 4f;

    public override void Activate(BattleManager battleManager)
    {
        var caster = battleManager.playerStats;
        var target = battleManager.enemyStats;

        if (caster == null || target == null)
        {
            Debug.LogError("Missing caster or target in ProjectileAbility.");
            return;
        }

        // Calculate damage
        int damage = Mathf.Max(1,
            caster.GetStatAtLevel(StatType.SpecialAttack, battleManager.playerLevel) - target.baseDefense);

        // Use the scene's spawn point
        Transform spawnPoint = battleManager.playerProjectileSpawn != null
            ? battleManager.playerProjectileSpawn
            : battleManager.transform;

        if (effectPrefab != null)
        {
            var projectile = Object.Instantiate(effectPrefab, spawnPoint.position, effectPrefab.transform.rotation);

            if (projectile.TryGetComponent(out Projectile proj))
            {
                proj.direction = new Vector2(1f, 0.6f); // diagonal
                proj.SetDamage(damage);
                proj.SetAbilityInfo(abilityName);
                proj.SetBattleManager(battleManager);
            }

            Object.Destroy(projectile, 4f);
        }
    }
}