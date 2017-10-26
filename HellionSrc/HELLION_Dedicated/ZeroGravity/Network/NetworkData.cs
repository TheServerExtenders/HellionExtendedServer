// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.NetworkData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;

namespace ZeroGravity.Network
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  [ProtoInclude(100, typeof (LogInRequest))]
  [ProtoInclude(101, typeof (LogOutRequest))]
  [ProtoInclude(102, typeof (ServerStatusRequest))]
  [ProtoInclude(103, typeof (PlayerSpawnRequest))]
  [ProtoInclude(104, typeof (PlayerRespawnRequest))]
  [ProtoInclude(105, typeof (SpawnObjectsRequest))]
  [ProtoInclude(106, typeof (SubscribeToObjectsRequest))]
  [ProtoInclude(107, typeof (UnsubscribeFromObjectsRequest))]
  [ProtoInclude(108, typeof (ManeuverCourseRequest))]
  [ProtoInclude(109, typeof (DistressCallRequest))]
  [ProtoInclude(110, typeof (SuicideRequest))]
  [ProtoInclude(111, typeof (PlayersOnServerRequest))]
  [ProtoInclude(112, typeof (AvailableSpawnPointsRequest))]
  [ProtoInclude(113, typeof (VesselSecurityRequest))]
  [ProtoInclude(114, typeof (VesselRequest))]
  [ProtoInclude(200, typeof (LogInResponse))]
  [ProtoInclude(201, typeof (LogOutResponse))]
  [ProtoInclude(202, typeof (ServerStatusResponse))]
  [ProtoInclude(203, typeof (PlayerSpawnResponse))]
  [ProtoInclude(204, typeof (PlayerRespawnResponse))]
  [ProtoInclude(205, typeof (SpawnObjectsResponse))]
  [ProtoInclude(206, typeof (ManeuverCourseResponse))]
  [ProtoInclude(207, typeof (PlayerDrillingResponse))]
  [ProtoInclude(208, typeof (DistressCallResponse))]
  [ProtoInclude(209, typeof (PlayersOnServerResponse))]
  [ProtoInclude(210, typeof (AvailableSpawnPointsResponse))]
  [ProtoInclude(211, typeof (VesselSecurityResponse))]
  [ProtoInclude(212, typeof (VesselRequestResponse))]
  [ProtoInclude(300, typeof (CheckConnectionMessage))]
  [ProtoInclude(301, typeof (EnvironmentReadyMessage))]
  [ProtoInclude(302, typeof (CharacterMovementMessage))]
  [ProtoInclude(303, typeof (DestroyObjectMessage))]
  [ProtoInclude(304, typeof (PlayerStatsMessage))]
  [ProtoInclude(305, typeof (PlayerShootingMessage))]
  [ProtoInclude(306, typeof (PlayerHitMessage))]
  [ProtoInclude(307, typeof (MoveDynamicObectMessage))]
  [ProtoInclude(308, typeof (MovementMessage))]
  [ProtoInclude(309, typeof (ShipStatsMessage))]
  [ProtoInclude(311, typeof (KillPlayerMessage))]
  [ProtoInclude(312, typeof (CorpseStatsMessage))]
  [ProtoInclude(313, typeof (PlayerRoomMessage))]
  [ProtoInclude(314, typeof (MoveCorpseObectMessage))]
  [ProtoInclude(316, typeof (DynamicObjectStatsMessage))]
  [ProtoInclude(317, typeof (TurretShootingMessage))]
  [ProtoInclude(318, typeof (DestroyVesselMessage))]
  [ProtoInclude(319, typeof (ResetServer))]
  [ProtoInclude(320, typeof (TextChatMessage))]
  [ProtoInclude(321, typeof (PlayerDrillingMessage))]
  [ProtoInclude(322, typeof (InitializeSpaceObjectMessage))]
  [ProtoInclude(323, typeof (ToggleGodModeMessage))]
  [ProtoInclude(324, typeof (DynamicObjectsInfoMessage))]
  [ProtoInclude(325, typeof (TransferResourceMessage))]
  [ProtoInclude(326, typeof (RefineResourceMessage))]
  [ProtoInclude(327, typeof (ShipCollisionMessage))]
  [ProtoInclude(328, typeof (ResetBaseBuilding))]
  [ProtoInclude(329, typeof (SaveGameMessage))]
  [ProtoInclude(330, typeof (VoiceCommDataMessage))]
  [ProtoInclude(331, typeof (CheckInMessage))]
  [ProtoInclude(332, typeof (ServerShutDownMessage))]
  [ProtoInclude(333, typeof (LatencyTestMessage))]
  [ProtoInclude(334, typeof (ServerUpdateMessage))]
  [ProtoInclude(335, typeof (NameTagMessage))]
  [ProtoInclude(336, typeof (PortableTurretShootingMessage))]
  [ProtoInclude(337, typeof (GrenadeBlastMessage))]
  [ProtoInclude(338, typeof (FabricateItemMessage))]
  [ProtoInclude(339, typeof (RepairItemMessage))]
  [ProtoInclude(340, typeof (RepairVesselMessage))]
  [ProtoInclude(341, typeof (HurtPlayerMessage))]
  [ProtoInclude(404, typeof (SignInRequest))]
  [ProtoInclude(406, typeof (DeleteCharacterRequest))]
  [ProtoInclude(408, typeof (CheckInRequest))]
  [ProtoInclude(500, typeof (MainServerGenericResponse))]
  [ProtoInclude(502, typeof (SignInResponse))]
  [ProtoInclude(504, typeof (CheckInResponse))]
  [ProtoInclude(506, typeof (DeleteCharacterResponse))]
  public abstract class NetworkData
  {
    public long Sender;

    public override string ToString()
    {
      return Json.Serialize((object) this, Json.Formatting.Indented);
    }
  }
}
