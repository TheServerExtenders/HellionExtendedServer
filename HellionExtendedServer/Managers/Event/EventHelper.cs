using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HellionExtendedServer.Common;
using HellionExtendedServer.Managers.Event.Player;
using ZeroGravity;
using ZeroGravity.Data;
using ZeroGravity.Helpers;
using ZeroGravity.Network;
using NetworkManager = HellionExtendedServer.Managers.NetworkManager;

namespace HellionExtendedServer.Managers.Event
{
    public enum EventID
    {
        None,
        CharacterDataRequest,
        CharacterDataResponse,
        CharacterListResponse,
        CharacterMovementMessage,
        CheckConnectionMessage,
        CheckDeletedMessage,
        CheckInMessage,
        CheckInRequest,
        CheckInResponse,
        CorpseStatsMessage,
        CreateCharacterRequest,
        CreateCharacterResponse,
        DeleteCharacterRequest,
        DestroyObjectMessage,
        DestroyShipMessage,
        DistressCallRequest,
        DistressCallResponse,
        DynamicObjectsInfoMessage,
        DynamicObjectStatsMessage,
        EnvironmentReadyMessage,
        GetDeletedCharactersRequest,
        InitializeSpaceObjectMessage,
        KillPlayerMessage,
        LatencyTestMessage,
        LogInRequest,
        LogInResponse,
        LogOutRequest,
        LogOutResponse,
        MainServerGenericResponse,
        ManeuverCourseRequest,
        ManeuverCourseResponse,
        MarkAsLoggedInRequest,
        MarkAsLoggedOutRequest,
        MoveCharacterToLimboRequest,
        MoveCorpseObectMessage,
        MoveDynamicObectMessage,
        MovementMessage,
        PlayerDrillingMessage,
        PlayerDrillingResponse,
        PlayerHitMessage,
        PlayerRespawnRequest,
        PlayerRespawnResponse,
        PlayerRoomMessage,
        PlayerShootingMessage,
        PlayersOnServerRequest,
        PlayersOnServerResponse,
        PlayerSpawnRequest,
        PlayerSpawnResponse,
        PlayerStatsMessage,
        RefineResourceMessage,
        ResetBaseBuilding,
        ResetServer,
        SaveGameMessage,
        ServerShutDownMessage,
        ServerStatusRequest,
        ServerStatusResponse,
        ShipCollisionMessage,
        ShipStatsMessage,
        SignInRequest,
        SignInResponse,
        SpawnObjectsRequest,
        SpawnObjectsResponse,
        SubscribeToObjectsRequest,
        SuicideRequest,
        TextChatMessage,
        ToggleGodModeMessage,
        TransferResourceMessage,
        TurretShootingMessage,
        UnsubscribeFromObjectsRequest,
        VoiceCommDataMessage
    };

    //TODO
    //Create a tick class
    /*
     * Ticks every 3 Secs
     * Looks for differenace between the 2 lists
     * Removes those events that are now removed
     * Adds those events that are now new
     */

    public class EventHelper
    {
        public EventSystem ES2;
        protected List<EventListener> RegiteredEvents = new List<EventListener>();
        private ThreadSafeDictionary<Type, EventSystem.NetworkDataDelegate> Main_NDG;
        private ThreadSafeDictionary<Type, EventSystem.NetworkDataDelegate> Last_NDG;
        private ThreadSafeDictionary<Type, EventSystem.NetworkDataDelegate> Link_NDG;

