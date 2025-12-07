using UnityEngine;
using UnityEngine.InputSystem;

public class MenuParallaxCamera : MonoBehaviour
{
    [Header("Quanto roda com o input")]
    public float rotationAmount = 10f;

    [Header("Velocidade com que suavemente segue")]
    public float rotationSmooth = 5f;

    private Vector2 lookInput;
    private Quaternion initialRot;

    void Start()
    {
        initialRot = transform.localRotation;
    }

    // Recebe input do UI Action Map
    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    void Update()
    {
        // Escala o input para evitar rotações exageradas
        Vector2 look = lookInput * 0.01f;

        // Cria a rotação desejada
        Quaternion targetRot = initialRot *
            Quaternion.Euler(
                -look.y * rotationAmount,  // inclina para cima/baixo
                 look.x * rotationAmount,   // vira esquerda/direita
                0f
            );

        // Interpola suavemente
        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRot,
            Time.deltaTime * rotationSmooth
        );
    }
}
