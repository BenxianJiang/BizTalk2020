Upgraded log4net to 2.0.8.0 and log4net.Ext.Serializable to the version in Nuget. The Nuget version references the correct log4net assembly.
Extract from Manifest

.assembly extern log4net
{
  .publickeytoken = (66 9E 0D DF 0B B1 AA 2A )                         // f......*
  .ver 1:2:15:0
}

Note that an assembly redirect will be required so that version 2.0.8.0 with matching public key token can be resolved. This should be added
to the machine.config (32 and 64 bit).

  <runtime>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <dependentAssembly>
        <assemblyIdentity name="log4net" publicKeyToken="669e0ddf0bb1aa2a" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-2.0.8.0" newVersion="2.0.8.0" />
      </dependentAssembly>
    </assemblyBinding>
  </runtime>

Also upgraded SSOSettingsFileReader to the latest from the BTDF distributiion (5.8 RC1)

Note that BTDF bundles the older versions of log4net and log4net.Ext.Serializable into its DeployTools folder and deploys these to the GAC 
if Includelog4net is True. I have left this as True so that the registry keys will get created and have included the new versions of
log4net and log4net.Ext.Serializable as additional Components. Componets are deployed after shared assemblies and are written to the GAC using /f
flag which should overwrite the previously GACed version of log4net.Ext.Serializable, though the old version of log4net will remain.

  <ItemGroup>
    <Components Include="ES.FS.WG.AIBP.Components.dll">
      <LocationPath>..\$(ProjectName).Components\bin\$(Configuration)</LocationPath>
    </Components>
    <!-- use the updated version of log4net and log4net.Ext.Serializable (from Nuget)-->
    <Components Include="ES.FS.WG.AIBP.Components.dll">
      <LocationPath>..\ExternalAssemblies\log4net.dll</LocationPath>
    </Components>  
    <Components Include="ES.FS.WG.AIBP.Components.dll">
      <LocationPath>..\ExternalAssemblies\log4net.Ext.Seriazable.dll</LocationPath>
    </Components>  
  </ItemGroup>

log4net.Ext.Serializable Nuget version 1.0.0.2 public key token 02e779351b902449
log4net.Ext.Serializable BTDF version 1.0.0.2 public key token 02e779351b902449

log4net Nuget version 2.0.8.0 public key token 669e0ddf0bb1aa2a
log4net BTDF version 1.0.0.2 puclic key token 1b44e1d426115821