// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.SubSystemRefinery
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
  public class SubSystemRefinery : SubSystem
  {
    public int CargoBayInSceneID;
    public float Capacity;
    public float ProcessingTime;
    public List<RefinedResourcesData> Resources;
    public float _CurrentProgress;
    private double unitsPerSec;
    private float quantityLeft;
    private float quantity;
    private ResourceType resourceType;
    private CargoBay targetCargo;

    public override float PowerUpTime
    {
      get
      {
        return 0.0f;
      }
    }

    public float CurrentProgress
    {
      get
      {
        return this._CurrentProgress;
      }
      set
      {
        if ((double) value == (double) this._CurrentProgress)
          return;
        this.StatusChanged = true;
        this._CurrentProgress = value;
      }
    }

    public override SubSystemType Type
    {
      get
      {
        return SubSystemType.Refinery;
      }
    }

    public SubSystemRefinery(SpaceObjectVessel vessel, VesselObjectID id, SubSystemData ssData)
      : base(vessel, id, ssData)
    {
    }

    public override void SetAuxData(SubSystemAuxData auxData)
    {
      SubSystemRefineryAuxData systemRefineryAuxData = auxData as SubSystemRefineryAuxData;
      this.CargoBayInSceneID = systemRefineryAuxData.CargoBayInSceneID;
      this.Capacity = systemRefineryAuxData.Capacity;
      this.ProcessingTime = systemRefineryAuxData.ProcessingTime;
      this.Resources = systemRefineryAuxData.Resources;
      this.unitsPerSec = (double) this.Capacity / (double) this.ProcessingTime;
    }

    public override IAuxDetails GetAuxDetails()
    {
      return (IAuxDetails) new RefineryAuxDetails()
      {
        CurrentProgress = this.CurrentProgress
      };
    }

    public override void Update(double duration)
    {
      base.Update(duration);
      if (this.Status != SystemStatus.OnLine)
        return;
      this.quantityLeft = this.quantityLeft - (float) (this.unitsPerSec * duration);
      this.CurrentProgress = MathHelper.Clamp((float) (1.0 - (double) this.quantityLeft / (double) this.quantity), 0.0f, 1f);
      if ((double) this.quantityLeft <= 0.0)
      {
        RefinedResourcesData refinedResourcesData = this.Resources.Find((Predicate<RefinedResourcesData>) (m => m.RawResource == this.resourceType));
        if (refinedResourcesData != null)
        {
          foreach (CargoResourceData refinedResource in refinedResourcesData.RefinedResources)
          {
            foreach (CargoCompartmentData compartment in this.targetCargo.Compartments)
            {
              if (compartment.AllowedResources != null && compartment.AllowedResources.Contains(refinedResource.ResourceType))
              {
                double num1 = (double) this.targetCargo.ChangeQuantityBy((int) compartment.ID, ResourceType.Reserved, -refinedResource.Quantity * this.quantity, false);
                double num2 = (double) this.targetCargo.ChangeQuantityBy((int) compartment.ID, refinedResource.ResourceType, refinedResource.Quantity * this.quantity, false);
                break;
              }
            }
          }
        }
        this.GoOffLine(false, false);
      }
    }

    public void Refine(RefineResourceMessage rrMsg, ICargo fromCargo, CargoBay toCargo)
    {
      if (this.Status != SystemStatus.OffLine)
        return;
      this.GoOnLine();
      this.quantity = -fromCargo.ChangeQuantityBy(rrMsg.FromCompartmentID, rrMsg.ResourceType, -rrMsg.Quantity, false);
      this.resourceType = rrMsg.ResourceType;
      this.quantityLeft = this.quantity;
      this.targetCargo = toCargo;
      this.CurrentProgress = 0.0f;
      foreach (CargoResourceData refinedResource in this.Resources.Find((Predicate<RefinedResourcesData>) (m => m.RawResource == rrMsg.ResourceType)).RefinedResources)
      {
        foreach (CargoCompartmentData compartment in toCargo.Compartments)
        {
          if (compartment.AllowedResources != null && compartment.AllowedResources.Contains(refinedResource.ResourceType))
          {
            double num = (double) toCargo.ChangeQuantityBy((int) compartment.ID, ResourceType.Reserved, refinedResource.Quantity * this.quantity, false);
            break;
          }
        }
      }
    }
  }
}
