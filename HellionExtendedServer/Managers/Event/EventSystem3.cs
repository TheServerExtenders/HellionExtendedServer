using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HellionExtendedServer.Common;
using HellionExtendedServer.Managers.Event.Player;
using ZeroGravity.Helpers;
using ZeroGravity.Network;

namespace HellionExtendedServer.Managers.Event
{
    class EventSystem3 : EventSystem
    {
        public EventSystem ES2;
        protected List<EventListener> RegiteredEvents = new List<EventListener>();

        

    public new void AddListener(Type group, EventSystem.NetworkDataDelegate function)
        {
            Log.Instance.Info("PACKET FOR dddddaaaaaaaaaaaaddddddd!");
            base.AddListener(group, function);
        }

        public new void RemoveListener(Type group, EventSystem.NetworkDataDelegate function)
        {
            Log.Instance.Info("PACKET FOR BPOIIIIIIIII");
            base.RemoveListener(group, function);
        }

        public void Invoke(NetworkData data)
        {
            Log.Instance.Info("PACKET FOR INVOTE!");
            MassEventHandeler(data);
        }
        /*
        public new void Invoke(NetworkData data)
        {
            Log.Instance.Info("PACKET FOR INVOTE!");
            EH.MassEventHandeler(data);
        }*/

        public new void InvokeQueuedData()
        {
            Log.Instance.Info("PACKET FOR INVOasdasdasdasdasdasdsaTE2");
            base.InvokeQueuedData();
        }

        public new void AddListener(EventSystem.InternalEventType group, EventSystem.InternalEventsDelegate function)
        {
            Log.Instance.Info("PACKET FOR ADDDDDDDDDDDDD!");
            base.AddListener(group, function);
        }

        public new void RemoveListener(EventSystem.InternalEventType group, EventSystem.InternalEventsDelegate function)
        {
            Log.Instance.Info("PACKET FOR INVOTE2ssssss");
            base.RemoveListener(group, function);
        }

        public new void Invoke(EventSystem.InternalEventData data)
        {
            Log.Instance.Info("PACKET FOR INVOTE2");
            base.Invoke(data);
        }

        public EventSystem3() : base()
        {
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
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.CharacterMovementMessage, data));
            else if (data is CheckConnectionMessage)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.CheckConnectionMessage,
                    data));
//else if (data is CheckDeletedMessage) ExecuteEvent(new GenericEvent(EventID.CheckDeletedMessage, data));
            else if (data is CheckInMessage)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.CheckInMessage, data));
            else if (data is CheckInRequest)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.CheckInRequest, data));
            else if (data is CheckInResponse)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.CheckInResponse, data));
            else if (data is CorpseStatsMessage)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.CorpseStatsMessage, data));
//else if (data is CreateCharacterRequest) ExecuteEvent(new GenericEvent(EventID.CreateCharacterRequest, data));
//else if (data is CreateCharacterResponse) ExecuteEvent(new GenericEvent(EventID.CreateCharacterResponse, data));
            else if (data is DeleteCharacterRequest)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.DeleteCharacterRequest,
                    data));
            else if (data is DestroyObjectMessage)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.DestroyObjectMessage, data))
                    ;
//else if (data is DestroyShipMessage) ExecuteEvent(new GenericEvent(EventID.DestroyShipMessage, data));
            else if (data is DistressCallRequest)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.DistressCallRequest, data));
            else if (data is DistressCallResponse)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.DistressCallResponse, data))
                    ;
            else if (data is DynamicObjectsInfoMessage)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.DynamicObjectsInfoMessage, data));
            else if (data is DynamicObjectStatsMessage)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.DynamicObjectStatsMessage, data));
            else if (data is EnvironmentReadyMessage)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.EnvironmentReadyMessage,
                    data));
//else if (data is GetDeletedCharactersRequest)
//ExecuteEvent(new GenericEvent(EventID.GetDeletedCharactersRequest, data));
            else if (data is InitializeSpaceObjectMessage)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.InitializeSpaceObjectMessage, data));
            else if (data is KillPlayerMessage)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.KillPlayerMessage, data));
            else if (data is LatencyTestMessage)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.LatencyTestMessage, data));
            else if (data is LogInRequest) ExecuteEvent(new GenericEvent(Managers.Event.EventID.LogInRequest, data));
            else if (data is LogInResponse) ExecuteEvent(new GenericEvent(Managers.Event.EventID.LogInResponse, data));
            else if (data is LogOutRequest) ExecuteEvent(new GenericEvent(Managers.Event.EventID.LogOutRequest, data));
            else if (data is LogOutResponse)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.LogOutResponse, data));
            else if (data is MainServerGenericResponse)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.MainServerGenericResponse, data));
            else if (data is ManeuverCourseRequest)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.ManeuverCourseRequest, data
                ));
            else if (data is ManeuverCourseResponse)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.ManeuverCourseResponse,
                    data));
