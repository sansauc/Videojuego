using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNew : MonoBehaviour
{
    //public static Action OnEndReached;
    //public static event Action OnEndReached;

    public static event Action<EnemyNew> OnEndReached;



    [SerializeField] private int idEnemy;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private int coinReward = 1;

    public int IdEnemy => idEnemy; // propiedad solo de lectura (getter)

    public int CoinReward => coinReward;

    /// <summary>
    /// The waypoint reference
    /// </summary>
    public Waypoint Waypoint { get; set; }

    public EnemyHealthNew EnemyHealth { get; set; }


    public Vector3 CurrentPointPosition => Waypoint.GetWaypointPosition(_currentWaypointIndex);

    private int _currentWaypointIndex;

    //Script de chatgpt para mover al personaje con sus animaciones
    private Animator _animator;
    private Vector3 _lastPosition;

    //Esto es nuevo, se utiliza para saber cuando se muere un enemigo
    public static Action OnEnemyKilled;

    //Esto es nuevo, se utiliza para detener a un enemigo cuando recibe daño

    private bool isStunned = false;
    private float stunDuration = 0.1f; // Puedes cambiar la duración del "paralizado"


    private void Start()
    {

        _currentWaypointIndex = 0;

        EnemyHealth = GetComponent<EnemyHealthNew>();


        _animator = GetComponent<Animator>();
        _lastPosition = transform.position;
    }

    private void Update()
    {
        if (!isStunned)
        {
            Move();
            UpdateAnimationDirection();

            if (CurrentPointPositionReached())
            {
                UpdateCurrentPointIndex();
            }
        }

        _lastPosition = transform.position;
    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position,
            CurrentPointPosition, moveSpeed * Time.deltaTime);
    }

    private bool CurrentPointPositionReached()
    {
        float distanceToNextPointPosition = (transform.position - CurrentPointPosition).magnitude;
        if (distanceToNextPointPosition < 0.1f)
        {
            return true;
        }

        return false;
    }

    private void UpdateCurrentPointIndex()
    {
        int lastWaypointIndex = Waypoint.Points.Length - 1;
        if (_currentWaypointIndex < lastWaypointIndex)
        {
            _currentWaypointIndex++;
        }
        else
        {
            ReachEnd();
        }

    }

    private void ReachEnd()
    {
        Debug.Log("⚠️ Enemy llegó al final del camino.");
        OnEndReached?.Invoke(this);
        ObjectPoolerNew.ReturnToPool(gameObject);
    }

    private void UpdateAnimationDirection()
    {
        Vector3 direction = transform.position - _lastPosition;

        // Solo actualiza si se está moviendo (evita parpadeo por ruido numérico)
        if (direction.magnitude < 0.001f)
        {
            _animator.SetFloat("MoveX", 0f);
            _animator.SetFloat("MoveY", 0f);
            return;
        }

        // Normalizamos para que no se acumulen valores altos
        Vector3 normalizedDirection = direction.normalized;

        //Debug.Log($"MoveX: {normalizedDirection.x}, MoveY: {normalizedDirection.y}");


        _animator.SetFloat("MoveX", normalizedDirection.x);
        _animator.SetFloat("MoveY", normalizedDirection.y);
    }

    // Llamado desde EnemyHealth cuando recibe daño
    public void Stun()
    {
        if (!isStunned)
            StartCoroutine(StunCoroutine());
    }

    private IEnumerator StunCoroutine()
    {
        isStunned = true;

        // Opcional: Puedes activar aquí alguna animación de recibir daño si tuvieras una
        _animator.SetFloat("MoveX", 0f);
        _animator.SetFloat("MoveY", 0f);

        yield return new WaitForSeconds(stunDuration);

        isStunned = false;
    }

    public void ResetEnemy()
    {
        _currentWaypointIndex = 0;
    }

}
