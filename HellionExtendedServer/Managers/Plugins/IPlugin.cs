using System;

namespace HellionExtendedServer.Common.Plugins
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
        string Description
        { get; }
        string Author
        { get; }
        string API
        { get; }
        #endregion

        #region Methods
        void Init(String ModDirectory);
        void Shutdown();
        #endregion
    }
}
