using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hook : MonoBehaviour
{
    public Transform hookedTransform;

    private Camera _mainCamera;
    private Collider2D _collider2D;
    private int _length;
    private int _strength; // how many fish we can have
    private int _fishCount;

    private bool _canMove;

    private List<Fish> _hookedFishes;

    private Tweener cameraTween;

    // Start is called before the first frame update
    void Awake()
    {
        _mainCamera = Camera.main;
        _collider2D = GetComponent<Collider2D>();
        _hookedFishes = new List<Fish>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_canMove && Input.GetMouseButton(0))
        {
            Vector3 vector = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 position = transform.position;
            position.x = vector.x;
            transform.position = position;
        }
    }

    public void StartFishing()
    {
        _length = IdleManager.instance.length - 20;
        _strength = IdleManager.instance.strength;
        _fishCount = 0;
        float time = (-_length) * 0.1f;

        cameraTween = _mainCamera.transform
            /*
             * Go deepen under the length
             */
            .DOMoveY(_length, 1 + time * 0.25f, false)
            .OnUpdate(delegate
            {
                if (_mainCamera.transform.position.y <= -11)
                {
                    transform.SetParent(_mainCamera.transform);
                }
            }).OnComplete(delegate
            {
                /*
                 * When we on needed deep we enable our collider
                 */
                _collider2D.enabled = true;
                /*
                 * Go up and stop fishing
                 * go up bit faster
                 */
                cameraTween = _mainCamera.transform
                    .DOMoveY(0, time * 5, false)
                    .OnUpdate(delegate
                    {
                        if (_mainCamera.transform.position.y >= -15)
                        {
                            StopFishing();
                        }
                    });
            });

        // Screen(GAME)
        ScreenManager.instance.ChangeScreens(Screens.GAME);
        _collider2D.enabled = false;
        _canMove = true;
        // Clear
        _hookedFishes.Clear();
    }

    private void StopFishing()
    {
        _canMove = false;
        cameraTween.Kill(false);
        cameraTween = _mainCamera.transform.DOMoveY(0, 2, false)
            .OnUpdate(delegate
            {
                if (_mainCamera.transform.position.y >= -11)
                {
                    transform.SetParent(null);
                    transform.position = new Vector2(transform.position.x, -6);
                }
            }).OnComplete(delegate
            {
                transform.position = Vector2.down * 6;
                _collider2D.enabled = true;
                int rezultPrice = 0;

                for (int i = 0; i < _hookedFishes.Count; i++)
                {
                    _hookedFishes[i].transform.SetParent(null);
                    _hookedFishes[i].ResetFish();
                    rezultPrice += _hookedFishes[i].Type.price;
                }
                // Clearing out the hook out of fishes
                IdleManager.instance.totalGain = rezultPrice;
                ScreenManager.instance.ChangeScreens(Screens.END);
            });
    }

    private void OnTriggerEnter2D(Collider2D target)
    {
        if (target.CompareTag("Fish") && _fishCount != _strength)
        {
            _fishCount++;
            Fish component = target.GetComponent<Fish>();
            component.Hooked();
            _hookedFishes.Add(component);
            target.transform.SetParent(transform);
            target.transform.position = hookedTransform.position;
            target.transform.rotation = hookedTransform.rotation;
            target.transform.localScale = Vector3.one;

            target.transform
                .DOShakeRotation(5, Vector3.forward * 45, 10, 90, false)
                .SetLoops(1, LoopType.Yoyo)
                .OnComplete(delegate
                {
                    target.transform.rotation = Quaternion.identity; // set rotation to normal
                });

            if (_fishCount == _strength)
            {
                StopFishing();
            }
        }
    }
}