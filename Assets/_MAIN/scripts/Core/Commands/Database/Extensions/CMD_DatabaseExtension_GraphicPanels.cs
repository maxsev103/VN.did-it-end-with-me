using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_GraphicPanels : CMDDatabaseExtension
    {
        private static string[] PARAM_PANEL = new string[] { "-p", "-panel" };
        private static string[] PARAM_LAYER = new string[] { "-l", "-layer" };
        private static string[] PARAM_MEDIA = new string[] { "-m", "-media" };
        private static string[] PARAM_SPEED = new string[] { "-spd", "-speed" };
        private static string[] PARAM_IMMEDIATE = new string[] { "-i", "-immediate" };
        private static string[] PARAM_BLENDTEX = new string[] { "-b", "-blend" };
        private static string[] PARAM_USEVIDEOAUDIO = new string[] { "-aud", "-audio" };

        private const string HOME_DIRECTORY_SYMBOL = "~/";

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("setlayermedia", new Func<string[], IEnumerator>(SetLayerMedia));
            database.AddCommand("clearlayermedia", new Func<string[], IEnumerator>(ClearLayerMedia));
        }
        
        private static IEnumerator SetLayerMedia(string[] data)
        {
            string panelName = "";
            int layer = 0;
            string mediaName = "";
            float transitionSpeed = 0;
            bool immediate = false;
            string blendTexName = "";
            bool useAudio = false;

            string pathToMedia = "";

            UnityEngine.Object media = null;
            Texture blendtex = null;

            var parameters = ConvertDataToParameters(data);

            // try to get the panel
            parameters.TryGetValue(PARAM_PANEL, out panelName);
            GraphicPanel panel = GraphicPanelManager.instance.GetPanel(panelName);

            if (panel == null)
            {
                Debug.LogError($"Unable to grab panel '{panelName}' because it is not a valid panel. Please check the panel name and adjust the command.");
                yield break;
            }

            // try to get the layer to apply the graphic
            parameters.TryGetValue(PARAM_LAYER, out layer, defaultValue: 0);

            // try to get the media
            parameters.TryGetValue(PARAM_MEDIA, out mediaName);

            // try to get if this is an immediate effect or not
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            //try to get the speed of the transition if it is not an immediate effect
            if (!immediate)
                parameters.TryGetValue(PARAM_SPEED, out transitionSpeed, defaultValue: 1f);

            // try to get the blending texture for the media if provided
            parameters.TryGetValue(PARAM_BLENDTEX, out blendTexName);

            // if this is a video, try to get whether we use audio from the video or not
            parameters.TryGetValue(PARAM_USEVIDEOAUDIO, out useAudio, defaultValue: false);

            // run the logic to display this media
            pathToMedia = FilePaths.GetPathToResources(FilePaths.resources_bgImages, mediaName);

            // try to get the media as an image
            media = Resources.Load<Texture>(pathToMedia);

            if (media == null)
            {
                // if no image was found with that name, check if it is a video
                pathToMedia = FilePaths.GetPathToResources(FilePaths.resources_bgVideos, mediaName);
                media = Resources.Load<VideoClip>(pathToMedia);
            }

            if (media == null)
            {
                Debug.LogError($"Could not find media file called '{mediaName}' in the Resources directories. Please specify the full path within resources and make sure that the file exists!");
                yield break;
            }

            if (!immediate && blendTexName != string.Empty) {
                blendtex = Resources.Load<Texture>(FilePaths.resources_blendTextures + blendTexName);
            }

            // try to get the layer to apply the media to
            GraphicLayer graphicLayer = panel.GetLayer(layer, createIfNotExisting: true);

            if (media is Texture)
            {
                if (!immediate)
                    CommandManager.instance.AddTerminationActionToCurrentProcess(() => { graphicLayer?.SetTexture(media as Texture, filePath: pathToMedia, immediate: true); });

                yield return graphicLayer.SetTexture(media as Texture, transitionSpeed, blendtex, pathToMedia, immediate);
            }
            else
            {
                if (!immediate)
                    CommandManager.instance.AddTerminationActionToCurrentProcess(() => { graphicLayer?.SetVideo(media as VideoClip, filePath: pathToMedia, immediate: true); });

                yield return graphicLayer.SetVideo(media as VideoClip, transitionSpeed, useAudio, blendtex, pathToMedia, immediate);
            }
        }
        
        private static IEnumerator ClearLayerMedia(string[] data)
        {
            string panelName = "";
            int layer = -1;
            string mediaName = "";
            float transitionSpeed = 0;
            bool immediate = false;
            string blendTexName = "";

            Texture blendTex = null;

            var parameters = ConvertDataToParameters(data);

            // try to get the panel
            parameters.TryGetValue(PARAM_PANEL, out panelName);
            GraphicPanel panel = GraphicPanelManager.instance.GetPanel(panelName);

            if (panel == null)
            {
                Debug.LogError($"Unable to grab panel '{panelName}' because it is not a valid panel. Please check the panel name and adjust the command.");
                yield break;
            }

            // try to get the layer to apply the graphic
            parameters.TryGetValue(PARAM_LAYER, out layer, defaultValue: 0);

            // try to get the media
            parameters.TryGetValue(PARAM_MEDIA, out mediaName);

            // try to get if this is an immediate effect or not
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            //try to get the speed of the transition if it is not an immediate effect
            if (!immediate)
                parameters.TryGetValue(PARAM_SPEED, out transitionSpeed, defaultValue: 1f);

            // try to get the blending texture for the media if provided
            parameters.TryGetValue(PARAM_BLENDTEX, out blendTexName);

            if (!immediate && blendTexName != string.Empty)
                blendTex = Resources.Load<Texture>(FilePaths.resources_blendTextures + blendTexName);

            if (layer == -1)
            {
                if (!immediate)
                    CommandManager.instance.AddTerminationActionToCurrentProcess(() => { panel.Clear(transitionSpeed, blendTex, immediate: true); });

                panel.Clear(transitionSpeed, blendTex, immediate);
                    
            }  
            else
            {
                GraphicLayer graphicLayer = panel.GetLayer(layer);

                if (graphicLayer == null)
                {
                    Debug.LogError($"Could not clear layer [{layer}] on panel '{panel.panelName}'");
                    yield break;
                }

                if (!immediate)
                    CommandManager.instance.AddTerminationActionToCurrentProcess(() => { graphicLayer.Clear(transitionSpeed, blendTex, immediate: true); });

                graphicLayer.Clear(transitionSpeed, blendTex, immediate);
            }
        }
    }
}