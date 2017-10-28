using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HellionExtendedServer.Managers.Event;
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

    public class EventHelper
    {
        public EventSystem ES2;
        protected List<EventListener> RegiteredEvents = new List<EventListener>();

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
                if (AddedTypes.Contains(entry.Key)) continue;
                AddedTypes.Add(entry.Key);
                NetworkManager.Instance.NetContoller.EventSystem.AddListener(entry.Key, MassEventHandeler);
                //Listen for Everything!
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