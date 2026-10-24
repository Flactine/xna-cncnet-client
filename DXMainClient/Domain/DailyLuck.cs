using System;
using System.Buffers.Binary;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

using ClientCore;

using Microsoft.Win32;

using Rampastring.Tools;

namespace DTAClient.Domain
{
    /// <summary>
    /// Generates the "daily luck" value shown on the main menu.
    /// The value is derived from the local date, the player name and the machine /
    /// installation identifier, so it stays the same for the whole day but differs
    /// between days, players and computers.
    /// </summary>
    public static class DailyLuck
    {
        /// <summary>
        /// The maximum possible luck value.
        /// </summary>
        public const int MAX_VALUE = 100;

        /// <summary>
        /// Version of the seed construction. Changing it re-rolls the values of
        /// every day, which is useful if the algorithm itself has to change.
        /// </summary>
        private const string SEED_VERSION = "luck-v1";

        /// <summary>
        /// Returns the local player's luck value for today.
        /// </summary>
        public static int GetDailyLuckValue() => GetDailyLuckValue(DateTime.Today);

        /// <summary>
        /// Returns the local player's luck value for the given date.
        /// </summary>
        /// <param name="date">The date to get the value for. Only the date part is used.</param>
        /// <returns>A value between 0 and <see cref="MAX_VALUE"/>.</returns>
        public static int GetDailyLuckValue(DateTime date)
        {
            string seedKey = SEED_VERSION + "|" +
                date.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + "|" +
                GetPlayerNameKey() + "|" +
                GetMachineIdKey();

            byte[] hash;

            // SHA256.HashData is not available on .NET Framework.
            using (SHA256 sha256 = SHA256.Create())
                hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(seedKey));

            ulong seed = BinaryPrimitives.ReadUInt64LittleEndian(hash);

            // splitmix64 finalizer. It gives a well-distributed result that does not
            // depend on the runtime version (unlike Random or GetHashCode).
            ulong value = seed + 0x9E3779B97F4A7C15UL;
            value = (value ^ (value >> 30)) * 0xBF58476D1CE4E5B9UL;
            value = (value ^ (value >> 27)) * 0x94D049BB133111EBUL;
            value ^= value >> 31;

            return (int)(value % (MAX_VALUE + 1));
        }

        /// <summary>
        /// Returns the player name in a normalized, case-insensitive form.
        /// </summary>
        private static string GetPlayerNameKey()
        {
            string playerName = ProgramConstants.PLAYERNAME;

            // The client uses this placeholder until the player name has been loaded.
            if (string.IsNullOrWhiteSpace(playerName) || playerName == "No name")
                playerName = UserINISettings.Instance.PlayerName.Value;

            return NormalizeKey(playerName);
        }

        /// <summary>
        /// Returns the machine identifier, falling back to a persistent
        /// per-installation identifier where the Windows machine GUID is unavailable.
        /// </summary>
        private static string GetMachineIdKey()
        {
            string machineGuid = TryGetWindowsMachineGuid();

            if (!string.IsNullOrWhiteSpace(machineGuid))
                return NormalizeKey(machineGuid);

            return GetOrCreateInstallationId();
        }

        /// <summary>
        /// Reads the Windows machine GUID, which is shared by all users of a computer.
        /// </summary>
        /// <returns>The machine GUID, or null if it is unavailable.</returns>
        private static string TryGetWindowsMachineGuid()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return null;

            try
            {
                using (RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (RegistryKey cryptographyKey = localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography"))
                {
                    return cryptographyKey?.GetValue("MachineGuid") as string;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Failed to read the Windows machine GUID: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Returns the persistent per-installation identifier, generating and saving it
        /// on first use.
        /// </summary>
        private static string GetOrCreateInstallationId()
        {
            string installationId = UserINISettings.Instance.InstallationId.Value;

            if (!string.IsNullOrWhiteSpace(installationId))
                return NormalizeKey(installationId);

            installationId = Guid.NewGuid().ToString("N").ToUpperInvariant();
            UserINISettings.Instance.InstallationId.Value = installationId;

            try
            {
                UserINISettings.Instance.SaveSettings();
            }
            catch (Exception ex)
            {
                Logger.Log("Failed to save the installation identifier: " + ex.Message);
            }

            return installationId;
        }

        private static string NormalizeKey(string value) =>
            (value ?? string.Empty).Trim().Normalize(NormalizationForm.FormC).ToUpperInvariant();
    }
}
