using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Item Slot (Inventory) Enumeration.
    /// </summary>
    public enum ItemSlot
    {
        /// <summary>
        /// Weapon Slot
        /// </summary>
        Weapon = 0,

        /// <summary>
        /// Shield Slot 
        /// </summary>
        Shield = 1,

        /// <summary>
        /// Hat/Head Slot 
        /// </summary>
        Hat = 2,

        /// <summary>
        /// Chest Slot 
        /// </summary>
        Chest = 3,

        /// <summary>
        /// Hand/Gloves Slot 
        /// </summary>
        Hand = 4,

        /// <summary>
        /// Belt/Waist Slot 
        /// </summary>
        Belt = 5,

        /// <summary>
        /// Legs Slot 
        /// </summary>
        Legs = 6,

        /// <summary>
        /// Feet/Shoes Slot 
        /// </summary>
        Feet = 7,

        /// <summary>
        /// First Ring Slot 
        /// </summary>
        Ring1 = 8,

        /// <summary>
        /// Second Ring Slot 
        /// </summary>
        Ring2 = 9,

        /// <summary>
        /// Third Ring Slot 
        /// </summary>
        Ring3 = 10,

        /// <summary>
        /// Fourth Ring Slot 
        /// </summary>
        Ring4 = 11,

        /// <summary>
        /// First Amulet Slot 
        /// </summary>
        Amulet1 = 12,

        /// <summary>
        /// Second Amulet Slot 
        /// </summary>
        Amulet2  = 13,

        /// <summary>
        /// Slot 1
        /// </summary>
        Slot1 = 14,

        /// <summary>
        /// Slot 2
        /// </summary>
        Slot2 = 15,

        /// <summary>
        /// Slot 3
        /// </summary>
        Slot3 = 16,

        /// <summary>
        /// Slot 4
        /// </summary>
        Slot4 = 17,

        /// <summary>
        /// Slot 5
        /// </summary>
        Slot5 = 18,

        /// <summary>
        /// Slot 6
        /// </summary>
        Slot6 = 19,

        /// <summary>
        /// Slot 7
        /// </summary>
        Slot7 = 20,

        /// <summary>
        /// Slot 8
        /// </summary>
        Slot8 = 21,

        /// <summary>
        /// Slot 9
        /// </summary>
        Slot9 = 22,

        /// <summary>
        /// Slot 10
        /// </summary>
        Slot10 = 23,

        /// <summary>
        /// Slot 11
        /// </summary>
        Slot11 = 24,

        /// <summary>
        /// Slot 12
        /// </summary>
        Slot12 = 25,

        /// <summary>
        /// Slot 13
        /// </summary>
        Slot13 = 26,

        /// <summary>
        /// Slot 14
        /// </summary>
        Slot14 = 27,

        /// <summary>
        /// Slot 15
        /// </summary>
        Slot15 = 28,

        /// <summary>
        /// Slot 16
        /// </summary>
        Slot16 = 29,

        /// <summary>
        /// Slot 17
        /// </summary>
        Slot17 = 30,

        /// <summary>
        /// Slot 18
        /// </summary>
        Slot18 = 31,

        /// <summary>
        /// Slot 19
        /// </summary>
        Slot19 = 32,

        /// <summary>
        /// Slot 20
        /// </summary>
        Slot20 = 33,

        /// <summary>
        /// Slot 21
        /// </summary>
        Slot21 = 34,

        /// <summary>
        /// Slot 22
        /// </summary>
        Slot22 = 35,

        /// <summary>
        /// Slot 23
        /// </summary>
        Slot23 = 36,

        /// <summary>
        /// Slot 24
        /// </summary>
        Slot24 = 37,

        /// <summary>
        /// Slot 25
        /// </summary>
        Slot25 = 38,

        /// <summary>
        /// Slot 26
        /// </summary>
        Slot26 = 39,

        /// <summary>
        /// Slot 27
        /// </summary>
        Slot27 = 40,

        /// <summary>
        /// Slot 28
        /// </summary>
        Slot28 = 41,

        /// <summary>
        /// Slot 29
        /// </summary>
        Slot29 = 42,

        /// <summary>
        /// Slot 30
        /// </summary>
        Slot30 = 43,

        /// <summary>
        /// Slot 31
        /// </summary>
        Slot31 = 44,

        /// <summary>
        /// Slot 32
        /// </summary>
        Slot32 = 45,

        /// <summary>
        /// Slot 33
        /// </summary>
        Slot33 = 46,

        /// <summary>
        /// Slot 34
        /// </summary>
        Slot34 = 47,

        /// <summary>
        /// Slot 35
        /// </summary>
        Slot35 = 48,

        /// <summary>
        /// Slot 36
        /// </summary>
        Slot36 = 49
    }
}

