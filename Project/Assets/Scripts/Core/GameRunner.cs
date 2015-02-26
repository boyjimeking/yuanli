using UnityEngine;
using System.Collections;

public class GameRunner : SingletonComponent<GameRunner>
{

    private const float FRAME_TIME = 1f / Constants.LOGIC_FPS;
    private int m_GameSpeed = 1;
    private int oldGameSpeed = 1;
    private bool paused;

    public int GameSpeed
    {
        get { return m_GameSpeed; }
        set
        {
            m_GameSpeed = value;
            //Time.timeScale = m_GameSpeed;
        }
    }

    public bool Paused
    {
        get { return paused; }
        set
        {
            paused = value;
            if (paused)
            {
                oldGameSpeed = GameSpeed;
                GameSpeed = 0;
            }
            else
            {
                GameSpeed = oldGameSpeed;
            }
        }
    }

    void Awake()
    {
        Time.fixedDeltaTime = FRAME_TIME;
    }

    void Start()
    {
        GameWorld.Instance.Init();
    }

    void FixedUpdate()
    {
        for (int i = 0; i < GameSpeed; i++)
        {
            GameWorld.Instance.Update(FRAME_TIME);
        }
    }

#if UNITY_EDITOR
    void OnGUI()
    {
        if (GameWorld.Instance.Loading)
            return;
        if (GameWorld.Instance.worldType == WorldType.Home)
        {
            if (GUI.Button(new Rect(120, 500, 50, 50), "回放"))
            {
                GameWorld.Instance.ChangeLoading(WorldType.Replay);
                return;
            }
        }
    }
#endif  //  UNITY_EDITOR
}
