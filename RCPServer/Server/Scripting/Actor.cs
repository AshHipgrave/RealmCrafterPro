using System;
using System.Collections.Generic;
using System.Text;
using Scripting.Math;

namespace Scripting
{
    /// <summary>
    /// Respresentation of an actor instance.
    /// </summary>
    public class Actor
    {
        /// <summary>
        /// Very important internal variable. It calls the mirrored commands in the main assembly which do not exist in the scripting library.
        /// </summary>
        protected static Internals.IScriptingCoreCommands Commands;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        public static void SetCommands(Internals.IScriptingCoreCommands c)
        {
            Commands = c;
        }

        #region Custom GUI
        /// <summary>
        /// Open the given dialog on the client machine. See documentation for more details.
        /// </summary>
        /// <param name="control">Handle of the form or control to open</param>
        public virtual void CreateDialog(Forms.Control control) { }

        /// <summary>
        /// Close the given dialog on the client and end local script.
        /// </summary>
        /// <param name="control">Handle of the form or control to destroy</param>
        public virtual void CloseDialog(Forms.Control control) { }

        /// <summary>
        /// Find a control with the given Name property.
        /// 
        /// NOTE: This only searches for the 'top level' master control which would have been passed to CreateDialogs, it cannot
        /// search the control hierarchy.
        /// </summary>
        /// <param name="name">Name value to search for, case sensitive, of the Forms.Control.Name property.</param>
        /// <returns>Control handle or null.</returns>
        public virtual Forms.Control FindControl(string name) { return null; }

        /// <summary>
        /// Post a 'property' packet to the client. This is ONLY to be used with custom dialog implementations.
        /// 
        /// Note: Improper use will result in unexpected behavior for players and may also cause crashes.
        /// </summary>
        /// <param name="pa">Packet to transmit</param>
        public virtual void PostPropertyPacket(PacketWriter pa) { }
        #endregion

        #region Ability Commands
        /// <summary>Adds an ability to the actors' spell list.</summary>
        /// <remarks>
        /// Adds an ability to the actors spell list based upon the name issued in the game editor. If 
        /// spell memorization is required, the player will need to remember the spell before being
        /// able to use it.
        /// </remarks>
        /// <param name="name">Name of the spell as decided in the game editor (Not case sensitive).</param>
        public virtual void AddAbility(string name) { }

        /// <summary>Adds an ability to the actors' spell list.</summary>
        /// <remarks>
        /// Adds an ability to the actors spell list based upon the name issued in the game editor. If 
        /// spell memorization is required, the player will need to remember the spell before being
        /// able to use it.
        /// </remarks>
        /// <param name="name">Name of the spell as decided in the game editor (Not case sensitive).</param>
        /// <param name="level">Initial level of the ability.</param>
        public virtual void AddAbility(string name, int level) { }

        /// <summary>
        /// Remove an ability from the actors' spell book.
        /// </summary>
        /// <param name="name">Name of the ability to find.</param>
        public virtual void RemoveAbility(string name) { }

        /// <summary>
        /// Get the level of a spell.
        /// </summary>
        /// <param name="name">Name to find.</param>
        /// <returns>New level to get.</returns>
        public virtual int GetAbilityLevel(string name) { return 0; }

        /// <summary>
        /// Set the level of a spell.
        /// </summary>
        /// <param name="name">Spell to change.</param>
        /// <param name="level">New level to set.</param>
        public virtual void SetAbilityLevel(string name, int level) { }

        /// <summary>
        /// Check to see if a particular ability is known.
        /// </summary>
        /// <param name="name">Name of the ability to find.</param>
        /// <returns>True if the ability has been added to the player.</returns>
        public virtual bool AbilityKnown(string name) { return false; }

        /// <summary>
        /// Check to see if a particular ability has been memorized.
        /// </summary>
        /// <param name="name">Name of the ability to find.</param>
        /// <returns>True if the ability is memorized.</returns>
        public virtual bool AbilityMemorized(string name) { return false; }
        #endregion

        /// <summary>
        /// Get a shader constant.
        /// </summary>
        /// <param name="name">Constant to find.</param>
        /// <returns></returns>
        public virtual Scripting.Math.ShaderParameter GetShaderParameter(string name) { return new Scripting.Math.ShaderParameter(0); }

        /// <summary>
        /// Set a shader constant.
        /// </summary>
        /// <param name="name">Name of constant to set.</param>
        /// <param name="parameter">Parameter data to set.</param>
        public virtual void SetShaderParameter(string name, Scripting.Math.ShaderParameter parameter) { }

        /// <summary>
        /// Send an environment update to the client.
        /// </summary>
        /// <param name="packetData">Data to send.</param>
        public virtual void SendEnvironmentUpdate(byte[] packetData) { }

        /// <summary>
        /// Get the actors' aggressiveness.
        /// </summary>
        public virtual int Aggressiveness { get { return 0; } }

        /// <summary>
        /// Get or set the actors' AI state.
        /// </summary>
        public virtual AIModes AIState { get { return AIModes.AI_Wait; } set { } }

        /// <summary>
        /// Get or set the actors beard mesh.
        /// </summary>
        public virtual int Beard { get { return 0; } set { } }

        /// <summary>
        /// Get or set the actors body texture.
        /// </summary>
        public virtual int Clothes { get { return 0; } set { } }

        /// <summary>
        /// Get or set the actors face texture.
        /// </summary>
        public virtual int Face { get { return 0; } set { } }

        /// <summary>
        /// Get or set the actors gender (1 = Male, 2 = Female, 3 (returned) = Has no gender).
        /// </summary>
        public virtual int Gender { get { return 0; } set { } }

