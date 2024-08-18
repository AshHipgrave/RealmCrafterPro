using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Script to create a zone instance.
    /// </summary>
    public class InstanceTest : ScriptBase
    {
        Actor Player = null;

        public void Main(Actor actor)
        {
            // Test cluster exception handling
            Player.Output("Player is null at the moment, so it'll cause an exception!");

            // Store actor handle for callback
            Player = actor;
            
            // Create an instance request for the zone 'OtherZone'
            ZoneInstanceRequest instanceRequest = new ZoneInstanceRequest("OtherZone");

            // Register callback for the server to call when the instance has been setup
            RegisterCallback(instanceRequest, "Completed", new ZoneInstanceRequestEventHandler(InstanceRequest_Completed));

            // Post it, and check it went through
            if (!RCScript.PostZoneInstanceRequest(instanceRequest))
            {
                RCScript.Log("Area does not exist!");
            }
            else
            {
                RCScript.Log("Request sent!");
            }
        }

        public void InstanceRequest_Completed(ZoneInstanceRequest e)
        {
            // Instance created?
            if (e.Succeeded)
            {
                // Log it and warp to it
                RCScript.Log("New Instance: " + e.ActualID);
                Player.Warp(e.ZoneName, "Start", e.ActualID);
            }
            else
            {
                // Failed?
                RCScript.Log("Instance Request Failed!");
            }

            // Always clear callbacks so the script will be closed
            ClearCallbacks();
        }
    }
}
