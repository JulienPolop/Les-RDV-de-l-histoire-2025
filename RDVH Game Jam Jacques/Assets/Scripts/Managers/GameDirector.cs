using System.Collections;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    public Camera MainCamera;
    public GameDirectorConfig config;
    public VaubanController VaubanController;
    public AudioManager AudioManager;
    [SerializeField] DialogueController DialogController;

    [SerializeField] private GameObject UIFin;
    [SerializeField] private GameObject UIMainMenu;

    private Coroutine currentShake;

    [Header("Camera")]
    [SerializeField] private Transform MainMenuCameraPosition;
    [SerializeField] private Vector3 MainMenuCameraRotation;

    [SerializeField] private Vector3 GameCameraRotation;

    // --- Garde les mêmes noms publics ---

    public IEnumerator ShowMainMenu()
    {
        //Petite animation d'entrée (bouger la caméra légèrement) + fade in

        //Son, seulement la musique
        if (AudioManager.musicSource != null)
            AudioManager.musicSource.Play();

        yield return MoveCamera(MainMenuCameraPosition.position, MainMenuCameraRotation);


        UIMainMenu.SetActive(true);
    }

    public IEnumerator Init(LevelEnvironment levelEnvironment)
    {
        UIMainMenu.SetActive(false);

        // ICI, faire l'intro
        Debug.Log("Init");
        yield return MoveCamera(levelEnvironment.cameraFocusPoint.position + config.cameraOffset, GameCameraRotation);
        //MainCamera.transform.position = levelEnvironment.cameraFocusPoint.position + config.cameraOffset;
        Debug.Log("Just Before Wait 2 seconds"); 
        yield return new WaitForSeconds(2f);
        Debug.Log("After 2s wait, Move Vauban");
        // Déplacement Vauban
        yield return MoveVauban(levelEnvironment.VaubanPosition.position);
        Debug.Log("After Vauban");
        // Dialogue de Vauban (même nom RunAsync, appelé en coroutine)
        if (DialogController != null)
            yield return DialogController.RunDialog(levelEnvironment.VaubanDialog);

        if (AudioManager.battleSource != null)
            AudioManager.battleSource.Play();
    }

    public IEnumerator EndStep()
    {
        CameraShake(0.2f, 0.1f);
        VaubanController.SetAnimation(VaubanController.AnimState.HAPPY);
        yield return new WaitForSeconds(1f);
        VaubanController.SetAnimation(VaubanController.AnimState.IDLE);
    }

    public IEnumerator GoTo(LevelEnvironment levelEnvironment)
    {
        // Lancer caméra + vauban en parallèle, puis attendre la fin des 2
        yield return WaitAll(
            MoveCamera(levelEnvironment.cameraFocusPoint.position + config.cameraOffset),
            MoveVauban(levelEnvironment.VaubanPosition.position)
        );

        if (DialogController != null)
            yield return DialogController.RunDialog(levelEnvironment.VaubanDialog);
    }

    // Garde le comportement d’origine: caméra lancée "en fond", on attend seulement Vauban
    public IEnumerator GoToOutro(LevelEnvironment levelEnvironment)
    {
        yield return WaitAll(
            MoveCamera(levelEnvironment.cameraFocusPoint.position + config.cameraOffset),
            MoveVaubanAndAnimate(levelEnvironment.VaubanPosition.position)
        );

        VaubanController.SetAnimation(VaubanController.AnimState.HAPPY);
        yield return new WaitForSeconds(1f);

        Vector3 vector3 = new Vector3(54f, 0, 0);
        yield return MoveCamera(MainMenuCameraPosition.position, vector3);

        UIFin.SetActive(true);
    }

    private IEnumerator MoveVaubanAndAnimate(Vector3 targetPos)
    {
        yield return MoveVauban(targetPos); // attend la fin
        VaubanController.SetAnimation(VaubanController.AnimState.HAPPY); // déclenche dès que Vauban a fini
    }

    // --- Mouvements ---

    private IEnumerator MoveCamera(Vector3 target, Vector3? rotationTarget = null)
    {
        Vector3 velocity = Vector3.zero;
        Vector3 angularVelocity = Vector3.zero;

        while ((MainCamera.transform.position - target).sqrMagnitude > 0.01f * 0.01f
            || (rotationTarget.HasValue && Quaternion.Angle(MainCamera.transform.rotation, Quaternion.Euler(rotationTarget.Value)) > 0.1f))
        {
            // Déplacement lissé
            MainCamera.transform.position = Vector3.SmoothDamp(
                MainCamera.transform.position,
                target,
                ref velocity,
                1f,
                Mathf.Infinity,
                Time.deltaTime
            );

            // Rotation lissée si rotation cible définie
            if (rotationTarget.HasValue)
            {
                MainCamera.transform.rotation = Quaternion.Euler(
                    Vector3.SmoothDamp(
                        MainCamera.transform.rotation.eulerAngles,
                        rotationTarget.Value,
                        ref angularVelocity,
                        1f,
                        Mathf.Infinity,
                        Time.deltaTime
                    )
                );
            }

            yield return null;
        }

        // Snap final
        MainCamera.transform.position = target;
        if (rotationTarget.HasValue)
            MainCamera.transform.rotation = Quaternion.Euler(rotationTarget.Value);
    }

    private IEnumerator MoveVauban(Vector3 target)
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

        // Rotation 2D (Z)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + facingOffsetDeg;
        Quaternion faceRot = Quaternion.Euler(0f, 0f, angle);

        // Si top-down 3D (Y up), adapte ici :
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
        t.rotation = faceRot;

        // 2) Avancer jusqu’à la cible
        VaubanController.SetAnimation(VaubanController.AnimState.RUN);
        while ((t.position - target).sqrMagnitude > stopDistance * stopDistance)
        {
            t.position = Vector3.MoveTowards(t.position, target, speed * Time.deltaTime);
            yield return null;
        }
        t.position = target;

        // 3) Revenir à l’angle initial
        VaubanController.SetAnimation(VaubanController.AnimState.IDLE);
        tRot = 0f;
        while (tRot < rotateDuration)
        {
            tRot += Time.deltaTime;
            float k = Mathf.Clamp01(tRot / rotateDuration);
            t.rotation = Quaternion.Slerp(faceRot, startRot, k);
            yield return null;
        }
        t.rotation = startRot;
    }

    // --- Camera Shake ---

    public void CameraShake(float duration, float magnitude)
    {
        if (currentShake != null)
            StopCoroutine(currentShake);

        currentShake = StartCoroutine(DoCameraShake(duration, magnitude));
    }

    private IEnumerator DoCameraShake(float duration, float magnitude)
    {
        Vector3 originalPos = MainCamera.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            MainCamera.transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        MainCamera.transform.localPosition = originalPos;
        currentShake = null;
    }

    // --- Helper: attendre 2 coroutines lancées en parallèle ---

    private IEnumerator WaitAll(IEnumerator a, IEnumerator b)
    {
        bool aDone = false, bDone = false;
        StartCoroutine(RunAndFlag(a, () => aDone = true));
        StartCoroutine(RunAndFlag(b, () => bDone = true));
        yield return new WaitUntil(() => aDone && bDone);
    }

    private IEnumerator RunAndFlag(IEnumerator routine, System.Action onDone)
    {
        yield return StartCoroutine(routine);
        onDone?.Invoke();
    }
}
