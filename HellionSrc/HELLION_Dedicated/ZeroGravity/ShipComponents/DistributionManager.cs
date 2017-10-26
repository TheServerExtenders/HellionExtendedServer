// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.DistributionManager
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using ZeroGravity.Data;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace ZeroGravity.ShipComponents
{
  public class DistributionManager
  {
    private Dictionary<IResourceProvider, DistributionManager.DistributionNode> resourceProviderNodes = new Dictionary<IResourceProvider, DistributionManager.DistributionNode>();
    private Dictionary<VesselObjectID, Generator> idGenerators = new Dictionary<VesselObjectID, Generator>();
    private Dictionary<VesselObjectID, VesselComponent> idMachineryPartSlots = new Dictionary<VesselObjectID, VesselComponent>();
    private Dictionary<VesselObjectID, SubSystem> idSubSystems = new Dictionary<VesselObjectID, SubSystem>();
    private Dictionary<VesselObjectID, Room> idRooms = new Dictionary<VesselObjectID, Room>();
    private Dictionary<VesselObjectID, ResourceContainer> idResourceContainers = new Dictionary<VesselObjectID, ResourceContainer>();
    private Dictionary<VesselObjectID, Door> idDoors = new Dictionary<VesselObjectID, Door>();
    private Dictionary<long, DistributionManager.DistributionNode> distributionNodes = new Dictionary<long, DistributionManager.DistributionNode>();
    private Dictionary<Room, DistributionManager.CompoundRoom> compoundRooms = new Dictionary<Room, DistributionManager.CompoundRoom>();
    private Dictionary<Player, Room> playerRooms = new Dictionary<Player, Room>();
    public Dictionary<DistributionSystemType, float> AvailableResourceCapacities = new Dictionary<DistributionSystemType, float>();
    public Dictionary<DistributionSystemType, float> AvailableResourceQuantities = new Dictionary<DistributionSystemType, float>();
    private Dictionary<VesselObjectID, PDUGroup> pduGroups = new Dictionary<VesselObjectID, PDUGroup>();
    private bool isCompoundDM = false;
    public SubSystemRCS RCS = (SubSystemRCS) null;
    public SubSystemEngine Engine = (SubSystemEngine) null;
    public SubSystemFTL FTL = (SubSystemFTL) null;
    public SubSystemRefinery Refinery = (SubSystemRefinery) null;
    public SubSystemFabricator Fabricator = (SubSystemFabricator) null;
    public GeneratorCapacitor Capacitor = (GeneratorCapacitor) null;
    private SpaceObjectVessel parentVessel = (SpaceObjectVessel) null;
    private static Dictionary<short, StructureSceneData> structureDefs = new Dictionary<short, StructureSceneData>();
    private DateTime prevSystemsUpdateTime;
    private SubSystemPDU pdu;

    static DistributionManager()
    {
      foreach (StructureSceneData structuresData in StaticData.StructuresDataList)
        DistributionManager.structureDefs[structuresData.ItemID] = structuresData;
    }

    public float PressureEquilizationTime(VesselObjectID doorID, out int direction, out float airSpeed)
    {
      airSpeed = 0.0f;
      direction = 0;
      Door door1 = this.GetDoor(doorID);
      if (door1 == null)
        return 0.0f;
      float num1 = door1.PassageArea;
      DistributionManager.CompoundRoom compoundRoom1 = (DistributionManager.CompoundRoom) null;
      DistributionManager.CompoundRoom compoundRoom2 = (DistributionManager.CompoundRoom) null;
      float num2 = 0.0f;
      float num3 = 1f;
      if (door1.Room1 != null)
        this.compoundRooms.TryGetValue(door1.Room1, out compoundRoom1);
      if (door1.Room2 != null)
        this.compoundRooms.TryGetValue(door1.Room2, out compoundRoom2);
      if (compoundRoom1 == compoundRoom2)
        return 0.0f;
      if (compoundRoom1 != null && compoundRoom2 != null)
      {
        float num4 = 0.0f;
        if ((double) compoundRoom1.AirPressure <= 1.40129846432482E-45)
        {
          foreach (Door door2 in compoundRoom1.AirConsumers.FindAll((Predicate<IAirConsumer>) (m =>
          {
            if (m is Door && (m as Door).isExternal)
              return !(m as Door).IsSealed;
            return false;
          })))
            num4 += door2.PassageArea;
          if ((double) num1 > (double) num4 && (double) num4 > 0.0)
            num1 = num4;
          direction = 2;
          num2 = compoundRoom2.Volume * compoundRoom2.AirPressure;
          num3 = (float) (0.61 * (double) num1 * System.Math.Sqrt(2.0 * (double) compoundRoom2.AirPressure * 100000.0 / 1.225));
        }
        else if ((double) compoundRoom2.AirPressure <= 1.40129846432482E-45)
        {
          foreach (Door door2 in compoundRoom2.AirConsumers.FindAll((Predicate<IAirConsumer>) (m =>
          {
            if (m is Door && (m as Door).isExternal)
              return !(m as Door).IsSealed;
            return false;
          })))
            num4 += door2.PassageArea;
          if ((double) num1 > (double) num4 && (double) num4 > 0.0)
            num1 = num4;
          direction = 1;
          num2 = compoundRoom1.Volume * compoundRoom1.AirPressure;
          num3 = (float) (0.61 * (double) num1 * System.Math.Sqrt(2.0 * (double) compoundRoom1.AirPressure * 100000.0 / 1.225));
        }
        if (direction == 0)
        {
          direction = (double) compoundRoom1.AirPressure <= (double) compoundRoom2.AirPressure ? 2 : 1;
          num2 = System.Math.Abs((float) (((double) compoundRoom2.Volume * ((double) compoundRoom1.Volume * (double) compoundRoom1.AirPressure) - (double) compoundRoom1.Volume * ((double) compoundRoom2.Volume * (double) compoundRoom2.AirPressure)) / ((double) compoundRoom1.Volume + (double) compoundRoom2.Volume)));
          num3 = (float) (0.61 * (double) num1 * System.Math.Sqrt(2.0 * System.Math.Abs(((double) compoundRoom1.AirPressure - (double) compoundRoom2.AirPressure) * 100000.0 / 1.225)));
        }
      }
      else if (compoundRoom1 != null && (double) compoundRoom1.AirPressure > 1.40129846432482E-45)
      {
        direction = 1;
        num2 = compoundRoom1.Volume * compoundRoom1.AirPressure;
        num3 = (float) (0.61 * (double) num1 * System.Math.Sqrt(2.0 * (double) compoundRoom1.AirPressure * 100000.0 / 1.225));
      }
      else if (compoundRoom2 != null && (double) compoundRoom2.AirPressure > 1.40129846432482E-45)
      {
        direction = 2;
        num2 = compoundRoom2.Volume * compoundRoom2.AirPressure;
        num3 = (float) (0.61 * (double) num1 * System.Math.Sqrt(2.0 * (double) compoundRoom2.AirPressure * 100000.0 / 1.225));
      }
      float num5;
      if ((double) num3 <= 1.40129846432482E-45 || (double) (num5 = num2 / num3) < 0.05)
      {
        direction = 0;
        return 0.0f;
      }
      airSpeed = num3 * (float) (2.0 / ((double) num1 * (double) num1));
      return num5;
    }

    private void initInstance()
    {
    }

    public DistributionManager(SpaceObjectVessel vessel, bool linkDockedVessels = false)
    {
      this.isCompoundDM = linkDockedVessels;
      this.initInstance();
      this.addShipDataStructure(vessel, vessel.VesselData.SceneID);
      if (this.isCompoundDM)
        this.linkNodesInDockedVessels(vessel, new List<SpaceObjectVessel>());
      this.parentVessel = vessel;
      if (this.pdu == null)
        this.pdu = new SubSystemPDU(vessel, new VesselObjectID(vessel.GUID, int.MaxValue), new SubSystemData()
        {
          Status = SystemStatus.OnLine
        });
      List<KeyValuePair<VesselObjectID, PDUGroup>> list = this.pduGroups.ToList<KeyValuePair<VesselObjectID, PDUGroup>>();
      list.Sort((Comparison<KeyValuePair<VesselObjectID, PDUGroup>>) ((pair1, pair2) => pair1.Value.CompareTo(pair2.Value)));
      this.pdu.PDUGroups = new List<VesselObjectID>();
      foreach (KeyValuePair<VesselObjectID, PDUGroup> keyValuePair in list)
        this.pdu.PDUGroups.Add(keyValuePair.Key);
      this.pdu.LimiterIndex = this.pdu.PDUGroups.Count;
      this.prevSystemsUpdateTime = DateTime.UtcNow;
      this.updateConnections();
    }

    private void linkNodesInDockedVessels(SpaceObjectVessel vessel, List<SpaceObjectVessel> traversedVessels)
    {
      if (traversedVessels.Contains(vessel))
        return;
      traversedVessels.Add(vessel);
      foreach (VesselDockingPort dockingPort in vessel.DockingPorts)
      {
        VesselDockingPort port = dockingPort;
        if (port.DockingStatus)
        {
          DistributionManager.DistributionNode distributionNode1 = this.distributionNodes[vessel.GUID];
          VesselDockingPort vesselDockingPort = port.DockedVessel.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == port.DockedToID.InSceneID));
          DistributionManager.DistributionNode distributionNode2 = this.distributionNodes[port.DockedVessel.GUID];
          this.linkDistributionNodes(distributionNode1, distributionNode2);
          foreach (short doorsId1 in port.DoorsIDs)
          {
            VesselObjectID sid1 = new VesselObjectID(vessel.GUID, (int) doorsId1);
            VesselObjectID vesselObjectId = (VesselObjectID) null;
            Door door1 = this.GetDoor(sid1);
            if (door1 != null && door1.isExternal)
            {
              double num = 100.0;
              Door door2 = (Door) null;
              foreach (short doorsId2 in vesselDockingPort.DoorsIDs)
              {
                VesselObjectID sid2 = new VesselObjectID(port.DockedVessel.GUID, (int) doorsId2);
                Door door3 = this.GetDoor(sid2);
                if (door3 != null)
                {
                  double magnitude = (door1.PositionRelativeToDockingPort - QuaternionD.AngleAxis(180.0, Vector3D.Up) * door3.PositionRelativeToDockingPort).Magnitude;
                  if (magnitude <= (double) port.DoorPairingDistance && magnitude < num)
                  {
                    door2 = door3;
                    vesselObjectId = sid2;
                  }
                }
              }
              if (door2 != null && door2.isExternal)
                this.pairDoors(door1, door2);
            }
          }
          this.linkNodesInDockedVessels(port.DockedVessel, traversedVessels);
        }
      }
    }

    private void pairDoors(Door door1, Door door2)
    {
      door1.PairedDoorID = door2.ID;
      door2.PairedDoorID = door1.ID;
      if (door1.LockedAutoToggle)
        door1.IsLocked = false;
      if (door2.LockedAutoToggle)
        door2.IsLocked = false;
      if (door1.Room1 != null)
      {
        Room room1 = (Room) null;
        if (door2.Room1 != null)
        {
          room1 = door2.Room1;
          door2.Room2 = door1.Room1;
        }
        else if (door2.Room2 != null)
        {
          room1 = door2.Room2;
          door2.Room1 = door1.Room1;
        }
        door1.Room2 = room1;
        this.linkRooms(room1, door1.Room1);
      }
      else
      {
        if (door1.Room2 == null)
          return;
        Room room1 = (Room) null;
        if (door2.Room1 != null)
        {
          room1 = door2.Room1;
          door2.Room2 = door1.Room2;
        }
        else if (door2.Room2 != null)
        {
          room1 = door2.Room2;
          door2.Room1 = door1.Room2;
        }
        door1.Room1 = room1;
        this.linkRooms(room1, door1.Room2);
      }
    }

    private void linkDistributionNodes(DistributionManager.DistributionNode node1, DistributionManager.DistributionNode node2)
    {
      node1.LinkedNodes.Add(node2);
      node2.LinkedNodes.Add(node1);
    }

    private void linkRooms(Room room1, Room room2)
    {
      room1.LinkedRooms.Add(room2);
      room2.LinkedRooms.Add(room1);
      if (room1.GravityAutoToggle)
      {
        bool flag = false;
        foreach (Door door in room1.Doors)
        {
          if (door.isExternal)
          {
            flag = true;
            break;
          }
        }
        room1.UseGravity = !flag;
      }
      if (!room2.GravityAutoToggle)
        return;
      bool flag1 = false;
      foreach (Door door in room2.Doors)
      {
        if (door.isExternal)
        {
          flag1 = true;
          break;
        }
      }
      room2.UseGravity = !flag1;
    }

    public void UnpairAllDoors()
    {
      foreach (KeyValuePair<VesselObjectID, Door> idDoor in this.idDoors)
      {
        Door door1 = idDoor.Value;
        if (door1.LockedAutoToggle)
          door1.IsLocked = true;
        if (door1.Room1 != null && door1.Room1.GravityAutoToggle)
          door1.Room1.UseGravity = false;
        if (door1.Room2 != null && door1.Room2.GravityAutoToggle)
          door1.Room2.UseGravity = false;
        Door door2;
        if (door1.PairedDoorID != null && this.idDoors.TryGetValue(door1.PairedDoorID, out door2))
          this.UnpairDoor(door2);
        this.UnpairDoor(door1);
      }
    }

    private void UnpairDoor(Door door)
    {
      VesselObjectID key1 = this.idRooms.FirstOrDefault<KeyValuePair<VesselObjectID, Room>>((Func<KeyValuePair<VesselObjectID, Room>, bool>) (m => m.Value == door.Room1)).Key;
      VesselObjectID key2 = this.idRooms.FirstOrDefault<KeyValuePair<VesselObjectID, Room>>((Func<KeyValuePair<VesselObjectID, Room>, bool>) (m => m.Value == door.Room2)).Key;
      if (key1 != null && key2 != null && (key1.VesselGUID != door.ID.VesselGUID || key2.VesselGUID != door.ID.VesselGUID))
      {
        door.Room1.LinkedRooms.Remove(door.Room2);
        door.Room2.LinkedRooms.Remove(door.Room1);
        if (key1.VesselGUID != door.ID.VesselGUID)
          door.Room1 = (Room) null;
        else if (key2.VesselGUID != door.ID.VesselGUID)
          door.Room2 = (Room) null;
      }
      door.PairedDoorID = (VesselObjectID) null;
    }

    public Door GetDoor(VesselObjectID sid)
    {
      Door door;
      if (this.idDoors.TryGetValue(sid, out door))
        return door;
      return (Door) null;
    }

    public Room GetRoom(VesselObjectID sid)
    {
      Room room;
      if (this.idRooms.TryGetValue(sid, out room))
        return room;
      return (Room) null;
    }

    public SubSystem GetSubSystem(VesselObjectID sid)
    {
      SubSystem subSystem;
      if (this.idSubSystems.TryGetValue(sid, out subSystem))
        return subSystem;
      return (SubSystem) null;
    }

    public Generator GetGenerator(VesselObjectID sid)
    {
      Generator generator;
      if (this.idGenerators.TryGetValue(sid, out generator))
        return generator;
      return (Generator) null;
    }

    internal VesselComponent GetVesselComponent(VesselObjectID sid)
    {
      SubSystem subSystem;
      if (this.idSubSystems.TryGetValue(sid, out subSystem))
        return (VesselComponent) subSystem;
      Generator generator;
      if (this.idGenerators.TryGetValue(sid, out generator))
        return (VesselComponent) generator;
      return (VesselComponent) null;
    }

    public ResourceContainer GetResourceContainer(VesselObjectID sid)
    {
      ResourceContainer resourceContainer;
      if (this.idResourceContainers.TryGetValue(sid, out resourceContainer))
        return resourceContainer;
      return (ResourceContainer) null;
    }

    public VesselComponent GetVesselComponentByPartSlot(VesselObjectID sid)
    {
      VesselComponent vesselComponent;
      if (this.idMachineryPartSlots.TryGetValue(sid, out vesselComponent))
        return vesselComponent;
      return (VesselComponent) null;
    }

    private DistributionManager.DistributionNode addShipDataStructure(SpaceObjectVessel vessel, GameScenes.SceneID sceneID)
    {
      StructureSceneData structureDef = DistributionManager.structureDefs[(short) sceneID];
      foreach (RoomData room1 in structureDef.Rooms)
      {
        VesselObjectID id = new VesselObjectID(vessel.GUID, room1.InSceneID);
        Room room2 = vessel.Rooms.Find((Predicate<Room>) (m => m.ID.Equals((object) id)));
        if (room2 != null)
          this.idRooms[id] = room2;
      }
      foreach (DoorData door1 in structureDef.Doors)
      {
        if (door1.IsSealable)
        {
          VesselObjectID id = new VesselObjectID(vessel.GUID, door1.InSceneID);
          Door door2;
          if (this.isCompoundDM)
          {
            door2 = vessel.DistributionManager.GetDoor(id);
          }
          else
          {
            door2 = vessel.Doors.Find((Predicate<Door>) (m => m.ID.Equals((object) id)));
            door2.Room1 = this.GetRoom(new VesselObjectID(vessel.GUID, door1.Room1ID));
            door2.Room2 = this.GetRoom(new VesselObjectID(vessel.GUID, door1.Room2ID));
            if (door2.Room1 != null)
            {
              door2.Room1.Doors.Add(door2);
              if (door2.isExternal)
                door2.Room1.AirConsumers.Add((IAirConsumer) door2);
            }
            if (door2.Room2 != null)
            {
              door2.Room2.Doors.Add(door2);
              if (door2.isExternal)
                door2.Room2.AirConsumers.Add((IAirConsumer) door2);
            }
          }
          this.idDoors[id] = door2;
        }
      }
      foreach (RoomData room in structureDef.Rooms)
      {
        if (room.ParentRoomID > 0)
        {
          Room idRoom = this.idRooms[new VesselObjectID(vessel.GUID, room.InSceneID)];
          Room room2;
          if (this.idRooms.TryGetValue(new VesselObjectID(vessel.GUID, room.ParentRoomID), out room2))
          {
            this.linkRooms(idRoom, room2);
            room2.LinkedRooms.Add(idRoom);
            idRoom.LinkedRooms.Add(room2);
          }
        }
      }
      DistributionManager.DistributionNode distributionNode = new DistributionManager.DistributionNode();
      foreach (ResourceContainerData resourceContainer1 in structureDef.ResourceContainers)
      {
        VesselObjectID index = new VesselObjectID(vessel.GUID, resourceContainer1.InSceneID);
        ResourceContainer resourceContainer2 = !this.isCompoundDM ? new ResourceContainer(vessel, index, resourceContainer1) : vessel.DistributionManager.GetResourceContainer(index);
        distributionNode.ResourceProviders.Add((IResourceProvider) resourceContainer2);
        this.idResourceContainers[index] = resourceContainer2;
        this.resourceProviderNodes[(IResourceProvider) resourceContainer2] = distributionNode;
      }
      foreach (SubSystemData subSystem1 in structureDef.SubSystems)
      {
        SubSystemData ssData = subSystem1;
        VesselObjectID sid = new VesselObjectID(vessel.GUID, ssData.InSceneID);
        SubSystem subSystem2;
        if (this.isCompoundDM)
        {
          subSystem2 = vessel.DistributionManager.GetSubSystem(sid);
        }
        else
        {
          VesselComponent vesselComponent = vessel.Systems.Find((Predicate<VesselComponent>) (m => m.ID.InSceneID == ssData.InSceneID));
          if (vesselComponent != null && vesselComponent is SubSystem)
          {
            subSystem2 = vesselComponent as SubSystem;
            if (subSystem2 is SubSystemRCS)
              this.RCS = subSystem2 as SubSystemRCS;
            else if (subSystem2 is SubSystemEngine)
              this.Engine = subSystem2 as SubSystemEngine;
            else if (subSystem2 is SubSystemFTL)
              this.FTL = subSystem2 as SubSystemFTL;
            else if (subSystem2 is SubSystemRefinery)
              this.Refinery = subSystem2 as SubSystemRefinery;
            else if (subSystem2 is SubSystemPDU)
            {
              if (this.pdu == null)
                this.pdu = (SubSystemPDU) subSystem2;
              else
                continue;
            }
            else if (subSystem2 is SubSystemFabricator)
              this.Fabricator = subSystem2 as SubSystemFabricator;
            foreach (short resourceContainer1 in ssData.ResourceContainers)
            {
              VesselObjectID key = new VesselObjectID(vessel.GUID, (int) resourceContainer1);
              ResourceContainer resourceContainer2 = (ResourceContainer) null;
              if (this.idResourceContainers.TryGetValue(key, out resourceContainer2))
              {
                if (!subSystem2.ResourceContainers.ContainsKey(resourceContainer2.OutputType))
                  subSystem2.ResourceContainers[resourceContainer2.OutputType] = new HashSet<ResourceContainer>();
                subSystem2.ResourceContainers[resourceContainer2.OutputType].Add(resourceContainer2);
              }
            }
            if (ssData.RoomID > 0)
            {
              Room idRoom = this.idRooms[new VesselObjectID(vessel.GUID, ssData.RoomID)];
              idRoom.VesselComponents.Add((VesselComponent) subSystem2);
              subSystem2.Room = idRoom;
              if (subSystem2 is ILifeSupportDevice)
                idRoom.LifeSupportDevices.Add(subSystem2 as ILifeSupportDevice);
              if (subSystem2 is SubSystemAirVent)
                idRoom.AirConsumers.Add((IAirConsumer) subSystem2);
            }
          }
          else
            continue;
        }
        distributionNode.ResourceConsumers.Add((IResourceConsumer) subSystem2);
        this.idSubSystems[sid] = subSystem2;
        if (ssData.MachineryPartSlots != null)
        {
          foreach (short machineryPartSlot in ssData.MachineryPartSlots)
          {
            short msId = machineryPartSlot;
            MachineryPartSlotData partSlotData = structureDef.AttachPoints.Find((Predicate<BaseAttachPointData>) (m => m.InSceneID == (int) msId)) as MachineryPartSlotData;
            if (partSlotData != null)
            {
              VesselObjectID slotKey = new VesselObjectID(vessel.GUID, (int) msId);
              this.idMachineryPartSlots[slotKey] = (VesselComponent) subSystem2;
              if (!this.isCompoundDM)
                subSystem2.InitMachineryPartSlot(slotKey, (MachineryPart) null, partSlotData);
            }
          }
        }
      }
      foreach (GeneratorData generator1 in structureDef.Generators)
      {
        GeneratorData gData = generator1;
        VesselObjectID sid = new VesselObjectID(vessel.GUID, gData.InSceneID);
        Generator generator2;
        if (this.isCompoundDM)
        {
          generator2 = vessel.DistributionManager.GetGenerator(sid);
        }
        else
        {
          VesselComponent vesselComponent = vessel.Systems.Find((Predicate<VesselComponent>) (m => m.ID.InSceneID == gData.InSceneID));
          if (vesselComponent != null && vesselComponent is Generator)
          {
            generator2 = vesselComponent as Generator;
            if (generator2 is GeneratorCapacitor)
              this.Capacitor = generator2 as GeneratorCapacitor;
            foreach (short resourceContainer1 in gData.ResourceContainers)
            {
              VesselObjectID key = new VesselObjectID(vessel.GUID, (int) resourceContainer1);
              ResourceContainer resourceContainer2 = (ResourceContainer) null;
              if (this.idResourceContainers.TryGetValue(key, out resourceContainer2))
              {
                if (!generator2.ResourceContainers.ContainsKey(resourceContainer2.OutputType))
                  generator2.ResourceContainers[resourceContainer2.OutputType] = new HashSet<ResourceContainer>();
                generator2.ResourceContainers[resourceContainer2.OutputType].Add(resourceContainer2);
              }
            }
          }
          else
            continue;
        }
        distributionNode.ResourceProviders.Add((IResourceProvider) generator2);
        distributionNode.ResourceConsumers.Add((IResourceConsumer) generator2);
        this.idGenerators[sid] = generator2;
        this.resourceProviderNodes[(IResourceProvider) generator2] = distributionNode;
        if (gData.MachineryPartSlots != null)
        {
          foreach (short machineryPartSlot in gData.MachineryPartSlots)
          {
            short msId = machineryPartSlot;
            MachineryPartSlotData partSlotData = structureDef.AttachPoints.Find((Predicate<BaseAttachPointData>) (m => m.InSceneID == (int) msId)) as MachineryPartSlotData;
            if (partSlotData != null)
            {
              VesselObjectID slotKey = new VesselObjectID(vessel.GUID, (int) msId);
              this.idMachineryPartSlots[slotKey] = (VesselComponent) generator2;
              if (!this.isCompoundDM)
                generator2.InitMachineryPartSlot(slotKey, (MachineryPart) null, partSlotData);
            }
          }
        }
      }
      foreach (PDUGroupData pduGroup1 in structureDef.PDUGroups)
      {
        List<VesselComponent> vesselComponentList = new List<VesselComponent>();
        foreach (int[] powerConsumersId in pduGroup1.PowerConsumersIds)
        {
          if (powerConsumersId[0] == 1)
            vesselComponentList.Add((VesselComponent) this.idSubSystems[new VesselObjectID(vessel.GUID, powerConsumersId[1])]);
          else if (powerConsumersId[0] == 2)
            vesselComponentList.Add((VesselComponent) this.idGenerators[new VesselObjectID(vessel.GUID, powerConsumersId[1])]);
        }
        PDUGroup pduGroup2 = new PDUGroup()
        {
          Priority = pduGroup1.Priority,
          PowerConsumers = vesselComponentList
        };
        this.pduGroups[new VesselObjectID(vessel.GUID, pduGroup1.InSceneID)] = pduGroup2;
      }
      this.distributionNodes[vessel.GUID] = distributionNode;
      if (this.isCompoundDM)
      {
        foreach (VesselDockingPort dockingPort in vessel.DockingPorts)
        {
          if (dockingPort.DockingStatus && !this.distributionNodes.ContainsKey(dockingPort.DockedVessel.GUID))
            this.addShipDataStructure(dockingPort.DockedVessel, dockingPort.DockedVessel.VesselData.SceneID);
        }
      }
      return distributionNode;
    }

    public void Refine(RefineResourceMessage rrMsg, ICargo fromCargo)
    {
      if (this.Refinery == null || this.parentVessel == null || (double) rrMsg.Quantity <= 0.0)
        return;
      CargoBay toCargo = this.parentVessel.CargoBays.Find((Predicate<CargoBay>) (m => m.InSceneID == this.Refinery.CargoBayInSceneID));
      if (fromCargo == null || toCargo == null || (double) rrMsg.Quantity <= 1.40129846432482E-45)
        return;
      this.Refinery.Refine(rrMsg, fromCargo, toCargo);
    }

    public void FabricateItem(ItemType itemType, ICargo fromCargo)
    {
      if (this.Fabricator != null && this.parentVessel != null && fromCargo != null)
        ;
    }

    public List<GeneratorDetails> GetGeneratorsDetails(bool changedOnly, long vesselGUID = -1)
    {
      List<GeneratorDetails> generatorDetailsList = new List<GeneratorDetails>();
      foreach (KeyValuePair<VesselObjectID, Generator> idGenerator in this.idGenerators)
      {
        if ((changedOnly && idGenerator.Value.StatusChanged || !changedOnly) && (vesselGUID == -1L || idGenerator.Key.VesselGUID == vesselGUID))
        {
          GeneratorDetails generatorDetails = new GeneratorDetails()
          {
            InSceneID = idGenerator.Key.InSceneID,
            Status = idGenerator.Value.Status,
            SecondaryStatus = idGenerator.Value.SecondaryStatus,
            Output = idGenerator.Value.Output,
            MaxOutput = idGenerator.Value.MaxOutput,
            OutputRate = idGenerator.Value.OperationRate,
            InputFactor = idGenerator.Value.InputFactor,
            InputFactorStandby = idGenerator.Value.InputFactorStandby,
            AuxDetails = idGenerator.Value.GetAuxDetails()
          };
          generatorDetailsList.Add(generatorDetails);
          idGenerator.Value.StatusChanged = false;
        }
      }
      return generatorDetailsList;
    }

    public List<SubSystemDetails> GetSubSystemsDetails(bool changedOnly, long vesselGUID = -1)
    {
      List<SubSystemDetails> subSystemDetailsList = new List<SubSystemDetails>();
      foreach (KeyValuePair<VesselObjectID, SubSystem> idSubSystem in this.idSubSystems)
      {
        if ((changedOnly && idSubSystem.Value.StatusChanged || !changedOnly) && (vesselGUID == -1L || idSubSystem.Key.VesselGUID == vesselGUID))
        {
          subSystemDetailsList.Add(new SubSystemDetails()
          {
            InSceneID = idSubSystem.Key.InSceneID,
            Status = idSubSystem.Value.Status,
            SecondaryStatus = idSubSystem.Value.SecondaryStatus,
            OperationRate = idSubSystem.Value.OperationRate,
            InputFactor = idSubSystem.Value.InputFactor,
            InputFactorStandby = idSubSystem.Value.InputFactorStandby,
            AuxDetails = idSubSystem.Value.GetAuxDetails()
          });
          idSubSystem.Value.StatusChanged = false;
        }
      }
      return subSystemDetailsList;
    }

    public List<RoomDetails> GetRoomsDetails(bool changedOnly, long vesselGUID = -1)
    {
      List<RoomDetails> roomDetailsList = new List<RoomDetails>();
      foreach (KeyValuePair<VesselObjectID, Room> idRoom in this.idRooms)
      {
        if ((changedOnly && idRoom.Value.StatusChanged || !changedOnly) && (vesselGUID == -1L || idRoom.Key.VesselGUID == vesselGUID))
        {
          roomDetailsList.Add(idRoom.Value.GetDetails());
          idRoom.Value.StatusChanged = false;
        }
      }
      return roomDetailsList;
    }

    public List<DoorDetails> GetDoorsDetails(bool changedOnly, long vesselGUID = -1)
    {
      List<DoorDetails> doorDetailsList = new List<DoorDetails>();
      foreach (KeyValuePair<VesselObjectID, Door> idDoor in this.idDoors)
      {
        if ((changedOnly && idDoor.Value.StatusChanged || !changedOnly) && (vesselGUID == -1L || idDoor.Key.VesselGUID == vesselGUID))
        {
          doorDetailsList.Add(idDoor.Value.GetDetails());
          idDoor.Value.StatusChanged = false;
        }
      }
      return doorDetailsList;
    }

    public List<ResourceContainerDetails> GetResourceContainersDetails(bool changedOnly, long vesselGUID = -1)
    {
      List<ResourceContainerDetails> containerDetailsList = new List<ResourceContainerDetails>();
      foreach (KeyValuePair<VesselObjectID, ResourceContainer> resourceContainer in this.idResourceContainers)
      {
        if ((changedOnly && resourceContainer.Value.StatusChanged || !changedOnly) && (vesselGUID == -1L || resourceContainer.Key.VesselGUID == vesselGUID))
        {
          containerDetailsList.Add(new ResourceContainerDetails()
          {
            InSceneID = resourceContainer.Key.InSceneID,
            Resources = resourceContainer.Value.GetCompartment(new int?()).Resources,
            QuantityChangeRate = resourceContainer.Value.QuantityChangeRate,
            Output = resourceContainer.Value.Output,
            OutputRate = resourceContainer.Value.OperationRate,
            IsInUse = resourceContainer.Value.IsInUse
          });
          resourceContainer.Value.StatusChanged = false;
        }
      }
      return containerDetailsList;
    }

    public List<ResourceContainer> GetResourceContainers()
    {
      return new List<ResourceContainer>((IEnumerable<ResourceContainer>) this.idResourceContainers.Values);
    }

    public List<VesselComponent> GetGenerators()
    {
      return new List<VesselComponent>((IEnumerable<VesselComponent>) this.idGenerators.Values);
    }

    public List<VesselComponent> GetSubSystems()
    {
      return new List<VesselComponent>((IEnumerable<VesselComponent>) this.idSubSystems.Values);
    }

    public List<Room> GetRooms()
    {
      return new List<Room>((IEnumerable<Room>) this.idRooms.Values);
    }

    public void UpdateSystems(bool connectionsChanged = true, bool compoundRoomsChanged = true)
    {
      double totalSeconds = (DateTime.UtcNow - this.prevSystemsUpdateTime).TotalSeconds;
      this.prevSystemsUpdateTime = DateTime.UtcNow;
      if (this.isCompoundDM)
      {
        this.parentVessel.DistributionManager.prevSystemsUpdateTime = this.prevSystemsUpdateTime;
        foreach (SpaceObjectVessel allDockedVessel in this.parentVessel.AllDockedVessels)
          allDockedVessel.DistributionManager.prevSystemsUpdateTime = this.prevSystemsUpdateTime;
      }
      Dictionary<DistributionManager.CompoundRoom, float[]> dictionary1 = new Dictionary<DistributionManager.CompoundRoom, float[]>();
      foreach (DistributionManager.CompoundRoom index in this.compoundRooms.Values.Distinct<DistributionManager.CompoundRoom>())
        dictionary1[index] = new float[2]
        {
          index.AirQuality,
          index.AirPressure
        };
      Dictionary<ResourceContainer, float> dictionary2 = new Dictionary<ResourceContainer, float>();
      foreach (ResourceContainer index in this.idResourceContainers.Values)
        dictionary2[index] = index.GetCompartment(new int?()).Resources.Count > 0 ? index.GetCompartment(new int?()).Resources[0].Quantity : 0.0f;
      foreach (VesselComponent vesselComponent in this.idGenerators.Values)
        vesselComponent.Update(totalSeconds);
      foreach (VesselComponent vesselComponent in this.idSubSystems.Values)
        vesselComponent.Update(totalSeconds);
      if (compoundRoomsChanged)
        this.createCompoundRooms();
      this.updateCompoundRooms((float) totalSeconds);
      if (connectionsChanged)
        this.updateConnections();
      this.updateConsumers((float) totalSeconds);
      foreach (ResourceContainer index in this.idResourceContainers.Values)
        index.QuantityChangeRate = totalSeconds > 0.0 ? (float) (((index.GetCompartment(new int?()).Resources.Count > 0 ? (double) index.GetCompartment(new int?()).Resources[0].Quantity : 0.0) - (double) dictionary2[index]) / totalSeconds) : 0.0f;
    }

    private void updateConnections()
    {
      foreach (SubSystem subSystem in this.idSubSystems.Values)
      {
        subSystem.ConnectedProviders.Clear();
        if (subSystem is SubSystemAirDevice)
        {
          SubSystemAirDevice subSystemAirDevice = subSystem as SubSystemAirDevice;
          if (!subSystemAirDevice.ConnectedProviders.ContainsKey(subSystemAirDevice.Room.AirTank.OutputType))
            subSystemAirDevice.ConnectedProviders[subSystemAirDevice.Room.AirTank.OutputType] = new SortedSet<IResourceProvider>((IComparer<IResourceProvider>) new DistributionManager.ResourceproviderComparer());
          subSystemAirDevice.ConnectedProviders[subSystemAirDevice.Room.AirTank.OutputType].Add((IResourceProvider) subSystemAirDevice.Room.AirTank);
          subSystemAirDevice.Room.AirTank.ConnectedConsumers.Add((IResourceConsumer) subSystemAirDevice);
        }
      }
      foreach (IResourceProvider key in this.resourceProviderNodes.Keys)
      {
        if (key is IResourceConsumer)
          ((IResourceConsumer) key).ConnectedProviders.Clear();
      }
      foreach (KeyValuePair<IResourceProvider, DistributionManager.DistributionNode> resourceProviderNode in this.resourceProviderNodes)
        this.updateConnectedConsumers(resourceProviderNode.Key, resourceProviderNode.Value, (HashSet<DistributionManager.DistributionNode>) null);
    }

    private void updateConsumers(float duration)
    {
      Dictionary<IResourceProvider, float> reservedCapacities = new Dictionary<IResourceProvider, float>();
      Dictionary<ResourceContainer, float> reservedQuantities = new Dictionary<ResourceContainer, float>();
      foreach (Generator generator in this.idGenerators.Values)
      {
        if (generator.Status == SystemStatus.OnLine && !generator.IsPowerConsumer)
          generator.CheckStatus(1f, duration, true, ref reservedCapacities, ref reservedQuantities);
      }
      foreach (Generator generator in this.idGenerators.Values)
      {
        if (generator.Status == SystemStatus.OnLine && generator.IsPowerConsumer)
          generator.CheckStatus(1f, duration, true, ref reservedCapacities, ref reservedQuantities);
      }
      foreach (VesselComponent vesselComponent in this.idSubSystems.Values)
        vesselComponent.CheckStatus(1f, duration, true, ref reservedCapacities, ref reservedQuantities);
      int num1 = 0;
      foreach (VesselObjectID pduGroup in this.pdu.PDUGroups)
      {
        foreach (VesselComponent powerConsumer in this.pduGroups[pduGroup].PowerConsumers)
        {
          if (powerConsumer is SubSystem)
          {
            SubSystem subSystem = (SubSystem) powerConsumer;
            subSystem.CheckStatus(subSystem.OperationRate, duration, false, ref reservedCapacities, ref reservedQuantities);
          }
          else if (powerConsumer is GeneratorCapacitor)
          {
            GeneratorCapacitor generatorCapacitor = powerConsumer as GeneratorCapacitor;
            if (generatorCapacitor.Status == SystemStatus.OnLine && this.pdu.LimiterIndex > num1 && (double) generatorCapacitor.Capacity < (double) generatorCapacitor.MaxCapacity)
            {
              float num2 = reservedCapacities.Where<KeyValuePair<IResourceProvider, float>>((Func<KeyValuePair<IResourceProvider, float>, bool>) (m => m.Key.OutputType == DistributionSystemType.Power)).Sum<KeyValuePair<IResourceProvider, float>>((Func<KeyValuePair<IResourceProvider, float>, float>) (n => n.Value));
              string debugText = (string) null;
              generatorCapacitor.CheckAvailableResources(1f, duration, false, ref reservedCapacities, ref reservedQuantities, ref debugText);
              debugText = "Vessel " + (object) this.parentVessel.GUID + "\n" + debugText;
              float num3 = reservedCapacities.Where<KeyValuePair<IResourceProvider, float>>((Func<KeyValuePair<IResourceProvider, float>, bool>) (m => m.Key.OutputType == DistributionSystemType.Power)).Sum<KeyValuePair<IResourceProvider, float>>((Func<KeyValuePair<IResourceProvider, float>, float>) (n => n.Value)) - num2;
              generatorCapacitor.Capacity = MathHelper.Clamp(generatorCapacitor.Capacity + num3, 0.0f, generatorCapacitor.MaxCapacity);
            }
          }
        }
        ++num1;
      }
      foreach (SubSystem subSystem in this.idSubSystems.Values)
      {
        if (!subSystem.IsPowerConsumer)
          subSystem.CheckStatus(subSystem.OperationRate, duration, false, ref reservedCapacities, ref reservedQuantities);
      }
      foreach (DistributionSystemType index in Enum.GetValues(typeof (DistributionSystemType)))
      {
        this.AvailableResourceCapacities[index] = 0.0f;
        this.AvailableResourceQuantities[index] = 0.0f;
      }
      foreach (ResourceContainer resourceContainer in this.idResourceContainers.Values)
      {
        Dictionary<DistributionSystemType, float> resourceCapacities = this.AvailableResourceCapacities;
        DistributionSystemType outputType1 = resourceContainer.OutputType;
        resourceCapacities[outputType1] = resourceCapacities[outputType1] + resourceContainer.NominalOutput;
        if (resourceContainer.IsInUse && resourceContainer.GetCompartment(new int?()).Resources.Count > 0)
        {
          Dictionary<DistributionSystemType, float> resourceQuantities = this.AvailableResourceQuantities;
          DistributionSystemType outputType2 = resourceContainer.OutputType;
          resourceQuantities[outputType2] = resourceQuantities[outputType2] + resourceContainer.GetCompartment(new int?()).Resources[0].Quantity;
        }
      }
      foreach (Generator generator in this.idGenerators.Values)
      {
        Dictionary<DistributionSystemType, float> resourceCapacities = this.AvailableResourceCapacities;
        DistributionSystemType outputType = generator.OutputType;
        resourceCapacities[outputType] = resourceCapacities[outputType] + generator.NominalOutput;
      }
      foreach (IResourceProvider key in this.resourceProviderNodes.Keys)
        key.Output = 0.0f;
      foreach (DistributionManager.CompoundRoom compoundRoom in this.compoundRooms.Values.Distinct<DistributionManager.CompoundRoom>())
      {
        if ((double) compoundRoom.AirQuality < 1.0 && (double) compoundRoom.AirPressure > 0.0)
        {
          float num2 = 0.0f;
          foreach (SubSystemScrubberDevice systemScrubberDevice in compoundRoom.LifeSupportDevices.FindAll((Predicate<ILifeSupportDevice>) (m => m is SubSystemScrubberDevice)))
          {
            if (systemScrubberDevice.Status == SystemStatus.OnLine && systemScrubberDevice.SecondaryStatus == SystemSecondaryStatus.None)
              num2 += systemScrubberDevice.MaxScrubbingCapacity * systemScrubberDevice.OperationRate * duration;
          }
          float num3 = num2 / (1f - compoundRoom.AirQuality);
          float num4 = compoundRoom.Volume * compoundRoom.AirPressure;
          compoundRoom.AirQuality = (double) num4 > 0.0 ? (num3 + (num4 - num3) * compoundRoom.AirQuality) / num4 : 0.0f;
        }
        if ((double) compoundRoom.AirPressure < 1.0 || (double) compoundRoom.AirQuality < 1.0)
        {
          float num2 = 0.0f;
          float num3 = 1f;
          foreach (SubSystemAirDevice subSystemAirDevice in compoundRoom.LifeSupportDevices.FindAll((Predicate<ILifeSupportDevice>) (m => m is SubSystemAirDevice)))
          {
            if (subSystemAirDevice.Status == SystemStatus.OnLine && subSystemAirDevice.SecondaryStatus == SystemSecondaryStatus.None && subSystemAirDevice.ConnectedProviders.ContainsKey(DistributionSystemType.Air))
            {
              SortedSet<IResourceProvider> connectedProvider = subSystemAirDevice.ConnectedProviders[DistributionSystemType.Air];
              if (connectedProvider != null && connectedProvider.Count > 0 && connectedProvider.First<IResourceProvider>() is RoomAirTank && (double) (connectedProvider.First<IResourceProvider>() as RoomAirTank).Quantity > 0.0)
              {
                float num4 = System.Math.Min(subSystemAirDevice.MaxOutput * subSystemAirDevice.OperationRate * duration, (connectedProvider.First<IResourceProvider>() as RoomAirTank).Quantity);
                if ((double) num2 + (double) num4 > 0.0)
                  num3 = (float) (((double) num2 * (double) num3 + (double) num4 * (double) (connectedProvider.First<IResourceProvider>() as RoomAirTank).AirQuality) / ((double) num2 + (double) num4));
              }
              num2 += subSystemAirDevice.MaxOutput * subSystemAirDevice.OperationRate * duration;
            }
          }
          float num5 = compoundRoom.Volume * compoundRoom.AirPressure;
          if ((double) num2 + (double) num5 > (double) compoundRoom.Volume)
            num5 = compoundRoom.Volume - num2;
          compoundRoom.AirQuality = (double) num5 + (double) num2 <= 0.0 ? 0.0f : (float) (((double) num5 * (double) compoundRoom.AirQuality + (double) num2 * (double) num3) / ((double) num5 + (double) num2));
          compoundRoom.AirPressure = (double) compoundRoom.Volume <= 0.0 ? 0.0f : (num5 + num2) / compoundRoom.Volume;
        }
      }
      foreach (KeyValuePair<IResourceProvider, float> keyValuePair in reservedCapacities)
      {
        keyValuePair.Key.Output = keyValuePair.Value;
        Dictionary<DistributionSystemType, float> resourceCapacities = this.AvailableResourceCapacities;
        DistributionSystemType outputType = keyValuePair.Key.OutputType;
        resourceCapacities[outputType] = resourceCapacities[outputType] - keyValuePair.Key.Output;
        if (keyValuePair.Key is RoomAirTank)
        {
          RoomAirTank key = keyValuePair.Key as RoomAirTank;
          key.Quantity = MathHelper.Clamp(key.Quantity - keyValuePair.Value, 0.0f, float.MaxValue);
        }
      }
      foreach (IResourceProvider key in this.idGenerators.Values)
      {
        if (!reservedCapacities.ContainsKey(key))
          key.Output = 0.0f;
      }
      foreach (IResourceProvider key in this.idResourceContainers.Values)
      {
        if (!reservedCapacities.ContainsKey(key))
          key.Output = 0.0f;
      }
      foreach (KeyValuePair<ResourceContainer, float> keyValuePair in reservedQuantities)
      {
        double num2 = (double) keyValuePair.Key.ConsumeResource(keyValuePair.Value);
      }
    }

    private void updateConnectedConsumers(IResourceProvider resourceProvider, DistributionManager.DistributionNode node, HashSet<DistributionManager.DistributionNode> traversedNodes = null)
    {
      if (traversedNodes == null)
        traversedNodes = new HashSet<DistributionManager.DistributionNode>();
      if (!traversedNodes.Add(node))
        return;
      HashSet<IResourceConsumer> resourceConsumerSet1 = new HashSet<IResourceConsumer>();
      HashSet<IResourceConsumer> resourceConsumerSet2 = new HashSet<IResourceConsumer>();
      foreach (IResourceConsumer resourceConsumer in node.ResourceConsumers)
      {
        if (resourceConsumer.ResourceRequirements.ContainsKey(resourceProvider.OutputType))
        {
          HashSet<ResourceContainer> resourceContainerSet = (HashSet<ResourceContainer>) null;
          resourceConsumer.ResourceContainers.TryGetValue(resourceProvider.OutputType, out resourceContainerSet);
          if (resourceProvider is ResourceContainer && resourceContainerSet != null && ((IEnumerable<IResourceProvider>) resourceContainerSet).Contains<IResourceProvider>(resourceProvider))
            resourceConsumerSet1.Add(resourceConsumer);
          else if (resourceContainerSet == null)
            resourceConsumerSet2.Add(resourceConsumer);
        }
      }
      foreach (IResourceConsumer resourceConsumer in resourceConsumerSet1.Count > 0 ? resourceConsumerSet1 : resourceConsumerSet2)
      {
        resourceProvider.ConnectedConsumers.Add(resourceConsumer);
        if (!(resourceConsumer is GeneratorCapacitor) || !(resourceProvider is GeneratorCapacitor))
        {
          if (!resourceConsumer.ConnectedProviders.ContainsKey(resourceProvider.OutputType))
            resourceConsumer.ConnectedProviders[resourceProvider.OutputType] = new SortedSet<IResourceProvider>((IComparer<IResourceProvider>) new DistributionManager.ResourceproviderComparer());
          resourceConsumer.ConnectedProviders[resourceProvider.OutputType].Add(resourceProvider);
        }
      }
      foreach (DistributionManager.DistributionNode linkedNode in node.LinkedNodes)
        this.updateConnectedConsumers(resourceProvider, linkedNode, traversedNodes);
    }

    public void RemovePlayerFromRoom(Player player)
    {
      try
      {
        Room playerRoom = this.playerRooms[player];
        playerRoom.AirConsumers.Remove((IAirConsumer) player);
        this.compoundRooms[playerRoom].AirConsumers.Remove((IAirConsumer) player);
      }
      catch
      {
        Dbg.Error("Error removing player from room.");
      }
    }

    public void SetPlayerRoom(Player player, VesselObjectID key)
    {
      if (this.playerRooms.ContainsKey(player))
      {
        Room index = (Room) null;
        this.playerRooms.TryGetValue(player, out index);
        DistributionManager.CompoundRoom compoundRoom1 = index != null ? this.compoundRooms[index] : (DistributionManager.CompoundRoom) null;
        if (index != null)
        {
          index.AirConsumers.Remove((IAirConsumer) player);
          compoundRoom1.AirConsumers.Remove((IAirConsumer) player);
          this.playerRooms.Remove(player);
        }
        if (key == null || !this.idRooms.ContainsKey(key))
          return;
        try
        {
          Room idRoom = this.idRooms[key];
          DistributionManager.CompoundRoom compoundRoom2 = this.compoundRooms[idRoom];
          if (compoundRoom1 != compoundRoom2)
          {
            if (compoundRoom1 != null)
              compoundRoom1.AirConsumers.Remove((IAirConsumer) player);
            compoundRoom2.AirConsumers.Add((IAirConsumer) player);
          }
          if (index != null)
            index.AirConsumers.Remove((IAirConsumer) player);
          idRoom.AirConsumers.Add((IAirConsumer) player);
          this.playerRooms[player] = idRoom;
        }
        catch
        {
        }
      }
      else
      {
        if (key == null)
          return;
        try
        {
          Room idRoom = this.idRooms[key];
          this.playerRooms[player] = idRoom;
          idRoom.AirConsumers.Add((IAirConsumer) player);
          this.compoundRooms[idRoom].AirConsumers.Add((IAirConsumer) player);
        }
        catch
        {
        }
      }
    }

    private void createCompoundRooms()
    {
      short num = 0;
      this.compoundRooms = new Dictionary<Room, DistributionManager.CompoundRoom>();
      foreach (Room room1 in this.idRooms.Values)
      {
        if (!this.compoundRooms.ContainsKey(room1))
        {
          DistributionManager.CompoundRoom compoundRoom = new DistributionManager.CompoundRoom(room1, num++);
          foreach (Room room2 in compoundRoom.Rooms)
            this.compoundRooms[room2] = compoundRoom;
        }
      }
    }

    private void updateCompoundRooms(float duration)
    {
      foreach (DistributionManager.CompoundRoom compoundRoom in this.compoundRooms.Values.Distinct<DistributionManager.CompoundRoom>())
      {
        bool? nullable1 = new bool?();
        bool flag1 = compoundRoom.AirConsumers.Find((Predicate<IAirConsumer>) (m => m is AirConsumerFire)) != null;
        if (!flag1 && (double) compoundRoom.AirQuality < 0.899999976158142 && (double) compoundRoom.AirPressure > 1.40129846432482E-45)
          nullable1 = new bool?(true);
        else if (flag1 || (double) compoundRoom.AirQuality >= 1.0 || (double) compoundRoom.AirPressure <= 1.40129846432482E-45)
          nullable1 = new bool?(false);
        bool flag2 = false;
        foreach (SubSystemScrubberDevice systemScrubberDevice in compoundRoom.LifeSupportDevices.FindAll((Predicate<ILifeSupportDevice>) (m => m is SubSystemScrubberDevice)))
        {
          if (nullable1.HasValue)
            systemScrubberDevice.OperationRate = nullable1.Value ? 1f : 0.0f;
          flag2 |= systemScrubberDevice.Status == SystemStatus.OnLine;
        }
        bool? nullable2 = new bool?();
        bool flag3 = compoundRoom.AirConsumers.Find((Predicate<IAirConsumer>) (m => m.AffectsQuantity)) != null;
        if (!flag3 && ((double) compoundRoom.AirPressure < 0.899999976158142 || !flag2 && (double) compoundRoom.AirQuality < 0.600000023841858 || !compoundRoom.IsAirOk))
          nullable2 = new bool?(true);
        else if (flag3 || (double) compoundRoom.AirPressure >= 1.0 && (flag2 || (double) compoundRoom.AirQuality >= 1.0))
          nullable2 = new bool?(false);
        if (nullable2.HasValue)
        {
          foreach (VesselComponent vesselComponent in compoundRoom.LifeSupportDevices.FindAll((Predicate<ILifeSupportDevice>) (m => m is SubSystemAirDevice)))
            vesselComponent.OperationRate = nullable2.Value ? 1f : 0.0f;
        }
        float num1 = 0.0f;
        float num2 = 0.0f;
        List<AirConsumerFire> airConsumerFireList = new List<AirConsumerFire>();
        foreach (IAirConsumer airConsumer in compoundRoom.AirConsumers.Where<IAirConsumer>((Func<IAirConsumer, bool>) (m => m is AirConsumerFire)))
        {
          if (compoundRoom.FireCanBurn)
          {
            num1 += (double) compoundRoom.AirPressure > 0.0 ? airConsumer.AirQualityDegradationRate / compoundRoom.Volume / compoundRoom.AirPressure * duration : 0.0f;
            num2 += airConsumer.AirQuantityDecreaseRate * duration;
          }
          else if (!(airConsumer as AirConsumerFire).Persistent)
            airConsumerFireList.Add((AirConsumerFire) airConsumer);
        }
        compoundRoom.AirQuality -= num1;
        compoundRoom.AirPressure = (compoundRoom.AirPressure * compoundRoom.Volume - num2) / compoundRoom.Volume;
        foreach (AirConsumerFire airConsumerFire in airConsumerFireList)
        {
          compoundRoom.AirConsumers.Remove((IAirConsumer) airConsumerFire);
          foreach (Room room in compoundRoom.Rooms)
            room.AirConsumers.Remove((IAirConsumer) airConsumerFire);
        }
        float num3 = 0.0f;
        float num4 = 0.0f;
        if ((double) duration > 0.0)
        {
          foreach (IAirConsumer airConsumer in compoundRoom.AirConsumers.Where<IAirConsumer>((Func<IAirConsumer, bool>) (m => !(m is AirConsumerFire))))
          {
            num3 += (double) compoundRoom.AirPressure > 0.0 ? airConsumer.AirQualityDegradationRate / compoundRoom.Volume / compoundRoom.AirPressure * duration : 0.0f;
            num4 += airConsumer.AirQuantityDecreaseRate * duration;
            if (airConsumer is SubSystemAirVent && (airConsumer as SubSystemAirVent).Status == SystemStatus.OnLine)
            {
              Room room = (airConsumer as SubSystemAirVent).Room;
              float num5 = airConsumer.AirQuantityDecreaseRate * duration;
              if ((double) num5 + (double) room.AirTank.Quantity > 0.0)
                room.AirTank.AirQuality = MathHelper.Clamp((float) (((double) num5 * (double) room.AirQuality + (double) room.AirTank.Quantity * (double) room.AirTank.AirQuality) / ((double) num5 + (double) room.AirTank.Quantity)), 0.0f, 1f);
              room.AirTank.Quantity = MathHelper.Clamp(room.AirTank.Quantity + num5, 0.0f, room.AirTank.Volume);
            }
          }
        }
        compoundRoom.AirQuality -= num3;
        compoundRoom.AirPressure = (compoundRoom.AirPressure * compoundRoom.Volume - num4) / compoundRoom.Volume;
      }
    }

    public static Dictionary<DistributionSystemType, ResourceRequirement> ResourceRequirementsToDictionary(ResourceRequirement[] rrArray)
    {
      Dictionary<DistributionSystemType, ResourceRequirement> dictionary = new Dictionary<DistributionSystemType, ResourceRequirement>();
      if (rrArray != null)
      {
        foreach (ResourceRequirement rr in rrArray)
          dictionary[rr.ResourceType] = rr;
      }
      return dictionary;
    }

    public class ShortArrayComparer : IEqualityComparer<short[]>
    {
      public bool Equals(short[] x, short[] y)
      {
        if (x.Length != y.Length)
          return false;
        for (int index = 0; index < x.Length; ++index)
        {
          if ((int) x[index] != (int) y[index])
            return false;
        }
        return true;
      }

      public int GetHashCode(short[] obj)
      {
        long num = 17;
        for (int index = 0; index < obj.Length; ++index)
          num = num * 23L + (long) obj[index];
        return (int) (num & 268435455L);
      }
    }

    public class CompoundRoom
    {
      public float Volume = 0.0f;
      public HashSet<Room> Rooms = new HashSet<Room>();
      private float _AirPressure = 1f;
      private float _AirQuality = 1f;
      public List<IAirConsumer> AirConsumers = new List<IAirConsumer>();
      public List<ILifeSupportDevice> LifeSupportDevices = new List<ILifeSupportDevice>();
      public short ID;

      public bool IsAirOk
      {
        get
        {
          return (double) this.AirPressure > -0.67 * (double) this.AirQuality + 1.0;
        }
      }

      public float AirPressure
      {
        get
        {
          return this._AirPressure;
        }
        set
        {
          this._AirPressure = MathHelper.Clamp(value, 0.0f, 1f);
          foreach (Room room in this.Rooms)
            room.AirPressure = this._AirPressure;
        }
      }

      public float AirQuality
      {
        get
        {
          return this._AirQuality;
        }
        set
        {
          this._AirQuality = MathHelper.Clamp(value, 0.0f, 1f);
          foreach (Room room in this.Rooms)
            room.AirQuality = this._AirQuality;
        }
      }

      public bool FireCanBurn
      {
        get
        {
          return (double) this.AirQuality * (double) this.AirPressure >= 0.25;
        }
      }

      public CompoundRoom(Room room, short id)
      {
        this.ID = id;
        this.addConnectedRooms(room);
        float num1 = 0.0f;
        float num2 = 0.0f;
        float num3 = 0.0f;
        float num4 = 0.0f;
        foreach (Room room1 in this.Rooms)
        {
          this.Volume = this.Volume + room1.Volume;
          num1 += room1.AirPressure * room1.Volume;
          num2 += room1.AirQuality * room1.AirPressure * room1.Volume;
          num3 += room1.Volume;
          num4 += room1.AirPressure * room1.Volume;
          if (room1.AirConsumers.Count > 0)
            this.AirConsumers.AddRange((IEnumerable<IAirConsumer>) room1.AirConsumers);
          if (room1.LifeSupportDevices.Count > 0)
            this.LifeSupportDevices.AddRange((IEnumerable<ILifeSupportDevice>) room1.LifeSupportDevices);
        }
        this._AirPressure = (double) num3 > 0.0 ? num1 / num3 : 0.0f;
        this._AirQuality = (double) num4 > 0.0 ? num2 / num4 : 0.0f;
      }

      private void addConnectedRooms(Room room)
      {
        if (!this.Rooms.Add(room))
          return;
        room.CompoundRoom = this;
        foreach (Room linkedRoom in room.LinkedRooms)
        {
          bool flag = true;
          foreach (Door door in room.Doors)
          {
            if ((door.Room1 == linkedRoom || door.Room2 == linkedRoom) && door.IsSealed)
            {
              flag = false;
              break;
            }
          }
          if (flag)
            this.addConnectedRooms(linkedRoom);
        }
      }
    }

    private class DistributionNode
    {
      [JsonIgnore]
      public HashSet<DistributionManager.DistributionNode> LinkedNodes = new HashSet<DistributionManager.DistributionNode>();
      public List<IResourceConsumer> ResourceConsumers = new List<IResourceConsumer>();
      public List<IResourceProvider> ResourceProviders = new List<IResourceProvider>();
    }

    public class ResourceproviderComparer : IComparer<IResourceProvider>
    {
      private static Dictionary<System.Type, int> types = new Dictionary<System.Type, int>()
      {
        {
          typeof (GeneratorSolar),
          1
        },
        {
          typeof (GeneratorPower),
          2
        },
        {
          typeof (GeneratorCapacitor),
          3
        },
        {
          typeof (RoomAirTank),
          1
        }
      };

      public int Compare(IResourceProvider x, IResourceProvider y)
      {
        if (DistributionManager.ResourceproviderComparer.types.ContainsKey(x.GetType()) && !DistributionManager.ResourceproviderComparer.types.ContainsKey(y.GetType()))
          return -1;
        if (!DistributionManager.ResourceproviderComparer.types.ContainsKey(x.GetType()) && DistributionManager.ResourceproviderComparer.types.ContainsKey(y.GetType()) || (!DistributionManager.ResourceproviderComparer.types.ContainsKey(x.GetType()) && !DistributionManager.ResourceproviderComparer.types.ContainsKey(y.GetType()) || x.GetType() == y.GetType()))
          return 1;
        return DistributionManager.ResourceproviderComparer.types[x.GetType()] > DistributionManager.ResourceproviderComparer.types[y.GetType()] || DistributionManager.ResourceproviderComparer.types[x.GetType()] == DistributionManager.ResourceproviderComparer.types[y.GetType()] && x is VesselComponent && (y is VesselComponent && (x as VesselComponent).ParentVessel != (y as VesselComponent).ParentVessel) ? 1 : -1;
      }
    }
  }
}
