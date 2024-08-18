    using System;
    using System.Threading;
    using System.Collections.Generic;
    using System.Text;
    using Scripting;

namespace UserScripts
{
    public class Attack : ScriptBase
    {
        Scripting.Timer AttackTimer;
        GetActorInfo actorinfo = new GetActorInfo();

        public void Main(Actor Attacker, Actor Defender)
        {
            int Attacker_Damage;
            int Attacker_CritDamage;

            // Void Attacker_Damage is 0 or lower
            if (actorinfo.GetDamage(Attacker, Defender) <= 0)
            {
                Attacker_Damage = 0;
            }
            else
            {
                Attacker_Damage = actorinfo.GetDamage(Attacker, Defender);
            }

            // Approve Attacker for Critical Chance if Attacker_CritRoll is Greater
            int Attacker_CritSuccess = 0;
            if (actorinfo.GetCritRoll(Attacker, Defender) >= actorinfo.GetCritRoll(Defender, Attacker))
            {
                Attacker_CritSuccess = 1;
            }

            // Void Attacker_CritDamage is 0 or lower
            if (actorinfo.GetCritDamage(Attacker, Defender) <= 0)
            {
                Attacker_CritDamage = 0;
            }
            else
            {
                Attacker_CritDamage = actorinfo.GetCritDamage(Attacker, Defender);
            }

            // Get random battle animation
            int Arand = actorinfo.RandomNumber(1, 3);
            switch (Arand)
            {
                case 1:
                    Attacker.Animate("Default Attack", 1.0f, true);
                    Attacker.PlaySound(1);
                    AttackTimer = new Scripting.Timer(500, false);
                    AttackTimer.Start();
                    break;
                case 2:
                    Attacker.Animate("Right Hand Attack", 1.0f, true);
                    Attacker.PlaySound(1);
                    AttackTimer = new Scripting.Timer(500, false);
                    AttackTimer.Start();
                    break;
                case 3:
                    Attacker.Animate("Two Handed Attack", 1.0f, true);
                    Attacker.PlaySound(1);
                    AttackTimer = new Scripting.Timer(500, false);
                    AttackTimer.Start();
                    break;
            }

            int AwardXP = actorinfo.GetAwardedXP(Attacker, Defender);

            // Check to see if Attacker's weapon is within range of hitting Target
            if (actorinfo.GetWeapon_Range(Attacker) >= actorinfo.GetRange(Attacker, Defender))
            {
                // Checks to see Target dodges
                if (actorinfo.GetDodge(Attacker, Defender) >= actorinfo.GetDodge(Defender, Attacker))
                {
                    // Checks to see if a critical chance is allowed for Attacker
                    if (Attacker_CritSuccess > 0)
                    {
                        // Checks to see if Target will dodge critical hit
                        if (actorinfo.GetCritDodge(Attacker, Defender) >= actorinfo.GetCritDodge(Defender, Attacker))
                        {
                            // Calculates critical damage to Target
                            int Target_Vitality = actorinfo.GetBaseStat(Defender, "Vitality") - Attacker_CritDamage;
                            Defender.SetAttribute("Vitality", Target_Vitality);
                            Attacker.CreateFloatingNumber(Attacker_CritDamage, System.Drawing.Color.White);
                            if (Attacker.Human)
                            {
                                Attacker.Output("You hit for " + Attacker_CritDamage + " critical damage!", System.Drawing.Color.White);
                            }
                            if (Defender.Human)
                            {
                                Defender.Output("You got hit for " + Attacker_CritDamage + " critical damage!", System.Drawing.Color.White);
                            }
                            AttackTimer = new Scripting.Timer(100, false);
                            AttackTimer.Start();

                            // Checks to see if critical hit killed Target
                            if (Target_Vitality <= Attacker_CritDamage)
                            {
                                // Calculates death information
                                Defender.SetAttribute("Vitality", 0);
                                if (Attacker.Human)
                                {
                                    Attacker.Output("You killed " + actorinfo.GetName(Defender) + "!", System.Drawing.Color.White);
                                    Attacker.GiveXP(AwardXP);
                                }
                                AttackTimer = new Scripting.Timer(1000, false);
                                AttackTimer.Start();
                                Attacker.Kill(Defender);
                                return;
                            }
                        }
                        else // If Target dodges a critical strike
                        {
                            if (Attacker.Human)
                            {
                                Attacker.Output("Your attack missed " + actorinfo.GetName(Defender) + "!", System.Drawing.Color.White);
                            }
                            if (Defender.Human)
                            {
                                Defender.Output(actorinfo.GetName(Attacker) + "'s attack missed you!", System.Drawing.Color.White);
                            }
                        }
                    }
                    else // if Attacker isnt allowed a critical chance, process normal attack
                    {
                        // Calculates damage to Target
                        int Target_Vitality = actorinfo.GetBaseStat(Defender, "Vitality") - Attacker_Damage;
                        Defender.SetAttribute("Vitality", Target_Vitality);
                        Attacker.CreateFloatingNumber(Attacker_Damage, System.Drawing.Color.White);
                        if (Attacker.Human)
                        {
                            Attacker.Output("You hit for " + Attacker_Damage + " damage!", System.Drawing.Color.White);
                        }
                        if (Defender.Human)
                        {
                            Defender.Output("You got hit for " + Attacker_Damage + " damage!", System.Drawing.Color.White);
                        }
                        AttackTimer = new Scripting.Timer(1000, false);
                        AttackTimer.Start();


                        // Checks to see if hit killed Target
                        if (Target_Vitality <= Attacker_Damage)
                        {
                            // Calculates death information
                            Defender.SetAttribute("Vitality", 0);
                            if (Attacker.Human)
                            {
                                Attacker.Output("You killed " + actorinfo.GetName(Defender) + "!", System.Drawing.Color.White);
                                Attacker.GiveXP(AwardXP);
                            }
                            AttackTimer = new Scripting.Timer(1000, false);
                            AttackTimer.Start();
                            Attacker.Kill(Defender);
                            return;
                        }
                    }
                }
                else // If Target dodges a normal strike
                {
                    if (Attacker.Human)
                    {
                        Attacker.Output("Your attack missed " + actorinfo.GetName(Defender) + "!", System.Drawing.Color.White);
                    }
                    if (Defender.Human)
                    {
                        Defender.Output(actorinfo.GetName(Attacker) + "'s attack missed you!", System.Drawing.Color.White);
                    }
                }
            }
            else // If Target is out of weapon range
            {
                if (Attacker.Human)
                {
                    Attacker.Output("You are out of range!", System.Drawing.Color.White);
                }
            }
        }
    }