        /// <summary>
        /// Get the dictionary of script global data for this actor.
        /// </summary>
        /// <remarks>
        /// Script Globals are data sets which are specific to a given actor.
        /// 
        /// The dictionary value allows globals to be accessed directly by a string key instead
        /// of an index, which keeps script access consistant and tidy.
        /// 
        /// Value data is a byte array, this is to avoid complex string processing with numeric data
        /// and to allow stream commands to be used with it. For example, read access can use a
        /// PacketReader and write access can use a PacketWriter.
        /// </remarks>
        /// <example>
        /// public void SetSomeGlobals(Actor player)
        /// {
        ///     string TempName = "John";
        ///     int TempAge = 25;
        /// 
        ///     PacketWriter writer = new PacketWriter();
        ///     writer.Write(TempName, true);
        ///     writer.Write(TempAge);
        /// 
        ///     player.Globals["UserInfo"] = writer.ToArray();
        /// }
        /// 
        /// public void GetSomeGlobals(Actor player)
        /// {
        ///     PacketReader reader = new PacketReader(player.Globals["UserInfo"]);
        /// 
        ///     string TempName = reader.ReadString();
        ///     int TempAge = reader.ReadInt32();
        /// 
        ///     player.Output(string.Format("Hello {0} you are {1} years old!", new object[] { TempName, TempAge }));
        /// }
        /// </example>
        public virtual System.Collections.Generic.Dictionary<string, byte[]> Globals { get { return null; } }

        /// <summary>
        /// Get or set the actors hair mesh.
        /// </summary>
        public virtual int Hair { get { return 0; } set { } }

        /// <summary>
        /// Get or set the actor level.
        /// </summary>
        public virtual int Level { get { return 0; } set { } }

        /// <summary>
        /// Get or set the actor which is being targeted.
        /// </summary>
        public virtual Actor Target { get { return null; } set { } }

        /// <summary>
        /// Get or set the destination of this actor.
        /// </summary>
        /// <remarks>
        /// Use Destination if you want the actor to 'walk' to the new location.
        /// </remarks>
        public virtual SectorVector Destination { get { return SectorVector.Zero; } }

        /// <summary>
        /// Get or set the actors leader if this actor is a pet.
        /// </summary>
        public virtual Actor Leader { get { return null; } set { } }

        /// <summary>
        /// Get or set the actors reputation.
        /// </summary>
        public virtual int Reputation { get { return 0; } set { } }

        /// <summary>
        /// Get or set the actors name.
        /// </summary>
        public virtual string Name { get { return ""; } set { } }

        /// <summary>
        /// Get or set the tag (Guild text) of this actor.
        /// </summary>
        public virtual string Tag { get { return ""; } set { } }

        /// <summary>
        /// Get or set the group that this actor is in.
        /// </summary>
        public virtual int ActorGroup { get { return 0; } set { } }

        /// <summary>
        /// Get or set the amount of money this actor has.
        /// </summary>
        public virtual int Money { get { return 0; } set { } }

        /// <summary>
        /// Get or set the XP for this actor.
        /// </summary>
        public virtual int XP { get { return 0; } set { } }

        /// <summary>
        /// Get the XP Multiplier for this actor.
        /// </summary>
        public virtual int XPMultiplier { get { return 0; } }

        /// <summary>
        /// Get the ZoneInstance (zone information) about this actors current zone.
        /// </summary>
        public virtual ZoneInstance Zone { get { return null; } }

        /// <summary>
        /// Get the actor that this actor is riding.
        /// </summary>
        public virtual Actor Mount { get { return null; } }

        /// <summary>
        /// Get the person riding this actor if its a mount.
        /// </summary>
        public virtual Actor Rider { get { return null; } }

        /// <summary>
        /// Check if the actor is outside.
        /// </summary>
        public virtual bool Outdoors { get { return false; } }

        /// <summary>
        /// Check if the actor is underwater.
        /// </summary>
        public virtual bool UnderWater { get { return false; } }

        /// <summary>
        /// Get the ID of the actor template.
        /// </summary>
        public virtual uint BaseActorID { get { return 0; } }

        /// <summary>
        /// Check if this actor is a human player or NPC.
        /// </summary>
        public virtual bool Human { get { return false; } }

        /// <summary>
        /// Get the class of this actor.
        /// </summary>
        public virtual string Class { get { return ""; } }

        /// <summary>
        /// Get the race of this actor.
        /// </summary>
        public virtual string Race { get { return ""; } }

        /// <summary>
        /// Get or set the faction of this actor.
        /// </summary>
        public virtual string HomeFaction { get { return ""; } set { } }

        /// <summary>
        /// Get the email address of this actors owner.
        /// </summary>
        public virtual string AccountEmail { get { return ""; } }

        /// <summary>
        /// Get the account name of this actor.
        /// </summary>
        public virtual string AccountName { get { return ""; } }

        /// <summary>
        /// Check if the actor is in the game.
        /// </summary>
        public virtual bool InGame { get { return false; } }

        /// <summary>
        /// Get or set the banned status of this actor.
        /// </summary>
        public virtual bool Banned { get { return false; } set { } }

        /// <summary>
        /// Get or set the GM status of this actor.
        /// </summary>
        public virtual bool GM { get { return false; } set { } }

        /// <summary>
        /// Set the value of an attribute.
        /// </summary>
        /// <param name="name">Attribute name to set.</param>
        /// <param name="value">New value.</param>
        public virtual void SetAttribute(string name, int value) { }

        /// <summary>
        /// Get the value of the given attribute.
        /// </summary>
        /// <param name="name">Attribute name to find.</param>
        /// <returns>Attribute value.</returns>
        public virtual int GetAttribute(string name) { return 0; }

        /// <summary>
        /// Set the maximum value of an attribute.
        /// </summary>
        /// <param name="name">Attribute name to set.</param>
        /// <param name="value">New value.</param>
        public virtual void SetAttributeMax(string name, int value) { }

        /// <summary>
        /// Get the maximum value of the given attribute.
        /// </summary>
        /// <param name="name">Attribute name to find.</param>
        /// <returns>Attribute maximum value.</returns>
        public virtual int GetAttributeMax(string name) { return 0; }

        /// <summary>
        /// Set the resistance level of a damage type.
        /// </summary>
        /// <param name="name">Damage type to set.</param>
        /// <param name="value">Resistance level.</param>
        public virtual void SetResistance(string name, int value) { }

