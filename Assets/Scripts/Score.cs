using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{

    [SerializeField]
    private float _moveSpeed,_maxOffset;

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
