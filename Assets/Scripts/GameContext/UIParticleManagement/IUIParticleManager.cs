namespace GameContext.UIParticleManagement
{
    public interface IUIParticleManager
    {
        TargetedUIParticlePlayHandle PlayTargetedParticles(TargetedUIParticlePlayInfo playInfo);
    }
}