        /// <summary>
        /// Get the resistance level of a damage type for this actor.
        /// </summary>
        /// <param name="name">Damage type to check.</param>
        /// <returns>Resistance level.</returns>
        public virtual int GetResistance(string name) { return 0; }

        /// <summary>
        /// Gets the resistance level of damage type for this actor. 
        /// </summary>
        /// <param name="name">Index of damage type to check.</param>
        /// <returns></returns>
        public virtual int GetResistance(int index) { return 0; }


        /// <summary>
        /// Get the position of this actor.
        /// </summary>
        /// <returns>Actor position.</returns>
        public virtual global::Scripting.Math.SectorVector GetPosition() { return global::Scripting.Math.SectorVector.Zero; }

        /// <summary>
        /// Set the absolute position of the actor.
        /// </summary>
        /// <remarks>
        /// Generally an actor is moved by setting their destination and having it walk there. If you are 'warping' the actor to a different
        /// location in the zone, you will need to reposition him instantly.
        /// </remarks>
        /// <param name="position">Location to move to.</param>
        public virtual void SetPosition(global::Scripting.Math.SectorVector position) { return; }

        /// <summary>
        /// Set the absolute position of the actor.
        /// </summary>
        /// <remarks>
        /// Generally an actor is moved by setting their destination and having it walk there. If you are 'warping' the actor to a different
        /// location in the zone, you will need to reposition him instantly.
        /// </remarks>
        /// <param name="position">Location to move to.</param>
        /// <param name="moveCamera">If true, the camera will move with the actor. If false, the camera will fly to the actor position.</param>
        public virtual void SetPosition(global::Scripting.Math.SectorVector position, bool moveCamera) { return; }

        /// <summary>
        /// Set the absolute position of the actor.
        /// </summary>
        /// <remarks>
        /// Generally an actor is moved by setting their destination and having it walk there. If you are 'warping' the actor to a different
        /// location in the zone, you will need to reposition him instantly.
        /// </remarks>
        /// <param name="position">Location to move to.</param>
        /// <param name="moveCamera">If true, the camera will move with the actor. If false, the camera will fly to the actor position.</param>
        /// <param name="useCollision">If true, the actor will collide with any object that are in the way. If false, the actor will move regardless
        /// of other scene objects.</param>
        public virtual void SetPosition(global::Scripting.Math.SectorVector position, bool moveCamera, bool useCollision) { return; }

        /// <summary>
        /// Get the number of party members in the actors party.
        /// </summary>
        /// <returns>Number of party members.</returns>
        public virtual int PartyMembersCount() { return 0; }

        /// <summary>
        /// Get an actor instance from the party based upon his index.
        /// </summary>
        /// <param name="index">Index of the actor to get (0 - PartyMembersCount() - 1)</param>
        /// <returns>Actor instance if valid. Null if invalid.</returns>
        public virtual Actor PartyMember(int index) { return null; }

        /// <summary>
        /// Give or take money from the actor.
        /// </summary>
        /// <param name="amount">Amount of money to give or take.</param>
        public virtual void ChangeMoney(int amount) { }

        /// <summary>
        /// Get an item from the actors' backpack.
        /// </summary>
        /// <remarks>
        /// You can obtain the item information of an item at a slot in the actors inventory. This function can be used to check if
        /// there is any item, or can be used to read specific item information.
        /// </remarks>
        /// <param name="slot">Inventory slot to look up.</param>
        /// <returns>An ItemInstance if an item exists or null if one doesn't.</returns>
        public virtual ItemInstance Backpack(ItemSlot slot) { return null; }

        /// <summary>
        /// Get the number of pets the actor has.
        /// </summary>
        /// <returns>Number of pets.</returns>
        public virtual int CountPets() { return 0; }

        /// <summary>
        /// Check that an actor has an effect (Buff) applied.
        /// </summary>
        /// <param name="effect">Name of the effect to find.</param>
        /// <returns>True if the effect is present.</returns>
        public virtual bool HasEffect(string effect) { return false; }

        /// <summary>
        /// Request help from all friendly actors nearby.
        /// </summary>
        public virtual void CallForHelp() { }

        /// <summary>
        /// Finds the distance between two actors.
        /// </summary>
        /// <remarks>
        /// This command finds the distance in units between two actors. You should try to avoid this function in favor of
        /// DistanceFromSq.
        /// </remarks>
        /// <param name="other">Actor to find the distance from.</param>
        /// <returns>Distance from 'other'.</returns>
        public virtual float DistanceFrom(Actor other) { return 0.0f; }

        /// <summary>
        /// Find the squared distance between two actors.
        /// </summary>
        /// <remarks>
        /// The squared distance is: XDist^2 + ZDist^2 without the square root being found.<br />
        /// <br />
        /// Calculating the root of a number is quite slow in comparison to other commands, so avoid doing so wherever possible.
        /// </remarks>
        /// <param name="other">Actor to find the distance from.</param>
        /// <returns>Squared distance from 'other'.</returns>
        public virtual float DistanceFromSq(Actor other) { return 0.0f; }

        /// <summary>
        /// Add an actor effect (Buff).
        /// </summary>
        /// <remarks>
        /// Actor Effects are attributes changes which can be caused by spells or items. Effects can include extra health or a stronger attack.<br />
        /// <br />
        /// When an effect is applied, an icon will appear on the clients' HUD to notify them of the effects presence.
        /// </remarks>
        /// <param name="effectName">Name of the effect to apply.</param>
        /// <param name="attributeName">Name of the attribute to modify.</param>
        /// <param name="attributeValue">Change in ths attributes' value.</param>
        /// <param name="timer">Length of time the effect will work for.</param>
        /// <param name="iconID">MediaID of the icon to display on the clients screen.</param>
        public virtual void AddEffect(string effectName, string attributeName, int attributeValue, int timer, int iconID) { }

        /// <summary>
        /// Remove an actor effect (Buff).
        /// </summary>
        /// <param name="effectName">Name of the effect to remove.</param>
        public virtual void RemoveEffect(string effectName) { }

