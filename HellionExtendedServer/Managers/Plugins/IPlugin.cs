using System;

namespace HellionExtendedServer.Common.Plugins
{
    public interface IPlugin
    {
        #region Properties

        Guid Id
        { get; }

        string GetName
        { get; }

        string Version
        { get; }

        string Description
        { get; }

        string Author
        { get; }

        string API
        { get; }

        #endregion Properties

        #region Methods

        void Init();

        void Shutdown();

        #endregion Methods
    }
}