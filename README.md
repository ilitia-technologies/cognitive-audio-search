# cognitive-audio-search
Project that uses audio files to generate indexes in Azure Cognitive Search used for NetCoreConf2020 in Madrid (Spain)
 
 BRIEF
 ----------------------------------------------------------
 This project allows users to generate a Azure Cloud Based System that allows audio files to be indexed using Azure Cognitive Search based in two main steps:
 1- NetCoreConf2020.Functions: Adapts audio files to json text format to be ingested by Azure Search indexers using Azure Speech. This part is cointained in one Azure Function contained 
 2- NetCoreConf2020.AzureSearchApp: Search itself process that takes json files stored in a Blob and generate all azure search structures without using Azure Portal.
 
 REQUIREMENTS
 ----------------------------------------------
 1- An Azure Account (free account is enough for all services)
 2- Several Azure services: Azure Functions, Speech, Search and zure Storage Account.
 
  STEPS TO MAKE IT WORK
 ----------------------------------------------
 1- In Function Project replace keys and variables included local.settings.json with yours. 
 2- In Search project replace keys and variables included in App.config with yours.
 3- Now you can publish or ejecute function project, as soon as any audio file is placed in AudioBlobUrlBase blob it will be analyzed and transcrypted to json in output blob. Now you can use json files to generate your index with azure search.