        /// <summary>
        /// Player an animation for this actor.
        /// </summary>
        /// <param name="animation">Animation Name from the Animations Tab in the Game Editor.</param>
        /// <param name="speed">Speed to play the animation at (1.0 is default).</param>
        /// <param name="fixedSpeed">##TODO:What is this?</param>
        public virtual void Animate(string animation, float speed, bool fixedSpeed) { }

        /// <summary>
        /// Change the BaseID of this actor in order to change its appearance.
        /// </summary>
        /// <remarks>
        /// If the BaseID of this actor is changed, it will change its appearance into the new ID. This is particularly useful
        /// if you want to morph a player into another form for spells.
        /// </remarks>
        /// <param name="newID">ActorID as found in the Actors Tab in the Game Editor.</param>
        public virtual void Change(uint newID) { }

        /// <summary>
        /// Fire a projectile at the targeted actor.
        /// </summary>
        /// <remarks>
        /// Projectiles are defined in the Projectiles Tab in the Game Editor. FireProjectile spawns the instance and will
        /// animate its movement towards the target actor.
        /// </remarks>
        /// <param name="target">Target that the projectile will hit.</param>
        /// <param name="projectileName">Name of the projectile to fire.</param>
        public virtual void FireProjectile(Actor target, string projectileName) { }
        
        /// <summary>
        /// Give XP to the actor after killing another actor.
        /// </summary>
        /// <param name="target">Actor that was killed.</param>
        public virtual void GiveKillXP(Actor target) { }

        /// <summary>
        /// Give XP to the actor.
        /// </summary>
        /// <param name="amount">Amount to increase/decrease the XP.</param>
        public virtual void GiveXP(int amount) { }

        /// <summary>
        /// Give XP to the actor.
        /// </summary>
        /// <param name="amount">Amount to increase/decrease the XP.</param>
        /// <param name="share">True if the XP should be shared amongth party members.</param>
        public virtual void GiveXP(int amount, bool share) { }

        /// <summary>
        /// Kill this actor.
        /// </summary>
        public virtual void Kill() { }

        /// <summary>
        /// Kill this actor.
        /// </summary>
        /// <param name="killer">Instance of the actor which killed this actor.</param>
        public virtual void Kill(Actor killer) { }

        /// <summary>
        /// Rotate the actor to face a new direction.
        /// </summary>
        /// <param name="yaw">Rotation in degrees.</param>
        public virtual void Rotate(float yaw) { }

        /// <summary>
        /// Modify the players XP Bar.
        /// </summary>
        /// <param name="amount">Amount to increase/decrease the display.</param>
        public virtual void UpdateXPBar(int amount) { }

        /// <summary>
        /// Move the actor to a new zone.
        /// </summary>
        /// <remarks>
        /// Warping will cause a client to load a new zone if the zoneName argument is different to the zone they are
        /// currently in.
        /// </remarks>
        /// <param name="zoneName">Name of the zone to move to.</param>
        /// <param name="portalName">Name of the portal to spawn the actor at.</param>
        /// <returns>True on success.</returns>
        public virtual bool Warp(string zoneName, string portalName) { return false; }

        /// <summary>
        /// Move the actor to a new zone.
        /// </summary>
        /// <remarks>
        /// Warping will cause a client to load a new zone if the zoneName argument is different to the zone they are
        /// currently in.
        /// </remarks>
        /// <param name="zoneName">Name of the zone to move to.</param>
        /// <param name="portalName">Name of the portal to spawn the actor at.</param>
        /// <param name="instanceNumber">The instance to place the actor in.</param>
        /// <returns>True on success.</returns>
        public virtual bool Warp(string zoneName, string portalName, int instanceNumber) { return false; }

        /// <summary>
        /// Emit a sound from this actor.
        /// </summary>
        /// <remarks>
        /// To play a sound effect that is not part of the actors' speech list you can use PlaySound.
        /// </remarks>
        /// <param name="soundID">MediaID of sound to play.</param>
        public virtual void PlaySound(int soundID) { }

        /// <summary>
        /// Emit a sound from this actor.
        /// </summary>
        /// <remarks>
        /// To play a sound effect that is not part of the actors' speech list you can use PlaySound.
        /// </remarks>
        /// <param name="soundID">MediaID of sound to play.</param>
        /// <param name="global">If True, then play sound to all local actors. If False, then play the sound only to this actor.</param>
        public virtual void PlaySound(int soundID, bool global) { }

        /// <summary>
        /// Play a pre-defined speed sound effect.
        /// </summary>
        /// <remarks>
        /// Speech is pre-defined in the Actors Tab in the Game Editor. Use PlaySpeech to output any of these effects.
        /// </remarks>
        /// <param name="type">Type of speech effect to play.</param>
        public virtual void PlaySpeech(SpeechType type) { }

        /// <summary>
        /// Write a message in a bubble above the actors head.
        /// </summary>
        /// <param name="message">Message to write.</param>
        public virtual void BubbleOutput(string message) { }

        /// <summary>
        /// Write a message in a bubble above the actors head.
        /// </summary>
        /// <param name="message">Message to write.</param>
        /// <param name="color">Color of the message.</param>
        public virtual void BubbleOutput(string message, System.Drawing.Color color) { }

        /// <summary>
        /// Write a message in the chat area.
        /// </summary>
        /// <param name="message">Message to write.</param>
        public virtual void Output(string message) { }

        /// <summary>
        /// Write a message in the chat area.
        /// </summary>
        /// <param name="message">Message to write.</param>
        /// <param name="color">Color of the message.</param>
        public virtual void Output(string message, System.Drawing.Color color) { }

        /// <summary>
        /// Write a message in the chat area.
        /// </summary>
        /// <param name="tabID">ID of tab to write to.</param>
        /// <param name="message">Message to write.</param>
        public virtual void Output(byte tabID, string message) { }

        /// <summary>
        /// Write a message in the chat area.
        /// </summary>
        /// <param name="tabID">ID of tab to write to.</param>
        /// <param name="message">Message to write.</param>
        /// <param name="color">Color of the message.</param>
        public virtual void Output(byte tabID, string message, System.Drawing.Color color) { }

