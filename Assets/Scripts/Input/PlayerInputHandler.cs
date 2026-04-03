using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookScreenPosition { get; private set; }

    public bool FireHeld { get; private set; }
    public bool FirePressedThisFrame { get; private set; }

    public bool ReloadPressedThisFrame { get; private set; }
    public bool Slot1PressedThisFrame { get; private set; }
    public bool Slot2PressedThisFrame { get; private set; }

    public bool PausePressedThisFrame { get; private set; }

    private InputAction pauseAction;
    private PlayerInput playerInput;
    private InputAction fireAction;
    private InputAction reloadAction;
    private InputAction slot1Action;
    private InputAction slot2Action;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        // 一定从 PlayerInput 的 actions 里取，别自己拿外部 asset 引用
        fireAction = playerInput.actions.FindAction("Fire", throwIfNotFound: false);
        reloadAction = playerInput.actions.FindAction("Reload", throwIfNotFound: false);
        slot1Action = playerInput.actions.FindAction("Slot1", throwIfNotFound: false);
        slot2Action = playerInput.actions.FindAction("Slot2", throwIfNotFound: false);
        pauseAction = playerInput.actions.FindAction("Pause", throwIfNotFound: false);
    }

    private void Update()
    {
        if (fireAction != null)
        {
            FireHeld = fireAction.IsPressed();

            if (fireAction.WasPressedThisFrame())
            {
                FirePressedThisFrame = true;
            }
        }

        if (reloadAction != null && reloadAction.WasPressedThisFrame())
        {
            ReloadPressedThisFrame = true;
        }

        if (slot1Action != null && slot1Action.WasPressedThisFrame())
        {
            Slot1PressedThisFrame = true;
        }

        if (slot2Action != null && slot2Action.WasPressedThisFrame())
        {
            Slot2PressedThisFrame = true;
        }
        if (pauseAction != null && pauseAction.WasPressedThisFrame())
        {
            PausePressedThisFrame = true;
        }
    }

    public void OnMove(InputValue value)
    {
        MoveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        LookScreenPosition = value.Get<Vector2>();
    }

    public bool ConsumeFirePressed()
    {
        bool value = FirePressedThisFrame;
        FirePressedThisFrame = false;
        return value;
    }

    public bool ConsumeReloadPressed()
    {
        bool value = ReloadPressedThisFrame;
        ReloadPressedThisFrame = false;
        return value;
    }

    public bool ConsumeSlot1Pressed()
    {
        bool value = Slot1PressedThisFrame;
        Slot1PressedThisFrame = false;
        return value;
    }

    public bool ConsumeSlot2Pressed()
    {
        bool value = Slot2PressedThisFrame;
        Slot2PressedThisFrame = false;
        return value;
    }

    public bool ConsumePausePressed()
    {
        bool value = PausePressedThisFrame;
        PausePressedThisFrame = false;
        return value;
    }
}