// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.CargoBay
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using System.Linq;
using ZeroGravity.Data;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace ZeroGravity.ShipComponents
{
  public class CargoBay : ICargo, IPersistantObject
  {
    public SpaceObjectVessel ParentVessel;
    public int InSceneID;
    public List<CargoCompartmentData> CargoCompartments;

    public List<CargoCompartmentData> Compartments
    {
      get
      {
        return this.CargoCompartments;
      }
    }

    public CargoBay(SpaceObjectVessel vessel, List<CargoCompartmentData> cargoCompartments)
    {
      this.ParentVessel = vessel;
      this.CargoCompartments = cargoCompartments;
      foreach (CargoCompartmentData cargoCompartment in this.CargoCompartments)
      {
        foreach (CargoResourceData resource in cargoCompartment.Resources)
        {
          foreach (ResourcesSpawnSettings spawnSetting in resource.SpawnSettings)
          {
            if (vessel.CheckTag(spawnSetting.Tag, spawnSetting.Case))
            {
              float num1 = MathHelper.RandomRange(spawnSetting.MinQuantity, spawnSetting.MaxQuantity);
              float num2 = cargoCompartment.Capacity - cargoCompartment.Resources.Sum<CargoResourceData>((Func<CargoResourceData, float>) (m => m.Quantity));
              resource.Quantity = 0.0f;
              if ((double) num1 < 0.0)
                num1 = 0.0f;
              else if ((double) num1 > (double) num2)
                num1 = num2;
              resource.Quantity = num1;
              break;
            }
          }
        }
      }
      if (this.Compartments == null)
        return;
      foreach (CargoCompartmentData compartment in this.Compartments)
        compartment.Resources.RemoveAll((Predicate<CargoResourceData>) (m =>
        {
          if (m != null)
            return (double) m.Quantity <= 1.40129846432482E-45;
          return true;
        }));
    }

    public CargoBayDetails GetDetails()
    {
      List<CargoCompartmentDetails> compartmentDetailsList = new List<CargoCompartmentDetails>();
      foreach (CargoCompartmentData cargoCompartment in this.CargoCompartments)
        compartmentDetailsList.Add(new CargoCompartmentDetails()
        {
          ID = cargoCompartment.ID,
          Resources = cargoCompartment.Resources
        });
      return new CargoBayDetails()
      {
        InSceneID = this.InSceneID,
        CargoCompartments = compartmentDetailsList
      };
    }

    public CargoCompartmentData GetCompartment(int? id = null)
    {
      if (id.HasValue)
        return this.CargoCompartments.Find((Predicate<CargoCompartmentData>) (m => (int) m.ID == id.Value));
      return this.CargoCompartments[0];
    }

    public float ChangeQuantityBy(int compartmentID, ResourceType resourceType, float quantity, bool wholeAmount = false)
    {
      CargoCompartmentData cargoCompartmentData = this.Compartments.Find((Predicate<CargoCompartmentData>) (m => (int) m.ID == compartmentID));
      CargoResourceData cargoResourceData = cargoCompartmentData.Resources.Find((Predicate<CargoResourceData>) (m => m.ResourceType == resourceType));
      if (cargoResourceData == null)
      {
        if (wholeAmount)
          return 0.0f;
        cargoResourceData = new CargoResourceData()
        {
          ResourceType = resourceType,
          Quantity = 0.0f
        };
        cargoCompartmentData.Resources.Add(cargoResourceData);
      }
      float num1 = cargoCompartmentData.Capacity - cargoCompartmentData.Resources.Sum<CargoResourceData>((Func<CargoResourceData, float>) (m => m.Quantity));
      float num2 = quantity;
      float quantity1 = cargoResourceData.Quantity;
      if (wholeAmount && (double) quantity1 - (double) quantity < 1.40129846432482E-45)
        return 0.0f;
      if ((double) quantity > 0.0 && (double) quantity > (double) num1)
        num2 = num1;
      else if ((double) quantity < 0.0 && -(double) num2 > (double) quantity1)
        num2 = -quantity1;
      cargoResourceData.Quantity = quantity1 + num2;
      if ((double) cargoResourceData.Quantity <= 1.40129846432482E-45)
        cargoCompartmentData.Resources.Remove(cargoResourceData);
      NetworkController networkController = Server.Instance.NetworkController;
      ShipStatsMessage shipStatsMessage = new ShipStatsMessage();
      shipStatsMessage.GUID = this.ParentVessel.GUID;
      shipStatsMessage.Temperature = new float?(this.ParentVessel.Temperature);
      shipStatsMessage.Health = new float?(this.ParentVessel.Health);
      shipStatsMessage.VesselObjects = new VesselObjects()
      {
        CargoBays = new List<CargoBayDetails>()
        {
          this.GetDetails()
        }
      };
      long skipPlalerGUID = -1;
      SpaceObject[] spaceObjectArray = new SpaceObject[1]
      {
        (SpaceObject) this.ParentVessel
      };
      networkController.SendToClientsSubscribedTo((NetworkData) shipStatsMessage, skipPlalerGUID, spaceObjectArray);
      return num2;
    }

    public PersistenceObjectData GetPersistenceData()
    {
      return (PersistenceObjectData) new PersistenceObjectDataCargo()
      {
        InSceneID = this.InSceneID,
        CargoCompartments = this.CargoCompartments
      };
    }

    public void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        PersistenceObjectDataCargo persistenceObjectDataCargo = persistenceData as PersistenceObjectDataCargo;
        if (persistenceObjectDataCargo == null)
        {
          Dbg.Warning("PersistenceObjectDataCargo data is null");
        }
        else
        {
          this.CargoCompartments = persistenceObjectDataCargo.CargoCompartments;
          foreach (CargoCompartmentData cargoCompartment in this.CargoCompartments)
            cargoCompartment.Resources.RemoveAll((Predicate<CargoResourceData>) (m => (double) m.Quantity <= 1.40129846432482E-45));
        }
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }
  }
}