//Using "RC_Core.rcm"
//; Attack script, used if you set the combat damage formula to "scripted"

//; You are responsible for applying any damage to the actor, and for animating actors or blood effects
//; The actor is the attacker, and the context actor is the actor being attacked

//Function Main()

//    Output(Actor(), "Actor Attacking!", 0, 255, 0)
//    Output(ContextActor(), "Monster Attacking!", 255, 0, 0)

//    ; *** GRAB PLAYERS NFO **********************************************************************************

//    Player = Actor()
//    Target = ContextActor()
//    PlayerIsHuman = ActorIsHuman(Player) ; Result 1 is human 0 is npc
//    PlayerName$ = Name(Player)
//    PlayerStrength% = Attribute (Player,"Strength")
//    PlayerSpeed% = Attribute(Player,"Speed")
//    PlayerDex% = Attribute(Player,"Dexterity")
//    PlayerLevel% = ActorLevel(Player)
//    TargetName$ = Name(Target)
//    TargetHealth% = Attribute(Target,"Health")
//    TargetDex% = Attribute(Target,"Dexterity") ;we only need target dexterity for now in this simple form
//    TargetStrength% = Attribute(Target,"Strength")
//    TargetLevel% = ActorLevel(Target)

//    ; ************ ARMOR DEFINITIONS *************************

//    HatArmor% = ItemArmor(ActorHat(Target))
//    ChestArmor% = ItemArmor(ActorChest(Target))
//    ShieldArmor% = ItemArmor (ActorShield(Target))
//    FeetArmor% = ItemArmor(ActorFeet(Target))
//    ArmorBonus% = HatArmor + ChestArmor + ShieldArmor + FeetArmor

//    ; ************ Weapon Damage AND MODIFIERS *****************************

//    WeaponDamage% = ItemDamage(ActorWeapon(Player))
//    WeaponModifier% = Weapondamage - 1
//    FinalWeaponDamage% = WeaponDamage + Rand(- WeaponModifier, 0.0)

//    ; ***************** SET UP FOR COMBAT *************************************************

//    AttackChance% = PlayerSpeed + Rand(-20,20) ; Added 20 percent chance for missing or hitting
//    DodgeChance% = TargetDex + Rand(-20,20) ; Added 20 percent chance for missing or hitting
//    Damage% = PlayerStrength
//    FinalDamage% = Damage + FinalWeaponDamage + Rand(0,10) - ArmorBonus

//    ; ******** SET UP FOR CRITS *********************************************

//    PlayerCrit% = PlayerStrength + PlayerDex
//    TargetCrit% = TargetStrength + TargetDex
//    PlayerCritRoll% = PlayerCrit + Rand(-10, 0)
//    TargetCritRoll% = TargetCrit + Rand(-10, 10)
//    CritDamage% = PlayerStrength + PlayerDex + Rand(-10,10)

//    ; ********** DECLARATION FOR AWARDING XP POINTS **********************************************

//    LevelDifference% = TargetLevel - PlayerLevel
//    AwardXp% = 0

//    ; Player is 10 lvls+ lower than target
//    If LevelDifference > 10
//        AwardXp = 0
//    ElseIf LevelDifference < -10 ; Player is 10 lvls+ higher target
//        AwardXp = 0
//    ElseIf LevelDifference < 0 ; Player and target are same lvl
//        AwardXp = TargetLevel * PlayerLevel + 24
//    ElseIf LevelDifference < 9 ; Player is up to 9 lvls lower than target
//        AwardXp = TargetLevel * PlayerLevel + LevelDifference
//    EndIf

//    ; ************************* COMBAT FORMULAS **************************************************

//    CritSuccess% = 0

//    If PlayerCritRoll >= TargetCritRoll
//        CritSuccess = 1
//    Else
//        CritSuccess = 0
//    EndIf

//    ; Make sure everyone does a minimal of 0 crit
//    If CritDamage <= 0
//        CritDamage = 0
//    EndIf

//    ; Make sure everyone does a minimum of 0 damage
//    If FinalDamage <= 0
//        FinalDamage = 0
//    EndIf

//    ; Calculate a hit or miss add a crit chance
//    If AttackChance >= DodgeChance
//        If CritSuccess > 0

