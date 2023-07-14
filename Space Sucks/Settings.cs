using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class Settings : MonoBehaviour
{

    public static ControlScheme Controls { get; set; }
    public static GraphicsSettings Graphics { get; set; }
    public static AudioSettings Audio { get; set; }

    public static void LoadSettings()
    {

        string path = Application.persistentDataPath + "/Setting.sav";
        if (File.Exists(path))
        {

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerProfile playerProfile = formatter.Deserialize(stream) as PlayerProfile;
            stream.Close();

            Debug.Log(playerProfile.mainVol);

            Controls = new ControlScheme()
            {

                moveForward = playerProfile.moveForward,
                moveRight = playerProfile.moveRight,
                moveLeft = playerProfile.moveLeft,
                moveBack = playerProfile.moveBack,
                jump = playerProfile.jump,
                sprint = playerProfile.sprint,
                toggleSprint = playerProfile.toggleSprint,
                crouchH = playerProfile.crouchH,
                crouchT = playerProfile.crouchT,
                reload = playerProfile.reload,
                primaryWeapon = playerProfile.primaryWeapon,
                secondaryWeapon = playerProfile.secondaryWeapon,
                interact = playerProfile.interact,
                sensitivity = 1f

            };

            Graphics = new GraphicsSettings()
            {
                resolutionWidth = playerProfile.resWidth,
                resolutionHeight = playerProfile.resHeight,
                qualityLevel = playerProfile.qualityLevel,
                fov = playerProfile.fov,
                fullScreen = playerProfile.fullScreen,

            };

            Audio = new AudioSettings()
            {

                mainVol = playerProfile.mainVol,
                musicVol = playerProfile.musicVol,
                effectVol = playerProfile.effectVol,

            };

        }
        else
        {

            Controls = new ControlScheme()
            {
                moveForward = KeyCode.W,
                moveRight = KeyCode.D,
                moveLeft = KeyCode.A,
                moveBack = KeyCode.S,
                jump = KeyCode.Space,
                sprint = KeyCode.None,
                toggleSprint = KeyCode.LeftShift,
                crouchH = KeyCode.C,
                crouchT = KeyCode.LeftControl,
                interact = KeyCode.E,
                sensitivity = 1f,
                reload = KeyCode.R,
                primaryWeapon = KeyCode.Alpha1,
                secondaryWeapon = KeyCode.Alpha2
            };

            Graphics = new GraphicsSettings()
            {
                resolutionWidth = 1980,
                resolutionHeight = 1080,
                qualityLevel = 5,
                fov = 90f,
                fullScreen = true

            };

            Audio = new AudioSettings()
            {

                mainVol = 1,
                musicVol = 1,
                effectVol = 1
            };
        }
    }

    // Save all settings
    public static void SaveSettings(OptionMenu option)
    {

        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Setting.sav";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerProfile playerProfile = new PlayerProfile(option);

        formatter.Serialize(stream, playerProfile);
        stream.Close();
    }

}

public struct GraphicsSettings
{
    public int resolutionWidth;
    public int resolutionHeight;
    public int qualityLevel;
    public float fov;
    public bool fullScreen;


}

public struct AudioSettings
{
    public float mainVol;
    public float musicVol;
    public float effectVol;

}

public struct ControlScheme
{

    #region Movement
    public KeyCode moveForward;
    public KeyCode moveRight;
    public KeyCode moveLeft;
    public KeyCode moveBack;
    public KeyCode jump;
    public KeyCode sprint;
    public KeyCode toggleSprint;
    public KeyCode crouchH;
    public KeyCode crouchT;
    #endregion

    #region Gunplay
    public KeyCode reload;
    public KeyCode primaryWeapon;
    public KeyCode secondaryWeapon;
    #endregion

    #region Other
    public KeyCode interact;
    public float sensitivity;
    #endregion

}