        public EventHelper()
        {
            ES2 = NetworkManager.Instance.NetContoller.EventSystem; //Copies Events

            //Unregister All Listeners
            //Get All Listeners to Register
            ThreadSafeDictionary<Type, EventSystem.NetworkDataDelegate> networkDataGroups = GetCurrentListenersNetwork();
            if (networkDataGroups == null)
            {
                Log.Instance.Error("Error starting EventHandeler! Could not find all events!");
                return;
            }
            List<Type> AddedTypes = new List<Type>();
            foreach (KeyValuePair<Type, EventSystem.NetworkDataDelegate> entry in networkDataGroups)
            {
                EventSystem.NetworkDataDelegate v = entry.Value;
                foreach (Delegate d in v.GetInvocationList())
                {
                    EventID eventid = EventID.None;
                    Type data = entry.Key;
                    //) eventid = 
                    if (data == typeof(CharacterMovementMessage)) eventid = EventID.CharacterMovementMessage;
                    else if (data == typeof(CheckConnectionMessage)) eventid = EventID.CheckConnectionMessage;
                    //else if (data == typeof(CheckDeletedMessage)) eventid = EventID.CheckDeletedMessage;
                    else if (data == typeof(CheckInMessage)) eventid = EventID.CheckInMessage;
                    else if (data == typeof(CheckInRequest)) eventid = EventID.CheckInRequest;
                    else if (data == typeof(CheckInResponse)) eventid = EventID.CheckInResponse;
                    else if (data == typeof(CorpseStatsMessage)) eventid = EventID.CorpseStatsMessage;
                    //else if (data == typeof(CreateCharacterRequest)) eventid = EventID.CreateCharacterRequest;
                    //else if (data == typeof(CreateCharacterResponse)) eventid = EventID.CreateCharacterResponse;
                    else if (data == typeof(DeleteCharacterRequest)) eventid = EventID.DeleteCharacterRequest;
                    else if (data == typeof(DestroyObjectMessage)) eventid = EventID.DestroyObjectMessage;
                    //else if (data == typeof(DestroyShipMessage)) eventid = EventID.DestroyShipMessage;
                    else if (data == typeof(DistressCallRequest)) eventid = EventID.DistressCallRequest;
                    else if (data == typeof(DistressCallResponse)) eventid = EventID.DistressCallResponse;
                    else if (data == typeof(DynamicObjectsInfoMessage)) eventid = EventID.DynamicObjectsInfoMessage;
                    else if (data == typeof(DynamicObjectStatsMessage)) eventid = EventID.DynamicObjectStatsMessage;
                    else if (data == typeof(EnvironmentReadyMessage)) eventid = EventID.EnvironmentReadyMessage;
                    //else if (data == typeof(GetDeletedCharactersRequest)) eventid = EventID.GetDeletedCharactersRequest;
                    else if (data == typeof(InitializeSpaceObjectMessage)) eventid = EventID.InitializeSpaceObjectMessage;
                    else if (data == typeof(KillPlayerMessage)) eventid = EventID.KillPlayerMessage;
                    else if (data == typeof(LatencyTestMessage)) eventid = EventID.LatencyTestMessage;
                    else if (data == typeof(LogInRequest)) eventid = EventID.LogInRequest;
                    else if (data == typeof(LogInResponse)) eventid = EventID.LogInResponse;
                    else if (data == typeof(LogOutRequest)) eventid = EventID.LogOutRequest;
                    else if (data == typeof(LogOutResponse)) eventid = EventID.LogOutResponse;
                    else if (data == typeof(MainServerGenericResponse)) eventid = EventID.MainServerGenericResponse;
                    else if (data == typeof(ManeuverCourseRequest)) eventid = EventID.ManeuverCourseRequest;
                    else if (data == typeof(ManeuverCourseResponse)) eventid = EventID.ManeuverCourseResponse;
                    //else if (data == typeof(MarkAsLoggedInRequest)) eventid = EventID.MarkAsLoggedInRequest;
                    //else if (data == typeof(MarkAsLoggedOutRequest)) eventid = EventID.MarkAsLoggedOutRequest;
                    //else if (data == typeof(MoveCharacterToLimboRequest)) eventid = EventID.MoveCharacterToLimboRequest;
                    else if (data == typeof(MoveCorpseObectMessage)) eventid = EventID.MoveCorpseObectMessage;
                    else if (data == typeof(MoveDynamicObectMessage)) eventid = EventID.MoveDynamicObectMessage;
                    else if (data == typeof(MovementMessage)) eventid = EventID.MovementMessage;
                    else if (data == typeof(PlayerDrillingMessage)) eventid = EventID.PlayerDrillingMessage;
                    else if (data == typeof(PlayerDrillingResponse)) eventid = EventID.PlayerDrillingResponse;
                    else if (data == typeof(PlayerHitMessage)) eventid = EventID.PlayerHitMessage;
                    else if (data == typeof(PlayerRespawnRequest)) eventid = EventID.PlayerRespawnRequest;
                    else if (data == typeof(PlayerRespawnResponse)) eventid = EventID.PlayerRespawnResponse;
                    else if (data == typeof(PlayerRoomMessage)) eventid = EventID.PlayerRoomMessage;
                    else if (data == typeof(PlayerShootingMessage)) eventid = EventID.PlayerShootingMessage;
                    else if (data == typeof(PlayersOnServerRequest)) eventid = EventID.PlayersOnServerRequest;
                    else if (data == typeof(PlayersOnServerResponse)) eventid = EventID.PlayersOnServerResponse;
                    else if (data == typeof(PlayerSpawnRequest)) eventid = EventID.PlayerSpawnRequest;
                    else if (data == typeof(PlayerSpawnResponse)) eventid = EventID.PlayerSpawnResponse;
                    else if (data == typeof(PlayerStatsMessage)) eventid = EventID.PlayerStatsMessage;
                    else if (data == typeof(RefineResourceMessage)) eventid = EventID.RefineResourceMessage;
                    else if (data == typeof(ResetBaseBuilding)) eventid = EventID.ResetBaseBuilding;
                    else if (data == typeof(ResetServer)) eventid = EventID.ResetServer;
                    else if (data == typeof(SaveGameMessage)) eventid = EventID.SaveGameMessage;
                    else if (data == typeof(ServerShutDownMessage)) eventid = EventID.ServerShutDownMessage;
                    else if (data == typeof(ServerStatusRequest)) eventid = EventID.ServerStatusRequest;
                    else if (data == typeof(ServerStatusResponse)) eventid = EventID.ServerStatusResponse;
                    else if (data == typeof(ShipCollisionMessage)) eventid = EventID.ShipCollisionMessage;
                    else if (data == typeof(ShipStatsMessage)) eventid = EventID.ShipStatsMessage;
                    else if (data == typeof(SignInRequest)) eventid = EventID.SignInRequest;
                    else if (data == typeof(SignInResponse)) eventid = EventID.SignInResponse;
                    else if (data == typeof(SpawnObjectsRequest)) eventid = EventID.SpawnObjectsRequest;
                    else if (data == typeof(SpawnObjectsResponse)) eventid = EventID.SpawnObjectsResponse;
                    else if (data == typeof(SubscribeToObjectsRequest)) eventid = EventID.SubscribeToObjectsRequest;
                    else if (data == typeof(SuicideRequest)) eventid = EventID.SuicideRequest;
                    else if (data == typeof(TextChatMessage)) eventid = EventID.TextChatMessage;
                    else if (data == typeof(ToggleGodModeMessage)) eventid = EventID.ToggleGodModeMessage;
                    else if (data == typeof(TransferResourceMessage)) eventid = EventID.TransferResourceMessage;
                    else if (data == typeof(TurretShootingMessage)) eventid = EventID.TurretShootingMessage;
                    else if (data == typeof(UnsubscribeFromObjectsRequest)) eventid = EventID.UnsubscribeFromObjectsRequest;
                    else if (data == typeof(VoiceCommDataMessage)) eventid = EventID.VoiceCommDataMessage;

                    RegisterEvent(new EventListener((EventSystem.NetworkDataDelegate) d, eventid));
                    ES2.RemoveListener(entry.Key, (EventSystem.NetworkDataDelegate) d);
                }
                if (AddedTypes.Contains(entry.Key)) continue;
                AddedTypes.Add(entry.Key);
                ES2.AddListener(entry.Key, MassEventHandeler);
                //Listen for Everything!
            }
        }

