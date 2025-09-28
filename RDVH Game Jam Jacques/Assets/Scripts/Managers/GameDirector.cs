using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    public Camera camera;
    public GameDirectorConfig config;
    public VaubanController VaubanController;
    [SerializeField] DialogueController DialogController;

    [SerializeField]
    private GameObject UIFin;

    private Coroutine currentShake;

    public async Task Init(LevelEnvironment levelEnvironment)
    {
        //ICI, faire l'intro

        camera.transform.position = levelEnvironment.cameraFocusPoint.position + config.cameraOffset;
        await Task.Delay(2000);
        // Démarre une nouvelle coroutine de déplacement
        await MoveVauban(levelEnvironment.VaubanPosition.position);
        // Dialogue de Vauban
        await DialogController.RunAsync(levelEnvironment.VaubanDialog);

    }

    public async Task EndStep()
    {
        CameraShake(0.2f, 0.1f);
        VaubanController.SetAnimation(VaubanController.AnimState.HAPPY);
        await Task.Delay(1000);
        VaubanController.SetAnimation(VaubanController.AnimState.IDLE);
    }

    public async Task GoTo(LevelEnvironment levelEnvironment)
    {
        //camera.transform.position = levelEnvironment.cameraFocusPoint.position + config.cameraOffset;


        // Démarre une nouvelle coroutine de déplacement
        Task camTask = MoveCamera(levelEnvironment.cameraFocusPoint.position + config.cameraOffset);

        // Démarre une nouvelle coroutine de déplacement
        Task vaubanTask = MoveVauban(levelEnvironment.VaubanPosition.position);

        // Continuer seulement quand les 2 sont terminées
        await Task.WhenAll(camTask, vaubanTask);

        await DialogController.RunAsync(levelEnvironment.VaubanDialog);
    }


    private async Task MoveCamera(Vector3 target)
    {
        Vector3 velocity = Vector3.zero;

        // Boucle jusqu’à ce qu’on soit assez proche
        while ((camera.transform.position - target).sqrMagnitude > 0.01f * 0.01f)
        {
            camera.transform.position = Vector3.SmoothDamp(
                camera.transform.position,
                target,
                ref velocity,
                1f,
                Mathf.Infinity,
                Time.deltaTime
            );

            await Task.Yield(); // attend la prochaine frame
        }

        // Snap final
        camera.transform.position = target;
    }

    private async Task MoveVauban(Vector3 target)
    {
        Transform t = VaubanController.transform;

        // --- Réglages ---
        float rotateDuration = 0.2f;   // durée de rotation aller/retour
        float speed = 3.0f;            // vitesse linéaire constante (unités / s)
        float stopDistance = 0.01f;    // seuil d’arrêt
        float facingOffsetDeg = 0f;    // ajuste selon l’orientation native du sprite/modèle

        // 1) Tourner vers la destination en 0.2s
        Quaternion startRot = t.rotation;

        Vector3 dir = (target - t.position);
        //// --- Rotation 2D (Z) ---
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + facingOffsetDeg;
        Quaternion faceRot = Quaternion.Euler(0f, 0f, angle);

        //// --- Si 3D top-down (Y up), utilise plutôt: ---
        Vector3 flatDir = new Vector3(dir.x, 0f, dir.z);
        if (flatDir.sqrMagnitude > 0.0001f) faceRot = Quaternion.LookRotation(flatDir, Vector3.up);
        else faceRot = t.rotation;

        float tRot = 0f;
        while (tRot < rotateDuration)
        {
            tRot += Time.deltaTime;
            float k = Mathf.Clamp01(tRot / rotateDuration);
            t.rotation = Quaternion.Slerp(startRot, faceRot, k);
            await Task.Yield();
        }
        t.rotation = faceRot; // snap final d'orientation

        // 2) Se déplacer à vitesse constante jusqu’à target
        VaubanController.SetAnimation(VaubanController.AnimState.RUN);
        while ((t.position - target).sqrMagnitude > stopDistance * stopDistance)
        {
            t.position = Vector3.MoveTowards(t.position, target, speed * Time.deltaTime);
            await Task.Yield();
        }
        t.position = target; // snap final de position

        // 3) Revenir à l’angle initial en 0.2s
        VaubanController.SetAnimation(VaubanController.AnimState.IDLE);
        tRot = 0f;
        while (tRot < rotateDuration)
        {
            tRot += Time.deltaTime;
            float k = Mathf.Clamp01(tRot / rotateDuration);
            t.rotation = Quaternion.Slerp(faceRot, startRot, k);
            await Task.Yield();
        }
        t.rotation = startRot;
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

    public async Task GoToOutro(LevelEnvironment levelEnvironment)
    {
        MoveCamera(levelEnvironment.cameraFocusPoint.position + config.cameraOffset);

        await MoveVauban(levelEnvironment.VaubanPosition.position);

        VaubanController.SetAnimation(VaubanController.AnimState.HAPPY);

        await Task.Delay(5000);

        UIFin.SetActive(true);
    }
}