        /// <summary>
        /// Creates a chat tab on the client machine to receive messages.
        /// </summary>
        /// <param name="id">A unique identification number of the tab (this is NOT an index).</param>
        /// <param name="name">Name to appear as the tab text.</param>
        /// <param name="width">Width (in pixels) of the tab button.</param>
        public virtual void CreateChatTab(byte id, string name, int width) { }

        /// <summary>
        /// Remove a chat tab on the client machine, this will destroy the chat log associated with that tab.
        /// </summary>
        /// <param name="id">Unique ID of tab to destroy.</param>
        public virtual void RemoveChatTab(byte id) { }

        /// <summary>
        /// Switch to the users chat tab with the given ID.
        /// </summary>
        /// <param name="id">Unique ID of tab to switch to.</param>
        public virtual void SwitchToTab(byte id) { }

        /// <summary>
        /// Open a new dialog window on the client machine.
        /// </summary>
        /// <remarks>
        /// Dialogs are the primary input/output method for scripts. Spawning a new one will allow you to output text data and 
        /// capture multiple choice inputs.
        /// </remarks>
        /// <param name="contextActor">Other player/actor to open the window with. (TODO: Investigate the use of this variable).</param>
        /// <param name="titleText">Window Title text of the dialog.</param>
        /// <returns>New instance of an ActorDialog.</returns>
        public virtual ActorDialog OpenDialog(Actor contextActor, string titleText) { return null; }

        /// <summary>
        /// Open a new dialog window on the client machine.
        /// </summary>
        /// <remarks>
        /// Dialogs are the primary input/output method for scripts. Spawning a new one will allow you to output text data and 
        /// capture multiple choice inputs.
        /// </remarks>
        /// <param name="contextActor">Other player/actor to open the window with. (TODO: Investigate the use of this variable).</param>
        /// <param name="titleText">Window Title text of the dialog.</param>
        /// <param name="backgroundID">MediaID of the texture to appear as the window background.</param>
        /// <returns>New instance of an ActorDialog.</returns>
        public virtual ActorDialog OpenDialog(Actor contextActor, string titleText, int backgroundID) { return null; }

        /// <summary>
        /// Give or Take an item to an actor.
        /// </summary>
        /// <param name="itemName">Name of the item to find.</param>
        /// <param name="amount">Number of items to give or take.</param>
        public virtual void GiveItem(string itemName, int amount) { }

        /// <summary>
        /// Give or Take an item to an actor.
        /// </summary>
        /// <param name="itemID">ID of the item to find.</param>
        /// <param name="amount">Number of items to give or take.</param>
        public virtual void GiveItem(ushort itemID, int amount) { }

        /// <summary>
        /// Check if an actor has a particular item in his inventory.
        /// </summary>
        /// <param name="itemName">Name of the item to find.</param>
        /// <returns>Number of instances of itemName the player owns.</returns>
        public virtual int HasItem(string itemName) { return 0; }

        /// <summary>
        /// Check if an actor has a particular item in his inventory.
        /// </summary>
        /// <param name="itemID">ID of the item to find.</param>
        /// <returns>Number of instances of itemID the player owns.</returns>
        public virtual int HasItem(ushort itemID) { return 0; }

        /// <summary>
        /// Set the faction rating of this actor.
        /// </summary>
        /// <remarks>
        /// Actors use the default faction rating when they are first created. It is possible to change this if a player is to become
        /// friendly to a group of other players.
        /// </remarks>
        /// <param name="factionName">Name of the faction to check against.</param>
        /// <param name="amount">Amount to change the faction rating.</param>
        public virtual void ChangeFactionRating(string factionName, int amount) { }

        /// <summary>
        /// Get the faction rating of this actor.
        /// </summary>
        /// <remarks>
        /// Actors use the default faction rating when they are first created. It is possible to change this if a player is to become
        /// friendly to a group of other players.
        /// </remarks>
        /// <param name="factionName">Name of the faction to check against.</param>
        /// <returns>Faction Rating in -100 to 100 range.</returns>
        public virtual int GetFactionRating(string factionName) { return 0; }

        /// <summary>
        /// Directly sets a faction rating value
        /// </summary>
        /// <param name="factionName">Name of the faction to change.</param>
        /// <param name="rating">Faction Rating in -100 to 100 range.</param>
        public virtual void SetFactionRating(string factionName, int rating) { }

        /// <summary>
        /// Create a new quest.
        /// </summary>
        /// <remarks>
        /// Quests are a vital gameplay element to most projects. An assigned quest will appear in the players log
        /// until it is deleted. An additional quest status is displayed to the player so they are aware of their
        /// progress.
        /// </remarks>
        /// <param name="name">Name of the quest to create.</param>
        /// <param name="status">Default quest status.</param>
        /// <param name="statusColor">Color that the status will appear as.</param>
        public virtual void CreateQuest(string name, string status, System.Drawing.Color statusColor) { }

        /// <summary>
        /// Remove a quest from the actor.
        /// </summary>
        /// <remarks>
        /// If a quest has been cancelled for any reason, it is possible to remove it from the players log.
        /// </remarks>
        /// <param name="name">Name of the quest to remove.</param>
        public virtual void RemoveQuest(string name) { }

        /// <summary>
        /// Mark a quest as completed
        /// </summary>
        /// <param name="name">Name of quest to complete.</param>
        public virtual void CompleteQuest(string name) { }

        /// <summary>
        /// Check if a quest is completed.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="name">Name of the quest to check.</param>
        /// <returns>True of the quest has been completed. False if the quest has not been completed or does not exist.</returns>
        public virtual bool QuestComplete(string name) { return false; }
        
        /// <summary>
        /// Get the status of a quest
        /// </summary>
        /// <remarks>
        /// Get the status of a quest for updating or processing.
        /// </remarks>
        /// <param name="name">Name of the quest to find.</param>
        /// <returns></returns>
        public virtual string QuestStatus(string name) { return ""; }

