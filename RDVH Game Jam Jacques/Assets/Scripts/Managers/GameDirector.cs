using System.Collections;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    public Camera camera;
    public GameDirectorConfig config;

    private Coroutine currentMove;
    private Coroutine currentShake;

    public void Init(LevelEnvironment levelEnvironment)
    {
        camera.transform.position = levelEnvironment.cameraFocusPoint.position + config.cameraOffset;
    }

    public void GoTo(LevelEnvironment levelEnvironment)
    {
        //camera.transform.position = levelEnvironment.cameraFocusPoint.position + config.cameraOffset;

        // Stoppe un éventuel déplacement en cours
        if (currentMove != null)
            StopCoroutine(currentMove);

        // Démarre une nouvelle coroutine de déplacement
        currentMove = StartCoroutine(MoveCamera(levelEnvironment.cameraFocusPoint.position + config.cameraOffset));
    }


    private IEnumerator MoveCamera(Vector3 target)
    {
        Vector3 velocity = Vector3.zero;

        // Boucle jusqu’à ce qu’on soit assez proche
        while ((camera.transform.position - target).sqrMagnitude > 0.0f * 0.0f)
        {
            camera.transform.position = Vector3.SmoothDamp(
                camera.transform.position,
                target,
                ref velocity,
                1f,
                Mathf.Infinity,
                Time.deltaTime
            );

            yield return null; // attend la prochaine frame
        }

        // Snap final
        camera.transform.position = target;

        // Fin du déplacement
        currentMove = null;
    }

    public void CameraShake(float duration, float magnitude)
    {
        if (currentShake != null)
            StopCoroutine(currentShake);

        currentShake = StartCoroutine(DoCameraShake(duration, magnitude));
    }

    private IEnumerator DoCameraShake(float duration, float magnitude)
    {
        Vector3 originalPos = camera.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            camera.transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        camera.transform.localPosition = originalPos;
        currentShake = null;
    }
}