//            ; Since we crit, Tally the damage
//            TargetHealth = TargetHealth - CritDamage
//            Output(Player, "You Hit For " + CritDamage + " Critical Damage ", 255, 0, 0)
//            DoEvents(200)

//            ; Give the damage to the target
//            SetAttribute(Target, "Health", TargetHealth) 


//            If TargetHealth <= CritDamage
//                If PlayerIsHuman = 1
//                    GiveXp(Player,AwardXp)
//                    SetAttribute(Target, "Health", 0)
//                    ; CreateFloatingNumber(Target, AwardXp, 255, 255, 255)
//                    DoEvents (500)
//                    ThreadExecute("MonsterDeath", "Main", Player, Target, 0)
//                    Output(Player, "Target DIED from a CRITICAL HIT ", 255, 0, 0) ;debug output
//                    Return
//                Else ; Player is NPC
//                    GiveXp(Player,AwardXp)
//                    SetAttribute(Target, "Health", 0)
//                    DoEvents (500)

//                    ; Reset monsters AI
//                    SetActorTarget(Player, 0)
//                    SetActorAIState(Player, 1)
//                    Doevents (60000)
//                    SetActorTarget(Player, 0)
//                    Return
//                EndIf
//            End If


//            ; Since we hit, tally the damage
//            TargetHealth = TargetHealth - FinalDamage 
//            Output(Player, PlayerName + " Has Hit" + TargetName + "for" + FinalDamage + "Damage", 0, 255, 255)
//            ; CreateFloatingNumber(Target, FinalDamage, 255, 255, 255)
//            DoEvents(200)
            
//            ; Give the damage to the target
//            SetAttribute(Target, "Health", TargetHealth) 

//            If TargetHealth <= FinalDamage
//                If PlayerIsHuman = 1
//                    GiveXp(Player,AwardXp)
//                    SetAttribute(Target, "Health", 0)
//                    ; CreateFloatingNumber(Target, AwardXp, 255, 0, 255)
//                    DoEvents (500)
//                    ThreadExecute("MonsterDeath", "Main", Player, Target, 0)
//                    Output(Player, "Target DIED from your Attack ", 255, 0, 0) ;debug output
//                    Return
//                Else ; Player is NPC
//                    GiveXp(Player,AwardXp)
//                    SetAttribute(Target, "Health", 0)
//                    DoEvents (500)
//                    SetActorTarget(Player, 0) ; Reset monsters AI
//                    SetActorAIState(Player, 1)
//                    Doevents (60000)
//                    SetActorTarget(Player, 0)
//                    Return
//                EndIf
//            EndIf
//        Else
            
//            ; Tell them they missed
//            Output(Player, PlayerName + " has Missed " + TargetName , 255, 255, 255)
//            ; PlaySound(Player, 25, 1)

//        EndIf ; CritSuccess > 0
//    EndIf ; AttackChance >= DodgeChance <<< THIS WAS MISSING!
    
//    Return

//End Function


//*************************************************************************************************************************

//MonsterDeath.rsl :

//Using "RC_Core.rcm"

//Function Main()

//    Player = Actor()
//    Target = ContextActor()
//    DropZone$ = ActorZone(Target)

//    XPos% = ActorX(Target) + Rand(-2, 2)
//    YPos% = ActorY(Target)
//    ZPos% = ActorZ(Target) + Rand(-2, 2)

//    Death = Rand(1, 3)
//    If Death = 1
//        AnimateActor(Target, "Death 1", 0.2, 0)
//    ElseIf Death = 2
//        AnimateActor(Target, "Death 2", 0.2, 0)
//    Else
//        AnimateActor(Target, "Death 3", 0.2, 0)
//    EndIf
//    DoEvents(500)

//    AnimateActor(Player, "Idle", .3, 0)
//    DoEvents(100)

//    DripDrop% = Rand(1,10)
//    Output(Player, "You Have Found " + DripDrop + " Drips")
//    ChangeMoney(Player, DripDrop)

//    ItemType% = Rand(1, 2)
//    Select ItemType
//        Case 1
//            ; Eddie figure this out later
//            ; Loot$ = "Boar Tusk"
//            ; LootAmnt = Rand(1, 2)
//        Case 2
//            ; Loot$ = "Heal Stone"
//            ; LootAmnt = Rand(1, 2)
//    End Select

//    ; SpawnItem(Loot, LootAmnt, DropZone, Xpos, Ypos, Zpos)

//    Return
//End Function