    public class GetActorInfo
    {
        public int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        public string GetName(Actor Actor)
        {
            string Actor_Name = Actor.Name;
            return Actor_Name;
        }

        public int GetLevel(Actor Actor)
        {
            int Actor_Level = Actor.Level;
            return Actor_Level;
        }

        public int GetBaseStat(Actor Actor, string Stat)
        {
            int Actor_Stat = Actor.GetAttribute(Stat);
            return Actor_Stat;
        }

        public int GetASpeed(Actor Actor)
        {
            int Actor_ASpeed = GetBaseStat(Actor, "Att. Speed") + GetBaseStat(Actor, "Accuracy");
            return Actor_ASpeed;
        }

        public float GetRange(Actor Actor, Actor Target)
        {
            float Actor_Range = Actor.DistanceFrom(Target);
            return Actor_Range;
        }

        public int GetHelm(Actor Actor)
        {
            int Actor_Helm = 0;
            if (Actor.Backpack(ItemSlot.Hat) != null)
            {
                Actor_Helm = Actor.Backpack(ItemSlot.Hat).Armor;
            }
            else
            {
                Actor_Helm = 0;
            }
            return Actor_Helm;
        }

        public int GetChest(Actor Actor)
        {
            int Actor_Chest = 0;
            if (Actor.Backpack(ItemSlot.Chest) != null)
            {
                Actor_Chest = Actor.Backpack(ItemSlot.Chest).Armor;
            }
            else
            {
                Actor_Chest = 0;
            }
            return Actor_Chest;
        }

        public int GetLegs(Actor Actor)
        {
            int Actor_Legs = 0;
            if (Actor.Backpack(ItemSlot.Legs) != null)
            {
                Actor_Legs = Actor.Backpack(ItemSlot.Legs).Armor;
            }
            else
            {
                Actor_Legs = 0;
            }
            return Actor_Legs;
        }

        public int GetFeet(Actor Actor)
        {
            int Actor_Feet = 0;
            if (Actor.Backpack(ItemSlot.Feet) != null)
            {
                Actor_Feet = Actor.Backpack(ItemSlot.Feet).Armor;
            }
            else
            {
                Actor_Feet = 0;
            }
            return Actor_Feet;
        }

        public int GetGloves(Actor Actor)
        {
            int Actor_Gloves = 0;
            if (Actor.Backpack(ItemSlot.Hand) != null)
            {
                Actor_Gloves = Actor.Backpack(ItemSlot.Hand).Armor;
            }
            else
            {
                Actor_Gloves = 0;
            }
            return Actor_Gloves;
        }

