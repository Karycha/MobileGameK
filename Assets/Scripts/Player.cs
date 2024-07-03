using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Player : MonoBehaviour
{ 
    private bool canMove;
    private bool canShoot;

    [SerializeField]
    private AudioClip _moveClip, _pointClip, _scoreClip, _loseClip;

    [SerializeField]
    private GameObject _explosionPrefab;

    private void Awake()
    {
        canShoot = false;
        canMove = false;
    }

    private void OnEnable()
    {
        GameManager.Instance.GameStarted += GameStarted;
        GameManager.Instance.GameEnded += OnGameEnded;
    }

    private void OnDisable()
    {
        GameManager.Instance.GameStarted -= GameStarted;
        GameManager.Instance.GameEnded -= OnGameEnded;
    }

    private void GameStarted()
    {
        canMove = true;
        canShoot = true;

        moveSpeed = 1 / _moveTime;
        currentMoveDistance = 0.5f;
        moveOffset = _moveEndPos - _moveStartPos;
    }

    private void Update()
    {
        if(canShoot && Input.GetMouseButtonDown(0))
        {
            moveSpeed *= -1f;
            AudioManager.Instance.PlaySound(_moveClip);
        }
    }

    [SerializeField] private float _moveTime;
    [SerializeField] private Vector3 _moveStartPos, _moveEndPos;

    private Vector3 moveOffset;
    private float moveSpeed;
    private float currentMoveDistance;

    private void FixedUpdate()
    {
        if (!canMove) return;

        if(currentMoveDistance > 1f || currentMoveDistance < 0f)
        {
            moveSpeed *= -1f;
            currentMoveDistance = currentMoveDistance > 0.5f ? 1f : 0f;
        }

        currentMoveDistance += moveSpeed * Time.fixedDeltaTime;
        transform.position = _moveStartPos + currentMoveDistance * moveOffset;
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.CompareTag(Constants.Tags.SCORE))
        {
            collision.gameObject.GetComponent<Score>().OnGameEnded();
            GameManager.Instance.UpdateScore();            
            AudioManager.Instance.PlaySound(_scoreClip);
        }

        if(collision.CompareTag(Constants.Tags.OBSTACLE))
        {
            Destroy(Instantiate(_explosionPrefab,transform.position,Quaternion.identity), 3f);
            AudioManager.Instance.PlaySound(_loseClip);
            GameManager.Instance.EndGame();
            Destroy(gameObject);
        }
    }

    [SerializeField] private float _destroyTime;

    public void OnGameEnded()
    {
        StartCoroutine(Rescale());
    }

    private IEnumerator Rescale()
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;
        Vector3 scaleOffset = endScale - startScale;
        float timeElapsed = 0f;
        float speed = 1 / _destroyTime;
        var updateTime = new WaitForFixedUpdate();
        while (timeElapsed < 1f)
        {
            timeElapsed += speed * Time.fixedDeltaTime;
            transform.localScale = startScale + timeElapsed * scaleOffset;
            yield return updateTime;
        }

        Destroy(gameObject);
    }
}