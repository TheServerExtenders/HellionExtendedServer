// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.PortableTurret
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Math;
using ZeroGravity.Network;

namespace ZeroGravity.Objects
{
  internal class PortableTurret : Item
  {
    private Player targetPlayer = (Player) null;
    public bool isActive;
    private PortableTurretStats _stats;
    public bool isStunned;

    public override DynamicObjectStats StatsNew
    {
      get
      {
        return (DynamicObjectStats) this._stats;
      }
    }

    public PortableTurret(IDynamicObjectAuxData data)
    {
      this._stats = new PortableTurretStats();
      Server.Instance.NetworkController.EventSystem.AddListener(typeof (PortableTurretShootingMessage), new EventSystem.NetworkDataDelegate(this.PortableTurretShootingMessageListener));
      if (data == null)
        return;
      this.SetData(data as PortableTurretData);
    }

    public override bool ChangeStats(DynamicObjectStats stats)
    {
      PortableTurretStats portableTurretStats = stats as PortableTurretStats;
      if (portableTurretStats.IsActive.HasValue)
        this.isActive = portableTurretStats.IsActive.Value;
      this.DynamicObj.SendStatsToClient();
      return false;
    }

    public override PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataPortableTurret dataPortableTurret = new PersistenceObjectDataPortableTurret();
      this.FillPersistenceData((PersistenceObjectDataItem) dataPortableTurret);
      dataPortableTurret.PortableTurretData = new PortableTurretData();
      dataPortableTurret.PortableTurretData.ItemType = this.Type;
      dataPortableTurret.PortableTurretData.IsActive = this.isActive;
      return (PersistenceObjectData) dataPortableTurret;
    }

    public override void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        base.LoadPersistenceData(persistenceData);
        PersistenceObjectDataPortableTurret dataPortableTurret = persistenceData as PersistenceObjectDataPortableTurret;
        if (dataPortableTurret == null)
          Dbg.Warning((object) "PersistenceObjectDataPortableTurret data is null", (object) this.GUID);
        else
          this.SetData(dataPortableTurret.PortableTurretData);
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }

    private void SetData(PortableTurretData ptd)
    {
      this.isActive = ptd.IsActive;
    }

    private void PortableTurretShootingMessageListener(NetworkData data)
    {
      PortableTurretShootingMessage turretShootingMessage = data as PortableTurretShootingMessage;
      if (turretShootingMessage.TurretGUID != this.GUID || this.isStunned)
        return;
      this.targetPlayer = Server.Instance.GetPlayer(turretShootingMessage.Sender);
      Server.Instance.NetworkController.SendToClientsSubscribedTo((NetworkData) turretShootingMessage, -1L, this.targetPlayer.Parent);
      if (turretShootingMessage.IsShooting)
        Server.Instance.SubscribeToTimer(UpdateTimer.TimerStep.Step_0_1_sec, new UpdateTimer.TimeStepDelegate(this.DamagePlayer));
      else
        Server.Instance.UnsubscribeFromTimer(UpdateTimer.TimerStep.Step_0_1_sec, new UpdateTimer.TimeStepDelegate(this.DamagePlayer));
    }

    public void UnStun()
    {
      if (this.DynamicObj.Parent == null)
        return;
      this.isStunned = false;
      this.StatsNew.IsStunned = new bool?(false);
      this.DynamicObj.SendStatsToClient();
    }

    public override void TakeDamage(Dictionary<TypeOfDamage, float> damages)
    {
      float num = 0.0f;
      foreach (KeyValuePair<TypeOfDamage, float> damage in damages)
      {
        num += damage.Value;
        if (damage.Key == TypeOfDamage.EMP)
        {
          this.isStunned = true;
          Extensions.Invoke((Action) (() => this.UnStun()), 10.0);
          this.StatsNew.IsStunned = new bool?(this.isStunned);
        }
      }
      this.Health = this.Health - num;
      this.StatsNew.Health = new float?(this.Health);
      this.StatsNew.Damages = damages;
      this.DynamicObj.SendStatsToClient();
    }

    private void DamagePlayer(double n)
    {
      if (this.targetPlayer != null && this.targetPlayer.IsAlive)
        this.targetPlayer.Stats.TakeDammage((float) (20.0 * n), new Vector3D?(), 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
      else
        Server.Instance.UnsubscribeFromTimer(UpdateTimer.TimerStep.Step_0_1_sec, new UpdateTimer.TimeStepDelegate(this.DamagePlayer));
    }
  }
}
