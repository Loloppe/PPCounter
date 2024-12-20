﻿using PPCounter.Calculators;
using Zenject;

namespace PPCounter.Installers
{
    class CalculatorsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ScoreSaberCalculator>().AsSingle();
            Container.BindInterfacesAndSelfTo<AccSaberCalculator>().AsSingle();
            Container.BindInterfacesAndSelfTo<BeatLeaderCalculator>().AsSingle();
        }
    }
}
