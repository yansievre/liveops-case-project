using UnityEngine;

namespace GameContext.CurrencySystem.Views
{
    public interface ICurrencyView
    {
        string CurrencyId { get; }
        RectTransform ParticleTarget { get; }
        CurrencyViewOffset CreateVisualOffset(long offsetAmount);
        void OnHitWithParticle();
    }
}