using System.Collections;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    private FinalScore finalscore;
    [SerializeField] CarPool carPool;
    private SpriteRenderer player;
    [SerializeField] float lerpDuration = 1f;
    [SerializeField] SpriteRenderer[] spritesBuildings;

    [Header("Red Assets")]
    [SerializeField] GameObject redPlanet;
    [SerializeField] Animator animationRedPlanet;

    [Header("Blue Assets")]
    [SerializeField] GameObject bluePlanet;
    [SerializeField] Animator animationBluePlanet;

    [Header("Yellow Assets")]
    [SerializeField] GameObject yellowPlanet;
    [SerializeField] Animator animationYellowPlanet;

    [Header("BlackHole Assets")]
    [SerializeField] GameObject blackHolePlanet;
    [SerializeField] Animator animationBlackHolePlanet;

    [Header("Red Colors")]
    public bool hasStartedLerpBackgroundRed = false;
    [SerializeField] string[] hexColorRedBuildings; 

    [Header("Blue Colors")]
    public bool hasStartedLerpBackgroundBlue = false;
    [SerializeField] string[] hexColorBlueBuildings;

    [Header("Yellow Colors")]
    public bool hasStartedLerpBackgroundYellow = false;
    [SerializeField] string[] hexColorYellowBuildings;

    [Header("BlackHole Colors")]
    public bool hasStartedLerpBackgroundBlackHole = false;
    [SerializeField] string[] hexColorBlackHoleBuildings;

    [Header("Normal Colors")]
    [SerializeField] SpriteRenderer spritePlayerDestruction;
    public bool hasStartedLerpBackgroundNormal = false;
    [SerializeField] string[] hexColorNormalBuildings;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        finalscore = FindFirstObjectByType<FinalScore>();
        player = FindFirstObjectByType<PlayerControl>().GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (finalscore.scoreInt == 5)
        {
            if (finalscore.isBackgroundRed && !hasStartedLerpBackgroundRed)
            {
                hasStartedLerpBackgroundRed = true;
                redPlanet.SetActive(true);
                animationRedPlanet.SetBool("isMovement", true);

                // Background color: #BF4242
                // Rest of Sprites: #FFAFAF
                for (int i = 0; i < spritesBuildings.Length; i++)
                {
                    StartCoroutine(ChangeColorLerp(spritesBuildings[i], hexColorRedBuildings[i], lerpDuration));
                }

                for (int i = 0; i < carPool.carListSprite.Count; i++)
                {
                    for (int j = 1; j < hexColorRedBuildings.Length; j++)
                    {
                        StartCoroutine(ChangeColorLerp(carPool.carListSprite[i], hexColorRedBuildings[j], lerpDuration));
                    }  
                }
            }
        }
        else if (finalscore.scoreInt == 10)
        {
            if (finalscore.isBackgroundBlue && !hasStartedLerpBackgroundBlue)
            {
                hasStartedLerpBackgroundBlue = true;
                bluePlanet.SetActive(true);
                animationBluePlanet.SetBool("isMovement", true);

                // Background color: #42A9BF
                // Rest of Sprites: #78ACFF - Left/Right: #97C3FF
                for (int i = 0; i < spritesBuildings.Length; i++)
                {
                    StartCoroutine(ChangeColorLerp(spritesBuildings[i], hexColorBlueBuildings[i], lerpDuration));
                }

                for (int i = 0; i < carPool.carListSprite.Count; i++)
                {
                    for (int j = 3; j < hexColorBlueBuildings.Length; j++)
                    {
                        StartCoroutine(ChangeColorLerp(carPool.carListSprite[i], hexColorBlueBuildings[j], lerpDuration));
                    }
                }
            }
        }
        else if (finalscore.scoreInt == 15)
        {
            if (finalscore.isBackgroundYellow && !hasStartedLerpBackgroundYellow)
            {
                hasStartedLerpBackgroundYellow = true;
                yellowPlanet.SetActive(true);
                animationYellowPlanet.SetBool("isMovement", true);

                // Background color: #FFEE04
                // Rest of Sprites: #FFE758 - Left/Right: #FFED83
                for (int i = 0; i < spritesBuildings.Length; i++)
                {
                    StartCoroutine(ChangeColorLerp(spritesBuildings[i], hexColorYellowBuildings[i], lerpDuration));
                }

                for (int i = 0; i < carPool.carListSprite.Count; i++)
                {
                    for (int j = 3; j < hexColorYellowBuildings.Length; j++)
                    {
                        StartCoroutine(ChangeColorLerp(carPool.carListSprite[i], hexColorYellowBuildings[j], lerpDuration));
                    }
                }
            }
        }
        else if (finalscore.scoreInt == 20)
        {
            if (finalscore.isBackgroundBlackHole && !hasStartedLerpBackgroundBlackHole)
            {
                hasStartedLerpBackgroundBlackHole = true;
                blackHolePlanet.SetActive(true);
                animationBlackHolePlanet.SetBool("isMovement", true);
                player.color = Color.black;

                // Background color: #FF0000
                // Left/Right: #000000
                // Clouds: #9A0000
                // BackgroundBuilding: #FF0000
                // Ships: #000000
                // Moon: #FF0000
                for (int i = 0; i < spritesBuildings.Length; i++)
                {
                    StartCoroutine(ChangeColorLerp(spritesBuildings[i], hexColorBlackHoleBuildings[i], lerpDuration));
                }

                for (int i = 0; i < carPool.carListSprite.Count; i++)
                {
                    for (int j = 10; j < hexColorBlackHoleBuildings.Length; j++)
                    {
                        StartCoroutine(ChangeColorLerp(carPool.carListSprite[i], hexColorBlackHoleBuildings[j], lerpDuration));
                    }
                }
            }
        }
        else if (finalscore.scoreInt == 25)
        {
            if (finalscore.isBackgroundNormal && !hasStartedLerpBackgroundNormal)
            {
                hasStartedLerpBackgroundNormal = true;
                player.color = Color.white;
                spritePlayerDestruction.color = Color.white;
                // Normal color: #FFFFFF

                for (int i = 0; i < spritesBuildings.Length; i++)
                {
                    StartCoroutine(ChangeColorLerp(spritesBuildings[i], hexColorNormalBuildings[i], lerpDuration));
                }

                for (int i = 0; i < carPool.carListSprite.Count; i++)
                {
                    for (int j = 0; j < hexColorNormalBuildings.Length; j++)
                    {
                        StartCoroutine(ChangeColorLerp(carPool.carListSprite[i], hexColorNormalBuildings[j], lerpDuration));
                    }
                }
            }
        }
    }

    IEnumerator ChangeColorLerp(SpriteRenderer sprite, string hexColor, float duration)
    {
        Color targetColor;

        if (!ColorUtility.TryParseHtmlString(hexColor, out targetColor))
            yield break;

        Color startColor = sprite.color;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            sprite.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        sprite.color = targetColor; // asegurar color final exacto
    }

}
