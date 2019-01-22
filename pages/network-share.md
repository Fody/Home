If you build from a network share you will experience the following exception

    Could not load file or assembly 'file:///S:\a\b\c\d.dll' or one of its dependencies. 
    The parameter is incorrect. (Exception from HRESULT: 0x80070057 (E_INVALIDARG))

This is due to the security restriction put in place by MSBuild.

You can work around this problem by adding `<loadFromRemoteSources enabled="true"/>` to the MSBuild.exe.config file of the build machine.

It is theoretically possible for Fody to work around this problem by copying files locally before loading them. However considering the negative effects on build time due to decreased IO building from a network share is not a recommended practice. As such Fody will not be supporting this approach.