using UnityEngine;

namespace GameBuilders.FPSBuilder.Core.Weapons
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(menuName = "Ammo Type", fileName = "Ammo Type", order = 201)]
    public sealed class AmmoType : ScriptableObject
    {
        /// <summary>
        /// How the damage inflicted by the projectiles can be calculated.
        /// </summary>
        public enum DamageMode
        {
            DecreaseByDistance,
            Constant
        }
    
        /// <summary>
        /// Name of the ammunition.
        /// </summary>
        [SerializeField]
        [Tooltip("Name of the ammunition.")]
        private string m_AmmunitionName = "Ammunition";

        #region BALLISTICS
    
        /// <summary>
        /// Bullet's mass in kilograms.
        /// </summary>
        [SerializeField]
        [Tooltip("Bullet's mass in kilograms.")]
        [Range(0.001f, 0.4f)]
        private float m_ProjectileMass = 0.02f;
        
        /// <summary>
        /// Defines whether or not a projectile can penetrate the Target's body.
        /// </summary>
        [SerializeField]
        [Tooltip("Defines whether or not a projectile can penetrate the Target's body.")]
        private bool m_CanPenetrate = true;

        /// <summary>
        /// The maximum distance that a projectile can travel penetrating an object.
        /// </summary>
        [SerializeField]
        [Tooltip("The maximum distance that a projectile can travel penetrating an object.")]
        [MinMax(0, Mathf.Infinity)]
        private float m_PenetrationPower = 1;
        
        /// <summary>
        /// Refraction is the effect of altering the bullet path after transferring energy on the collision.
        /// </summary>
        [SerializeField]
        [MinMaxSlider(-1, 1, "Refraction is the effect of altering the bullet path after transferring energy on the collision.", "F2")]
        private Vector2 m_Refraction = new Vector2(-0.5f, 0.5f);
        
        /// <summary>
        /// The density influence defines how much the object density will affect the refraction in the bullet direction.
        /// </summary>
        [SerializeField]
        [Tooltip("The density influence defines how much the object density will affect the refraction in the bullet direction.")]
        [MinMax(0, Mathf.Infinity)]
        private float m_DensityInfluence = 3;

        /// <summary>
        /// Defines how the damage inflicted by the projectiles will be calculated.
        /// </summary>
        [SerializeField]
        [Tooltip("Defines how the damage inflicted by the projectiles will be calculated.")]
        private DamageMode m_DamageMode = DamageMode.Constant;

        /// <summary>
        /// Defines the minimum and maximum damage value inflicted by this projectile.
        /// </summary>
        [SerializeField]
        [MinMaxSlider(0, 100, "Defines the minimum and maximum damage value inflicted by this projectile.")]
        private Vector2 m_Damage = new Vector2(15, 30);

        /// <summary>
        /// Defines how the damage will be calculated based on the distance. 
        /// (The X axis is the target distance, in which 0 means 0 units and 1 means the full effective range and the Y axis is the damage percent.)
        /// </summary>
        [SerializeField]
        [Tooltip("Defines how the damage will be calculated based on the distance. " +
                 "(The X axis is the target distance, in which 0 means 0 units and 1 means the full effective range and the Y axis is the damage percent)")]
        private AnimationCurve m_DamageFalloffCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.4f, 1), new Keyframe(0.6f, 0.5f), new Keyframe(1, 0.5f));
    
        /// <summary>
        /// Bullet's initial velocity in meters per second.
        /// </summary>
        [SerializeField]
        [Tooltip("Bullet's initial velocity in meters per second.")]
        private float m_ProjectileSpeed = 600;
    
        /// <summary>
        /// Defines the maximum impact force that can applied by the projectile.
        /// </summary>
        [SerializeField]
        [Tooltip("Defines the maximum impact force that can applied by the projectile.")]
        private float m_MaximumImpactForce = 150;
        
        /// <summary>
        /// Allow the projectile ricochet when not able to penetrate the target's body.
        /// </summary>
        [SerializeField]
        [Tooltip("Allow the projectile ricochet when not able to penetrate the target's body.")]
        private bool m_Ricochet = true;

        /// <summary>
        /// Chance of bullet bouncing off a surface when it is shot at an angle.
        /// </summary>
        [SerializeField] 
        [Tooltip("Chance of bullet bouncing off a surface when it is shot at an angle. ")] 
        [Range(0, 1)]
        private float m_RicochetChance = 0.1f;

        /// <summary>
        /// Angle of incidence is the angle between a ray incident on a surface and the normal at the point of incidence.
        /// Maximum Incident Angle is the highest angle that a bullet can bounce off a surface. Any angle greater than this value
        /// will make the bullet penetrate the object.
        /// </summary>
        [SerializeField]
        [Tooltip("Angle of incidence is the angle between a ray incident on a surface and the normal at the point of incidence. " +
                 "Maximum Incident Angle is the highest angle that a bullet can bounce off a surface. Any angle greater than this value " +
                 "will make the bullet penetrate the object.")]
        [Range(0, 90)]
        private float m_MaxIncidentAngle = 45;

        /// <summary>
        /// Defines the trajectory variation of a bullet after ricocheting a surface.
        /// </summary>
        [SerializeField]
        [Tooltip("Defines the trajectory variation of a bullet after ricocheting a surface.")]
        [Range(0, 1)]
        private float m_TrajectoryDeflection = 0.75f;

        /// <summary>
        /// Defines the density threshold so that a bullet can bounce off a surface.
        /// </summary>
        [SerializeField]
        [Tooltip("Defines the density threshold so that a bullet can bounce off a surface.")]
        [MinMax(0.001f, Mathf.Infinity)]
        private float m_RicochetDensityThreshold = 1;
        
        /// <summary>
        /// Allow the projectile to splitting/splintering after entering Target's body.
        /// </summary>
        [SerializeField]
        [Tooltip("Allow the projectile to splitting/splintering after entering Target's body.")]
        private bool m_Fragmentation = true;

        /// <summary>
        /// A likelihood of a bullet splitting/splintering after entering Target's body.
        /// </summary>
        [SerializeField]
        [Tooltip("A likelihood of a bullet splitting/splintering after entering Target's body.")]
        [Range(0, 1)]
        private float m_FragmentationChance = 0.1f;
    
        /// <summary>
        /// Maximum number of fragments a bullet can splinter into.
        /// </summary>
        [SerializeField]
        [Tooltip("Maximum number of fragments a bullet can splinter into.")]
        [MinMax(0, Mathf.Infinity)]
        private int m_MaxFragments = 3;
    
        /// <summary>
        /// Change in the movement direction of a bullet fragment because of a collision with another object.
        /// </summary>
        [SerializeField]
        [MinMaxSlider(-1, 1, "Change in the movement direction of a bullet fragment because of a collision with another object.", "F2")]
        private Vector2 m_FragmentScattering = new Vector2(-0.5f, 0.5f);
        
        /// <summary>
        /// Defines the density threshold so that a bullet can splinter into small fragments.
        /// </summary>
        [SerializeField]
        [Tooltip("Defines the density threshold so that a bullet can splinter into small fragments.")]
        [MinMax(0.001f, Mathf.Infinity)]
        private float m_FragmentationDensityThreshold = 0.25f;

        /// <summary>
        /// Projectiles that have Fragmented after impact deal bonus Damage to Targets.
        /// </summary>
        [SerializeField]
        [Tooltip("Projectiles that have Fragmented after impact deal high bonus Damage to Targets.")]
        [MinMax(1, Mathf.Infinity)]
        private float m_FragmentationDamageMultiplier = 1.5f;

        #endregion
    
        #region PROPERTIES

        /// <summary>
        /// Name of the ammunition.
        /// </summary>
        public string AmmunitionName => m_AmmunitionName;
    
        /// <summary>
        /// Bullet's mass in kilograms.
        /// </summary>
        public float ProjectileMass => m_ProjectileMass;
        
        /// <summary>
        /// Defines whether or not a projectile can penetrate the Target's body.
        /// </summary>
        public bool CanPenetrate => m_CanPenetrate;
    
        /// <summary>
        /// Defines the maximum distance that a projectile can travel penetrating an object. (Read Only)
        /// </summary>
        public float PenetrationPower => m_PenetrationPower;
        
        /// <summary>
        /// Refraction is the effect of altering the bullet path after transferring energy on the collision.
        /// </summary>
        public Vector2 Refraction => m_Refraction;
        
        /// <summary>
        /// The density influence defines how much the object density will affect the refraction in the bullet direction.
        /// </summary>
        public float DensityInfluence => m_DensityInfluence;
    
        /// <summary>
        /// Defines how the damage inflicted by the projectiles will be calculated. (Read Only)
        /// </summary>
        public DamageMode DamageType => m_DamageMode;
    
        /// <summary>
        /// Bullet's initial velocity in meters per second. (Read Only)
        /// </summary>
        public float ProjectileSpeed => m_ProjectileSpeed;

        /// <summary>
        /// Returns the damage inflicted by the projectile.
        /// </summary>
        public float Damage => Random.Range(m_Damage.x, m_Damage.y);

        /// <summary>
        /// Defines how the damage will be calculated based on the distance. (Read Only)
        /// </summary>
        public AnimationCurve DamageFalloffCurve => m_DamageFalloffCurve;
        
        /// <summary>
        /// Allow the projectile ricochet when not able to penetrate the target's body.
        /// </summary>
        public bool Ricochet => m_Ricochet;
    
        /// <summary>
        /// Chance of bullet bouncing off a surface when it is shot at an angle.
        /// </summary>
        public float RicochetChance => m_RicochetChance;
    
        /// <summary>
        /// Angle of incidence is the angle between a ray incident on a surface and the normal at the point of incidence.
        /// Maximum Incident Angle is the highest angle that a bullet can bounce off a surface. Any angle greater than this value
        /// will make the bullet penetrate the object.
        /// </summary>
        public float MaxIncidentAngle => m_MaxIncidentAngle;
    
        /// <summary>
        /// Defines the trajectory variation of a bullet after ricocheting a surface.
        /// </summary>
        public float TrajectoryDeflection => m_TrajectoryDeflection;
    
        /// <summary>
        /// Defines the density threshold so that a bullet can bounce off a surface.
        /// </summary>
        public float RicochetDensityThreshold => m_RicochetDensityThreshold;
        
        /// <summary>
        /// Allow the projectile to splitting/splintering after entering Target's body.
        /// </summary>
        public bool Fragmentation => m_Fragmentation;
    
        /// <summary>
        /// A likelihood of a bullet splitting/splintering after entering Target's body.
        /// </summary>
        public float FragmentationChance => m_FragmentationChance;
    
        /// <summary>
        /// Maximum number of fragments a bullet can splinter into.
        /// </summary>
        public int MaxFragments => m_MaxFragments;
    
        /// <summary>
        /// Change in the movement direction of a bullet fragment because of a collision with another object.
        /// </summary>
        public Vector2 FragmentScattering => m_FragmentScattering;
        
        /// <summary>
        /// Defines the density threshold so that a bullet can splinter into small fragments.
        /// </summary>
        public float FragmentationDensityThreshold => m_FragmentationDensityThreshold;
    
        /// <summary>
        /// Projectiles that have Fragmented after impact deal bonus Damage to Targets.
        /// </summary>
        public float FragmentationDamageMultiplier => m_FragmentationDamageMultiplier;
    
        #endregion

        /// <summary>
        /// Returns how much impact force will be applied by the projectile based on the distance.
        /// </summary>
        /// <param name="distance">Distance traveled by the projectile.</param>
        /// <returns></returns>
        public float CalculateImpactForce(float distance)
        {
            return Mathf.Min(0.5f * m_ProjectileMass * m_ProjectileSpeed * m_ProjectileSpeed / distance, m_MaximumImpactForce);
        }
    }
}
