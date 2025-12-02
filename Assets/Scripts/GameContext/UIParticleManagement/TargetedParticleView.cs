using System.Linq;
using System.Threading;
using Coffee.UIExtensions;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

namespace GameContext.UIParticleManagement
{
    public class TargetedParticleView : MonoBehaviour, IPoolable
    {
        public class Pool : MonoMemoryPool<TargetedParticleView>
        {
            protected override void OnSpawned(TargetedParticleView item)
            {
                base.OnSpawned(item);
                item.OnSpawned();
            }

            protected override void OnDespawned(TargetedParticleView item)
            {
                base.OnDespawned(item);
                item.OnDespawned();
            }
        }
        [SerializeField]
        private UIParticle _uiParticleSystem;
        [SerializeField]
        private ParticleSystem _particleSystem;
        [SerializeField]
        private UIParticleAttractor _particleAttractor;

        private Transform _attractorOriginalParent;
        private Canvas _parentCanvas;
        private Subject<Unit> _onParticleReachedTargetSubject = new Subject<Unit>();
        private CancellationTokenSource _cancellationTokenSource;

        public Observable<Unit> OnParticleReachedTarget => _onParticleReachedTargetSubject;
        
        private void Awake()
        {
            _attractorOriginalParent = _particleAttractor.transform.parent;
            _parentCanvas = GetComponentInParent<Canvas>();
            if (_parentCanvas == null)
            {
                Debug.LogWarning("CurrencyParticleView: No Canvas found in parent.");
                return;
            }
        }

        public void OnDespawned()
        {
            _cancellationTokenSource?.Cancel();
            _particleAttractor.transform.SetParent(_attractorOriginalParent, false);
            _particleSystem.Stop();
        }

        public void OnSpawned()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _particleAttractor.transform.SetParent(_parentCanvas.transform, true);
        }
        
        public void SetPositions(Transform from, Transform to)
        {
            if (_parentCanvas == null)
                return;

            MatchSelfPositionToTarget(from);
            MatchAttractorPositionToTargetLoop(to, _cancellationTokenSource.Token).Forget();
        }

        private async UniTaskVoid MatchAttractorPositionToTargetLoop(Transform target, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && target != null)
            {
                Camera cam = _parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _parentCanvas.worldCamera;
                Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(cam, target.position);
                RectTransform canvasRect = _parentCanvas.transform as RectTransform;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        canvasRect, screenPos, cam, out var localPos))
                {
                    _particleAttractor.transform.localPosition = localPos;
                }
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
        }

        private void MatchSelfPositionToTarget(Transform target)
        {
            Camera cam = _parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _parentCanvas.worldCamera;

            // Convert 'from' world position to screen position
            Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(cam, target.position);

            // Convert screen position to Canvas local position
            RectTransform canvasRect = _parentCanvas.transform as RectTransform;
            Vector2 localPos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect, screenPos, cam, out localPos))
            {
                // Set this view's RectTransform anchoredPosition
                RectTransform myRect = transform as RectTransform;
                myRect.anchoredPosition = localPos;
            }
        }

        public void SetMaterial(Material material)
        {
            _particleSystem.GetComponent<ParticleSystemRenderer>().sharedMaterial = material;
            _uiParticleSystem.RefreshParticles();
        }
        
        public float GetEstimatedDuration()
        {
            return _particleSystem.main.duration + _particleSystem.main.startLifetime.constantMax;
        }
        
        public void Play(int[] bursts, float interval)
        {
            var emission = _particleSystem.emission;
            //Add each bursts with time delay based on index
            emission.SetBursts(bursts.Select((b, i) => new ParticleSystem.Burst(i * interval, (short)b)).ToArray());
            _particleSystem.Play();
        }
        
        public void OnParticleReachedTargetCallback()
        {
            _onParticleReachedTargetSubject.OnNext(Unit.Default);
        }

        private void OnDestroy()
        {
            _onParticleReachedTargetSubject.Dispose();
        }
    }
}