using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

namespace UserScripts
{
    /// <summary>
    /// Default zone weather/entry script.
    /// </summary>
    public class DefaultZoneScript : ScriptBase
    {
        // Static randomizer stops all zones from sharing the same climate
        static Random Rand = new Random(Environment.TickCount);

        // Dictionary stores the running zone scripts, this is so that we can send the current weather to a player when he joins
        static Dictionary<ZoneInstance, DefaultZoneScript> ActiveZones = new Dictionary<ZoneInstance, DefaultZoneScript>();

        // Handle of the zone instance
        ZoneInstance Instance;

        // The current weather of the zone (always sunny by default)
        Weather CurrentWeather = Weather.Sun;

        // Chance that a weather selection will be of a specific type, must add to 100%
        static int[] WeatherChances = new int[] {
            78, // Sun
            10, // Rain
            1, // Snow
            5, // Fog
            1, // Storm
            5 // Wind
        };

        // Time to change the weather
        Timer WeatherTimer = null;

        // Method to select a new weather
        void NewWeather(object sender, Scripting.Forms.FormEventArgs e)
        {
            // Get the next weather
            int RandomWeather = Rand.Next(0, 100);
            int Counter = 0;

            for (int i = 0; i < 6; ++i)
            {
                if (RandomWeather >= Counter && RandomWeather < Counter + WeatherChances[i])
                {
                    CurrentWeather = (Weather)i;
                    break;
                }

                Counter += WeatherChances[i];
            }

            // Time for weather to remain active
            uint WeatherTime = (uint)Rand.Next(20000, 40000);

            // Inform all players
            PacketWriter Packet = CreateDefaultPacket();
            Packet.Write((byte)CurrentWeather);

            // We must use ToArray() before calling the update method,
            // this prevents hundreds of useless allocations.
            byte[] PacketBytes = Packet.ToArray();

            LinkedList<Actor> Actors = Instance.GetActors(ActorType.Player, 0, 0, 65535, 65535);
            foreach(Actor AI in Actors)
                AI.SendEnvironmentUpdate(PacketBytes);

            //RCScript.Log(Instance.Name + " weather changed to: " + CurrentWeather.ToString() + " for " + (WeatherTime / 1000) + " seconds");

            // Restart timer
            WeatherTimer.Interval = WeatherTime;
            WeatherTimer.Start();
        }

        // All environment packets are preceeded by important environment data.
        // These include the time and date.
        PacketWriter CreateDefaultPacket()
        {
            PacketWriter Pa = new PacketWriter();

            // The first four bytes must contain empty data
            // This does not exist, the packet will be corrupted!
            Pa.Write(0);

            Pa.Write(0); // Year as Int32
            Pa.Write((ushort)WorldEnvironment.Day); // Day as ushort
            Pa.Write((byte)WorldEnvironment.Hour); // Hour as byte
            Pa.Write((byte)WorldEnvironment.Minute); // Minute as byte
            Pa.Write((byte)WorldEnvironment.TimeFactor); // TimeFactor as byte

            return Pa;
        }

        public void OnActivate(ZoneInstance instance, byte virtualZoneID)
        {
            // Setup script
            WeatherTimer = new Timer();
            Instance = instance;
            if (!ActiveZones.ContainsKey(instance))
                ActiveZones.Add(instance, this);
            else
                ActiveZones[instance] = this;

            RegisterCallback(WeatherTimer, "Tick", new Scripting.Forms.EventHandler(NewWeather));

            // Start weather
            NewWeather(WeatherTimer, null);
        }

        public void OnDeactivate(ZoneInstance instance, byte virtualZoneID)
        {
            // Clear current timers
            WeatherTimer.Stop();
            ClearCallbacks();
            ActiveZones.Remove(Instance);
        }

        public void OnEnter(Actor actor, ZoneInstance zone)
        {
            // Tell him
            actor.Output("You have entered: " + zone.Name);

            // Send weather update to player
            PacketWriter Packet = CreateDefaultPacket();
            if (ActiveZones.ContainsKey(zone))
                Packet.Write((byte)ActiveZones[zone].CurrentWeather);
            else
                Packet.Write((byte)0);

            actor.SendEnvironmentUpdate(Packet.ToArray());
        }

        public void OnExit(Actor actor)
        {
        }
    }
}
