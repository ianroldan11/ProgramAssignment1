<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="WebCrawlerModule" generation="1" functional="0" release="0" Id="653ccc0d-422f-4999-906f-8a8c053fe710" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="WebCrawlerModuleGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="WebRole1:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/LB:WebRole1:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="BleacherReportWorkerRole:APPINSIGHTS_INSTRUMENTATIONKEY" defaultValue="">
          <maps>
            <mapMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/MapBleacherReportWorkerRole:APPINSIGHTS_INSTRUMENTATIONKEY" />
          </maps>
        </aCS>
        <aCS name="BleacherReportWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/MapBleacherReportWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="BleacherReportWorkerRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/MapBleacherReportWorkerRoleInstances" />
          </maps>
        </aCS>
        <aCS name="CNNWorkerRole:APPINSIGHTS_INSTRUMENTATIONKEY" defaultValue="">
          <maps>
            <mapMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/MapCNNWorkerRole:APPINSIGHTS_INSTRUMENTATIONKEY" />
          </maps>
        </aCS>
        <aCS name="CNNWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/MapCNNWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="CNNWorkerRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/MapCNNWorkerRoleInstances" />
          </maps>
        </aCS>
        <aCS name="ESPNWorkerRole:APPINSIGHTS_INSTRUMENTATIONKEY" defaultValue="">
          <maps>
            <mapMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/MapESPNWorkerRole:APPINSIGHTS_INSTRUMENTATIONKEY" />
          </maps>
        </aCS>
        <aCS name="ESPNWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/MapESPNWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="ESPNWorkerRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/MapESPNWorkerRoleInstances" />
          </maps>
        </aCS>
        <aCS name="ForbesWorkerRole:APPINSIGHTS_INSTRUMENTATIONKEY" defaultValue="">
          <maps>
            <mapMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/MapForbesWorkerRole:APPINSIGHTS_INSTRUMENTATIONKEY" />
          </maps>
        </aCS>
        <aCS name="ForbesWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/MapForbesWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="ForbesWorkerRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/MapForbesWorkerRoleInstances" />
          </maps>
        </aCS>
        <aCS name="IMDBWorkerRole:APPINSIGHTS_INSTRUMENTATIONKEY" defaultValue="">
          <maps>
            <mapMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/MapIMDBWorkerRole:APPINSIGHTS_INSTRUMENTATIONKEY" />
          </maps>
        </aCS>
        <aCS name="IMDBWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/MapIMDBWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="IMDBWorkerRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/MapIMDBWorkerRoleInstances" />
          </maps>
        </aCS>
        <aCS name="WebRole1:APPINSIGHTS_INSTRUMENTATIONKEY" defaultValue="">
          <maps>
            <mapMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/MapWebRole1:APPINSIGHTS_INSTRUMENTATIONKEY" />
          </maps>
        </aCS>
        <aCS name="WebRole1:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/MapWebRole1:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="WebRole1Instances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/MapWebRole1Instances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:WebRole1:Endpoint1">
          <toPorts>
            <inPortMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/WebRole1/Endpoint1" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapBleacherReportWorkerRole:APPINSIGHTS_INSTRUMENTATIONKEY" kind="Identity">
          <setting>
            <aCSMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/BleacherReportWorkerRole/APPINSIGHTS_INSTRUMENTATIONKEY" />
          </setting>
        </map>
        <map name="MapBleacherReportWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/BleacherReportWorkerRole/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapBleacherReportWorkerRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/BleacherReportWorkerRoleInstances" />
          </setting>
        </map>
        <map name="MapCNNWorkerRole:APPINSIGHTS_INSTRUMENTATIONKEY" kind="Identity">
          <setting>
            <aCSMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/CNNWorkerRole/APPINSIGHTS_INSTRUMENTATIONKEY" />
          </setting>
        </map>
        <map name="MapCNNWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/CNNWorkerRole/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapCNNWorkerRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/CNNWorkerRoleInstances" />
          </setting>
        </map>
        <map name="MapESPNWorkerRole:APPINSIGHTS_INSTRUMENTATIONKEY" kind="Identity">
          <setting>
            <aCSMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/ESPNWorkerRole/APPINSIGHTS_INSTRUMENTATIONKEY" />
          </setting>
        </map>
        <map name="MapESPNWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/ESPNWorkerRole/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapESPNWorkerRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/ESPNWorkerRoleInstances" />
          </setting>
        </map>
        <map name="MapForbesWorkerRole:APPINSIGHTS_INSTRUMENTATIONKEY" kind="Identity">
          <setting>
            <aCSMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/ForbesWorkerRole/APPINSIGHTS_INSTRUMENTATIONKEY" />
          </setting>
        </map>
        <map name="MapForbesWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/ForbesWorkerRole/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapForbesWorkerRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/ForbesWorkerRoleInstances" />
          </setting>
        </map>
        <map name="MapIMDBWorkerRole:APPINSIGHTS_INSTRUMENTATIONKEY" kind="Identity">
          <setting>
            <aCSMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/IMDBWorkerRole/APPINSIGHTS_INSTRUMENTATIONKEY" />
          </setting>
        </map>
        <map name="MapIMDBWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/IMDBWorkerRole/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapIMDBWorkerRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/IMDBWorkerRoleInstances" />
          </setting>
        </map>
        <map name="MapWebRole1:APPINSIGHTS_INSTRUMENTATIONKEY" kind="Identity">
          <setting>
            <aCSMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/WebRole1/APPINSIGHTS_INSTRUMENTATIONKEY" />
          </setting>
        </map>
        <map name="MapWebRole1:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/WebRole1/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapWebRole1Instances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/WebRole1Instances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="BleacherReportWorkerRole" generation="1" functional="0" release="0" software="C:\Users\Ian Francis Roldan\source\repos\WebCrawlerModule\WebCrawlerModule\csx\Debug\roles\BleacherReportWorkerRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <settings>
              <aCS name="APPINSIGHTS_INSTRUMENTATIONKEY" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;BleacherReportWorkerRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;BleacherReportWorkerRole&quot; /&gt;&lt;r name=&quot;CNNWorkerRole&quot; /&gt;&lt;r name=&quot;ESPNWorkerRole&quot; /&gt;&lt;r name=&quot;ForbesWorkerRole&quot; /&gt;&lt;r name=&quot;IMDBWorkerRole&quot; /&gt;&lt;r name=&quot;WebRole1&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/BleacherReportWorkerRoleInstances" />
            <sCSPolicyUpdateDomainMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/BleacherReportWorkerRoleUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/BleacherReportWorkerRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="CNNWorkerRole" generation="1" functional="0" release="0" software="C:\Users\Ian Francis Roldan\source\repos\WebCrawlerModule\WebCrawlerModule\csx\Debug\roles\CNNWorkerRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <settings>
              <aCS name="APPINSIGHTS_INSTRUMENTATIONKEY" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;CNNWorkerRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;BleacherReportWorkerRole&quot; /&gt;&lt;r name=&quot;CNNWorkerRole&quot; /&gt;&lt;r name=&quot;ESPNWorkerRole&quot; /&gt;&lt;r name=&quot;ForbesWorkerRole&quot; /&gt;&lt;r name=&quot;IMDBWorkerRole&quot; /&gt;&lt;r name=&quot;WebRole1&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/CNNWorkerRoleInstances" />
            <sCSPolicyUpdateDomainMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/CNNWorkerRoleUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/CNNWorkerRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="ESPNWorkerRole" generation="1" functional="0" release="0" software="C:\Users\Ian Francis Roldan\source\repos\WebCrawlerModule\WebCrawlerModule\csx\Debug\roles\ESPNWorkerRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <settings>
              <aCS name="APPINSIGHTS_INSTRUMENTATIONKEY" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;ESPNWorkerRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;BleacherReportWorkerRole&quot; /&gt;&lt;r name=&quot;CNNWorkerRole&quot; /&gt;&lt;r name=&quot;ESPNWorkerRole&quot; /&gt;&lt;r name=&quot;ForbesWorkerRole&quot; /&gt;&lt;r name=&quot;IMDBWorkerRole&quot; /&gt;&lt;r name=&quot;WebRole1&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/ESPNWorkerRoleInstances" />
            <sCSPolicyUpdateDomainMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/ESPNWorkerRoleUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/ESPNWorkerRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="ForbesWorkerRole" generation="1" functional="0" release="0" software="C:\Users\Ian Francis Roldan\source\repos\WebCrawlerModule\WebCrawlerModule\csx\Debug\roles\ForbesWorkerRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <settings>
              <aCS name="APPINSIGHTS_INSTRUMENTATIONKEY" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;ForbesWorkerRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;BleacherReportWorkerRole&quot; /&gt;&lt;r name=&quot;CNNWorkerRole&quot; /&gt;&lt;r name=&quot;ESPNWorkerRole&quot; /&gt;&lt;r name=&quot;ForbesWorkerRole&quot; /&gt;&lt;r name=&quot;IMDBWorkerRole&quot; /&gt;&lt;r name=&quot;WebRole1&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/ForbesWorkerRoleInstances" />
            <sCSPolicyUpdateDomainMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/ForbesWorkerRoleUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/ForbesWorkerRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="IMDBWorkerRole" generation="1" functional="0" release="0" software="C:\Users\Ian Francis Roldan\source\repos\WebCrawlerModule\WebCrawlerModule\csx\Debug\roles\IMDBWorkerRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <settings>
              <aCS name="APPINSIGHTS_INSTRUMENTATIONKEY" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;IMDBWorkerRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;BleacherReportWorkerRole&quot; /&gt;&lt;r name=&quot;CNNWorkerRole&quot; /&gt;&lt;r name=&quot;ESPNWorkerRole&quot; /&gt;&lt;r name=&quot;ForbesWorkerRole&quot; /&gt;&lt;r name=&quot;IMDBWorkerRole&quot; /&gt;&lt;r name=&quot;WebRole1&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/IMDBWorkerRoleInstances" />
            <sCSPolicyUpdateDomainMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/IMDBWorkerRoleUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/IMDBWorkerRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="WebRole1" generation="1" functional="0" release="0" software="C:\Users\Ian Francis Roldan\source\repos\WebCrawlerModule\WebCrawlerModule\csx\Debug\roles\WebRole1" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="-1" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="APPINSIGHTS_INSTRUMENTATIONKEY" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;WebRole1&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;BleacherReportWorkerRole&quot; /&gt;&lt;r name=&quot;CNNWorkerRole&quot; /&gt;&lt;r name=&quot;ESPNWorkerRole&quot; /&gt;&lt;r name=&quot;ForbesWorkerRole&quot; /&gt;&lt;r name=&quot;IMDBWorkerRole&quot; /&gt;&lt;r name=&quot;WebRole1&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/WebRole1Instances" />
            <sCSPolicyUpdateDomainMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/WebRole1UpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/WebRole1FaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="WebRole1UpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="CNNWorkerRoleUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="BleacherReportWorkerRoleUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="IMDBWorkerRoleUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="ESPNWorkerRoleUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="ForbesWorkerRoleUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="BleacherReportWorkerRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="CNNWorkerRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="ESPNWorkerRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="ForbesWorkerRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="IMDBWorkerRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="WebRole1FaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="BleacherReportWorkerRoleInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="CNNWorkerRoleInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="ESPNWorkerRoleInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="ForbesWorkerRoleInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="IMDBWorkerRoleInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="WebRole1Instances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="081e2511-c35c-47a7-a85f-62ff95ce88b5" ref="Microsoft.RedDog.Contract\ServiceContract\WebCrawlerModuleContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="c4bf8835-ab25-480a-8bc9-1150086bc388" ref="Microsoft.RedDog.Contract\Interface\WebRole1:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/WebCrawlerModule/WebCrawlerModuleGroup/WebRole1:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>