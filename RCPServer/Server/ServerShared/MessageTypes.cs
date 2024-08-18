using System;
using System.Collections.Generic;
using System.Text;

namespace RCPServer
{
    public class MessageTypes
    {
        public  const int P_CreateAccount = 1;
        public  const int P_VerifyAccount = 2;
        public  const int P_FetchCharacter = 3;
        public  const int P_CreateCharacter = 4;
        public  const int P_DeleteCharacter = 5;
        public  const int P_ChangePassword = 6;
        public  const int P_FetchActors = 7;
        public  const int P_FetchItems = 8;
        public  const int P_ChangeArea = 9;
        public  const int P_FetchUpdateFiles = 10;
        public  const int P_NewActor = 11;
        public  const int P_StartGame = 12;
        public  const int P_ActorGone = 13;
        public  const int P_StandardUpdate = 14;
        public  const int P_InventoryUpdate = 15;
        public  const int P_ChatMessage = 16;
        public  const int P_WeatherChange = 17;
        public  const int P_AttackActor = 18;
        public  const int P_ActorDead = 19;
        public  const int P_RightClick = 20;
        public  const int P_Dialog = 21;
        public  const int P_StatUpdate = 22;
        public  const int P_QuestLog = 23;
        public  const int P_GoldChange = 24;
        public  const int P_NameChange = 25;
        public  const int P_KnownSpellUpdate = 26;
        public  const int P_SpellUpdate = 27;
        public  const int P_CreateEmitter = 28;
        public  const int P_Sound = 29;
        public  const int P_AnimateActor = 30;
        public  const int P_ActionBarUpdate = 31;
        public  const int P_XPUpdate = 32;
        public  const int P_ScreenFlash = 33;
        public  const int P_Music = 34;
        public  const int P_SceneryInteract = 35;
        public  const int P_ActorEffect = 36;
        public  const int P_Projectile = 37;
        public  const int P_PartyUpdate = 38;
        public  const int P_AppearanceUpdate = 39;
        public  const int P_ShaderConstant = 40;
        //public  const int P_UpdateTrading = 41;
        public  const int P_SelectScenery = 42;
        public  const int P_ItemScript = 43;
        public  const int P_EatItem = 44;
        public  const int P_ItemHealth = 45;
        public  const int P_Jump = 46;
        public  const int P_Dismount = 47;
        public  const int P_FloatingNumber = 48;
        public  const int P_RepositionActor = 49;
        public  const int P_Speech = 50;
        public  const int P_ProgressBar = 51;
        public  const int P_BubbleMessage = 52;
        public  const int P_ScriptInput = 53;
        public  const int P_KickedPlayer = 60;

        public const int P_ServerConnected = 61; // Server connection to master server
        public const int P_ConnectInit = 62;
        public const int P_ServerStat = 63;
        public const int P_ServerList = 64;
        public const int P_ChangeAreaRequest = 65;
        public const int P_ReAllocateIDs = 66;
        public const int P_ClientDrop = 67;
        public const int P_ClientInfo = 68;
        public const int P_Instance = 69;
        public const int P_RemoveInstance = 70;

        // GUI
        public const int P_OpenForm = 71;
        public const int P_PropertiesUpdate = 72;
        public const int P_GUIEvent = 73;
        public const int P_CloseForm = 74;
        
        // Proxy msg from PS->ZA (2.50 MS3)
        public const int P_ProxyConnected = 75;

        // Actor Info Request
        public const int P_ActorInfoRequest = 76;
        public const int P_ActorInfoCommand = 77;

        // For chat tabs
        public const int P_ChatTab = 78;

        public const int P_DebugMessage = 79;

        public const int P_Ping = 80;
    }
}
