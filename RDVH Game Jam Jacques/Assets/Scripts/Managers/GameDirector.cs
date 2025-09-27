using System.Collections;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    public Camera camera;
    public GameDirectorConfig config;
    public GameObject VaubanFigurine;

    private Coroutine currentMove;
    private Coroutine currentMoveVauban;
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

        if (currentMoveVauban != null)
            StopCoroutine(currentMoveVauban);

        // Démarre une nouvelle coroutine de déplacement
        currentMoveVauban = StartCoroutine(MoveVauban(levelEnvironment.VaubanPosition.position));
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

    private IEnumerator MoveVauban(Vector3 target)
    {
        Transform t = VaubanFigurine.transform;

        // --- Réglages ---
        float rotateDuration = 0.2f;   // durée de rotation aller/retour
        float speed = 3.0f;            // vitesse linéaire constante (unités / s)
        float stopDistance = 0.01f;    // seuil d’arrêt
        float facingOffsetDeg = 0f;    // ajuste selon l’orientation native du sprite/modèle

        // 1) Tourner vers la destination en 0.2s
        Quaternion startRot = t.rotation;

        Vector3 dir = (target - t.position);
        // --- Rotation 2D (Z) ---
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + facingOffsetDeg;
        Quaternion faceRot = Quaternion.Euler(0f, 0f, angle);

        // --- Si 3D top-down (Y up), utilise plutôt: ---
        Vector3 flatDir = new Vector3(dir.x, 0f, dir.z);
        if (flatDir.sqrMagnitude > 0.0001f) faceRot = Quaternion.LookRotation(flatDir, Vector3.up);
        else faceRot = t.rotation;

        float tRot = 0f;
        while (tRot < rotateDuration)
        {
            tRot += Time.deltaTime;
            float k = Mathf.Clamp01(tRot / rotateDuration);
            t.rotation = Quaternion.Slerp(startRot, faceRot, k);
            yield return null;
        }
        t.rotation = faceRot; // snap final d'orientation

        // 2) Se déplacer à vitesse constante jusqu’à target
        while ((t.position - target).sqrMagnitude > stopDistance * stopDistance)
        {
            t.position = Vector3.MoveTowards(t.position, target, speed * Time.deltaTime);
            yield return null;
        }
        t.position = target; // snap final de position

        // 3) Revenir à l’angle initial en 0.2s
        tRot = 0f;
        while (tRot < rotateDuration)
        {
            tRot += Time.deltaTime;
            float k = Mathf.Clamp01(tRot / rotateDuration);
            t.rotation = Quaternion.Slerp(faceRot, startRot, k);
            yield return null;
        }
        t.rotation = startRot;

        currentMoveVauban = null;
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
