﻿using System.Collections.Generic;
using Rampastring.XNAUI;
using System.Linq;
using System;
using System.IO;
using System.Reflection;
using Rampastring.Tools;
using SixLabors.ImageSharp;

namespace ClientCore.CnCNet5
{
    /// <summary>
    /// A class for storing the collection of supported CnCNet games.
    /// </summary>
    public class GameCollection
    {
        public List<CnCNetGame> GameList { get; private set; }

        public void Initialize()
        {
            GameList = new List<CnCNetGame>();

            var assembly = Assembly.GetAssembly(typeof(GameCollection));
            using Stream dtaiconStream = assembly.GetManifestResourceStream("ClientCore.Resources.dtaicon.png");
            using Stream tiiconStream = assembly.GetManifestResourceStream("ClientCore.Resources.tiicon.png");
            using Stream tsiconStream = assembly.GetManifestResourceStream("ClientCore.Resources.tsicon.png");
            using Stream moiconStream = assembly.GetManifestResourceStream("ClientCore.Resources.moicon.png");
            using Stream yriconStream = assembly.GetManifestResourceStream("ClientCore.Resources.yricon.png");
            using Stream rriconStream = assembly.GetManifestResourceStream("ClientCore.Resources.rricon.png");
            using Stream cncriconStream = assembly.GetManifestResourceStream("ClientCore.Resources.cncricon.png");
            using Stream cncneticonStream = assembly.GetManifestResourceStream("ClientCore.Resources.cncneticon.png");
            using Stream tdiconStream = assembly.GetManifestResourceStream("ClientCore.Resources.tdicon.png");
            using Stream raiconStream = assembly.GetManifestResourceStream("ClientCore.Resources.raicon.png");
            using Stream unknowniconStream = assembly.GetManifestResourceStream("ClientCore.Resources.unknownicon.png");
            using var dtaicon = Image.Load(dtaiconStream);
            using var tiicon = Image.Load(tiiconStream);
            using var tsicon = Image.Load(tsiconStream);
            using var moicon = Image.Load(moiconStream);
            using var yricon = Image.Load(yriconStream);
            using var rricon = Image.Load(rriconStream);
            using var cncricon = Image.Load(cncriconStream);
            using var cncneticon = Image.Load(cncneticonStream);
            using var tdicon = Image.Load(tdiconStream);
            using var raicon = Image.Load(raiconStream);
            using var unknownicon = Image.Load(unknowniconStream);

            // Default supported games.
            CnCNetGame[] defaultGames = {
                new()
                {
                    ChatChannel = "#cncnet-dta",
                    ClientExecutableName = "DTA.exe",
                    GameBroadcastChannel = "#cncnet-dta-games",
                    InternalName = "dta",
                    RegistryInstallPath = "HKCU\\Software\\TheDawnOfTheTiberiumAge",
                    UIName = "Dawn of the Tiberium Age",
                    Texture = AssetLoader.TextureFromImage(dtaicon)
                },

                new()
                {
                    ChatChannel = "#cncnet-ti",
                    ClientExecutableName = "TI_Launcher.exe",
                    GameBroadcastChannel = "#cncnet-ti-games",
                    InternalName = "ti",
                    RegistryInstallPath = "HKCU\\Software\\TwistedInsurrection",
                    UIName = "Twisted Insurrection",
                    Texture = AssetLoader.TextureFromImage(tiicon)
                },

                new()
                {
                    ChatChannel = "#cncnet-ts",
                    ClientExecutableName = "TiberianSun.exe",
                    GameBroadcastChannel = "#cncnet-ts-games",
                    InternalName = "ts",
                    RegistryInstallPath = "HKLM\\Software\\Westwood\\Tiberian Sun",
                    UIName = "Tiberian Sun",
                    Texture = AssetLoader.TextureFromImage(tsicon)
                },

                new()
                {
                    ChatChannel = "#cncnet-mo",
                    ClientExecutableName = "MentalOmegaClient.exe",
                    GameBroadcastChannel = "#cncnet-mo-games",
                    InternalName = "mo",
                    RegistryInstallPath = "HKCU\\Software\\MentalOmega",
                    UIName = "Mental Omega",
                    Texture = AssetLoader.TextureFromImage(moicon)
                },

                new()
                {
                    ChatChannel = "#cncnet-yr",
                    ClientExecutableName = "CnCNetClientYR.exe",
                    GameBroadcastChannel = "#cncnet-yr-games",
                    InternalName = "yr",
                    RegistryInstallPath = "HKLM\\Software\\Westwood\\Yuri's Revenge",
                    UIName = "Yuri's Revenge",
                    Texture = AssetLoader.TextureFromImage(yricon)
                },

                new()
                {
                    ChatChannel = "#redres-lobby",
                    ClientExecutableName = "RRLauncher.exe",
                    GameBroadcastChannel = "#redres-games",
                    InternalName = "rr",
                    RegistryInstallPath = "HKML\\Software\\RedResurrection",
                    UIName = "YR Red-Resurrection",
                    Texture = AssetLoader.TextureFromImage(rricon)
                },

                new()
                {
                    ChatChannel = "#cncreloaded",
                    ClientExecutableName = "CnCReloadedClient.exe",
                    GameBroadcastChannel = "#cncreloaded-games",
                    InternalName = "cncr",
                    RegistryInstallPath = "HKCU\\Software\\CnCReloaded",
                    UIName = "C&C: Reloaded",
                    Texture = AssetLoader.TextureFromImage(cncricon)
                }
            };

            // CnCNet chat + unsupported games.
            CnCNetGame[] otherGames = {
                new()
                {
                    ChatChannel = "#cncnet",
                    InternalName = "cncnet",
                    UIName = "General CnCNet Chat",
                    AlwaysEnabled = true,
                    Texture = AssetLoader.TextureFromImage(cncneticon)
                },

                new()
                {
                    ChatChannel = "#cncnet-td",
                    InternalName = "td",
                    UIName = "Tiberian Dawn",
                    Supported = false,
                    Texture = AssetLoader.TextureFromImage(tdicon)
                },

                new()
                {
                    ChatChannel = "#cncnet-ra",
                    InternalName = "ra",
                    UIName = "Red Alert",
                    Supported = false,
                    Texture = AssetLoader.TextureFromImage(raicon)
                },

                new()
                {
                    ChatChannel = "#cncnet-d2",
                    InternalName = "d2",
                    UIName = "Dune 2000",
                    Supported = false,
                    Texture = AssetLoader.TextureFromImage(unknownicon)
                }
            };

            GameList.AddRange(defaultGames);
            GameList.AddRange(GetCustomGames(defaultGames.Concat(otherGames).ToList()));
            GameList.AddRange(otherGames);

            if (GetGameIndexFromInternalName(ClientConfiguration.Instance.LocalGame) == -1)
            {
                throw new ClientConfigurationException("Could not find a game in the game collection matching LocalGame value of " +
                    ClientConfiguration.Instance.LocalGame + ".");
            }
        }

