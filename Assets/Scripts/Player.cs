using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent   
{
    public static Player Instance { get; private set; }

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private Transform kitchenObjectHoldPoint;

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;

    private KitchenObject kitchenObject;
    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;

    private void Awake()
    { 
        if(Instance != null)
        {
            Debug.LogError("There are more than one player");
        }
        Instance = this;
    }

    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (selectedCounter)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (selectedCounter)
        {
            selectedCounter.Interact(this);
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleInteract();
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleInteract()
    {


        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float interactionDistance = 2f;
        if(moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }
        if (Physics.Raycast(transform.position, lastInteractDir,out RaycastHit raycastHit, interactionDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                if (selectedCounter != baseCounter)
                {
                    SetSelectedCounter(baseCounter);
                }

            } else
            {
                SetSelectedCounter(null);
            }
        } else
        {
            SetSelectedCounter(null);
        }
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs { selectedCounter = selectedCounter });

    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        Vector3 moveDirX = new Vector3(inputVector.x, 0f, 0);
        Vector3 moveDirZ = new Vector3(0, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float PlayerRadius = .7f;
        float playerHeight = 2;
        bool canMoveX = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, PlayerRadius, moveDirX, moveDistance);
        bool canMoveZ = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, PlayerRadius, moveDirZ, moveDistance);

        if (canMoveX)
        {
            if (!canMoveZ)
            {
                moveDirX = moveDirX.normalized;
                transform.position += moveDirX * moveDistance;

            }
            else
            {
                transform.position += moveDirX * moveDistance;

            }

        }
        if (canMoveZ)
        {
            if (!canMoveX)
            {
                moveDirZ = moveDirZ.normalized;
                transform.position += moveDirZ * moveDistance;

            }
            else
            {
                transform.position += moveDirZ * moveDistance;

            }

        }

        isWalking = moveDir != Vector3.zero;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
    }
    public KitchenObject GetKitchenObject()
    {
        return this.kitchenObject;
    }
    public void ClearKitchenObject()
    {
        this.kitchenObject = null;
    }
    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}
