using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainGameScript : MonoBehaviour {

    private List<MoleScript> moles = new List<MoleScript>();
    private bool gameEnd;
    private int score;
    private int timeLimitMS;
    private int moleLimit;

    public Camera gameCam;
    public tk2dSpriteAnimator dustAnimator;

    public AudioClip moleHit;       //âm thanh khi đập chuột

    private static MainGameScript instance;

    public static MainGameScript Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("MainGameScript instance does not exist");
            }
            return instance;
        }
    }

    void Awake()
    {
        instance = this;
    }

    IEnumerator Start()
    {
        gameEnd = false;
        timeLimitMS = 3000;
        score = 0;
        moleLimit = 3;

        yield return 0;  

        dustAnimator.gameObject.SetActive(false);
        StartCoroutine(MainGameLoop());
    }

    void Update()
    {
        #region đập chuột = chuột trái
        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = gameCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(ray, out hit))
            {
                foreach (MoleScript mole in moles)
                {
                    if (mole.sprite.gameObject.activeSelf && mole.ColliderTransform == hit.transform)
                    {
                        //play âm thanh đập
                        AudioSource.PlayClipAtPoint(moleHit, new Vector3());
                        //tăng điểm
                        ScoreScript.Score += mole.Whacked ? 0 : 100;
                        mole.Whack();
                        StartCoroutine(CallAnim(mole));
                    }
                }
            }
        }
        #endregion
    }

    private IEnumerator MainGameLoop()
    {
        float hitTimeLimit = 1.0f;
        int randomMole;

        while (!gameEnd)
        {
            yield return StartCoroutine(OkToTrigger());
            yield return new WaitForSeconds((float)Random.Range(1, timeLimitMS) / 1000.0f);

            int availableMoles = 0;
            for (int i = 0; i < moles.Count; ++i)
            {
                if (!moles[i].sprite.gameObject.activeSelf)
                {
                    availableMoles++;
                }
            }

            if (availableMoles > 0)
            {
                randomMole = (int)Random.Range(0, moles.Count);
                while (moles[randomMole].sprite.gameObject.activeSelf)
                {
                    randomMole = (int)Random.Range(0, moles.Count);
                }

                moles[randomMole].Trigger(hitTimeLimit);
                hitTimeLimit -= hitTimeLimit <= 0.0f ? 0.0f : 0.01f;
            }

            yield return null;
        }
    }

    public void RegisterMole(MoleScript who)
    {
        moles.Add(who);
    }

    private IEnumerator OkToTrigger()
    {
        int molesActive;

        do
        {
            yield return null;
            molesActive = 0;

            foreach (MoleScript mole in moles)
            {
                molesActive += mole.sprite.gameObject.activeSelf ? 1 : 0;
            }
        }
        while (molesActive >= moleLimit);

        yield break;
    }

    private IEnumerator CallAnim(MoleScript mole)
    {
        yield return new WaitForSeconds(0.25f);

        tk2dSpriteAnimator newAnimator;

        newAnimator = Instantiate(dustAnimator, new Vector3(
            mole.transform.position.x, mole.transform.position.y,
            dustAnimator.transform.position.z), 
            dustAnimator.transform.rotation) as tk2dSpriteAnimator;

        newAnimator.gameObject.SetActive(true);
        newAnimator.Play("DustCloud");

        while (newAnimator.IsPlaying("DustCloud"))
        {
            yield return null;
        }

        Destroy(newAnimator.gameObject);
    }
}




