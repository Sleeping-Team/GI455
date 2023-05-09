using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingFadeEffect : SingletorPersistent<LoadingFadeEffect>
{
    public static bool s_canLoad;

    [SerializeField] private Image m_loadingBackground;

    [SerializeField] [Range(0.0f, 0.5f)] private float m_loadingStepTime = 0.1f;

    [SerializeField] [Range(0.0f, 0.5f)] private float m_loadingStepValue = 0.5f;

    IEnumerator FadeAllEffect()
    {
        yield return StartCoroutine(FadeInEffect());

        yield return new WaitForSeconds(1);

        yield return StartCoroutine(FadeOutEffect());
    }

    IEnumerator FadeInEffect()
    {
        Color backgroundColor = m_loadingBackground.color;

        backgroundColor.a = 0;

        m_loadingBackground.color = backgroundColor;
        
        m_loadingBackground.gameObject.SetActive(true);

        while (backgroundColor.a <= 1)
        {
            yield return new WaitForSeconds(m_loadingStepTime);

            backgroundColor.a += m_loadingStepValue;

            m_loadingBackground.color = backgroundColor;
        }

        s_canLoad = true;
    }

    IEnumerator FadeOutEffect()
    {
        s_canLoad = false;

        Color backgroundColor = m_loadingBackground.color;

        while (backgroundColor.a >= 0)
        {
            yield return new WaitForSeconds(m_loadingStepTime);

            backgroundColor.a -= m_loadingStepValue;

            m_loadingBackground.color = backgroundColor;
        }
        
        m_loadingBackground.gameObject.SetActive(false);
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInEffect());
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutEffect());
    }

    public void FadeAll()
    {
        StartCoroutine(FadeAllEffect());
    }
}