        public void Tick()
        {
            Main_NDG = GetCurrentListenersNetwork();
            if (Last_NDG == null) return;
            foreach (KeyValuePair<Type, EventSystem.NetworkDataDelegate> m in Main_NDG)
            {
                foreach (KeyValuePair<Type, EventSystem.NetworkDataDelegate> l in Last_NDG)
                {
                    //Same Key
                    if (m.Key == l.Key)
                    {
                        foreach (Delegate mi in m.Value.GetInvocationList())
                        {
                            bool found = false;
                            foreach (Delegate li in l.Value.GetInvocationList())
                            {
                                if (li == mi) found = true;
                            }
                            //If found
                        }
                    }
                }
            }
        }

        public ThreadSafeDictionary<Type, EventSystem.NetworkDataDelegate>
            GetCurrentListenersNetwork()
        {
            //HACK
            try
            {
                BindingFlags bf = BindingFlags.Instance | BindingFlags.NonPublic;
                FieldInfo mi =
                    NetworkManager.Instance.NetContoller.EventSystem.GetType().GetField("networkDataGroups", bf);
                ThreadSafeDictionary<Type, EventSystem.NetworkDataDelegate> a =
                    (ThreadSafeDictionary<Type, EventSystem.NetworkDataDelegate>)
                    mi.GetValue(NetworkManager.Instance.NetContoller.EventSystem);
                ThreadSafeDictionary<Type, EventSystem.NetworkDataDelegate> b =
                    new ThreadSafeDictionary<Type, EventSystem.NetworkDataDelegate>();
                foreach (KeyValuePair<Type, EventSystem.NetworkDataDelegate> entry in a)
                {
                    b.Add(entry.Key, entry.Value);
                }
                return b;
            }
            catch (Exception Ex)
            {
                Log.Instance.Info("ERROR!" + Ex.ToString());
            }

            return null;
        }


