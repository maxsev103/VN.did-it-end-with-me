using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_Audio : CMDDatabaseExtension
    {
        private static string[] PARAM_SFX = new string[] { "-s", "-sfx" };
        private static string[] PARAM_VOLUME = new string[] { "-v", "-volume" };
        private static string[] PARAM_LOOP = new string[] { "-lp", "-loop" };

        private static string[] PARAM_CHANNEL = new string[] { "-c", "-channel" };
        private static string[] PARAM_IMMEDIATE = new string[] { "-i", "-immediate" };
        private static string[] PARAM_START_VOLUME = new string[] { "-sv", "-startvolume" };
        private static string[] PARAM_SONG = new string[] { "-sn", "-song" };
        private static string[] PARAM_AMBIENCE = new string[] { "-a", "-ambience" };

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("playsfx", new Action<string[]>(PlaySFX));
            database.AddCommand("stopsfx", new Action<string>(StopSFX));

            database.AddCommand("playvoice", new Action<string[]>(PlayVoice));
            database.AddCommand("stopvoice", new Action<string>(StopVoice));

            database.AddCommand("playsong", new Action<string[]>(PlaySong));
            database.AddCommand("playambience", new Action<string[]>(PlayAmbience));

            database.AddCommand("stopsong", new Action<string>(StopSong));
            database.AddCommand("stopambience", new Action<string>(StopAmbience));
        }

        private static void PlaySFX(string[] data)
        {
            string filePath;
            float volume;
            bool loop;

            var parameters = ConvertDataToParameters(data);

            // try to get the name or path of the sfx
            parameters.TryGetValue(PARAM_SFX, out filePath);

            // try to get the volume
            parameters.TryGetValue(PARAM_VOLUME, out volume, defaultValue: 0.7f);

            // try to get if the sfx will loop or not
            parameters.TryGetValue(PARAM_LOOP, out loop, defaultValue: false);

            string resourcesPath = FilePaths.GetPathToResources(FilePaths.resources_sfx, filePath);
            AudioClip sound = Resources.Load<AudioClip>(resourcesPath);

            if (sound == null)
                return;

            AudioManager.instance.PlaySoundEffect(sound, filePath: resourcesPath, volume: volume, loop: loop);
        }

        private static void StopSFX(string data)
        {
            AudioManager.instance.StopSoundEffect(data);
        }

        private static void PlayVoice(string[] data)
        {
            string filePath;
            float volume;
            bool loop;

            var parameters = ConvertDataToParameters(data);

            // try to get the name or path of the sfx
            parameters.TryGetValue(PARAM_SFX, out filePath);

            // try to get the volume
            parameters.TryGetValue(PARAM_VOLUME, out volume, defaultValue: 0.7f);

            // try to get if the sfx will loop or not
            parameters.TryGetValue(PARAM_LOOP, out loop, defaultValue: false);

            string resourcesPath = FilePaths.GetPathToResources(FilePaths.resources_voice, filePath);
            AudioClip sound = Resources.Load<AudioClip>(resourcesPath);

            if (sound == null) {
                Debug.LogError($"Was not able to load voice '{filePath}'");
                return;
            }

            AudioManager.instance.PlayVoice(sound, filePath: resourcesPath, volume: volume, loop: loop);

        }

        private static void StopVoice(string data) => StopSFX(data);

        private static void PlaySong(string[] data)
        {
            string filePath;
            int channel;

            var parameters = ConvertDataToParameters(data);

            // try to get the name or path to the track
            parameters.TryGetValue(PARAM_SONG, out filePath);
            filePath = FilePaths.GetPathToResources(FilePaths.resources_music, filePath);

            // try to get the channel
            parameters.TryGetValue(PARAM_CHANNEL, out channel, defaultValue: 1);

            PlayTrack(filePath, channel, parameters);
        }

        private static void PlayAmbience(string[] data)
        {
            string filePath;
            int channel;

            var parameters = ConvertDataToParameters(data);

            // try to get the name or path to the track
            parameters.TryGetValue(PARAM_AMBIENCE, out filePath);
            filePath = FilePaths.GetPathToResources(FilePaths.resources_ambience, filePath);

            // try to get the channel
            parameters.TryGetValue(PARAM_CHANNEL, out channel, defaultValue: 0);

            PlayTrack(filePath, channel, parameters);
        }

        private static void PlayTrack(string filePath, int channel, CommandParameters parameters)
        {
            bool loop;
            float volumeCap, startVolume;

            // try to get the volume cap of the track
            parameters.TryGetValue(PARAM_VOLUME, out volumeCap, defaultValue: 0.8f);

            // try to get the start volume of the track
            parameters.TryGetValue(PARAM_START_VOLUME, out startVolume, defaultValue: 0f);

            // try to get if this track loops or not
            parameters.TryGetValue(PARAM_LOOP, out loop, defaultValue: true);

            AudioClip sound = Resources.Load<AudioClip>(filePath);

            if (sound == null)
            {
                Debug.LogError($"Was not able to load track '{filePath.Substring(FilePaths.resources_audio.Length)}'");
                return;
            }

            AudioManager.instance.PlayTrack(filePath, channel, loop, startVolume, volumeCap);
        }

        private static void StopSong(string data)
        {
            if (data == string.Empty)
                StopTrack("1");
            else
                StopTrack(data);
        }

        private static void StopAmbience(string data)
        {
            if (data == string.Empty)
                StopTrack("0");
            else
                StopTrack(data);
        }

        private static void StopTrack(string data)
        {
            if (int.TryParse(data, out int channel))
                AudioManager.instance.StopTrack(channel);
            else
                AudioManager.instance.StopTrack(data);
        }
    }
}