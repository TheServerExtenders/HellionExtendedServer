// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.PlayerStats
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;
using System.Linq;
using System.Timers;
using ZeroGravity.Math;
using ZeroGravity.Network;

namespace ZeroGravity.Objects
{
  public class PlayerStats
  {
    public bool GodMode = false;
    public float HealthPoints = 100f;
    public float MaxHealthPoints = 100f;
    public Dictionary<int, PlayerStats.HitInfo> hitQueue = new Dictionary<int, PlayerStats.HitInfo>();
    private int hitIdentyfy = 0;
    private PlayerStatsMessage psm = new PlayerStatsMessage();
    public double lastMeleeTime;
    public const float MeleeDamage = 30f;
    public const float MeleeRateOfFire = 1f;
    public Player pl;
    private float acummulatedDamage;
    private Timer healTimer;
    private float amountToHeal;
    private float amountToHealStep;

    public PlayerStats()
    {
      this.healTimer = new Timer(100.0);
      this.healTimer.Enabled = false;
      this.healTimer.Elapsed += (ElapsedEventHandler) ((sender, args) => this.HealOverTimeStep());
    }

    public int QueueHit(PlayerStats.HitBoxType colType, float damage, Vector3D direction, bool isMelee)
    {
      this.hitIdentyfy = this.hitIdentyfy + 1;
      this.hitQueue.Add(this.hitIdentyfy, new PlayerStats.HitInfo()
      {
        damage = damage,
        hitType = colType,
        direction = direction,
        isMelee = isMelee
      });
      Timer timer = new Timer(1000.0);
      int tmpHitIdentifier = this.hitIdentyfy;
      timer.Elapsed += (ElapsedEventHandler) ((sender, args) => PlayerStats.WaitForHit(sender, tmpHitIdentifier));
      return this.hitIdentyfy;
    }

    public void UnqueueHit(int hitId)
    {
      if (!this.hitQueue.ContainsKey(hitId))
        return;
      this.hitQueue.Remove(hitId);
    }

    private static void WaitForHit(object source, int id)
    {
      PlayerStats playerStats = source as PlayerStats;
      if (!playerStats.hitQueue.ContainsKey(id))
        return;
      playerStats.TakeHitDamage(id);
    }

    public void SendAcumulatedMessage(double time)
    {
      Server.Instance.NetworkController.SendToGameClient(this.pl.GUID, (NetworkData) this.psm);
    }

    public void TakeDammage(float shot = 0.0f, Vector3D? shotDirection = null, float heat = 0.0f, float frost = 0.0f, float suffocate = 0.0f, float impact = 0.0f, float pressure = 0.0f)
    {
      if (this.GodMode || this.pl.CurrentSpawnPoint != null && this.pl.CurrentSpawnPoint.Executer != null && this.pl.CurrentSpawnPoint.IsPlayerInSpawnPoint)
        return;
      shot = (double) shot > 0.0 ? shot : 0.0f;
      heat = (double) heat > 0.0 ? heat : 0.0f;
      frost = (double) frost > 0.0 ? frost : 0.0f;
      suffocate = (double) suffocate > 0.0 ? suffocate : 0.0f;
      pressure = (double) pressure > 0.0 ? pressure : 0.0f;
      impact = (double) impact > 0.0 ? impact : 0.0f;
      float num = shot + heat + frost + suffocate + impact + pressure;
      SortedDictionary<float, CauseOfDeath> sortedDictionary = new SortedDictionary<float, CauseOfDeath>();
      sortedDictionary[shot] = CauseOfDeath.Shot;
      sortedDictionary[suffocate] = CauseOfDeath.Suffocate;
      sortedDictionary[pressure] = CauseOfDeath.Pressure;
      sortedDictionary[frost] = CauseOfDeath.Frost;
      sortedDictionary[heat] = CauseOfDeath.Heat;
      sortedDictionary[impact] = CauseOfDeath.Impact;
      sortedDictionary[float.Epsilon] = CauseOfDeath.None;
      CauseOfDeath causeOfdeath = sortedDictionary.Values.Last<CauseOfDeath>();
      if ((double) num <= 1.40129846432482E-45)
        return;
      this.HealthPoints = MathHelper.Clamp(this.HealthPoints - num, 0.0f, this.MaxHealthPoints);
      if ((double) this.HealthPoints <= 1.40129846432482E-45)
      {
        this.pl.KillYourself(causeOfdeath, true);
      }
      else
      {
        this.acummulatedDamage = this.acummulatedDamage + num;
        this.psm.ShotDammage = shot;
        this.psm.ShotDirection = shotDirection.HasValue ? shotDirection.Value.ToFloatArray() : (float[]) null;
        this.psm.HeatDammage += heat;
        this.psm.FrostDammage += frost;
        this.psm.SuffocateDammage += suffocate;
        this.psm.InpactDammage += impact;
        this.psm.PressureDammage += pressure;
        if ((double) this.acummulatedDamage <= 1.0)
          return;
        this.psm.GUID = this.pl.FakeGuid;
        this.psm.HealthPoints = (int) this.HealthPoints;
        Server.Instance.NetworkController.SendToGameClient(this.pl.GUID, (NetworkData) this.psm);
        this.psm = new PlayerStatsMessage();
        this.acummulatedDamage = 0.0f;
      }
    }