        public void MassEventHandeler(NetworkData data)
        {
            //TDOD add all types
            //if (data is CharacterDataRequest) ExecuteEvent(new GenericEvent(EventID.CharacterDataRequest, data));
            //else if (data is CharacterDataResponse) ExecuteEvent(new GenericEvent(EventID.CharacterDataResponse, data));
            //else if (data is CharacterListResponse) ExecuteEvent(new GenericEvent(EventID.CharacterListResponse, data));
            if (data is CharacterMovementMessage)
                ExecuteEvent(new GenericEvent(EventID.CharacterMovementMessage, data));
            else if (data is CheckConnectionMessage) ExecuteEvent(new GenericEvent(EventID.CheckConnectionMessage, data));
            //else if (data is CheckDeletedMessage) ExecuteEvent(new GenericEvent(EventID.CheckDeletedMessage, data));
            else if (data is CheckInMessage) ExecuteEvent(new GenericEvent(EventID.CheckInMessage, data));
            else if (data is CheckInRequest) ExecuteEvent(new GenericEvent(EventID.CheckInRequest, data));
            else if (data is CheckInResponse) ExecuteEvent(new GenericEvent(EventID.CheckInResponse, data));
            else if (data is CorpseStatsMessage) ExecuteEvent(new GenericEvent(EventID.CorpseStatsMessage, data));
            //else if (data is CreateCharacterRequest) ExecuteEvent(new GenericEvent(EventID.CreateCharacterRequest, data));
            //else if (data is CreateCharacterResponse) ExecuteEvent(new GenericEvent(EventID.CreateCharacterResponse, data));
            else if (data is DeleteCharacterRequest) ExecuteEvent(new GenericEvent(EventID.DeleteCharacterRequest, data));
            else if (data is DestroyObjectMessage) ExecuteEvent(new GenericEvent(EventID.DestroyObjectMessage, data));
            //else if (data is DestroyShipMessage) ExecuteEvent(new GenericEvent(EventID.DestroyShipMessage, data));
            else if (data is DistressCallRequest) ExecuteEvent(new GenericEvent(EventID.DistressCallRequest, data));
            else if (data is DistressCallResponse) ExecuteEvent(new GenericEvent(EventID.DistressCallResponse, data));
            else if (data is DynamicObjectsInfoMessage)
                ExecuteEvent(new GenericEvent(EventID.DynamicObjectsInfoMessage, data));
            else if (data is DynamicObjectStatsMessage)
                ExecuteEvent(new GenericEvent(EventID.DynamicObjectStatsMessage, data));
            else if (data is EnvironmentReadyMessage) ExecuteEvent(new GenericEvent(EventID.EnvironmentReadyMessage, data));
            //else if (data is GetDeletedCharactersRequest)
            //ExecuteEvent(new GenericEvent(EventID.GetDeletedCharactersRequest, data));
            else if (data is InitializeSpaceObjectMessage)
                ExecuteEvent(new GenericEvent(EventID.InitializeSpaceObjectMessage, data));
            else if (data is KillPlayerMessage) ExecuteEvent(new GenericEvent(EventID.KillPlayerMessage, data));
            else if (data is LatencyTestMessage) ExecuteEvent(new GenericEvent(EventID.LatencyTestMessage, data));
            else if (data is LogInRequest) ExecuteEvent(new GenericEvent(EventID.LogInRequest, data));
            else if (data is LogInResponse) ExecuteEvent(new GenericEvent(EventID.LogInResponse, data));
            else if (data is LogOutRequest) ExecuteEvent(new GenericEvent(EventID.LogOutRequest, data));
            else if (data is LogOutResponse) ExecuteEvent(new GenericEvent(EventID.LogOutResponse, data));
            else if (data is MainServerGenericResponse)
                ExecuteEvent(new GenericEvent(EventID.MainServerGenericResponse, data));
            else if (data is ManeuverCourseRequest) ExecuteEvent(new GenericEvent(EventID.ManeuverCourseRequest, data));
            else if (data is ManeuverCourseResponse) ExecuteEvent(new GenericEvent(EventID.ManeuverCourseResponse, data));
            //else if (data is MarkAsLoggedInRequest) ExecuteEvent(new GenericEvent(EventID.MarkAsLoggedInRequest, data));
            //else if (data is MarkAsLoggedOutRequest) ExecuteEvent(new GenericEvent(EventID.MarkAsLoggedOutRequest, data));
            //else if (data is MoveCharacterToLimboRequest)
            //ExecuteEvent(new GenericEvent(EventID.MoveCharacterToLimboRequest, data));
            else if (data is MoveCorpseObectMessage) ExecuteEvent(new GenericEvent(EventID.MoveCorpseObectMessage, data));
            else if (data is MoveDynamicObectMessage) ExecuteEvent(new GenericEvent(EventID.MoveDynamicObectMessage, data));
            else if (data is MovementMessage) ExecuteEvent(new GenericEvent(EventID.MovementMessage, data));
            else if (data is PlayerDrillingMessage) ExecuteEvent(new GenericEvent(EventID.PlayerDrillingMessage, data));
            else if (data is PlayerDrillingResponse) ExecuteEvent(new GenericEvent(EventID.PlayerDrillingResponse, data));
            else if (data is PlayerHitMessage) ExecuteEvent(new GenericEvent(EventID.PlayerHitMessage, data));
            else if (data is PlayerRespawnRequest) ExecuteEvent(new GenericEvent(EventID.PlayerRespawnRequest, data));
            else if (data is PlayerRespawnResponse) ExecuteEvent(new GenericEvent(EventID.PlayerRespawnResponse, data));
            else if (data is PlayerRoomMessage) ExecuteEvent(new GenericEvent(EventID.PlayerRoomMessage, data));
            else if (data is PlayerShootingMessage) ExecuteEvent(new GenericEvent(EventID.PlayerShootingMessage, data));
            else if (data is PlayersOnServerRequest) ExecuteEvent(new GenericEvent(EventID.PlayersOnServerRequest, data));
            else if (data is PlayersOnServerResponse) ExecuteEvent(new GenericEvent(EventID.PlayersOnServerResponse, data));
            else if (data is PlayerSpawnRequest) ExecuteEvent(new GenericEvent(EventID.PlayerSpawnRequest, data));
            else if (data is PlayerSpawnResponse) ExecuteEvent(new GenericEvent(EventID.PlayerSpawnResponse, data));
            else if (data is PlayerStatsMessage) ExecuteEvent(new GenericEvent(EventID.PlayerStatsMessage, data));
            else if (data is RefineResourceMessage) ExecuteEvent(new GenericEvent(EventID.RefineResourceMessage, data));
            else if (data is ResetBaseBuilding) ExecuteEvent(new GenericEvent(EventID.ResetBaseBuilding, data));
            else if (data is ResetServer) ExecuteEvent(new GenericEvent(EventID.ResetServer, data));
            else if (data is SaveGameMessage) ExecuteEvent(new GenericEvent(EventID.SaveGameMessage, data));
            else if (data is ServerShutDownMessage) ExecuteEvent(new GenericEvent(EventID.ServerShutDownMessage, data));
            else if (data is ServerStatusRequest) ExecuteEvent(new GenericEvent(EventID.ServerStatusRequest, data));
            else if (data is ServerStatusResponse) ExecuteEvent(new GenericEvent(EventID.ServerStatusResponse, data));
            else if (data is ShipCollisionMessage) ExecuteEvent(new GenericEvent(EventID.ShipCollisionMessage, data));
            else if (data is ShipStatsMessage) ExecuteEvent(new GenericEvent(EventID.ShipStatsMessage, data));
            else if (data is SignInRequest) ExecuteEvent(new GenericEvent(EventID.SignInRequest, data));
            else if (data is SignInResponse) ExecuteEvent(new GenericEvent(EventID.SignInResponse, data));
            else if (data is SpawnObjectsRequest) ExecuteEvent(new GenericEvent(EventID.SpawnObjectsRequest, data));
            else if (data is SpawnObjectsResponse) ExecuteEvent(new GenericEvent(EventID.SpawnObjectsResponse, data));
            else if (data is SubscribeToObjectsRequest) ExecuteEvent(new GenericEvent(EventID.SubscribeToObjectsRequest, data));
            else if (data is SuicideRequest) ExecuteEvent(new GenericEvent(EventID.SuicideRequest, data));
            else if (data is TextChatMessage) ExecuteEvent(new GenericEvent(EventID.TextChatMessage, data));
            else if (data is ToggleGodModeMessage) ExecuteEvent(new GenericEvent(EventID.ToggleGodModeMessage, data));
            else if (data is TransferResourceMessage) ExecuteEvent(new GenericEvent(EventID.TransferResourceMessage, data));
            else if (data is TurretShootingMessage) ExecuteEvent(new GenericEvent(EventID.TurretShootingMessage, data));
            else if (data is UnsubscribeFromObjectsRequest) ExecuteEvent(new GenericEvent(EventID.UnsubscribeFromObjectsRequest, data));
            else if (data is VoiceCommDataMessage) ExecuteEvent(new GenericEvent(EventID.VoiceCommDataMessage, data));
        }


        public void RegisterEvent(EventListener e)
        {
            RegiteredEvents.Add(e);
            //TODO notify of Regerstration
        }

        public void ExecuteEvent(GenericEvent e)
        {
            foreach (EventListener evnt in RegiteredEvents)
            {
                if (e.GetEventType == evnt.GetEventType)
                {
                    if (!e.IsCanceled || e.IsCanceled && evnt.IgnoreCanceledEvent)
                    {
                        evnt.Execute(e);
                    }
                }
            }
        }
    }
}