using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float parallaxSpeedX; // Vitesse sur l'axe X (plus petit = plus lointain)
    [SerializeField] private float parallaxSpeedY; // Vitesse sur l'axe Y

    private Vector3 startPosition = Vector3.zero;
    private Vector3 startCamPosition = Vector3.zero;
    private Camera cam = null;

    private void Start()
    {
        cam = Camera.main;  // Utilise automatiquement la cam�ra principale

        startPosition = transform.position;
        startCamPosition = cam.transform.position;
    }

    private void Update()
    {
        Vector3 camDelta = cam.transform.position - startCamPosition + new Vector3(-100, 0, 0);

        // Applique la vitesse de parallaxe diff�rente pour chaque axe
        transform.position = startPosition + new Vector3(camDelta.x * parallaxSpeedX, camDelta.y * parallaxSpeedY, 0);
    }
}
