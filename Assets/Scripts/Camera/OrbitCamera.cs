using UnityEngine;
using UnityEngine.EventSystems;

public enum ZoomMode
{
    CameraFieldOfView,
    ZAxisDistance,
    Blended
}


/**
 * Implementation of controlled camera movement in Analysis mode.
 */
public class OrbitCamera : MonoBehaviour
{

    [Header("Automatic Rotation")]
    [Tooltip("Toggles whether the camera will automatically rotate around it's target")]
    public bool autoRotate = true;
    [Tooltip("The speed at which the camera will auto-pan.")]
    public float rotationSpeed = 0.1f;
    [Tooltip("The rotation along the y-axis the camera will have at start.")]
    public float startRotation = 180;
    [Header("Manual Rotation")]
    [Tooltip("The smoothness coming to a stop of the camera afer the uses pans the camera and releases. Lower values result in significantly smoother results. This means the camera will take longer to stop rotating")]
    public float rotationSmoothing = 2f;
    [Tooltip("The object the camera will focus on.")]
    public Transform target;
    [Tooltip("How sensative the camera-panning is when the user pans -- the speed of panning.")]
    public float rotationSensitivity = 1f;
    [Tooltip("The min and max distance along the Y-axis the camera is allowed to move when panning.")]
    public Vector2 rotationLimit = new Vector2(5, 80);
    [Tooltip("The position along the Z-axis the camera game object is.")]
    public float zAxisDistance = 0.45f;
    [Header("Zooming")]
    [Tooltip("Whether the camera should zoom by adjusting it's FOV or by moving it closer/further along the z-axis")]
    public ZoomMode zoomMode = ZoomMode.CameraFieldOfView;
    [Tooltip("The minimum and maximum range the camera can zoon using the camera's FOV.")]
    public Vector2 cameraZoomRangeFOV = new Vector2(10, 60);
    [Tooltip("The minimum and maximum range the camera can zoon using the camera's z-axis position.")]
    public Vector2 cameraZoomRangeZAxis = new Vector2(10, 60);
    public float zoomSoothness = 10f;
    [Tooltip("How sensative the camera zooming is -- the speed of the zooming.")]
    public float zoomSensitivity = 2;

    [Header("Control Assist")]
    [Tooltip("Control assist sets this to override camera movement")]
    public bool controlAssistActive = false;

    new private Camera camera;
    private float cameraFieldOfView;
    new private Transform transform;
    public float xVelocity;
    public float yVelocity;
    public float xRotationAxis;
    public float yRotationAxis;
    public float zoomVelocity;
    public float zoomVelocityZAxis;
    Vector3 lastDragPosition;
    Ray lastMousePosition;
    public Vector3 camDefaultPosition;
    public Vector3 camDefaultRotation;
    public float camDefaultFov;


    private void Awake()
    {
        camera = GetComponent<Camera>();
        transform = GetComponent<Transform>();
#if UNITY_WEBGL || UNITY_WEBGL_API || PLATFORM_WEBGL
        rotationSensitivity = rotationSensitivity * 0.25f;             // Fix for out of control sensitivity on webgl
#endif
    }

    private void Start()
    {
        cameraFieldOfView = camera.fieldOfView;
        xRotationAxis = startRotation / rotationSpeed;
        camDefaultPosition = Camera.main.transform.position;
        camDefaultRotation = Camera.main.transform.rotation.eulerAngles;
        camDefaultFov = Camera.main.fieldOfView;
    }

    private void Update()
    {
        Zoom();

        if (Input.GetMouseButtonDown(0))
        {
            int layer_mask = LayerMask.GetMask("Specimens");
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f, layer_mask))
            {
                target = hit.transform;
            }
        }
    }

    private void LateUpdate()
    {

        //if (EventSystem.current.IsPointerOverGameObject() || controlAssistActive) return;  // Nothing after this will be executed if cursor is over UI object

        //If auto rotation is enabled, just increment the xVelocity value by the rotationSensitivity.
        if (autoRotate)
        {
            xVelocity += rotationSensitivity * Time.deltaTime;
        }

        Quaternion rotation;
        Vector3 position;
        float deltaTime = Time.deltaTime;

        if (Input.GetMouseButton(0))
        {
            xVelocity += Input.GetAxis("Mouse X") * rotationSensitivity * 2f; 
            yVelocity -= Input.GetAxis("Mouse Y") * rotationSensitivity;
        }

        xRotationAxis += xVelocity;
        yRotationAxis += yVelocity;

        //Clamp the rotation along the y-axis between the limits we set. 
        //Limits of 360 or -360 on any axis will allow the camera to rotate unrestricted
        yRotationAxis = ClampAngleBetweenMinAndMax(yRotationAxis, rotationLimit.x, rotationLimit.y);

        if (Input.GetMouseButtonDown(2))
        {
            lastDragPosition = Input.mousePosition;
            target = null;
        }
        if (!target)
        {
            if (Input.GetMouseButton(2))
            {
                var delta = lastDragPosition - Input.mousePosition;
                transform.Translate(delta * Time.deltaTime * 0.1f);
                lastDragPosition = Input.mousePosition;
            }

            rotation = Quaternion.Euler(yRotationAxis, xRotationAxis * rotationSpeed, 0);

            transform.rotation = rotation;
            xVelocity = Mathf.Lerp(xVelocity, 0, deltaTime * 17f);
            yVelocity = Mathf.Lerp(yVelocity, 0, deltaTime * 17f);
        }
        else
        {
            rotation = Quaternion.Euler(yRotationAxis, xRotationAxis * rotationSpeed, 0);
            position = rotation * new Vector3(0f, 0f, -zAxisDistance) + target.position;

            transform.rotation = rotation;
            transform.position = position;

            xVelocity = Mathf.Lerp(xVelocity, 0, deltaTime * rotationSmoothing);
            yVelocity = Mathf.Lerp(yVelocity, 0, deltaTime * rotationSmoothing);
        }


    }

    private void Zoom()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return; // Escapes if we're on a ui object. Necessary for UI scroll view.

        /*If the user's on a touch screen device like:
        an Android iOS or Windows phone/tablet, we'll detect if there are two fingers touching the screen.
        If the touches are moving closer together from where they began, we zoom out.
        If the touches are moving further apart, then we zoom in.*/
