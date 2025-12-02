using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Utils;
using Zenject;

namespace GameContext.UIParticleManagement
{
    [Serializable]
    public struct TargetedUIParticlePlayInfo
    {
        public RectTransform from;
        public RectTransform to;
        public Material particleMaterial;
        public int[] bursts;
        public float burstInterval;
        public bool deliverAllParticlesOnDispose;
    }
    

    public class TargetedUIParticlePlayHandle : IDisposable
    {
        private readonly TargetedParticleView.Pool _pool;
        private readonly TargetedParticleView _activeView;
        private readonly int _totalParticles;
        private readonly bool _deliverAllParticlesOnDispose;
        private bool _isDisposed;
        private int _completedParticles;

        private DisposableBag _disposableBag;
        private Subject<int> _onDeliveredParticlesSubject = new Subject<int>();
        
        public bool IsDisposed => _isDisposed;
        public bool IsComplete => _completedParticles >= _totalParticles;
        public Observable<int> OnDeliveredParticles => _onDeliveredParticlesSubject;

        public TargetedUIParticlePlayHandle(TargetedParticleView.Pool pool, TargetedParticleView activeView, TargetedUIParticlePlayInfo playInfo)
        {
            _disposableBag = new DisposableBag();
            _pool = pool;
            _activeView = activeView;
            _totalParticles = playInfo.bursts.Sum();
            _deliverAllParticlesOnDispose = playInfo.deliverAllParticlesOnDispose;
            activeView.OnParticleReachedTarget.Subscribe(OnParticleReachedTarget).AddTo(ref _disposableBag);
        }

        private void OnParticleReachedTarget()
        {
            _completedParticles++;
            _onDeliveredParticlesSubject.OnNext(1);
        }

        public void Dispose()
        {
            if(_isDisposed)
                return;
            if (!IsComplete && _deliverAllParticlesOnDispose)
            {
                _onDeliveredParticlesSubject.OnNext(_totalParticles - _completedParticles);
            }
            _disposableBag.Dispose();
            _pool.Despawn(_activeView);
            _isDisposed = true;
        }
    }
    
    public class UIParticleManager : ITickable, IUIParticleManager, IDisposable
    {
        private readonly TargetedParticleView.Pool _targetedParticleViewPool;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private List<TargetedUIParticlePlayHandle> _activeHandles = new List<TargetedUIParticlePlayHandle>();

        public UIParticleManager(
            TargetedParticleView.Pool targetedParticleViewPool)
        {
            _targetedParticleViewPool = targetedParticleViewPool;
        }
        
        public TargetedUIParticlePlayHandle PlayTargetedParticles(TargetedUIParticlePlayInfo playInfo)
        {
            var view = _targetedParticleViewPool.Spawn();
            view.SetPositions(playInfo.from, playInfo.to);
            view.SetMaterial(playInfo.particleMaterial);
            view.Play(playInfo.bursts, playInfo.burstInterval);
            var handle = new TargetedUIParticlePlayHandle(_targetedParticleViewPool, view, playInfo);
            _activeHandles.Add(handle);
            var timeout = view.GetEstimatedDuration() + 1;
            _ = UniTask.Delay(TimeSpan.FromSeconds(timeout), cancellationToken: _cancellationTokenSource.Token)
                .ContinueWith(() =>
                {
                    if (!handle.IsDisposed)
                    {
                        Debug.LogWarning(
                            $"UIParticleManager: Force disposing particle handle after timeout of {timeout} seconds.");
                        handle.Dispose();
                    }
                });
            return handle;
        }

        public void Tick()
        {
            for (int i = _activeHandles.Count - 1; i >= 0; i--)
            {
                if (_activeHandles[i].IsDisposed)
                {
                    _activeHandles.RemoveAt(i);
                }else if (_activeHandles[i].IsComplete)
                {
                    _activeHandles[i].Dispose();
                    _activeHandles.RemoveAt(i);
                }
            }
        }

        public void Dispose()
        {
            _targetedParticleViewPool.Clear();
            _cancellationTokenSource?.Dispose();
        }
    }
}