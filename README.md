# Recommended Renderings Module

## What is it?

Recommended Rendering Module provides a list of recommended renderings that potentially can be used with current item and the collection of placeholder names where selected rendering can be added:

[![N|Solid](http://www.brimit.com/~/media/images/Blog/recommendedPlaceholderList.png)](http://www.brimit.com/~/media/images/Blog/recommendedPlaceholderList.png)

## Configuration

The module has three configuration files: 
* **RecommendedRenderings.config** - includes a general settings of module and contains the following settings:
   * *ContentRootPath* - Recommended Renderings will be shown just for the selected root item and his children. It is better to specify a content root which you currently work with. It will reduce a time of rebuild index and preparing the recommended renderings list. 
Default value: /sitecore/content

   * *DatabaseName* - Determines the database that is used by the Recommended Renderings module to prepare the list of recommendations. It have to be the same database which is configured for the "recommended-renderings-index" index. Usually it is "master" and wouldn't be changed. 
Default value: master
   * *IsExpandedDefault* - If true, the Recommended Renderings tree will be expanded by default.
Default value: false
   * *PlaceholderSettingsRootPath* - Recommended placeholders list will be shown just for the selected root item and his children. It is better to specify a content root which you currently work with. It will reduce a time of rebuild index and preparing the recommended placeholders list. 
Default value: /sitecore/layout/Placeholder Settings
   * *TreeviewRootIcon* - Default icon for root element of the recommended renderings list.
Default: Applications/16x16/windows.png

* **Z.RecommendedRenderings.ContentSearch.Configuration.config** - defines a search index configuration. It’s different for each of versions of Sitecore and should not be changed.
* **Z.RecommendedRenderings.ContentSearch.Index.config** - defines a search index. It’s also different for each of versions of Sitecore. The Crowling locations should be the same with pathes that defined in the ContentRootPath and PlaceholderSettingsRootPath settings of RecommendedRenderings.config

## Installation

**WARNING:**  The module will override the Sitecore’s default SelectRendering and EditDevice dialogs. You always will be able to restore them if needed (see the [Uninstall](#Uninstall) section). BUT make sure that you haven’t already overridden this dialogs on your environment. if it is, It is not recommended to install the module. Please [contact me](mailto:apr.dev@gmail.com) in this case and we will solve it individually. 

Follow the steps below for installing module:
- Make sure that you downloaded the correct version of the module.
- Install downloaded package using installation wizard.

Optional Step.
The install package has a post install step that should rebuild the index automatically, but if in case module doesn't work after installation, you need to rebuild index manually:
- Go to the Index Manager and build the following indexes:
**recommended-renderings-index** and **recommended-placeholders-index**
*NOTE*: If you use the Sitecore v.8.2 or earlier version, you will need to build just 
	recommended-renderings-index.

## Uninstall 
If you don’t like the module or in some other reasons you want to uninstall the module, remove files by the following paths:
- sitecore\shell\Override\Applications\Dialogs\SelectRendering\SelectRendering.xml
- sitecore\shell\Override\Applications\Layouts\DeviceEditor\DeviceEditor.xml
- App_Config\Include\RecommendedRenderings
- bin\RecommendedRenderings.dll
