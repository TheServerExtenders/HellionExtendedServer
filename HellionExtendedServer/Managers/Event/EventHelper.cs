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
    };

    public class EventHelper
    {

        protected List<EventListener> RegiteredEvents = new List<EventListener>();

        public EventHelper()
        {
            //TODO Register Each Event here
            //Active
            NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(PlayerSpawnRequest), HandelPlayerSpawnEvent);
            NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(CharacterDataResponse), HandelPlayerTransferEvent);

            //TODO
            //NetworkController.Instance.NetContoller.EventSystem.AddListener(EventSystem.InternalEventType.GetPlayer, new EventSystem.InternalEventsDelegate(LoginPlayer));//TODO maybe NEVER!
            //NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(CharacterListResponse), new EventSystem.NetworkDataDelegate(this.RemoveDeletedCharacters));
            //NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(CheckDeletedMessage), new EventSystem.NetworkDataDelegate(this.CheckDeleted));
            //NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(PlayerSpawnRequest), new EventSystem.NetworkDataDelegate(this.PlayerSpawnRequestListener));
            //NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(PlayerRespawnRequest), new EventSystem.NetworkDataDelegate(this.PlayerRespawnRequestListener));
            //NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(SpawnObjectsRequest), new EventSystem.NetworkDataDelegate(this.SpawnObjectsRequestListener));
            //NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(SubscribeToObjectsRequest), new EventSystem.NetworkDataDelegate(this.SubscribeToSpaceObjectListener));
            //NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(UnsubscribeFromObjectsRequest), new EventSystem.NetworkDataDelegate(this.UnsubscribeFromSpaceObjectListener));
            //NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(TextChatMessage), new EventSystem.NetworkDataDelegate(this.TextChatMessageListener));
            //NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(TransferResourceMessage), new EventSystem.NetworkDataDelegate(this.TransferResourcesMessageListener));
            //NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(RefineResourceMessage), new EventSystem.NetworkDataDelegate(this.RefineResourceMessageListener));
            //NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(CheckInResponse), new EventSystem.NetworkDataDelegate(this.CheckInResponseListener));
            //NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(PlayersOnServerRequest), new EventSystem.NetworkDataDelegate(this.PlayersOnServerRequestListener));
            // NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(LatencyTestMessage), new EventSystem.NetworkDataDelegate(this.LatencyTestListener));
            // NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(SaveGameMessage), new EventSystem.NetworkDataDelegate(this.SaveGameMessageListener));


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
                ExecuteEvent(new HESPlayerTransferEvent(Response,Message,CharacterData,CharacterName,SteamId,CharacterId));
            }
            catch (Exception ex)
            {
                Log.Instance.Error("Hellion Extended Server [SPAWN REQUEST EVENT ERROR] : " + ex.InnerException.ToString());
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
                Log.Instance.Error("Hellion Extended Server [SPAWN REQUEST EVENT ERROR] : " + ex.InnerException.ToString());
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
