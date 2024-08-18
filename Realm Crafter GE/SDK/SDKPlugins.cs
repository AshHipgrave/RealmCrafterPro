//##############################################################################################################################
// Realm Crafter Professional																									
// Copyright (C) 2013 Solstar Games, LLC. All rights reserved																	
// contact@solstargames.com																																																		
//
// Grand Poohbah: Mark Bryant
// Programmer: Jared Belkus
// Programmer: Frank Puig Placeres
// Programmer: Rob Williams
// 																										
// Program: 
//																																
//This is a licensed product:
//BY USING THIS SOURCECODE, YOU ARE CONFIRMING YOUR ACCEPTANCE OF THE SOFTWARE AND AGREEING TO BECOME BOUND BY THE TERMS OF 
//THIS AGREEMENT. IF YOU DO NOT AGREE TO BE BOUND BY THESE TERMS, THEN DO NOT USE THE SOFTWARE.
//																		
//Licensee may NOT: 
// (i)   create any derivative works of the Engine, including translations Or localizations, other than Games;
// (ii)  redistribute, encumber, sell, rent, lease, sublicense, Or otherwise transfer rights To the Engine// or
// (iii) remove Or alter any trademark, logo, copyright Or other proprietary notices, legends, symbols Or labels in the Engine.
// (iv)   licensee may Not distribute the source code Or documentation To the engine in any manner, unless recipient also has a 
//       license To the Engine.													
// (v)  use the Software to develop any software or other technology having the same primary function as the Software, 
//       including but not limited to using the Software in any development or test procedure that seeks to develop like 
//       software or other technology, or to determine if such software or other technology performs in a similar manner as the
//       Software																																
//##############################################################################################################################
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Reflection;

namespace RealmCrafter.SDK
{
    public class SDKPlugins
    {
        public static List<ISDKEnvironmentConfigurator> EnvironmentConfigurators = new List<ISDKEnvironmentConfigurator>();

        public static void LoadPlugins()
        {
            String PluginsDir = Path.GetDirectoryName(Application.ExecutablePath) + "\\Plugins\\";

            try
            {
                String[] Files = Directory.GetFiles(PluginsDir);
                foreach (String PluginPath in Files)
                {
                    if (Path.GetExtension(PluginPath).Equals(".dll", StringComparison.CurrentCultureIgnoreCase)
                        && !Path.GetFileNameWithoutExtension(PluginPath).Equals("SDKPlugin.dll", StringComparison.CurrentCultureIgnoreCase))
                    {
                        LoadPlugin(PluginPath);
                    }
                }

                RealmCrafter_GE.Program.GE.ZoneSetupForm.WorldZoneEnvironmentList.Items.Clear();
                foreach (ISDKEnvironmentConfigurator Co in EnvironmentConfigurators)
                {
                    RealmCrafter_GE.Program.GE.ZoneSetupForm.WorldZoneEnvironmentList.Items.Add(Co.GetName());
                }
            }
            catch (System.IO.DirectoryNotFoundException)
            {

            }
        }

        private static void LoadPlugin(String path)
        {
            try
            {
                Assembly LoadingAssembly = null;
                String TypeName = String.Empty;
                List<Type> PluginTypes = new List<Type>();

                if (File.Exists(path))
                {
                    LoadingAssembly = Assembly.LoadFile(path);
                }
                else
                {
                    return;
                }

                foreach (Type type in LoadingAssembly.GetTypes())
                {
                    if (type.IsAbstract)
                        continue;

                    //if (type.IsSubclassOf(typeof(ISDKEnvironmentConfigurator)))
//                     if(type.BaseType == typeof(ISDKEnvironmentConfigurator))
//                         PluginTypes.Add(type);
                    if (type.IsDefined(typeof(RealmCrafter.SDK.EnvironmentConfigAttribute), true))
                        PluginTypes.Add(type);
                }

                foreach (Type PluginType in PluginTypes)
                {
                    if (PluginType != null)
                    {
                        ISDKEnvironmentConfigurator Provider = Activator.CreateInstance(PluginType) as ISDKEnvironmentConfigurator;
                        if (Provider != null)
                            EnvironmentConfigurators.Add(Provider);
                        else
                            throw new Exception("Could not cast provider from CreateInstance!");
                    }
                }
            }
            catch (InvalidCastException)
            {
                MessageBox.Show("Plugin did not implement the correct interface. Maybe it is out of date?");
            }
            catch (BadImageFormatException)
            {
                //MessageBox.Show("Invalid plugin file. Its either corrupt or not a plugin!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
