// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.SubSystemFTL
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace ZeroGravity.ShipComponents
{
  public class SubSystemFTL : SubSystem
  {
    public Dictionary<int, VesselObjectID> WarpCellSlots = new Dictionary<int, VesselObjectID>();
    private bool canGoOnline = true;
    public WarpData[] WarpsData;

    public override SubSystemType Type
    {
      get
      {
        return SubSystemType.FTL;
      }
    }

    public override bool AutoReactivate
    {
      get
      {
        return false;
      }
    }

    public SubSystemFTL(SpaceObjectVessel vessel, VesselObjectID id, SubSystemData ssData)
      : base(vessel, id, ssData)
    {
      this._OperationRate = 0.0f;
    }

    public override void FitPartToSlot(VesselObjectID slotKey, MachineryPart part)
    {
      base.FitPartToSlot(slotKey, part);
      this.StatusChanged = true;
    }

    public override void RemovePartFromSlot(VesselObjectID slotKey)
    {
      base.RemovePartFromSlot(slotKey);
      this.StatusChanged = true;
    }

    public override void InitMachineryPartSlot(VesselObjectID slotKey, MachineryPart part, MachineryPartSlotData partSlotData)
    {
      base.InitMachineryPartSlot(slotKey, part, partSlotData);
      if (partSlotData.SlotType != MachineryPartType.WarpCell)
        return;
      this.WarpCellSlots[partSlotData.SlotIndex] = slotKey;
    }

    public override void SetAuxData(SubSystemAuxData auxData)
    {
      this.WarpsData = (auxData as SubSystemFTLAuxData).WarpsData;
    }

    public override IAuxDetails GetAuxDetails()
    {
      return (IAuxDetails) new FTLAuxDetails()
      {
        WarpCellsFuel = this.GetWarpCellsFuel()
      };
    }

    public Dictionary<int, float?> GetWarpCellsFuel()
    {
      Dictionary<int, float?> dictionary = new Dictionary<int, float?>();
      foreach (KeyValuePair<int, VesselObjectID> warpCellSlot in this.WarpCellSlots)
      {
        MachineryPart machineryPart = (MachineryPart) null;
        this.machineryParts.TryGetValue(warpCellSlot.Value, out machineryPart);
        dictionary[warpCellSlot.Key] = machineryPart == null ? new float?() : new float?(machineryPart.Health);
      }
      return dictionary;
    }

    public void ConsumeWarpResources(List<int> slots, float warpFuel)
    {
      this.ParentVessel.DistributionManager.Capacitor.Capacity = MathHelper.Clamp(this.ParentVessel.DistributionManager.Capacitor.Capacity - this.ResourceRequirements[DistributionSystemType.Power].Nominal, 0.0f, this.ParentVessel.DistributionManager.Capacitor.MaxCapacity);
      List<MachineryPart> machineryPartList = new List<MachineryPart>();
      foreach (int slot in slots)
      {
        VesselObjectID key = (VesselObjectID) null;
        if (this.WarpCellSlots.TryGetValue(slot, out key))
        {
          MachineryPart machineryPart = (MachineryPart) null;
          this.machineryParts.TryGetValue(key, out machineryPart);
          if (machineryPart != null)
            machineryPartList.Add(machineryPart);
        }
      }
      machineryPartList.Sort((Comparison<MachineryPart>) ((x, y) =>
      {
        if ((double) x.Health == (double) y.Health)
          return 0;
        return (double) x.Health <= (double) y.Health ? -1 : 1;
      }));
      foreach (MachineryPart machineryPart in machineryPartList)
      {
        if ((double) machineryPart.Health < (double) warpFuel)
        {
          warpFuel -= machineryPart.Health;
          machineryPart.Health = 0.0f;
        }
        else
        {
          machineryPart.Health -= (float) (double) warpFuel;
          warpFuel = 0.0f;
        }
        machineryPart.DynamicObj.SendStatsToClient();
      }
    }

    public override void Update(double duration)
    {
      base.Update(duration);
      bool flag = true;
      foreach (KeyValuePair<VesselObjectID, MachineryPartSlotData> machineryPartSlot in this.machineryPartSlots)
      {
        MachineryPartType slotType = machineryPartSlot.Value.SlotType;
        float basicModifier = machineryPartSlot.Value.BasicModifier;
        bool isActive = machineryPartSlot.Value.IsActive;
        MachineryPart machineryPart = this.machineryParts[machineryPartSlot.Key];
        if (slotType == MachineryPartType.SingularityCellDetonator)
        {
          flag = false;
          this.powerUpFactor = 1f + basicModifier;
          if (machineryPart != null & isActive && (double) machineryPart.MaxHealth > 0.0)
          {
            flag = true;
            this.powerUpFactor = this.powerUpFactor - basicModifier * (machineryPart.Health / machineryPart.MaxHealth) * machineryPart.BoostFactor;
          }
        }
      }
      this.canGoOnline = flag;
      this.cooldownFactor = this.powerUpFactor;
    }

    public override bool CheckAvailableResources(float consumptionFactor, float duration, bool standby, ref Dictionary<IResourceProvider, float> reservedCapacities, ref Dictionary<ResourceContainer, float> reservedQuantities, ref string debugText)
    {
      if (!this.canGoOnline || this.Defective)
        return false;
      return base.CheckAvailableResources(consumptionFactor, duration, standby, ref reservedCapacities, ref reservedQuantities, ref debugText);
    }
  }
}