        /// <summary>
        /// Modify the status of a quest.
        /// </summary>
        /// <remarks>
        /// When a quest has changed (ie. been completed or abandoned); its' status must be recommunicated with the client.
        /// </remarks>
        /// <param name="name">Name of the quest to update.</param>
        /// <param name="status">New status of the quest.</param>
        /// <param name="statusColor">New color for the status.</param>
        public virtual void UpdateQuest(string name, string status, System.Drawing.Color statusColor) { }

        /// <summary>
        /// Get tagged data associated with the given quest name.
        /// </summary>
        /// <param name="name">Name of the quest.</param>
        /// <returns>Tag data as byte array.</returns>
        public virtual byte[] GetQuestData(string name) { return null; }

        /// <summary>
        /// Set the tagged data associated with the given quest name.
        /// </summary>
        /// <param name="name">Name of quest.</param>
        /// <param name="data">Tag data to set as byte array.</param>
        public virtual void SetQuestData(string name, byte[] data) { }

        /// <summary>
        /// Creates and posts a WaitItemRequest object which will be triggered when the associated actor has the given number of items in his inventory.
        /// 
        /// The "OnWaitItem" event must be registered in order for the request to work without timing out.
        /// 
        /// NOTE: This method relies upon a script delegate (RegisterCallback) which will be closed if the player
        /// logs out or zones to another physical server. In order to use this method successfully, you must serialize
        /// the request on script shutdown or use CreateWaitItemRequest(string scriptName, string methodName, int itemID, int itemCount);
        /// </summary>
        /// <param name="itemName">Name of item type.</param>
        /// <param name="itemCount">Number of items required to trigger the event.</param>
        /// <returns></returns>
        public virtual WaitItemRequest CreateWaitItemRequest(string itemName, int itemCount) { return null; }

        /// <summary>
        /// Creates and posts a WaitItemRequest object which will be triggered when the associated actor has the given number of items in his inventory.
        /// 
        /// NOTE: When using this method you MUST NOT register the OnWaitItem event as it related only to script callbacks. This version
        /// of the method will execute a <i>new</i> script instance rather than continuing an existing one. This method is safe to use
        /// for zone clusters and will remain active offline.
        /// </summary>
        /// <param name="scriptName">Script to instance.</param>
        /// <param name="methodName">Script entrypoint to call.</param>
        /// <param name="itemName">\Name of item type.</param>
        /// <param name="itemCount">Number of items required to trigger the event.</param>
        /// <returns></returns>
        public virtual WaitItemRequest CreateWaitItemRequest(string scriptName, string methodName, string itemName, int itemCount) { return null; }

        /// <summary>
        /// Get all active WaitItemRequest objects associated with this actor.
        /// </summary>
        /// <returns></returns>
        public virtual WaitItemRequest[] GetWaitItemRequests() { return null; }


        /// <summary>
        /// Creates and posts a WaitKillRequest object which will be triggered when the associated player kills the given number of actors.
        /// 
        /// The "OnKill" event must be registered in order for the request to work without timing out.
        /// 
        /// NOTE: This method relies upon a script delegate (RegisterCallback) which will be closed if the player
        /// logs out or zones to another physical server. In order to use this method successfully, you must serialize
        /// the request on script shutdown or use CreateWaitKillRequest(string scriptName, string methodName, int actorID, int killCount);
        /// </summary>
        /// <param name="actorID">ID of actor type to be killed</param>
        /// <param name="killCount">Number of actors to kill before event is triggered</param>
        /// <returns></returns>
        public virtual WaitKillRequest CreateWaitKillRequest(uint actorID, int killCount) { return null; }

        /// <summary>
        /// Creates and posts a WaitKillRequest object which will be triggered when the associated player kills the given number of actors.
        /// 
        /// NOTE: When using this method you MUST NOT register the OnKill event as it related only to script callbacks. This version
        /// of the method will execute a <i>new</i> script instance rather than continuing an existing one. This method is safe to use
        /// for zone clusters and will remain active offline.
        /// </summary>
        /// <param name="scriptName">Script to instance.</param>
        /// <param name="methodName">Script entrypoint to call.</param>
        /// <param name="actorID">ID of actor type to be killed</param>
        /// <param name="killCount">Number of actors to kill before event is triggered</param>
        /// <returns></returns>
        public virtual WaitKillRequest CreateWaitKillRequest(string scriptName, string methodName, uint actorID, int killCount) { return null; }

        /// <summary>
        /// Get all active WaitKillRequest objects associated with this actor.
        /// </summary>
        /// <returns></returns>
        public virtual WaitKillRequest[] GetWaitKillRequests() { return null; }



        /// <summary>
        /// Creates and posts a WaitSpeakRequest object which will be triggered when the associated player interacts with the NPC.
        /// 
        /// The "OnInteract" event must be registered in order for the request to work without timing out.
        /// 
        /// NOTE: This method relies upon a script delegate (RegisterCallback) which will be closed if the player
        /// logs out or zones to another physical server. In order to use this method successfully, you must serialize
        /// the request on script shutdown or use CreateWaitSpeakRequest(string scriptName, string methodName, string zoneName, string actorName);
        /// </summary>
        /// <param name="zoneName">Zone in which the target actor resides (can be empty for any zone).</param>
        /// <param name="actorName">Name of the actor to wait for.</param>
        /// <returns></returns>
        public virtual WaitSpeakRequest CreateWaitSpeakRequest(string zoneName, string actorName) { return null; }

        /// <summary>
        /// Creates and posts a WaitSpeakRequest object which will be triggered when the associated player interacts with the NPC.
        /// 
        /// NOTE: When using this method you MUST NOT register the OnInteract event as it related only to script callbacks. This version
        /// of the method will execute a <i>new</i> script instance rather than continuing an existing one. This method is safe to use
        /// for zone clusters and will remain active offline.
        /// </summary>
        /// <param name="scriptName">Script to instance.</param>
        /// <param name="methodName">Script entrypoint to call.</param>
        /// <param name="zoneName">Zone in which the target actor resides (can be empty for any zone).</param>
        /// <param name="actorName">Name of the actor to wait for.</param>
        /// <returns></returns>
        public virtual WaitSpeakRequest CreateWaitSpeakRequest(string scriptName, string methodName, string zoneName, string actorName) { return null; }