#if UNITY_ANDROID || UNITY_IOS || UNITY_WSA
            if (Input.touchCount == 2) {
                Touch touch0 = Input.GetTouch(0);
                Touch touch1 = Input.GetTouch(1);
                Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
                Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
                float prevTouchDeltaMag = (touch0PrevPos - touch1PrevPos).magnitude;
                float touchDeltaMag = (touch0.position - touch1.position).magnitude;
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                camera.fieldOfView = cameraFieldOfView;
                cameraFieldOfView += deltaMagnitudeDiff * zoomSensitivity;
                cameraFieldOfView = Mathf.Clamp(cameraFieldOfView, cameraZoomRangeFOV.x, cameraZoomRangeFOV.y);
            }
#endif
        DoZoom(Input.GetAxis("Mouse ScrollWheel"), Time.deltaTime);
    }

    public void DoZoom(float amt, float time)
    {
        //Zooms the camera in using the mouse scroll wheel
        if (amt > 0f) {
            switch (zoomMode) {
                case ZoomMode.CameraFieldOfView:
                    cameraFieldOfView = Mathf.SmoothDamp(cameraFieldOfView, cameraZoomRangeFOV.x, ref zoomVelocity,
                        time * zoomSoothness);

                    //prevents the field of view from going below the minimum value
                    if (cameraFieldOfView <= cameraZoomRangeFOV.x) {
                        cameraFieldOfView = cameraZoomRangeFOV.x;
                    }

                    break;
                case ZoomMode.ZAxisDistance:

                    zAxisDistance = Mathf.SmoothDamp(zAxisDistance, cameraZoomRangeZAxis.x, ref zoomVelocityZAxis,
                        time * zoomSoothness);

                    //prevents the z axis distance from going below the minimum value
                    if (zAxisDistance <= cameraZoomRangeZAxis.x) {
                        zAxisDistance = cameraZoomRangeZAxis.x;
                    }

                    break;
                case ZoomMode.Blended:
                    cameraFieldOfView = Mathf.SmoothDamp(cameraFieldOfView, cameraZoomRangeFOV.x, ref zoomVelocity,
                        time * zoomSoothness);
                    zAxisDistance = Mathf.SmoothDamp(zAxisDistance, cameraZoomRangeZAxis.x, ref zoomVelocityZAxis,
                        time * zoomSoothness);

                    //prevents the field of view from going below the minimum value
                    cameraFieldOfView = Mathf.Clamp(cameraFieldOfView, cameraZoomRangeFOV.x, cameraZoomRangeFOV.y);
                    zAxisDistance = Mathf.Clamp(zAxisDistance, cameraZoomRangeZAxis.x, cameraZoomRangeZAxis.y);
                    break;
            }
        } else if (amt < 0f) {

            //Zooms the camera out using the mouse scroll wheel
            switch (zoomMode) {
                case ZoomMode.CameraFieldOfView:
                    cameraFieldOfView = Mathf.SmoothDamp(cameraFieldOfView, cameraZoomRangeFOV.y, ref zoomVelocity,
                        time * zoomSoothness);

                    //prevents the field of view from going below the minimum value
                    if (cameraFieldOfView <= cameraZoomRangeFOV.x) {
                        cameraFieldOfView = cameraZoomRangeFOV.x;
                    }

                    break;
                case ZoomMode.ZAxisDistance:

                    zAxisDistance = Mathf.SmoothDamp(zAxisDistance, cameraZoomRangeZAxis.y, ref zoomVelocityZAxis,
                        time * zoomSoothness);

                    //prevents the z axis distance from going below the minimum value
                    if (zAxisDistance <= cameraZoomRangeZAxis.x) {
                        zAxisDistance = cameraZoomRangeZAxis.x;
                    }

                    break;
                case ZoomMode.Blended:
                    cameraFieldOfView = Mathf.SmoothDamp(cameraFieldOfView, cameraZoomRangeFOV.y, ref zoomVelocity,
                        time * zoomSoothness);
                    zAxisDistance = Mathf.SmoothDamp(zAxisDistance, cameraZoomRangeZAxis.y, ref zoomVelocityZAxis,
                        time * zoomSoothness);

                    //prevents the field of view from going below the minimum value
                    cameraFieldOfView = Mathf.Clamp(cameraFieldOfView, cameraZoomRangeFOV.x, cameraZoomRangeFOV.y);
                    zAxisDistance = Mathf.Clamp(zAxisDistance, cameraZoomRangeZAxis.x, cameraZoomRangeZAxis.y);
                    break;
            }
        }

        //We're just ensuring that when we're zooming using the camera's FOV, that the FOV will be updated to match the value we got when we scrolled.
        if (Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetAxis("Mouse ScrollWheel") < 0) {
            camera.fieldOfView = cameraFieldOfView;
        }
    }


    public void SetTarget(Transform t)
    {
        target = t;
    }


    //Prevents the camera from locking after rotating a certain amount if the rotation limits are set to 360 degrees.
    public float ClampAngleBetweenMinAndMax(float angle, float min, float max)
    {
        if (angle < -360)
        {
            angle += 360;
        }
        if (angle > 360)
        {
            angle -= 360;
        }
        return Mathf.Clamp(angle, min, max);
    }

}
