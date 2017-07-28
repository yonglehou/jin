//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using Tunynet;
using Tunynet.Repositories;

namespace Tunynet.Settings
{
    /// <summary>
    /// 设置管理器
    /// </summary>
    /// <typeparam name="TSettingsEntity"></typeparam>
    public class SettingManager<TSettingsEntity> : ISettingsManager<TSettingsEntity> where TSettingsEntity : class, IEntity, new()
    {
        ISettingsRepository<TSettingsEntity> repository = new SettingsRepository<TSettingsEntity>();

        public TSettingsEntity Get()
        {
            return repository.Get();
        }

        public void Save(TSettingsEntity settings)
        {
            repository.Save(settings);
        }
    }
}