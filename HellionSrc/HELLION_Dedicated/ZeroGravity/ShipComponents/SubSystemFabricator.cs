// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.SubSystemFabricator
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
  public class SubSystemFabricator : SubSystem
  {
    private List<ItemType> craftableItems;
    public Vector3D SpawnPosition;
    public Vector3D SpawnForward;
    public Vector3D SpawnUp;

    public SubSystemFabricator(SpaceObjectVessel vessel, VesselObjectID id, SubSystemData ssData)
      : base(vessel, id, ssData)
    {
    }

    public override SubSystemType Type
    {
      get
      {
        return SubSystemType.Fabricator;
      }
    }

    public void FabricateItem(ItemType type, CargoBay fromCargo)
    {
      if (!this.craftableItems.Contains(type))
        return;
      CargoCompartmentData cargoCompartmentData = fromCargo.Compartments.Find((Predicate<CargoCompartmentData>) (x => x.Type == CargoCompartmentType.RefinedResources));
      if (StaticData.ItemRecepiesList.ContainsKey(type))
      {
        foreach (KeyValuePair<ResourceType, float> keyValuePair in StaticData.ItemRecepiesList[type])
        {
          if ((double) System.Math.Abs(fromCargo.ChangeQuantityBy((int) cargoCompartmentData.ID, keyValuePair.Key, -keyValuePair.Value, true)) <= 1.40129846432482E-45)
            return;
        }
        DynamicObject.SpawnDynamicObject(type, GenericItemSubType.None, MachineryPartType.None, this.ParentVessel, -1, new Vector3D?(this.SpawnPosition), new Vector3D?(this.SpawnForward), new Vector3D?(this.SpawnUp));
      }
    }

    public override void SetAuxData(SubSystemAuxData auxData)
    {
      SubSystemFabricatorAuxData fabricatorAuxData = auxData as SubSystemFabricatorAuxData;
      this.craftableItems = fabricatorAuxData.CraftableItems;
      this.SpawnPosition = fabricatorAuxData.SpawnPosition.ToVector3D();
      this.SpawnForward = fabricatorAuxData.SpawnForward.ToVector3D();
      this.SpawnUp = fabricatorAuxData.SpawnUp.ToVector3D();
    }
  }
}