        public int GetShield(Actor Actor)
        {
            int Actor_Shield = 0;
            if (Actor.Backpack(ItemSlot.Shield) != null)
            {
                Actor_Shield = Actor.Backpack(ItemSlot.Shield).Armor;
            }
            else
            {
                Actor_Shield = 0;
            }
            return Actor_Shield;
        }

        public int GetArmorBonus(Actor Actor)
        {
            int Actor_ArmorBonus = (((GetHelm(Actor) + GetChest(Actor)) + (GetLegs(Actor) + GetFeet(Actor))) + (GetGloves(Actor) + GetShield(Actor)));
            return Actor_ArmorBonus;
        }

        public int GetWeapon_Damage(Actor Actor)
        {
            int Actor_Weapon_Damage = 0;
            if (Actor.Backpack(ItemSlot.Weapon) != null)
            {
                Actor_Weapon_Damage = Actor.Backpack(ItemSlot.Weapon).Damage;
            }
            else
            {
                Actor_Weapon_Damage = 5;
            }
            return Actor_Weapon_Damage;
        }

        public float GetWeapon_Range(Actor Actor)
        {
            float Actor_Weapon_Range = 0;
            if (Actor.Backpack(ItemSlot.Weapon) != null)
            {
                Actor_Weapon_Range = Actor.Backpack(ItemSlot.Weapon).Range;
            }
            else
            {
                Actor_Weapon_Range = 5;
            }
            return Actor_Weapon_Range;
        }

        public int GetDamage(Actor Actor, Actor Target)
        {
            int Actor_Damage = ((GetBaseStat(Actor, "Attack") + GetWeapon_Damage(Actor)) + RandomNumber(0, 20)) - (GetArmorBonus(Target) + GetBaseStat(Target, "Defence"));
            return Actor_Damage;
        }

        public int GetDodge(Actor Actor, Actor Target)
        {
            int Actor_Dodge = (GetBaseStat(Target, "Evasion") + GetASpeed(Target)) - (GetASpeed(Actor) + RandomNumber(-20, 20));
            return Actor_Dodge;
        }

        public int GetCritDodge(Actor Actor, Actor Target)
        {
            int Actor_CritDodge = (GetBaseStat(Actor, "Crit. Dodge") + GetASpeed(Actor)) - (GetASpeed(Target) + RandomNumber(-20, 20));
            return Actor_CritDodge;
        }

        public int GetCritical(Actor Actor, Actor Target)
        {
            int Actor_Critical = (GetBaseStat(Actor, "Crit. Chance") + GetBaseStat(Actor, "Accuracy")) - GetBaseStat(Target, "Crit. Chance");
            return Actor_Critical;
        }

        public int GetCritRoll(Actor Actor, Actor Target)
        {
            int Actor_CritRoll = GetCritical(Actor, Target) + RandomNumber(-10, 10);
            return Actor_CritRoll;
        }

        public int GetCritDamage(Actor Actor, Actor Target)
        {
            int Actor_CritDamage = ((GetDamage(Actor, Target) * GetBaseStat(Actor, "Crit. Damage")) + RandomNumber(-10, 10)) - ((GetBaseStat(Actor, "Crit. Defence") * GetBaseStat(Actor, "Defence")) + RandomNumber(-10, 10));
            return Actor_CritDamage;
        }


        public int GetAwardedXP(Actor Actor, Actor Target)
        {
            int AwardXP = 0;
            int Player_Level = Actor.Level;
            int Target_Level = Target.Level;
            int LevelDifference = Target_Level - Player_Level;

            if (Target.Human)
            {
                if (LevelDifference >= 10)
                {
                    AwardXP = (Target_Level * Player_Level) + LevelDifference + 24;
                }
                else if (LevelDifference >= 0 && LevelDifference <= 9)
                {
                    AwardXP = (Target_Level * Player_Level) + LevelDifference;
                }
                else
                {
                    for (int i = AwardXP; i >= LevelDifference; i--)
                    {
                        AwardXP = (Target_Level * Player_Level) + (i * (12 * 3));
                    }
                }
                return AwardXP;
            }
            else
            {
                if (LevelDifference >= 10)
                {
                    AwardXP = (Target_Level * Player_Level) + LevelDifference + 24;
                }
                else if (LevelDifference >= 0 && LevelDifference <= 9)
                {
                    AwardXP = (Target_Level * Player_Level) + LevelDifference;
                }
                else if (LevelDifference <= 0 || LevelDifference >= -10)
                {
                    for (int i = AwardXP; i >= LevelDifference; i--)
                    {
                        AwardXP = (Target_Level * Player_Level) + i;
                    }
                }
                else
                {
                    AwardXP = 0;
                }
                return AwardXP;
            }
        }
    }
}
    
    
    

