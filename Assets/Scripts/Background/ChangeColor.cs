using System.Collections;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    private FinalScore finalscore;
    public SpriteRenderer spriteBackground, spriteEdificioGrande, spriteLuna, spriteNave;
    public SpriteRenderer[] spriteEdficioLargo, spriteNubes;
    [SerializeField] GameObject redPlanet;
    [SerializeField] Animator animationRedPlanet;

    [SerializeField] CarPool carPool;

    [Header("Red Settings")]
    public bool hasStartedLerpBackgroundRed = false;
    [SerializeField] string hexColorRedBackground = "#BF4242";
    [SerializeField] string hexColorRedBuildings = "#FFAFAF";
    [SerializeField] float lerpDuration = 1f;

    [Header("Blue Settings")]
    public bool hasStartedLerpBackgroundBlue = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        finalscore = FindFirstObjectByType<FinalScore>();
    }

    // Update is called once per frame
    void Update()
    {
        if (finalscore.isBackgroundRed && !hasStartedLerpBackgroundRed)
        {
            hasStartedLerpBackgroundRed = true;
            StartCoroutine(ChangeColorLerp(spriteBackground, hexColorRedBackground, lerpDuration));
            StartCoroutine(ChangeColorLerp(spriteEdificioGrande, hexColorRedBuildings, lerpDuration));
            StartCoroutine(ChangeColorLerp(spriteLuna, hexColorRedBuildings, lerpDuration));

            for(int i = 0; i < carPool.carListSprite.Count; i++)
            {
                StartCoroutine(ChangeColorLerp(carPool.carListSprite[i], hexColorRedBuildings, lerpDuration));
            }

            for (int i = 0; i < spriteEdficioLargo.Length; i++)
            {
                StartCoroutine(ChangeColorLerp(spriteEdficioLargo[i], hexColorRedBuildings, lerpDuration));
            }

            for (int i = 0; i < spriteNubes.Length; i++)
            {
                StartCoroutine(ChangeColorLerp(spriteNubes[i], hexColorRedBuildings, lerpDuration));
            }

            redPlanet.SetActive(true);
            animationRedPlanet.SetBool("isMovement", true);
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
