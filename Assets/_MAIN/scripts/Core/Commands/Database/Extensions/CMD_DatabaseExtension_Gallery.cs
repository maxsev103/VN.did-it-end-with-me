using System;
using System.Collections;
using UnityEngine;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_Gallery : CMDDatabaseExtension
    {
        private static string[] PARAM_MEDIA = new string[] { "-m", "-media" };
        private static string[] PARAM_SPEED = new string[] { "-spd", "-speed" };
        private static string[] PARAM_IMMEDIATE = new string[] { "-i", "-immediate" };
        private static string[] PARAM_BLENDTEX = new string[] { "-b", "-blend" };

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("showgalleryimage", new Func<string[], IEnumerator>(ShowGalleryImage));
            database.AddCommand("hidegalleryimage", new Func<string[], IEnumerator>(HideGalleryImage));
        }

        public static IEnumerator HideGalleryImage(string[] data)
        {
            GraphicLayer graphicLayer = GraphicPanelManager.instance.GetPanel("cg").GetLayer(0, createIfNotExisting: true);

            if (graphicLayer.currentGraphic == null)
                yield break;

            float transitionSpeed = 1f;
            bool immediate = false;
            string blendTexName = "";
            Texture blendTex = null;

            var parameters = ConvertDataToParameters(data);

            // try to get if this is an immediate effect or not
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            //try to get the speed of the transition if it is not an immediate effect
            if (!immediate)
                parameters.TryGetValue(PARAM_SPEED, out transitionSpeed, defaultValue: 1f);

            // try to get the blending texture for the media if provided
            parameters.TryGetValue(PARAM_BLENDTEX, out blendTexName);
            if (!immediate && blendTexName != string.Empty)
                blendTex = Resources.Load<Texture>(FilePaths.resources_blendTextures + blendTexName);

            if (!immediate)
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { graphicLayer.Clear(immediate: true); });

            graphicLayer.Clear(transitionSpeed, blendTex, immediate);

            if (graphicLayer.currentGraphic != null)
            {
                var graphicObject = graphicLayer.currentGraphic;

                yield return new WaitUntil(() => graphicObject == null);
            }
        }

        public static IEnumerator ShowGalleryImage(string[] data)
        {
            string mediaName = "";
            float transitionSpeed = 1f;
            bool immediate = false;
            string blendTexName = "";
            Texture blendTex = null;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_MEDIA, out mediaName);

            // try to get if this is an immediate effect or not
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            //try to get the speed of the transition if it is not an immediate effect
            if (!immediate)
                parameters.TryGetValue(PARAM_SPEED, out transitionSpeed, defaultValue: 1f);

            // try to get the blending texture for the media if provided
            parameters.TryGetValue(PARAM_BLENDTEX, out blendTexName);

            string pathToGraphic = FilePaths.resources_gallery + mediaName;
            Texture graphic = Resources.Load<Texture>(pathToGraphic);

            if (graphic == null)
            {
                Debug.LogError($"Could not find gallery image called '{mediaName}' in the Resources '{FilePaths.resources_gallery}' directory.");
                yield break;
            }

            if (!immediate && blendTexName != string.Empty)
                blendTex = Resources.Load<Texture>(FilePaths.resources_blendTextures + blendTexName);

            GraphicLayer graphicLayer = GraphicPanelManager.instance.GetPanel("cg").GetLayer(0, createIfNotExisting: true);

            if (!immediate)
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { graphicLayer?.SetTexture(graphic, filePath: pathToGraphic, immediate: true); });

            GalleryConfig.UnlockImage(mediaName);

            yield return graphicLayer.SetTexture(graphic, transitionSpeed, blendTex, pathToGraphic, immediate);
        }
    }
}