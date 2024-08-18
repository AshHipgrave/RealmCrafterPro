using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

namespace Scripts
{
    public class DefaultAttack : ScriptBase
    {
        // Default Attribute names. If you change them in GE you will need to change them here.
        const string StrengthAttribute = "Strength";
        const string ToughnessAttribute = "Toughness";
        const string HealthAttribute = "Health";

       // This is the default attack script used when you do not set RC to use a
       // custom one in GE You CAN modify this. But it's probably better to take
       // what you like out of it and add it to Attack.cs for custom attack scripts. 


        public void NormalFormula(Actor actor, Actor target)
        {

            // 90% chance to hit
            Random random = new Random(Environment.TickCount);

            int ToHit = random.Next(100);
            
            int damage = 0;
            int damageType = 0;

            if (ToHit > 10)
            {
                // Initial damage
                int Strength = actor.GetAttribute(StrengthAttribute);
                ItemInstance weapon = actor.Backpack(ItemSlot.Weapon);
              
                if (weapon != null)
                {
                    // Actor has weapon
                    if (weapon.Health > 0)
                    {
                        // Not broken weapon
                        damage = weapon.Damage;
                        if (Strength < damage)
                            damage -= random.Next(5, 8);
                        else if (Strength > damage)
                            damage += random.Next(5, 8);
                        else
                            damage += random.Next(-5, 5);

                        damageType = weapon.DamageType;
                    }
                    else
                    {
                        // Broken weapon
                        damage = (Strength / 8) +random.Next(-5, 5);
                        damageType = actor.DefaultDamageType;
                       
                    }
                }
                else
                {
                    // Unarmed / Chuck Norrissing
                    damage = (Strength / 8) + random.Next(-5, 5);
                    damageType = actor.DefaultDamageType;
                }

                // Critical damage
                if (random.Next(1, 10) == 1)
                {
                    damage *= 2;
                    actor.Output("You hit a critical!");

                }                  



                // Armour
                int ap = GetArmourLevel(target) + (target.GetResistance(damageType) - 100);
                ap += (target.GetAttribute(ToughnessAttribute) / 8);
               // damage -= ap;

                // Minimum of 1
                if (damage < 1)
                    damage = 1;

                // Animation
                if (weapon != null)
                    actor.Animate("Default Attack", 1f, true);
                else
                    actor.Animate("Default Attack", 1f, true);
           
                target.SetAttribute(HealthAttribute, target.GetAttribute(HealthAttribute) - damage);

                DamageWeapon(random, weapon);
                DamageArmour(random, target);
            }
            else // Miss!
            {
                // damage = -1;
            }
        }

        public void NoBonusOrPenaltyFormula(Actor actor, Actor target)
        {          
            // 90% chance to hit
            Random random = new Random(Environment.TickCount);

            int ToHit = random.Next(100);
            int damage = 0;
            int damageType = 0;

            if (ToHit > 10)
            {
                // Initial damage
                ItemInstance weapon = actor.Backpack(ItemSlot.Weapon);
                if (weapon != null)
                {
                    // Actor has weapon
                    if (weapon.Health > 0)
                    {
                        // Weapon isn't broken
                        damage = weapon.Damage;
                        damageType = weapon.DamageType;
                    }
                    else
                    {
                        // Weapons broke
                        damage = (actor.GetAttribute(StrengthAttribute) / 8) + random.Next(-5, 5);
                        damageType = actor.DefaultDamageType;
                    }
                }
                else
                {
                    // Has no weapon. Karate kidding it up. 
                    damage = (actor.GetAttribute(StrengthAttribute) / 8) + random.Next(-5, 5);
                    damageType = actor.DefaultDamageType;
                }

                // Critical damage
                if (random.Next(1, 10) == 1)
                {
                    damage *= 2;
                    actor.Output("You hit a critical!");

                }

                // Armour
                int ap = GetArmourLevel(target) + (target.GetResistance(damageType) - 100);
                ap = ap + (target.GetAttribute(ToughnessAttribute) / 8);
                //damage -= ap;

                // Minimum of 1
                if (damage < 1)
                    damage = 1;

                target.SetAttribute(HealthAttribute, target.GetAttribute(HealthAttribute) - damage);

                DamageWeapon(random, weapon);
                DamageArmour(random, target);
            }
            else // Miss!
            {
               
            }
        }

        public void MultipliedFormula(Actor actor, Actor target)
        {
            int damage = 0;
            int damageType = 0;
            Random random = new Random(Environment.TickCount);
            
            // 90% chance to hit
            int ToHit = random.Next(100);
            if (ToHit > 10)
            {
                // Initial damage
                int strength = actor.GetAttribute(StrengthAttribute);
                ItemInstance weapon = actor.Backpack(ItemSlot.Weapon);

                if (weapon != null)
                {
                    // Has weapon
                    if (weapon.Health > 0)
                    {
                        // Weapon is not broken
                        damage = weapon.Damage * strength;
                        damage = weapon.DamageType;
                    }
                    else
                    {
                        // Weapon is broken
                        damage = strength + random.Next(-10, 10);
                        damageType = actor.DefaultDamageType;
                    }
                }
                else
                {
                    damage = strength + random.Next(-10, 10);
                    damageType = actor.DefaultDamageType;
                }

                // Critical damage
                if (random.Next(1, 10) == 1)
                {
                    damage *= 2;
                    actor.Output("You hit a critical!");

                }

                // Armour
                int ap = GetArmourLevel(target) + (target.GetResistance(damageType) - 100);  
                ap *= target.GetAttribute(ToughnessAttribute);

                //damage -= ap;

                // Minimum of 1
                if (damage < 1)
                    damage = 1;

                target.SetAttribute(HealthAttribute, target.GetAttribute(HealthAttribute) - damage);


                DamageWeapon(random, weapon);
                DamageArmour(random, target);
            }
            else // Miss!
            {
              
            }

        }

        private void DamageWeapon(Random random, ItemInstance weapon)
        {
            // 1 in 5 chance to damage item
            if (weapon != null ? random.Next(1, 5) == 1 : false)
            {
               weapon.Health = weapon.Health - 1;
               if (weapon.Health < 0)
                   weapon.Health = 0;
            }
        }

        private void DamageArmour(Random random, Actor actor)
        {
            // All wearable items
            for (int i = (int)ItemSlot.Shield; i <= (int)ItemSlot.Feet; ++i)
            {
                if (random.Next(1, 5) == 1)
                {
                    ItemInstance item = actor.Backpack((ItemSlot)i);
                    if (item != null)
                    {
                        item.Health -= 1;
                    }

                }
            }

        }

        // Gets total level of armour points from items an actor is sporting. 
        private int GetArmourLevel(Actor a)
        {
            int ap = 0;
            // Pro tip - ItemSlot max value is 49.
            for(int i = 0; i <= 49; i++)
            {
                ItemInstance item = a.Backpack((ItemSlot)i);
                if(item != null)
                {
                    ap += item.Armor;
                }
            }

            return ap;
        }
    }
}
