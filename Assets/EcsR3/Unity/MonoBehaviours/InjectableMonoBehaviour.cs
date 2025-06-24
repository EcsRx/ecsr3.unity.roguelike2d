﻿using SystemsR3.Events;
using EcsR3.Unity.Dependencies;
using UnityEngine;
using Zenject;

namespace EcsR3.Unity.MonoBehaviours
{
    public abstract class InjectableMonoBehaviour : MonoBehaviour
    {
        [Inject]
        public IEventSystem EventSystem { get; private set; }

        [Inject]
        protected IInstantiator Instantiator { get; set; }

        /// <summary>
        /// This is the point in which any injected dependencies will have been resolved for use
        /// </summary>
        [Inject]
        public abstract void DependenciesResolved();

        protected GameObject InstantiateAndInject(GameObject prefab, 
            Vector3 position = default(Vector3),
            Quaternion rotation = default(Quaternion))
        {
            var createdPrefab = Instantiator.InstantiatePrefab(prefab);
            createdPrefab.transform.position = position;
            createdPrefab.transform.rotation = rotation;
            return createdPrefab;
        }
    }
}