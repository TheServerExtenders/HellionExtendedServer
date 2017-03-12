using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HellionExtendedServer.Common;
using HellionExtendedServer.Managers.Event.Player;
using ZeroGravity;
using ZeroGravity.Data;
using ZeroGravity.Network;
using NetworkController = HellionExtendedServer.Controllers.NetworkController;

namespace HellionExtendedServer.Managers.Event
{
    public enum EventID
    {
        None = 0,
        SpawnEvent = 1,
        DeathEvent = 2,
        RemoveDeletedCharcters = 3,
        HESRespawnEvent = 4,
        HESSpawnObjects = 4,
        HESSubscribeToObject = 4,
        HESUnsubscribeFromObject = 4,
        HESTextChatMessage = 4,
    };

    public class EventHelper
    {
        protected List<EventListener> RegiteredEvents = new List<EventListener>();

        public EventHelper()
        {
            //TODO Register Each Event here
            //Active
            NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(PlayerSpawnRequest),
                HandelPlayerSpawnEvent);
            NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(CharacterDataResponse),
                HandelPlayerTransferEvent);
            NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(CharacterListResponse),
                RemoveDeletedCharacters);
            NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(CheckDeletedMessage),
                CheckDeleted);
            NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(PlayerRespawnRequest),
                PlayerRespawnRequestListener);
            NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(SpawnObjectsRequest),
                SpawnObjectsRequestListener);
            NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(SubscribeToObjectsRequest),
                SubscribeObjectsRequestListener);
            NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(UnsubscribeFromObjectsRequest),
                UnsubscribeFromSpaceObjectListener);
            NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(TextChatMessage),
                TextChatMessageListener);
            //NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(TransferResourceMessage),TransferResourcesMessageListener);
            //NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(RefineResourceMessage),(this.RefineResourceMessageListener));
            //NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(CheckInResponse),(this.CheckInResponseListener));
            //NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(PlayersOnServerRequest),(this.PlayersOnServerRequestListener));
            // NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(LatencyTestMessage),(this.LatencyTestListener));
            // NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(SaveGameMessage),(this.SaveGameMessageListener));
            //NetworkController.Instance.NetContoller.EventSystem.AddListener(EventSystem.InternalEventType.GetPlayer, new EventSystem.InternalEventsDelegate(LoginPlayer));//TODO maybe NEVER!
        }

        public void TextChatMessageListener(NetworkData data)
        {
            try
            {
                TextChatMessage TextChatMessage = data as TextChatMessage;

                long GUID = TextChatMessage.GUID;
                bool Local = TextChatMessage.Local;
                string Name = TextChatMessage.Name;
                string MessageText = TextChatMessage.MessageText;

                ExecuteEvent(new HESTextChatMessage(GUID,Local,Name,MessageText));
            }
            catch (Exception ex)
            {
                Log.Instance.Error("Hellion Extended Server [SpawnObjectsRequestListener ERROR] : " +
                                   ex.InnerException.ToString());
            }
        }
        public void UnsubscribeFromSpaceObjectListener(NetworkData data)
        {
            try
            {
                UnsubscribeFromObjectsRequest UnsubscribeFromObjectsRequest = data as UnsubscribeFromObjectsRequest;

                List<long> GUIDs = UnsubscribeFromObjectsRequest.GUIDs;

                ExecuteEvent(new HESUnsubscribeFromObject(GUIDs));
            }
            catch (Exception ex)
            {
                Log.Instance.Error("Hellion Extended Server [SpawnObjectsRequestListener ERROR] : " +
                                   ex.InnerException.ToString());
            }
        }
        public void SubscribeObjectsRequestListener(NetworkData data)
        {
            try
            {
                SubscribeToObjectsRequest SubscribeToObjectsRequest = data as SubscribeToObjectsRequest;

                //TODO
                List<long> GUIDs = SubscribeToObjectsRequest.GUIDs;

                ExecuteEvent(new HESSubscribeToObject(GUIDs));
            }
            catch (Exception ex)
            {
                Log.Instance.Error("Hellion Extended Server [SpawnObjectsRequestListener ERROR] : " +
                                   ex.InnerException.ToString());
            }
        }
        public void SpawnObjectsRequestListener(NetworkData data)
        {
            try
            {
                SpawnObjectsRequest SpawnObjectsRequest = data as SpawnObjectsRequest;

                List<long> GUIDs = SpawnObjectsRequest.GUIDs;

                ExecuteEvent(new HESSpawnObjects(GUIDs));
            }
            catch (Exception ex)
            {
                Log.Instance.Error("Hellion Extended Server [SpawnObjectsRequestListener ERROR] : " +
                                   ex.InnerException.ToString());
            }
        }

        public void PlayerRespawnRequestListener(NetworkData data)
        {
            try
            {
                PlayerRespawnRequest PlayerRespawnRequest = data as PlayerRespawnRequest;

                long GUID = PlayerRespawnRequest.GUID;

                ExecuteEvent(new HESRespawnEvent(GUID));
            }
            catch (Exception ex)
            {
                Log.Instance.Error("Hellion Extended Server [CheckDeleted ERROR] : " +
                                   ex.InnerException.ToString());
            }
        }
        public void CheckDeleted(NetworkData data)
        {
            try
            {
                //Unsued Event
            }
            catch (Exception ex)
            {
                Log.Instance.Error("Hellion Extended Server [CheckDeleted ERROR] : " +
                                   ex.InnerException.ToString());
            }
        }
        public void RemoveDeletedCharacters(NetworkData data)
        {
            try
            {
                CharacterListResponse CharacterListResponse = data as CharacterListResponse;


                ResponseResult Response = CharacterListResponse.Response;
                string Message = CharacterListResponse.Message;
                Dictionary<long, CharacterData> Characters = CharacterListResponse.Characters;


                ExecuteEvent(new HESRemoveDeletedCharcters(Response, Message, Characters));
            }
            catch (Exception ex)
            {
                Log.Instance.Error("Hellion Extended Server [RemoveDeletedCharacters ERROR] : " +
                                   ex.InnerException.ToString());
            }
        }

        public void HandelPlayerTransferEvent(NetworkData data)
        {
            try
            {
                CharacterDataResponse CharacterDataResponse = data as CharacterDataResponse;


                ResponseResult Response = CharacterDataResponse.Response;
                string Message = CharacterDataResponse.Message;
                string CharacterData = CharacterDataResponse.CharacterData;
                string CharacterName = CharacterDataResponse.CharacterName;
                string SteamId = CharacterDataResponse.SteamId;
                long CharacterId = CharacterDataResponse.CharacterId;
                ExecuteEvent(new HESPlayerTransferEvent(Response, Message, CharacterData, CharacterName, SteamId,
                    CharacterId));
            }
            catch (Exception ex)
            {
                Log.Instance.Error("Hellion Extended Server [HandelPlayerTransferEvent ERROR] : " +
                                   ex.InnerException.ToString());
            }
        }

        public void HandelPlayerSpawnEvent(NetworkData data)
        {
            try
            {
                PlayerSpawnRequest playerSpawnRequest = data as PlayerSpawnRequest;


                SpawnPointLocationType SpawnType = playerSpawnRequest.SpawnType;
                long SpawPointParentID = playerSpawnRequest.SpawPointParentID;
                GameScenes.SceneID ShipItemID = playerSpawnRequest.ShipItemID;
                ExecuteEvent(new HESSpawnEvent(SpawnType, SpawPointParentID, ShipItemID));
            }
            catch (Exception ex)
            {
                Log.Instance.Error("Hellion Extended Server [HandelPlayerSpawnEvent ERROR] : " +
                                   ex.InnerException.ToString());
            }
        }
        /*
         * TODO LOADS OF WORK!
        public void HandelPlayerLoginEvent(EventSystem.InternalEventData data)
        {
            if (!this.WorldInitialized)
                this.InitializeWorld();
            long index = (long)data.Objects[0];
            if (this._players.ContainsKey(index))
                this.NetworkController.ConnectPlayer(this._players[index], true);
            else if (this.NetworkController.IsLocal)
            {
                CharacterDataResponse characterDataResponse = new CharacterDataResponse();
                characterDataResponse.Response = ResponseResult.Success;
                characterDataResponse.CharacterId = index;
                string str1 = "Player_";
                int localPlayerCounter = this.localPlayerCounter;
                this.localPlayerCounter = localPlayerCounter + 1;
                // ISSUE: variable of a boxed type
                __Boxed<int> local = (ValueType)localPlayerCounter;
                string str2 = str1 + (object)local;
                characterDataResponse.CharacterName = str2;
                this.PlayerTransfer((NetworkData)characterDataResponse);
            }
            else
                this.NetworkController.GetPlayerData(index);
        }
        */

        public void RegisterEvent(EventListener e)
        {
            RegiteredEvents.Add(e);
            //TODO notify of Regerstration
        }

        public void ExecuteEvent(Event e)
        {
            foreach (EventListener evnt in RegiteredEvents)
            {
                if (e.GetEventType == evnt.GetEventType)
                {
                    evnt.Execute(e);
                }
            }
        }
    }
}