//else if (data is MarkAsLoggedInRequest) ExecuteEvent(new GenericEvent(EventID.MarkAsLoggedInRequest, data));
//else if (data is MarkAsLoggedOutRequest) ExecuteEvent(new GenericEvent(EventID.MarkAsLoggedOutRequest, data));
//else if (data is MoveCharacterToLimboRequest)
//ExecuteEvent(new GenericEvent(EventID.MoveCharacterToLimboRequest, data));
            else if (data is MoveCorpseObectMessage)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.MoveCorpseObectMessage,
                    data));
            else if (data is MoveDynamicObectMessage)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.MoveDynamicObectMessage,
                    data));
            else if (data is MovementMessage)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.MovementMessage, data));
            else if (data is PlayerDrillingMessage)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.PlayerDrillingMessage, data
                ));
            else if (data is PlayerDrillingResponse)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.PlayerDrillingResponse,
                    data));
            else if (data is PlayerHitMessage)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.PlayerHitMessage, data));
            else if (data is PlayerRespawnRequest)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.PlayerRespawnRequest, data))
                    ;
            else if (data is PlayerRespawnResponse)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.PlayerRespawnResponse, data
                ));
            else if (data is PlayerRoomMessage)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.PlayerRoomMessage, data));
            else if (data is PlayerShootingMessage)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.PlayerShootingMessage, data
                ));
            else if (data is PlayersOnServerRequest)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.PlayersOnServerRequest,
                    data));
            else if (data is PlayersOnServerResponse)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.PlayersOnServerResponse,
                    data));
            else if (data is PlayerSpawnRequest)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.PlayerSpawnRequest, data));
            else if (data is PlayerSpawnResponse)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.PlayerSpawnResponse, data));
            else if (data is PlayerStatsMessage)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.PlayerStatsMessage, data));
            else if (data is RefineResourceMessage)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.RefineResourceMessage, data
                ));
            else if (data is ResetBaseBuilding)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.ResetBaseBuilding, data));
            else if (data is ResetServer) ExecuteEvent(new GenericEvent(Managers.Event.EventID.ResetServer, data));
            else if (data is SaveGameMessage)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.SaveGameMessage, data));
            else if (data is ServerShutDownMessage)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.ServerShutDownMessage, data
                ));
            else if (data is ServerStatusRequest)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.ServerStatusRequest, data));
            else if (data is ServerStatusResponse)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.ServerStatusResponse, data))
                    ;
            else if (data is ShipCollisionMessage)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.ShipCollisionMessage, data))
                    ;
            else if (data is ShipStatsMessage)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.ShipStatsMessage, data));
            else if (data is SignInRequest) ExecuteEvent(new GenericEvent(Managers.Event.EventID.SignInRequest, data));
            else if (data is SignInResponse)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.SignInResponse, data));
            else if (data is SpawnObjectsRequest)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.SpawnObjectsRequest, data));
            else if (data is SpawnObjectsResponse)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.SpawnObjectsResponse, data))
                    ;
            else if (data is SubscribeToObjectsRequest)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.SubscribeToObjectsRequest, data));
            else if (data is SuicideRequest)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.SuicideRequest, data));
            else if (data is TextChatMessage)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.TextChatMessage, data));
            else if (data is ToggleGodModeMessage)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.ToggleGodModeMessage, data))
                    ;
            else if (data is TransferResourceMessage)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.TransferResourceMessage,
                    data));
            else if (data is TurretShootingMessage)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.TurretShootingMessage, data
                ));
            else if (data is UnsubscribeFromObjectsRequest)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.UnsubscribeFromObjectsRequest, data));
            else if (data is VoiceCommDataMessage)
                ExecuteEvent(new GenericEvent(Managers.Event.EventID.VoiceCommDataMessage, data))
                    ;
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
