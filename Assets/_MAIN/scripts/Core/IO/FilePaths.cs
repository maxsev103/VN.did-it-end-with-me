using UnityEngine;

public class FilePaths
{
    private const string HOME_DIRECTORY_SYMBOL = "~/";

    public static readonly string root = $"{Application.dataPath}/gameData/";

    //Runtime Paths
    public static readonly string gameSaves = $"{runtimePath}Save Files/";

    // Resources Paths
    public static readonly string resources_fonts = "Fonts/";

    public static readonly string resources_graphics = "Graphics/";
    public static readonly string resources_bgImages = $"{resources_graphics}BG Images/";
    public static readonly string resources_bgVideos = $"{resources_graphics}BG Videos/";
    public static readonly string resources_blendTextures = $"{resources_graphics}Transition Effects/";
    public static readonly string resources_uiElements = $"{resources_graphics}UI Elements/";
    public static readonly string resources_gallery = $"{resources_graphics}Gallery/";

    public static readonly string resources_audio = "Audio/";
    public static readonly string resources_sfx = $"{resources_audio}SFX/";
    public static readonly string resources_voice = $"{resources_audio}Voice/";
    public static readonly string resources_music = $"{resources_audio}Music/";
    public static readonly string resources_ambience = $"{resources_audio}Ambience/";

    public static readonly string resources_dialogueFiles = "Dialogue Files/";


    public static string GetPathToResources(string defaultPath, string resourceName)
    {
        if (resourceName.StartsWith(HOME_DIRECTORY_SYMBOL))
            return resourceName.Substring(HOME_DIRECTORY_SYMBOL.Length);

        return defaultPath + resourceName;
    }

    public static string runtimePath
    {
        get
        {
            #if UNITY_EDITOR
                return "Assets/appdata/";
            #else
                return Application.persistentDataPath + "/appdata";
            #endif
        }
    }
}
