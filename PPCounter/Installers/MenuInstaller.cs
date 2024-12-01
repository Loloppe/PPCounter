using PPCounter.UI;
using Zenject;

namespace PPCounter.Installers
{
    internal class MenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<PPView>().AsSingle();
        }
    }
}
