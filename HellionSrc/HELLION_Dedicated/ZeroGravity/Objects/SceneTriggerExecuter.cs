// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.SceneTriggerExecuter
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Network;

namespace ZeroGravity.Objects
{
  public class SceneTriggerExecuter : IPersistantObject
  {
    public int DefaultStateID = 0;
    private int _stateID = 0;
    public Dictionary<int, SceneTriggerExceuterState> States = new Dictionary<int, SceneTriggerExceuterState>();
    public long PlayerThatActivated = 0;
    public SceneTriggerExecuter Child = (SceneTriggerExecuter) null;
    public SceneTriggerExecuter Parent = (SceneTriggerExecuter) null;
    public int InSceneID;
    public Ship ParentShip;
    public ShipSpawnPoint SpawnPoint;
    public Dictionary<int, SceneTriggerProximity> ProximityTriggers;

    public int StateID
    {
      get
      {
        return this._stateID;
      }
      set
      {
        if (!this.States.ContainsKey(value))
          return;
        this._stateID = value;
        if (this.Child != null)
        {
          this.Child._stateID = value;
        }
        else
        {
          if (this.Parent == null)
            return;
          this.Parent._stateID = value;
        }
      }
    }

    public bool IsMerged
    {
      get
      {
        return this.Child != null || this.Parent != null;
      }
    }

    public void MergeWith(SceneTriggerExecuter child)
    {
      if (child == null)
      {
        if (this.Child != null)
        {
          this.Child.Parent = (SceneTriggerExecuter) null;
          this.Child = (SceneTriggerExecuter) null;
        }
        if (this.Parent == null)
          return;
        this.Parent.Child = (SceneTriggerExecuter) null;
        this.Parent = (SceneTriggerExecuter) null;
      }
      else
      {
        this.Child = child;
        this.Child.Parent = this;
        this.Child.StateID = this.StateID;
      }
    }

    public void SetStates(List<SceneTriggerExecuterStateData> states, int defaultStateID)
    {
      foreach (SceneTriggerExecuterStateData state in states)
      {
        Dictionary<int, SceneTriggerExceuterState> states1 = this.States;
        int stateId = state.StateID;
        SceneTriggerExceuterState triggerExceuterState = new SceneTriggerExceuterState();
        triggerExceuterState.StateID = state.StateID;
        triggerExceuterState.PlayerDisconnectToStateID = state.PlayerDisconnectToStateID;
        int num = state.PlayerDisconnectToStateImmediate ? 1 : 0;
        triggerExceuterState.PlayerDisconnectToStateImmediate = num != 0;
        states1.Add(stateId, triggerExceuterState);
      }
      if (!this.States.ContainsKey(defaultStateID))
        return;
      this.DefaultStateID = defaultStateID;
      this.StateID = defaultStateID;
    }

    public SceneTriggerExecuterDetails ChangeState(long sender, SceneTriggerExecuterDetails details)
    {
      Player player = (Player) null;
      try
      {
        player = Server.Instance.GetPlayer(sender);
        details.PlayerThatActivated = player.FakeGuid;
      }
      catch
      {
        details.PlayerThatActivated = 0L;
      }
      if (details.ProximityTriggerID.HasValue)
      {
        if (this.ProximityTriggers == null || !this.ProximityTriggers.ContainsKey(details.ProximityTriggerID.Value))
        {
          details.IsFail = true;
          return details;
        }
        bool flag = this.ProximityTriggers[details.ProximityTriggerID.Value].ObjectsInTrigger.Contains(sender);
        if (details.ProximityIsEnter.Value)
        {
          if (!flag)
            this.ProximityTriggers[details.ProximityTriggerID.Value].ObjectsInTrigger.Add(sender);
          if (this.ProximityTriggers[details.ProximityTriggerID.Value].ObjectsInTrigger.Count == 0)
            details.IsFail = true;
        }
        else
        {
          if (flag)
            this.ProximityTriggers[details.ProximityTriggerID.Value].ObjectsInTrigger.Remove(sender);
          if (this.ProximityTriggers[details.ProximityTriggerID.Value].ObjectsInTrigger.Count > 0)
            details.IsFail = true;
        }
      }
      if (this.StateID == details.NewStateID && (!details.IsImmediate.HasValue || !details.IsImmediate.Value))
        details.IsFail = true;
      if (!this.States.ContainsKey(details.NewStateID))
        details.IsFail = true;
      if (this.SpawnPoint != null && (this.SpawnPoint.Type == SpawnPointType.WithAuthorization && this.SpawnPoint.State == SpawnPointState.Locked || this.SpawnPoint.Player != null && this.SpawnPoint.Player != player || this.SpawnPoint.Player == null))
        details.IsFail = true;
      if (!details.IsFail)
      {
        this.StateID = details.NewStateID;
        if (this.SpawnPoint != null && this.SpawnPoint.Player != null)
        {
          if (this.StateID == this.SpawnPoint.ExecuterStateID || this.SpawnPoint.ExecuterOccupiedStateIDs != null && this.SpawnPoint.ExecuterOccupiedStateIDs.Contains(this.StateID))
          {
            this.SpawnPoint.Player.IsInsideSpawnPoint = true;
            this.SpawnPoint.IsPlayerInSpawnPoint = true;
          }
          else
          {
            this.SpawnPoint.Player.IsInsideSpawnPoint = false;
            this.SpawnPoint.IsPlayerInSpawnPoint = false;
            if (this.SpawnPoint.Type == SpawnPointType.SimpleSpawn)
            {
              this.SpawnPoint.Player.SetSpawnPoint((ShipSpawnPoint) null);
              this.SpawnPoint.Player = (Player) null;
            }
          }
        }
        try
        {
          this.PlayerThatActivated = Server.Instance.GetPlayer(sender).FakeGuid;
        }
        catch
        {
          this.PlayerThatActivated = 0L;
        }
      }
      return details;
    }