        private List<CnCNetGame> GetCustomGames(List<CnCNetGame> existingGames)
        {
            IniFile iniFile = new IniFile(SafePath.CombineFilePath(ProgramConstants.GetBaseResourcePath(), "GameCollectionConfig.ini"));

            List<CnCNetGame> customGames = new List<CnCNetGame>();

            var section = iniFile.GetSection("CustomGames");

            if (section == null)
                return customGames;

            HashSet<string> customGameIDs = new HashSet<string>();
            foreach (var kvp in section.Keys)
            {
                if (!iniFile.SectionExists(kvp.Value))
                    continue;

                string ID = iniFile.GetStringValue(kvp.Value, "InternalName", string.Empty).ToLowerInvariant();

                if (string.IsNullOrEmpty(ID))
                    throw new GameCollectionConfigurationException("InternalName for game " + kvp.Value + " is not defined or set to an empty value.");

                if (ID.Length > ProgramConstants.GAME_ID_MAX_LENGTH)
                {
                    throw new GameCollectionConfigurationException("InternalGame for game " + kvp.Value + " is set to a value that exceeds length limit of " +
                        ProgramConstants.GAME_ID_MAX_LENGTH + " characters.");
                }

                if (existingGames.Find(g => g.InternalName == ID) != null || customGameIDs.Contains(ID))
                    throw new GameCollectionConfigurationException("Game with InternalName " + ID.ToUpperInvariant() + " already exists in the game collection.");

                string iconFilename = iniFile.GetStringValue(kvp.Value, "IconFilename", ID + "icon.png");
                using Stream unknowniconStream = Assembly.GetAssembly(typeof(GameCollection)).GetManifestResourceStream("ClientCore.Resources.unknownicon.png");
                using var unknownicon = Image.Load(unknowniconStream);
                customGames.Add(new CnCNetGame
                {
                    InternalName = ID,
                    UIName = iniFile.GetStringValue(kvp.Value, "UIName", ID.ToUpperInvariant()),
                    ChatChannel = GetIRCChannelNameFromIniFile(iniFile, kvp.Value, "ChatChannel"),
                    GameBroadcastChannel = GetIRCChannelNameFromIniFile(iniFile, kvp.Value, "GameBroadcastChannel"),
                    ClientExecutableName = iniFile.GetStringValue(kvp.Value, "ClientExecutableName", string.Empty),
                    RegistryInstallPath = iniFile.GetStringValue(kvp.Value, "RegistryInstallPath", "HKCU\\Software\\"
                            + ID.ToUpperInvariant()),
                    Texture = AssetLoader.AssetExists(iconFilename) ? AssetLoader.LoadTexture(iconFilename) :
                            AssetLoader.TextureFromImage(unknownicon)
                });
                customGameIDs.Add(ID);
            }

            return customGames;
        }