        /// <summary>
        /// Get all active WaitSpeakRequest objects associated with this actor.
        /// </summary>
        /// <returns></returns>
        public virtual WaitSpeakRequest[] GetWaitSpeakRequests() { return null; }


        /// <summary>
        /// Creates and posts a WaitTimeRequest object which will be triggered at the given hour and minute.
        /// 
        /// The "Elapsed" event must be registered in order for the request to work without timing out.
        /// 
        /// NOTE: This method relies upon a script delegate (RegisterCallback) which will be closed if the player
        /// logs out or zones to another physical server. In order to use this method successfully, you must serialize
        /// the request on script shutdown or use CreateWaitTimeRequest(string scriptName, string methodName, int hour, int minute);
        /// </summary>
        /// <param name="hour">Hour to wait for.</param>
        /// <param name="minute">Minute to wait for.</param>
        /// <returns></returns>
        public virtual WaitTimeRequest CreateWaitTimeRequest(int hour, int minute) { return null; }

        /// <summary>
        /// Creates and posts WaitTimeRequest object which will be triggered at the given hour and minute.
        /// 
        /// NOTE: When using this method you MUST NOT register the Elapsed event as it relates only script callbacks. This version
        /// of the method will execute a <i>new</i> script instance rather than continuing an existing one.
        /// </summary>
        /// <param name="scriptName">Script to instance.</param>
        /// <param name="methodName">Script entrypoint to call.</param>
        /// <param name="hour">Hour to wait for.</param>
        /// <param name="minute">Minute to wait for.</param>
        /// <returns></returns>
        public virtual WaitTimeRequest CreateWaitTimeRequest(string scriptName, string methodName, int hour, int minute) { return null; }

        /// <summary>
        /// Get all active WaitTimeRequest objects associated with this actor.
        /// </summary>
        /// <returns></returns>
        public virtual WaitTimeRequest[] GetWaitTimeRequests() { return null; }

        /// <summary>
        /// Creates a particle emitter attached to the actor.
        /// </summary>
        /// <remarks>
        /// Emitters can be created on the fly to demonstrate the effect of a spell or other combat ability.
        /// </remarks>
        /// <param name="emitterName">Name of the emitter to create (as named in the Particles tab of the Game Editor).</param>
        /// <param name="textureID">MediaID of the texture to use. This can be obtained from the Media Tab of the Game Editor.</param>
        /// <param name="timeLength">Length of time the emitter will exist.</param>
        public virtual void CreateEmitter(string emitterName, int textureID, int timeLength) { }

        /// <summary>
        /// Creates a particle emitter attached to the actor.
        /// </summary>
        /// <remarks>
        /// Emitters can be created on the fly to demonstrate the effect of a spell or other combat ability.
        /// </remarks>
        /// <param name="emitterName">Name of the emitter to create (as named in the Particles tab of the Game Editor).</param>
        /// <param name="textureID">MediaID of the texture to use. This can be obtained from the Media Tab of the Game Editor.</param>
        /// <param name="timeLength">Length of time the emitter will exist.</param>
        /// <param name="offset">Position of the emitter relative to the actor location.</param>
        public virtual void CreateEmitter(string emitterName, int textureID, int timeLength, global::Scripting.Math.Vector3 offset) { }

        /// <summary>
        /// Displays a number floating above an actor.
        /// </summary>
        /// <remarks>
        /// Floating numbers can be used for damage display in combat and XP gains. The number will rise a few pixels from the players head
        /// so that more numbers can be used without filling the screen.
        /// </remarks>
        /// <param name="value">Integer value of the </param>
        /// <param name="color"></param>
        public virtual void CreateFloatingNumber(int value, System.Drawing.Color color) { }

        /// <summary>
        /// Flashes the player screen with an image.
        /// </summary>
        /// <remarks>
        /// ScreenFlash is used to draw a fullscreen image (or color) to simulate a lightning flash or attack strike.
        /// </remarks>
        /// <param name="color">Color of the flash. Default is Colors.White.</param>
        /// <param name="timeLength">Length of the flash in milliseconds.</param>
        /// <param name="textureID">MediaID of the texture to use. This can be obtained from the Media Manager inside the Game Editor.</param>
        public virtual void ScreenFlash(System.Drawing.Color color, int timeLength, int textureID) { }

        /// <summary>
        /// Creates an onscreen progress bar for the player.
        /// </summary>
        /// <remarks>
        /// Progressbars are useful to represent timers or wait events. The bar assumes the default skin layout used in the client but will use an
        /// alternative color based upon the input parameter.
        /// </remarks>
        /// <param name="color">Color of the progressbar. For the default color; use Colors.White.</param>
        /// <param name="x">Screen space X position (0-1) scale.</param>
        /// <param name="y">Screen space Y position (0-1) scale.</param>
        /// <param name="width">Screen space Width (0-1) scale.</param>
        /// <param name="height">Screen space Height (0-1) scale.</param>
        /// <param name="maxiumum">Maximum value the progressbar will reach. The drawn amount will be Value/Maximum.</param>
        /// <param name="value">Initial value of the progressbar. Must be greater than 0 and less than maximum.</param>
        /// <returns>Handle of the progressbar instance.</returns>
        public virtual ProgressBar CreateProgressBar(System.Drawing.Color color, float x, float y, float width, float height, int maxiumum, int value) { return null; }
        
        /// <summary>
        /// Creates an onscreen progress bar for the player.
        /// </summary>
        /// <remarks>
        /// Progressbars are useful to represent timers or wait events. The bar assumes the default skin layout used in the client but will use an
        /// alternative color based upon the input parameter.
        /// </remarks>
        /// <param name="color">Color of the progressbar. For the default color; use Colors.White.</param>
        /// <param name="x">Screen space X position (0-1) scale.</param>
        /// <param name="y">Screen space Y position (0-1) scale.</param>
        /// <param name="width">Screen space Width (0-1) scale.</param>
        /// <param name="height">Screen space Height (0-1) scale.</param>
        /// <param name="maxiumum">Maximum value the progressbar will reach. The drawn amount will be Value/Maximum.</param>
        /// <param name="value">Initial value of the progressbar. Must be greater than 0 and less than maximum.</param>
        /// <param name="label">Text to draw over the top of the progressbar.</param>
        /// <returns>Handle of the progressbar instance.</returns>
        public virtual ProgressBar CreateProgressBar(System.Drawing.Color color, float x, float y, float width, float height, int maxiumum, int value, string label) { return null; }