    public SceneTriggerExecuterDetails RemovePlayerFromExecuter(Player pl)
    {
      if (this.PlayerThatActivated != pl.FakeGuid || !this.States.ContainsKey(this._stateID) || this.States[this._stateID].PlayerDisconnectToStateID == 0 || !this.States.ContainsKey(this.States[this._stateID].PlayerDisconnectToStateID))
        return (SceneTriggerExecuterDetails) null;
      SceneTriggerExecuterDetails triggerExecuterDetails = new SceneTriggerExecuterDetails();
      triggerExecuterDetails.InSceneID = this.InSceneID;
      int num1 = 0;
      triggerExecuterDetails.IsFail = num1 != 0;
      int stateId = this.StateID;
      triggerExecuterDetails.CurrentStateID = stateId;
      int disconnectToStateId = this.States[this._stateID].PlayerDisconnectToStateID;
      triggerExecuterDetails.NewStateID = disconnectToStateId;
      bool? nullable = new bool?(this.States[this._stateID].PlayerDisconnectToStateImmediate);
      triggerExecuterDetails.IsImmediate = nullable;
      long num2 = 0;
      triggerExecuterDetails.PlayerThatActivated = num2;
      return triggerExecuterDetails;
    }

    public SceneTriggerExecuterDetails RemovePlayerFromProximity(Player pl)
    {
      if (this.ProximityTriggers == null || this.ProximityTriggers.Count == 0)
        return (SceneTriggerExecuterDetails) null;
      SceneTriggerProximity triggerProximity = (SceneTriggerProximity) null;
      bool flag = false;
      foreach (KeyValuePair<int, SceneTriggerProximity> proximityTrigger in this.ProximityTriggers)
      {
        if (proximityTrigger.Value.ObjectsInTrigger.Contains(pl.GUID))
        {
          proximityTrigger.Value.ObjectsInTrigger.Remove(pl.GUID);
          if (proximityTrigger.Value.ObjectsInTrigger.Count == 0)
            triggerProximity = proximityTrigger.Value;
        }
        if (proximityTrigger.Value.ObjectsInTrigger.Count > 0)
          flag = true;
      }
      if (flag || triggerProximity == null || this.StateID != triggerProximity.ActiveStateID)
        return (SceneTriggerExecuterDetails) null;
      SceneTriggerExecuterDetails triggerExecuterDetails = new SceneTriggerExecuterDetails();
      triggerExecuterDetails.InSceneID = this.InSceneID;
      int num1 = 0;
      triggerExecuterDetails.IsFail = num1 != 0;
      int stateId = this.StateID;
      triggerExecuterDetails.CurrentStateID = stateId;
      int inactiveStateId = triggerProximity.InactiveStateID;
      triggerExecuterDetails.NewStateID = inactiveStateId;
      long num2 = 0;
      triggerExecuterDetails.PlayerThatActivated = num2;
      int? nullable1 = new int?(triggerProximity.TriggerID);
      triggerExecuterDetails.ProximityTriggerID = nullable1;
      bool? nullable2 = new bool?(false);
      triggerExecuterDetails.ProximityIsEnter = nullable2;
      return triggerExecuterDetails;
    }

    public bool AreStatesEqual(SceneTriggerExecuter other)
    {
      return this.States.Count == other.States.Count;
    }

    public PersistenceObjectData GetPersistenceData()
    {
      return (PersistenceObjectData) new PersistenceObjectDataExecuter()
      {
        InSceneID = this.InSceneID,
        StateID = this.StateID,
        PlayerThatActivated = this.PlayerThatActivated
      };
    }

    public void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        PersistenceObjectDataExecuter objectDataExecuter = persistenceData as PersistenceObjectDataExecuter;
        if (objectDataExecuter == null)
        {
          Dbg.Warning("PersistenceObjectDataExecuter data is null");
        }
        else
        {
          this.StateID = objectDataExecuter.StateID;
          this.PlayerThatActivated = objectDataExecuter.PlayerThatActivated;
        }
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }
  }
}
