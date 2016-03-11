using UnityEngine;
using System.Collections;

public class MoleScript : MonoBehaviour {

    public tk2dClippedSprite sprite;    //khai báo đối tượng mole với kiểu là tk2dClippedSprite

    private float height;
    private float speed;
    private float timeLimit;
    private Rect spriteRec;
    private bool whacked;
    private float transformY;

    //âm thanh hiệu ứng của mole
    public AudioClip moleUp;    
    public AudioClip moleDown;

    private Transform colliderTransform;

    public Transform ColliderTransform
    {
        get
        {
            return colliderTransform;
        }
    }



    public void Trigger(float tl)
    {
        sprite.gameObject.SetActive(true);
        whacked = false;
        sprite.SetSprite("Mole_Normal");
        timeLimit = tl;
        StartCoroutine(MainLoop());
    }

    void Start()
    {
        timeLimit = 1.0f;
        speed = 2.0f;


        Bounds bounds = sprite.GetUntrimmedBounds();
        height = bounds.max.y - bounds.min.y;


        spriteRec = sprite.ClipRect;
        spriteRec.y = 1.0f;
        sprite.ClipRect = spriteRec;

        colliderTransform = sprite.transform;

        Vector3 localPos = sprite.transform.localPosition;
        transformY = localPos.y;
        localPos.y = transformY - (height * sprite.ClipRect.y);
        sprite.transform.localPosition = localPos;

        sprite.gameObject.SetActive(false);

        MainGameScript.Instance.RegisterMole(this);
    }

    private IEnumerator MainLoop()
    {
        yield return StartCoroutine(MoveUp());
        yield return StartCoroutine(WaitForHit());
        yield return StartCoroutine(MoveDown());
    }


    private IEnumerator MoveUp()
    {
        AudioSource.PlayClipAtPoint(moleUp, new Vector3());
        while (spriteRec.y > 0.0f)
        {
            spriteRec = sprite.ClipRect;
            float newYPos = spriteRec.y - speed * Time.deltaTime;
            spriteRec.y = newYPos < 0.0f ? 0.0f : newYPos;
            sprite.ClipRect = spriteRec;

            Vector3 localPos = sprite.transform.localPosition;
            localPos.y = transformY - (height * sprite.ClipRect.y);
            sprite.transform.localPosition = localPos;

            yield return null;
        }
    }

    private IEnumerator WaitForHit()
    {
        float time = 0.0f;

        while (!whacked && time < timeLimit)
        {
            time += Time.deltaTime;
            yield return null;
        }
    }


    private IEnumerator MoveDown()
    {
        AudioSource.PlayClipAtPoint(moleDown, new Vector3());
        while (spriteRec.y < 1.0f)
        {
            spriteRec = sprite.ClipRect;
            float newYPos = spriteRec.y + speed * Time.deltaTime;
            spriteRec.y = newYPos > 1.0f ? 1.0f : newYPos;
            sprite.ClipRect = spriteRec;

            Vector3 localPos = sprite.transform.localPosition;
            localPos.y = transformY - (height * sprite.ClipRect.y);
            sprite.transform.localPosition = localPos;

            yield return null;
        }

        sprite.gameObject.SetActive(false);
    }


    public void Whack()
    {
        whacked = true;
        sprite.SetSprite("Mole_Hit");
    }

    public bool Whacked
    {
        get
        {
            return whacked;
        }
    }
}