        private string GetIRCChannelNameFromIniFile(IniFile iniFile, string section, string key)
        {
            string channel = iniFile.GetStringValue(section, key, string.Empty);

            if (string.IsNullOrEmpty(channel))
                throw new GameCollectionConfigurationException(key + " for game " + section + " is not defined or set to an empty value.");

            if (channel.Contains(' ') || channel.Contains(',') || channel.Contains((char)7))
                throw new GameCollectionConfigurationException(key + " for game " + section + " contains characters not allowed on IRC channel names.");

            if (!channel.StartsWith("#"))
                return "#" + channel;

            return channel;
        }

        /// <summary>
        /// Gets the index of a CnCNet supported game based on its internal name.
        /// </summary>
        /// <param name="gameName">The internal name (suffix) of the game.</param>
        /// <returns>The index of the specified CnCNet game. -1 if the game is unknown or not supported.</returns>
        public int GetGameIndexFromInternalName(string gameName)
        {
            for (int gId = 0; gId < GameList.Count; gId++)
            {
                CnCNetGame game = GameList[gId];

                if (gameName.ToLowerInvariant() == game.InternalName)
                    return gId;
            }

            return -1;
        }

        /// <summary>
        /// Seeks the supported game list for a specific game's internal name and if found,
        /// returns the game's full name. Otherwise returns the internal name specified in the param.
        /// </summary>
        /// <param name="gameName">The internal name of the game to seek for.</param>
        /// <returns>The full name of a supported game based on its internal name.
        /// Returns the given parameter if the name isn't found in the supported game list.</returns>
        public string GetGameNameFromInternalName(string gameName)
        {
            CnCNetGame game = GameList.Find(g => g.InternalName == gameName.ToLowerInvariant());

            if (game == null)
                return gameName;

            return game.UIName;
        }

        /// <summary>
        /// Returns the full UI name of a game based on its index in the game list.
        /// </summary>
        /// <param name="gameIndex">The index of the CnCNet supported game.</param>
        /// <returns>The UI name of the game.</returns>
        public string GetFullGameNameFromIndex(int gameIndex)
        {
            return GameList[gameIndex].UIName;
        }

        /// <summary>
        /// Returns the internal name of a game based on its index in the game list.
        /// </summary>
        /// <param name="gameIndex">The index of the CnCNet supported game.</param>
        /// <returns>The internal name (suffix) of the game.</returns>
        public string GetGameIdentifierFromIndex(int gameIndex)
        {
            return GameList[gameIndex].InternalName;
        }

        public string GetGameBroadcastingChannelNameFromIdentifier(string gameIdentifier)
        {
            CnCNetGame game = GameList.Find(g => g.InternalName == gameIdentifier.ToLowerInvariant());
            if (game == null)
                return null;
            return game.GameBroadcastChannel;
        }

        public string GetGameChatChannelNameFromIdentifier(string gameIdentifier)
        {
            CnCNetGame game = GameList.Find(g => g.InternalName == gameIdentifier.ToLowerInvariant());
            if (game == null)
                return null;
            return game.ChatChannel;
        }
    }

    /// <summary>
    /// An exception that is thrown when configuration for a game to add to game collection
    /// contains invalid or unexpected settings / data or required settings / data are missing.
    /// </summary>
    class GameCollectionConfigurationException : Exception
    {
        public GameCollectionConfigurationException(string message) : base(message)
        {
        }
    }
}