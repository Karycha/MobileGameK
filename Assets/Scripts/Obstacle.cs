using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed, _maxOffset;

    [SerializeField] private List<Vector3> _spawnPos;

    private bool hasGameFinished;


    private void Start()
    {
        hasGameFinished = false;


        Vector3 currentSpawnPos;
        int spawnIndex;
        spawnIndex = Random.Range(0, _spawnPos.Count);
        currentSpawnPos = _spawnPos[spawnIndex];
        transform.position = currentSpawnPos;

        bool isShort = Random.Range(0, 3) == 0;
        if(isShort)
        {
            var col = GetComponent<BoxCollider2D>();
            col.size = new Vector2(col.size.x - 1.6f, col.size.y);
            col.offset = new Vector2(col.offset.x - 0.8f, col.offset.y);
            GetComponent<SpriteRenderer>().size = col.size;
        }

        if(transform.position.x > 0f)
        {
            Vector3 temp = transform.localScale;
            temp.x *= -1f;
            transform.localScale = temp;
        }
    }

    private void OnEnable()
    {
        GameManager.Instance.GameEnded += OnGameEnded;
    }

    private void OnDisable()
    {
        GameManager.Instance.GameEnded -= OnGameEnded;
    }


    private void FixedUpdate()
    {
        if (hasGameFinished) return;

        transform.position += _moveSpeed * Time.fixedDeltaTime * Vector3.down;

        if (transform.position.x > _maxOffset || transform.position.x < -_maxOffset)
        {
            Destroy(gameObject);
        }
    }

    public void OnGameEnded()
    {
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(Rescale());
    }

    [SerializeField] private float _destroyTime;

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