    public void Heal(float amount)
    {
      amount = (double) amount > 0.0 ? amount : 0.0f;
      if ((double) amount <= 1.40129846432482E-45 || (double) this.HealthPoints == (double) this.MaxHealthPoints)
        return;
      this.HealthPoints = MathHelper.Clamp(this.HealthPoints + amount, 0.0f, this.MaxHealthPoints);
      Server.Instance.NetworkController.SendToGameClient(this.pl.GUID, (NetworkData) new PlayerStatsMessage()
      {
        GUID = this.pl.FakeGuid,
        HealthPoints = (int) this.HealthPoints
      });
    }

    private void HealOverTimeStep()
    {
      this.amountToHeal = this.amountToHeal - this.amountToHealStep;
      float amountToHealStep = this.amountToHealStep;
      if ((double) this.amountToHeal <= 0.0)
      {
        float num = amountToHealStep + this.amountToHeal;
        this.Heal(this.amountToHealStep);
        this.healTimer.Enabled = false;
      }
      else
        this.Heal(this.amountToHealStep);
    }

    public void HealOverTime(float amountOverSec, float duration)
    {
      if (this.healTimer.Enabled)
      {
        this.amountToHeal = this.amountToHeal + amountOverSec * duration;
        this.amountToHealStep = (float) (((double) this.amountToHealStep + (double) amountOverSec * 0.100000001490116) * 0.5);
      }
      else
      {
        this.amountToHeal = amountOverSec * duration;
        this.amountToHealStep = amountOverSec * 0.1f;
        this.healTimer.Enabled = true;
      }
    }

    public void DoCollisionDamage(float speed)
    {
      double num = 5.0;
      float impact = 0.0f;
      if ((double) speed >= num)
        impact = ((float) (((double) speed - num) * ((double) speed - num) / 10.0) + speed) * (this.pl.PlayerInventory.CurrOutfit != null ? this.pl.PlayerInventory.CurrOutfit.CollisionResistance : 1f);
      this.TakeDammage(0.0f, new Vector3D?(), 0.0f, 0.0f, 0.0f, impact, 0.0f);
    }

    public bool TakeHitDamage(int id)
    {
      if (this.GodMode || this.pl.CurrentSpawnPoint != null && this.pl.CurrentSpawnPoint.Executer != null && this.pl.CurrentSpawnPoint.IsPlayerInSpawnPoint)
      {
        if (!this.hitQueue.ContainsKey(id))
          ;
        return false;
      }
      if (!this.hitQueue.ContainsKey(id))
        return false;
      Outfit currOutfit = this.pl.PlayerInventory.CurrOutfit;
      Helmet currentHelmet = this.pl.CurrentHelmet;
      float num1 = 1f;
      float num2 = 0.0f;
      float num3 = 1f;
      switch (this.hitQueue[id].hitType)
      {
        case PlayerStats.HitBoxType.None:
          num1 = 0.0f;
          Dbg.Error((object) "UNKNOWN HITBOX TYPE", (object) this.pl.GUID);
          break;
        case PlayerStats.HitBoxType.Head:
          num3 = 10f;
          num1 = currentHelmet != null ? currentHelmet.DamageResistance : 1f;
          num2 = currentHelmet != null ? currentHelmet.DamageReduction : 0.0f;
          break;
        case PlayerStats.HitBoxType.Torso:
          num3 = 5f;
          if (currOutfit != null)
          {
            num1 = currOutfit.DamageResistanceTorso;
            num2 = currOutfit.DamageReductionTorso;
            break;
          }
          break;
        case PlayerStats.HitBoxType.Arms:
          num3 = 1f;
          if (currOutfit != null)
          {
            num1 = currOutfit.DamageResistanceArms;
            num2 = currOutfit.DamageReductionArms;
            break;
          }
          break;
        case PlayerStats.HitBoxType.Legs:
          num3 = 1f;
          if (currOutfit != null)
          {
            num1 = currOutfit.DamageResistanceLegs;
            num2 = currOutfit.DamageReductionLegs;
            break;
          }
          break;
        case PlayerStats.HitBoxType.Abdomen:
          num3 = 2f;
          if (currOutfit != null)
          {
            num1 = currOutfit.DamageResistanceAbdomen;
            num2 = currOutfit.DamageReductionAbdomen;
            break;
          }
          break;
        default:
          Dbg.Error((object) "UNKNOWN HITBOX TYPE DEFAULT", (object) this.pl.GUID);
          break;
      }
      if (this.hitQueue[id].isMelee)
        num3 = 1f;
      this.TakeDammage((this.hitQueue[id].damage - num2) * num1 * num3, new Vector3D?(this.hitQueue[id].direction), 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
      this.UnqueueHit(id);
      return true;
    }

    public class HitInfo
    {
      public float damage;
      public bool isMelee;
      public PlayerStats.HitBoxType hitType;
      public Vector3D direction;
    }

    public enum HitBoxType
    {
      None = -1,
      Head = 0,
      Torso = 1,
      Arms = 2,
      Legs = 3,
      Abdomen = 4,
    }
  }
}
