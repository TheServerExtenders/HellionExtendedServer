using System;

namespace HellionExtendedServer.Plugins
{
    public interface IPlugin
    {
        #region Fields
        #endregion

        #region Events
        #endregion

        #region Properties
        Guid Id
        { get; }
        string Name
        { get; }
        string Version
        { get; }
        #endregion

        #region Methods
        void Init(String ModDirectory);
        void Shutdown();
        #endregion
    }
}
