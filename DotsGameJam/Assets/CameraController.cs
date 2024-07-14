using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private EntityManager _entityManager;
    private EntityQuery _playerQuery;

    // Start is called before the first frame update
    void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _playerQuery = _entityManager.CreateEntityQuery(typeof(PlayerTag), typeof(LocalTransform));
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerQuery.TryGetSingleton(out LocalTransform localTransform))
        {
            var transform1 = transform;
            transform1.position = localTransform.Position;
            transform1.rotation = localTransform.Rotation;
        }
    }
}