        /// <summary>
        /// Sets the destination SectorVector of the actor
        /// </summary>
        /// <param name="position">SectorVector of destination</param>
        public virtual void SetDestination(SectorVector position) { }

        /// <summary>
        /// Returns the actors DefaultDamageType as an integer.
        /// </summary>
        public virtual int DefaultDamageType { get { return 0; } }


        /// <summary>
        /// Gets current party actor is in. 
        /// </summary>
        public virtual PartyInstance GetCurrentParty() { return null; }

        /// <summary>
        /// Join's the actor to the given Party.
        /// </summary>
        /// <param name="party"></param>
        public virtual void JoinParty(PartyInstance party) { }
        
        /// <summary>
        /// Creates a new party and join's the actor to it.
        /// </summary>
        public virtual void CreateParty(){}

        /// <summary>
        /// Removes actor from current party.
        /// </summary>
        public virtual void  LeaveParty() { }


        /// <summary>
        /// Gets the ID of a specific actor type.
        /// </summary>
        /// <param name="raceName">Name of the actors race.</param>
        /// <param name="className">Name of the actors class.</param>
        /// <returns>ID of an base actor.</returns>
        public static uint ActorID(string raceName, string className) { return Commands.ActorID(raceName, className); }

        /// <summary>
        /// Spawns an actor in the specified zone.
        /// </summary>
        /// <remarks>
        /// This method spawns a new actor inside the named zone (and instance if specified). The ID of the new actor is
        /// the of an actor type in the Game Editor. If you're attempting to clone an actor, you can use OtherActor.BaseActorID
        /// to obtain the actor type ID. It is possible to obtain the world position from the Game Editor (by using the camera 
        /// position) or by using OtherActor.GetPosition().
        /// </remarks>
        /// <param name="id">ID of the actor to spawn.</param>
        /// <param name="zoneName">Name of the zone in which to spawn the actor.</param>
        /// <param name="position">3D Location of the actor.</param>
        /// <returns>The new actor.</returns>
        public static Actor Spawn(uint id, string zoneName, global::Scripting.Math.SectorVector position)
        {
            return Spawn(id, zoneName, position, "", 0);
        }

        /// <summary>
        /// Spawns an actor in the specified zone.
        /// </summary>
        /// <remarks>
        /// This method spawns a new actor inside the named zone (and instance if specified). The ID of the new actor is
        /// the of an actor type in the Game Editor. If you're attempting to clone an actor, you can use OtherActor.BaseActorID
        /// to obtain the actor type ID. It is possible to obtain the world position from the Game Editor (by using the camera 
        /// position) or by using OtherActor.GetPosition().
        /// 
        /// The interaction script name is the class names of the script to execute; the 'Main' function will
        /// be executed if found.
        /// </remarks>
        /// <param name="id">ID of the actor to spawn.</param>
        /// <param name="zoneName">Name of the zone in which to spawn the actor.</param>
        /// <param name="position">3D Location of the actor.</param>
        /// <param name="script">Script to execute when a player attempts to interact with the actor.</param>
        /// <returns>The new actor.</returns>
        public static Actor Spawn(uint id, string zoneName, global::Scripting.Math.SectorVector position, string script)
        {
            return Spawn(id, zoneName, position, script, 0);
        }
        
        /// <summary>
        /// Spawns an actor in the specified zone.
        /// </summary>
        /// <remarks>
        /// This method spawns a new actor inside the named zone (and instance if specified). The ID of the new actor is
        /// the of an actor type in the Game Editor. If you're attempting to clone an actor, you can use OtherActor.BaseActorID
        /// to obtain the actor type ID. It is possible to obtain the world position from the Game Editor (by using the camera 
        /// position) or by using OtherActor.GetPosition().
        /// 
        /// The interaction script and death script names are the class names of the script to execute; the 'Main' function will
        /// be executed if found.
        /// </remarks>
        /// <param name="id">ID of the actor to spawn.</param>
        /// <param name="zoneName">Name of the zone in which to spawn the actor.</param>
        /// <param name="position">3D Location of the actor.</param>
        /// <param name="script">Script to execute when a player attempts to interact with the actor.</param>
        /// <param name="instanceIndex">Zone InstanceID in which to spawn the actor.</param>
        /// <returns>The new actor.</returns>
        public static Actor Spawn(uint id, string zoneName, global::Scripting.Math.SectorVector position, string script, int instanceIndex)
        {
            return Commands.Spawn(id, zoneName, position, script, instanceIndex);
        }

        /// <summary>
        /// Depreciated. Use uint values for actors now. This is kept for legacy code and internally just casts ID to uint. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="zonename"></param>
        /// <param name="position"></param>
        /// <param name="script"></param>
        /// <returns></returns>
        public static Actor Spawn(int id, string zonename, global::Scripting.Math.SectorVector position, string script)
        {
            return Commands.Spawn((uint)id, zonename, position, script, 0);
        }
        


        /// <summary>
        /// Finds the preset faction rating between two named factions.
        /// </summary>
        /// <remarks>
        /// A Faction Rating is a value which determines the friend/foe relationship between two actor types. The base rating
        /// is specified inside the Game Editor under the Actors tab. This method can be used in custom combat scripts or to
        /// control whether an actor will chase after a potential enemy.
        /// </remarks>
        /// <param name="factionName">Name of the first faction to test as specified in the Game Editor.</param>
        /// <param name="otherFactionName">Name of the second faction to test as specified in the Game Editor.</param>
        /// <returns>An integer faction rating. Range is -100 to 100.</returns>
        public static int DefaultFactionRating(string factionName, string otherFactionName) { return Commands.DefaultFactionRating(factionName, otherFactionName); }


    }
}
