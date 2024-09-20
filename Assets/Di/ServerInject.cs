using Sfs2X;
using Zenject;

public class ServerInject : MonoInstaller
{
    public override void InstallBindings()
    {
        BindServer();
    }

    private void BindServer()
    {
        Container.BindInterfacesAndSelfTo<SmartFox>().AsSingle().NonLazy();
    }
}
